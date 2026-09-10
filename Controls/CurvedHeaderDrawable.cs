namespace Equiparts.Controls;

// Draws the wavy navy header behind the login logo badge: short/high on the
// left, sweeping down to long/low on the right. Plain XAML shapes can't
// produce this curve, so it's drawn directly on a GraphicsView canvas, with
// the orange accent traced along the same edge.
public class CurvedHeaderDrawable : IDrawable
{
    private static readonly Color NavyTop = Color.FromArgb("#0B2E4A");
    private static readonly Color NavyBottom = Color.FromArgb("#08152C");
    private static readonly Color Accent = Color.FromArgb("#F58220");

    // 0..1: lets the splash-to-login transition grow this same curve in from
    // nothing instead of only ever drawing it fully formed (default 1, so
    // LoginPage's own static usage is unaffected). Every curve point is
    // lerped from the bottom edge (h) toward its final position, so the
    // right side (already close to h) settles before the left side does -
    // which reads as the wave sweeping up from the bottom-right, matching
    // the reference animation, with no separate easing logic needed.
    public float GrowthProgress { get; set; } = 1f;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float w = dirtyRect.Width;
        float h = dirtyRect.Height;
        float p = Math.Clamp(GrowthProgress, 0f, 1f);

        if (p <= 0.001f)
            return;

        float Lerp(float finalY) => h + (finalY - h) * p;

        float y1 = Lerp(h * 0.82f);
        float cy1 = Lerp(h * 0.95f);
        float cy2 = Lerp(h * 0.62f);
        float y2 = Lerp(h * 0.40f);

        var boundary = new PathF();
        boundary.MoveTo(w, y1);
        boundary.CurveTo(w * 0.62f, cy1, w * 0.28f, cy2, 0, y2);

        var fill = new PathF();
        fill.MoveTo(0, 0);
        fill.LineTo(w, 0);
        fill.LineTo(w, y1);
        fill.CurveTo(w * 0.62f, cy1, w * 0.28f, cy2, 0, y2);
        fill.Close();

        // Drawn before the fill: on this canvas, SetFillPaint/FillPath resets the
        // stroke paint, so a stroke issued afterwards silently never appears. The
        // navy fill painted next covers the inner half of this line, leaving a
        // clean orange edge traced along the curve.
        canvas.StrokeColor = Accent;
        canvas.StrokeSize = 5;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.DrawPath(boundary);

        var gradient = new LinearGradientPaint
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1),
            GradientStops =
            [
                new PaintGradientStop(0, NavyTop),
                new PaintGradientStop(1, NavyBottom)
            ]
        };

        canvas.SetFillPaint(gradient, dirtyRect);
        canvas.FillPath(fill);
    }
}
