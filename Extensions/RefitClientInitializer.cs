using System.Diagnostics;
using System.Text;
using Equiparts.Configuration;
using Equiparts.Handlers;
using Equiparts.Interfaces;
using Refit;

namespace Equiparts.Extensions;

public static class RefitClientInitializer
{
    public static MauiAppBuilder RegisterRefitClients(this MauiAppBuilder builder)
    {
        var baseAddress = new Uri(ApiConstants.BaseUrl);
        builder.Services.AddTransient<HttpMessageLogHandler>();
        // AddRefitGeneratedClient (not AddRefitClient) - wires up Refit's compile-time
        // source-generated implementation instead of the reflection-based request
        // builder, which needs a separate Refit.Reflection package and isn't AOT/trim
        // friendly on mobile targets. Our interfaces are plain enough to fully
        // source-generate (no RF006 diagnostics).

        // Unauthenticated - must NOT go through AuthHandler or login/refresh would recurse.
        builder.Services.AddRefitGeneratedClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress)
#if DEBUG
            .AddHttpMessageHandler<HttpMessageLogHandler>()
#endif
            ;

        builder.Services.AddRefitGeneratedClient<ICatalogApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress)
#if DEBUG
            .AddHttpMessageHandler<HttpMessageLogHandler>()
#endif
            ;

        // Authenticated - AuthHandler attaches the bearer token and handles 401/refresh/retry.
        builder.Services.AddRefitGeneratedClient<IProfileApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress)
            .AddHttpMessageHandler<AuthHandler>()
#if DEBUG
            .AddHttpMessageHandler<HttpMessageLogHandler>()
#endif
            ;

        builder.Services.AddRefitGeneratedClient<IOrderApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress)
            .AddHttpMessageHandler<AuthHandler>()
#if DEBUG
            .AddHttpMessageHandler<HttpMessageLogHandler>()
#endif
            ;

        builder.Services.AddRefitGeneratedClient<ICartApi>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress)
            .AddHttpMessageHandler<AuthHandler>()
#if DEBUG
            .AddHttpMessageHandler<HttpMessageLogHandler>()
#endif
            ;

        return builder;
    }

    public class HttpMessageLogHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var req = request;
            var id = Guid.NewGuid().ToString();
            var msg = $"[{id} -   ]";
            StringBuilder apiDetails = new();

            Debug.WriteLine($"{msg}========Start==========");
            Debug.WriteLine($"{msg} {req.Method} {req.RequestUri.PathAndQuery} {req.RequestUri.Scheme}/{req.Version}");
            Debug.WriteLine($"{msg} Host: {req.RequestUri.Scheme}://{req.RequestUri.Host}");

            apiDetails.Append($"{"Starting Api Call "}{req.Method}");
            apiDetails.Append($"{"RequestUri "}{req.RequestUri.PathAndQuery}");

            foreach (var header in req.Headers)
                Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

            if (req.Content != null)
            {
                foreach (var header in req.Content.Headers)
                {
                    Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                    apiDetails.Append($"{"Header Key "}{header.Key}");
                    apiDetails.Append($"{"Header Value "}{header.Value}");
                }

                if (req.Content is StringContent)
                {
                    var result = await req.Content.ReadAsStringAsync(cancellationToken);
                    var reqString = System.Text.Json.JsonSerializer.Serialize(result, System.Text.Json.JsonSerializerOptions.Default);

                    Debug.WriteLine($"{msg} Content:");
                    Debug.WriteLine($"{msg} {string.Join("", reqString)}");
                    apiDetails.Append($"{"Request "}{reqString}");
                }
                else
                {
                    string body = await request.Content.ReadAsStringAsync();

                    var reqString = System.Text.Json.JsonSerializer.Serialize(body, System.Text.Json.JsonSerializerOptions.Default);

                    Debug.WriteLine($"{msg} Content:");
                    Debug.WriteLine($"{msg} {string.Join("", reqString)}");
                    apiDetails.Append($"{"Request "}{reqString}");
                }
            }

            var start = DateTime.Now;
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var end = DateTime.Now;

            Debug.WriteLine($"{msg} Duration: {end - start}");
            Debug.WriteLine($"{msg}==========End==========");
            apiDetails.Append($"{msg} Duration: {end - start}");

            msg = $"[{id} - Response]";
            Debug.WriteLine($"{msg}=========Start=========");
            apiDetails.Append($"{msg} [{id} - Response]");

            var resp = response;
            foreach (var header in resp.Headers)
            {
                Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                apiDetails.Append($"{"Header Key "}{header.Key}");
                apiDetails.Append($"{"Header Value "}{header.Value}");
            }
            try
            {
                if (resp.Content != null)
                {
                    var result = await resp.Content.ReadAsStringAsync(cancellationToken);
                    start = DateTime.Now;
                    Debug.WriteLine($"{msg} Content:{result.ToString()}");
                    apiDetails.Append($"{"Content "}{result.ToString()}");
                    end = DateTime.Now;
                    Debug.WriteLine($"{msg} Duration: {end - start} Time span");
                    apiDetails.Append($"{"Duration "}{end - start} Time span");
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Failed - HandleException [{exceptionName}] \n{exceptionToString}", exception.GetType().Name, exception.ToString());
            }
            Debug.WriteLine($"{msg}==========End==========");
            Console.WriteLine(apiDetails);
            return response;
        }

        private static string Combine(string baseUrl, string relativeUrl) =>
         $"{baseUrl.TrimEnd('/')}/{relativeUrl.TrimStart('/')}";
    }

}
