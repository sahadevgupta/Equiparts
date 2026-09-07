using Equiparts.Controls;

namespace Equiparts.Views;

public partial class InitialLoadingActivityIndicator : BasePage
{
    private const string OuterSpinAnimationName = "OuterRingSpin";
    private const string InnerSpinAnimationName = "InnerRingSpin";
    private const string LogoPulseAnimationName = "LogoPulse";

    // Matches the outer/inner ring geometry and the EP-mark pulse of the
    // approved svgviewer-output reference (208x208 viewBox, cx/cy 104).
    private static readonly Color TrackColor = new(1f, 1f, 1f, 0.08f);
    private static readonly Color OuterArcColor = Color.FromArgb("#E9943A");
    private static readonly Color InnerArcColor = new(1f, 1f, 1f, 0.55f);

    public InitialLoadingActivityIndicator()
    {
        InitializeComponent();

        OuterRingGraphics.Drawable = new RingSegmentDrawable(
            radius: 102.5f, strokeWidth: 2f, trackColor: TrackColor, arcColor: OuterArcColor, arcSweepDegrees: 90f);

        InnerRingGraphics.Drawable = new RingSegmentDrawable(
            radius: 85.4f, strokeWidth: 2f, trackColor: TrackColor, arcColor: InnerArcColor, arcSweepDegrees: 90f);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        OuterRingGraphics.Rotation = 0;
        var outerSpin = new Animation(v => OuterRingGraphics.Rotation = v, 0, 360);
        outerSpin.Commit(this, OuterSpinAnimationName, length: 1150,
            easing: CubicBezierEasing.Create(0.65, 0.1, 0.35, 0.9), repeat: () => true);

        InnerRingGraphics.Rotation = 0;
        var innerSpin = new Animation(v => InnerRingGraphics.Rotation = v, 0, -360);
        innerSpin.Commit(this, InnerSpinAnimationName, length: 2400, easing: Easing.Linear, repeat: () => true);

        LogoImage.Scale = 1.0;
        var pulse = new Animation();
        pulse.Add(0.0, 0.5, new Animation(v => LogoImage.Scale = v, 1.0, 1.05));
        pulse.Add(0.5, 1.0, new Animation(v => LogoImage.Scale = v, 1.05, 1.0));
        pulse.Commit(this, LogoPulseAnimationName, length: 2200, easing: Easing.Linear, repeat: () => true);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        this.AbortAnimation(OuterSpinAnimationName);
        this.AbortAnimation(InnerSpinAnimationName);
        this.AbortAnimation(LogoPulseAnimationName);
    }
}
