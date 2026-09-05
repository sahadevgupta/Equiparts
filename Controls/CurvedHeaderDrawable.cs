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

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float w = dirtyRect.Width;
        float h = dirtyRect.Height;

        var boundary = new PathF();
        boundary.MoveTo(w, h * 0.82f);
        boundary.CurveTo(w * 0.62f, h * 0.95f, w * 0.28f, h * 0.62f, 0, h * 0.40f);

        var fill = new PathF();
        fill.MoveTo(0, 0);
        fill.LineTo(w, 0);
        fill.LineTo(w, h * 0.82f);
        fill.CurveTo(w * 0.62f, h * 0.95f, w * 0.28f, h * 0.62f, 0, h * 0.40f);
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
