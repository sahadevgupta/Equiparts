namespace Equiparts.Controls;

// One ring of the splash-screen loader: a faint full-circle track with a
// shorter colored arc on top, both centered in the drawable's own bounds.
// The GraphicsView hosting this is rotated continuously in code-behind (each
// ring at its own speed/direction), so only the arc's on-screen angle
// changes; its shape here stays fixed. Mirrors the two-ring design from the
// approved svgviewer-output reference (outer orange ring, inner pale ring).
public class RingSegmentDrawable : IDrawable
{
    private readonly float _radius;
    private readonly float _strokeWidth;
    private readonly Color _trackColor;
    private readonly Color _arcColor;
    private readonly float _arcSweepDegrees;

    public RingSegmentDrawable(float radius, float strokeWidth, Color trackColor, Color arcColor, float arcSweepDegrees)
    {
        _radius = radius;
        _strokeWidth = strokeWidth;
        _trackColor = trackColor;
        _arcColor = arcColor;
        _arcSweepDegrees = arcSweepDegrees;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var cx = dirtyRect.Width / 2f;
        var cy = dirtyRect.Height / 2f;
        var diameter = _radius * 2f;
        var x = cx - _radius;
        var y = cy - _radius;

        canvas.StrokeSize = _strokeWidth;
        canvas.StrokeLineCap = LineCap.Round;

        canvas.StrokeColor = _trackColor;
        canvas.DrawEllipse(x, y, diameter, diameter);

        canvas.StrokeColor = _arcColor;
        canvas.DrawArc(x, y, diameter, diameter, 90, 90 - _arcSweepDegrees, true, false);
    }
}
