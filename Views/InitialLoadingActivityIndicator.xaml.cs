using Equiparts.Controls;
using Equiparts.Interfaces;

namespace Equiparts.Views;

public partial class InitialLoadingActivityIndicator : BasePage
{
    // Mirrors the reveal choreography authored in
    // Resources/Images/ep_logo_animated.svg (the approved brand splash asset):
    // each glyph's outline traces on first, then floods to a solid fill - E's
    // top bar, then its body, then P - followed by a brief glow pulse once
    // the mark is fully assembled. Every window below is a 55/45 outline/fill
    // split of that same source SVG's own start/end keyframes (converted from
    // its 3.8s keyTimes to milliseconds), not a compressed/invented duration.
    private const int ETopStartMs = 190, ETopOutlineMs = 481, ETopFillMs = 393;   // window 190-1064ms
    private const int EBodyStartMs = 950, EBodyOutlineMs = 564, EBodyFillMs = 462; // window 950-1976ms
    private const int PStartMs = 1824, POutlineMs = 564, PFillMs = 462;            // window 1824-2850ms
    private const int SettleHoldMs = 320;
    private const int ExitFadeMs = 180;

    // The splash-to-login handoff: the EP mark shrinks from its big centered
    // splash size down into its exact Login-page badge size/position while
    // the wavy header grows in behind it, so the page swap lands on pixels
    // that already match - no separate "badge fades in again" replay on
    // LoginPage's side (see LoginPage.xaml.cs AnimateEntranceAsync, which
    // only animates the card now). 90x78 is LogoGraphics' size once settled
    // at true badge scale (1:1); 160x139 was its size during the reveal, so
    // LogoScene starts scaled up by that ratio and animates down to 1.0.
    private const double LogoSceneBigScale = 160.0 / 90.0;
    private const double BadgeCenterYFromTop = 135.0 + 48.0; // matches LoginPage's badge Margin + half its 96 size
    private const int TransitionMs = 650;

    private const string PulseGlowUpAnimationName = "SplashPulseGlowUp";
    private const string PulseGlowDownAnimationName = "SplashPulseGlowDown";
    private const string HeaderGrowAnimationName = "SplashHeaderGrow";

    // Same cubic-bezier(0.4, 0, 0.2, 1) spline the source SVG uses for every
    // reveal keyframe (a standard material "ease-in-out").
    private static readonly Easing StandardEasing = CubicBezierEasing.Create(0.4, 0, 0.2, 1);

    private readonly IAppLifeCycleCoordinator _coordinator;
    private readonly ITokenService _tokenService;
    private readonly LogoRevealDrawable _drawable = new();
    private readonly CurvedHeaderDrawable _headerDrawable = new();

    private bool _hasStarted;

    public InitialLoadingActivityIndicator(IAppLifeCycleCoordinator coordinator, ITokenService tokenService)
    {
        InitializeComponent();
        _coordinator = coordinator;
        _tokenService = tokenService;
        LogoGraphics.Drawable = _drawable;
        TransitionHeaderGraphics.Drawable = _headerDrawable;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_hasStarted)
            return;

        _hasStarted = true;
        _ = RunSplashAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        this.AbortAnimation("ETopOutline");
        this.AbortAnimation("ETopFill");
        this.AbortAnimation("EBodyOutline");
        this.AbortAnimation("EBodyFill");
        this.AbortAnimation("POutline");
        this.AbortAnimation("PFill");
        this.AbortAnimation(PulseGlowUpAnimationName);
        this.AbortAnimation(PulseGlowDownAnimationName);
        this.AbortAnimation(HeaderGrowAnimationName);
    }

    private async Task RunSplashAsync()
    {
        await RunRevealSequenceAsync();

        var hasSession = await _tokenService.HasStoredSessionAsync();
        if (hasSession)
        {
            // Home has no wave/badge header to morph into, so a quick dissolve
            // (rather than the Login-shaped transition below) reads correctly.
            await RootLayer.FadeToAsync(0, ExitFadeMs, Easing.CubicIn);
        }
        else
        {
            await RunTransitionToLoginAsync();
        }

        await _coordinator.NavigateToInitialDestinationAsync();
    }

    private async Task RunRevealSequenceAsync()
    {
        RootLayer.Opacity = 1;
        AmbientGlow.Opacity = 0;
        SparkleLayer.Opacity = 0;
        LogoGraphics.Scale = 1.0;
        LogoGlow.Radius = 0;
        LogoGlow.Opacity = 0;
        LogoScene.Scale = LogoSceneBigScale;
        LogoScene.TranslationY = 0;
        BadgeRing.Opacity = 0;
        TransitionPageBackdrop.Opacity = 0;
        TransitionCard.Opacity = 0;
        TransitionHeaderGraphics.Opacity = 0;
        _headerDrawable.GrowthProgress = 0;

        _drawable.ETopOutline = _drawable.ETopFill = 0;
        _drawable.EBodyOutline = _drawable.EBodyFill = 0;
        _drawable.POutline = _drawable.PFill = 0;
        _drawable.GlowBoost = 0;
        LogoGraphics.Invalidate();

        var ambientIn = AmbientGlow.FadeToAsync(1, 400, Easing.CubicOut);
        var sparklesIn = SparkleLayer.FadeToAsync(1, 500, Easing.CubicOut);

        var eTop = RevealGlyphAsync(v => _drawable.ETopOutline = v, v => _drawable.ETopFill = v,
            ETopStartMs, ETopOutlineMs, ETopFillMs, "ETop");
        var eBody = RevealGlyphAsync(v => _drawable.EBodyOutline = v, v => _drawable.EBodyFill = v,
            EBodyStartMs, EBodyOutlineMs, EBodyFillMs, "EBody");
        var pGlyph = RevealGlyphAsync(v => _drawable.POutline = v, v => _drawable.PFill = v,
            PStartMs, POutlineMs, PFillMs, "P");

        await Task.WhenAll(ambientIn, sparklesIn, eTop, eBody, pGlyph);

        await PlayCompletionPulseAsync();
        await Task.Delay(SettleHoldMs);
    }

    private async Task RevealGlyphAsync(Action<float> setOutline, Action<float> setFill,
        int startDelayMs, int outlineDurationMs, int fillDurationMs, string animationNamePrefix)
    {
        await Task.Delay(startDelayMs);
        await AnimateProgressAsync(setOutline, outlineDurationMs, animationNamePrefix + "Outline");
        await AnimateProgressAsync(setFill, fillDurationMs, animationNamePrefix + "Fill");
    }

    private async Task AnimateProgressAsync(Action<float> setter, int durationMs, string animationName)
    {
        var tcs = new TaskCompletionSource<bool>();
        var animation = new Animation(v =>
        {
            setter((float)v);
            LogoGraphics.Invalidate();
        }, 0, 1);
        animation.Commit(this, animationName, 16, (uint)durationMs, StandardEasing, finished: (_, __) => tcs.TrySetResult(true));
        await tcs.Task;
    }

    // Approximates the source asset's "pulse bloom + specular shimmer" moment
    // (a brief brightness flash right as the mark finishes assembling) with a
    // soft glow-and-scale flourish, native to MAUI's Shadow/Scale animation.
    private async Task PlayCompletionPulseAsync()
    {
        var tcsUp = new TaskCompletionSource<bool>();
        var glowUp = new Animation(v =>
        {
            LogoGlow.Radius = (float)(v * 14);
            LogoGlow.Opacity = (float)(v * 0.85);
            _drawable.GlowBoost = (float)v;
            LogoGraphics.Invalidate();
        }, 0, 1);
        glowUp.Commit(this, PulseGlowUpAnimationName, 16, 170, Easing.CubicOut, finished: (_, __) => tcsUp.TrySetResult(true));

        await Task.WhenAll(tcsUp.Task, LogoGraphics.ScaleToAsync(1.05, 170, Easing.CubicOut));

        var tcsDown = new TaskCompletionSource<bool>();
        var glowDown = new Animation(v =>
        {
            LogoGlow.Radius = (float)(v * 14);
            LogoGlow.Opacity = (float)(v * 0.85);
            _drawable.GlowBoost = (float)v;
            LogoGraphics.Invalidate();
        }, 1, 0);
        glowDown.Commit(this, PulseGlowDownAnimationName, 16, 220, Easing.CubicIn, finished: (_, __) => tcsDown.TrySetResult(true));

        await Task.WhenAll(tcsDown.Task, LogoGraphics.ScaleToAsync(1.0, 220, Easing.CubicIn));
    }

    // Continuous splash-to-login handoff: the mark shrinks into its badge size
    // and slides up to the badge's exact position while the wavy header grows
    // in behind it and the ambient glow/sparkles fade away - so by the time
    // Shell actually swaps to LoginPage, the header+badge already look
    // identical to LoginPage's own static ones and the cut is imperceptible.
    private async Task RunTransitionToLoginAsync()
    {
        var currentCenterY = RootLayer.Height / 2.0;
        var translationY = BadgeCenterYFromTop - currentCenterY;

        var fadeOutAmbient = AmbientGlow.FadeToAsync(0, 300, Easing.CubicIn);
        var fadeOutSparkles = SparkleLayer.FadeToAsync(0, 300, Easing.CubicIn);
        var backdropFadeIn = TransitionPageBackdrop.FadeToAsync(1, 200, Easing.CubicOut);
        var cardFadeIn = TransitionCard.FadeToAsync(1, 200, Easing.CubicOut);
        var headerFadeIn = TransitionHeaderGraphics.FadeToAsync(1, 200, Easing.CubicOut);
        var ringFadeIn = BadgeRing.FadeToAsync(1, 300, Easing.CubicOut);
        var headerGrow = AnimateHeaderGrowthAsync();
        var sceneScale = LogoScene.ScaleToAsync(1.0, TransitionMs, Easing.CubicInOut);
        var sceneMove = LogoScene.TranslateToAsync(0, translationY, TransitionMs, Easing.CubicInOut);

        await Task.WhenAll(fadeOutAmbient, fadeOutSparkles, backdropFadeIn, cardFadeIn, headerFadeIn, ringFadeIn, headerGrow, sceneScale, sceneMove);
    }

    private async Task AnimateHeaderGrowthAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        var animation = new Animation(v =>
        {
            _headerDrawable.GrowthProgress = (float)v;
            TransitionHeaderGraphics.Invalidate();
        }, 0, 1);
        animation.Commit(this, HeaderGrowAnimationName, 16, TransitionMs, StandardEasing, finished: (_, __) => tcs.TrySetResult(true));
        await tcs.Task;
    }
}
