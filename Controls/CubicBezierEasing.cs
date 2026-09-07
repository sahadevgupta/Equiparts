namespace Equiparts.Controls;

// Reproduces CSS's cubic-bezier(x1,y1,x2,y2) timing function as a MAUI Easing,
// so the outer loader ring can use the exact spline from the approved
// svgviewer-output reference (keySplines="0.65 0.1 0.35 0.9") instead of the
// closest built-in Easing.
public static class CubicBezierEasing
{
    public static Easing Create(double x1, double y1, double x2, double y2) =>
        new(t => SolveY(t, x1, y1, x2, y2));

    private static double SampleCurve(double t, double p1, double p2) =>
        3 * (1 - t) * (1 - t) * t * p1 + 3 * (1 - t) * t * t * p2 + t * t * t;

    private static double SampleCurveDerivative(double t, double p1, double p2) =>
        3 * (1 - t) * (1 - t) * p1 + 6 * (1 - t) * t * (p2 - p1) + 3 * t * t * (1 - p2);

    private static double SolveY(double x, double x1, double y1, double x2, double y2)
    {
        var t = x;
        for (var i = 0; i < 8; i++)
        {
            var xError = SampleCurve(t, x1, x2) - x;
            if (Math.Abs(xError) < 1e-6)
                break;

            var derivative = SampleCurveDerivative(t, x1, x2);
            if (Math.Abs(derivative) < 1e-6)
                break;

            t -= xError / derivative;
        }

        return SampleCurve(t, y1, y2);
    }
}
