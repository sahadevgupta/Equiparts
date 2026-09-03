using System.Net;
using System.Net.Http.Headers;
using Equiparts.Interfaces;
using Microsoft.Extensions.Logging;

namespace Equiparts.Handlers;

// Attaches a Bearer token to every request on the authenticated Refit clients it's
// registered against, refreshing first if the token is expired/about to expire, and
// retries once on a 401 after forcing a refresh. Never registered on IAuthApi, so
// login/refresh calls can't recurse back through this handler.
public sealed class AuthHandler : DelegatingHandler
{
    private static readonly HttpRequestOptionsKey<bool> IsRetryKey = new("Equiparts.AuthHandler.IsRetry");

    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthHandler> _logger;

    public AuthHandler(ITokenService tokenService, ILogger<AuthHandler> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Buffer the body into memory up front (before the first send) so the request
        // can be safely rebuilt for a retry even if the original content was a
        // non-seekable stream (e.g. a multipart file upload).
        if (request.Content is not null)
            await request.Content.LoadIntoBufferAsync();

        var accessToken = await _tokenService.GetValidAccessTokenAsync(cancellationToken);

        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogWarning("No valid session for authenticated request to {Uri}; short-circuiting with 401.", request.RequestUri);
            return new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                RequestMessage = request,
                ReasonPhrase = "No valid session"
            };
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        var alreadyRetried = request.Options.TryGetValue(IsRetryKey, out var retried) && retried;

        if (alreadyRetried)
        {
            _logger.LogWarning("Request to {Uri} still returned 401 after a refresh-and-retry; giving up.", request.RequestUri);
            return response;
        }

        _logger.LogInformation("API returned 401 for {Uri}. Retrying request after token refresh.", request.RequestUri);
        response.Dispose();

        var refreshedToken = await _tokenService.ForceRefreshAccessTokenAsync(accessToken, cancellationToken);

        if (string.IsNullOrEmpty(refreshedToken))
        {
            _logger.LogWarning("Token refresh after 401 failed. Session expired.");
            return new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                RequestMessage = request,
                ReasonPhrase = "Session expired"
            };
        }

        var retryRequest = await CloneRequestAsync(request);
        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshedToken);
        retryRequest.Options.Set(IsRetryKey, true);

        return await base.SendAsync(retryRequest, cancellationToken);
    }

    // HttpRequestMessage instances can't be sent twice, so retrying requires a fresh
    // clone that preserves method, URI, headers, and body (query parameters are part
    // of the URI and are copied along with it).
    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri)
        {
            Version = original.Version
        };

        if (original.Content is not null)
        {
            var buffer = await original.Content.ReadAsByteArrayAsync();
            var clonedContent = new ByteArrayContent(buffer);

            foreach (var header in original.Content.Headers)
                clonedContent.Headers.TryAddWithoutValidation(header.Key, header.Value);

            clone.Content = clonedContent;
        }

        foreach (var header in original.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return clone;
    }
}
