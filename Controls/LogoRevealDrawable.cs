using System.Globalization;
using System.Text.RegularExpressions;

namespace Equiparts.Controls;

// Redraws the exact three glyph paths from the approved
// Resources/Images/ep_logo_animated.svg (same coordinates, untouched - no
// artwork change) so each stroke can be revealed the way the reference
// animation shows it: a glowing outline traces on first, then the shape
// floods to a solid fill. A flat bitmap can only ever be masked open, it
// can't show a hollow, still-unfilled outline, so this has to be vector.
public class LogoRevealDrawable : IDrawable
{
    private const float ViewW = 410f;
    private const float ViewH = 356f;

    private static readonly Color White = Color.FromArgb("#C92027");
    private static readonly Color Accent = Color.FromArgb("#0E1826");

    // Same d="..." data as the three <path> elements in ep_logo_animated.svg.
    private readonly PathF _eTop = ParsePath(
        "M2600 2793 c0 -8 -79 -101 -200 -232 -52 -58 -105 -115 -116 -128 l-21 -23 -727 0 c-670 0 -726 1 -726 17 0 9 22 86 49 172 27 86 52 166 56 179 l6 22 840 0 c461 0 839 -3 839 -7z");

    private readonly PathF _eBody = ParsePath(
        "M2090 2231 c0 -7 -73 -94 -110 -131 -3 -3 -27 -32 -55 -65 -27 -33 -75 -87 -105 -120 l-55 -60 -349 -5 -350 -5 -21 -60 c-12 -33 -25 -72 -29 -87 l-6 -28 430 0 c476 0 447 4 416 -63 -14 -30 -91 -274 -103 -324 l-5 -23 -623 0 c-342 0 -635 -2 -651 -5 -65 -12 -64 -4 14 237 40 123 79 237 87 253 7 17 23 62 35 100 12 39 26 79 30 90 5 11 27 79 49 150 23 72 44 136 46 143 3 9 148 12 680 12 404 0 675 -4 675 -9z");

    private readonly PathF _p = ParsePath(
        "M3664 2220 c108 -54 139 -146 98 -286 -18 -61 -68 -204 -115 -330 -53 -143 -133 -236 -255 -296 l-67 -33 -474 -5 c-408 -4 -475 -7 -482 -20 -9 -19 -46 -137 -100 -320 -23 -80 -46 -148 -52 -151 -13 -9 -447 -12 -447 -3 0 10 50 174 72 234 37 101 95 274 128 380 18 58 47 142 63 188 l30 82 571 0 572 0 27 28 c15 15 27 32 27 36 0 4 7 23 16 41 8 18 14 46 12 62 l-3 28 -583 3 -583 2 12 22 c15 27 59 166 59 185 0 7 11 36 25 63 14 27 25 54 25 59 0 5 4 21 10 36 l10 26 677 -3 678 -3 49 -25z");

    // Matches the source <clipPath> rects (already in outer 410x356 space).
    private static readonly RectF ETopTarget = new(0, 0, 320, 356);
    private static readonly RectF EBodyTarget = new(0, 70, 240, 260);
    private static readonly RectF PTarget = new(145, 70, 255, 260);

    // 0..1 reveal progress for each glyph's outline trace and its fill flood.
    public float ETopOutline { get; set; }
    public float ETopFill { get; set; }
    public float EBodyOutline { get; set; }
    public float EBodyFill { get; set; }
    public float POutline { get; set; }
    public float PFill { get; set; }

    // Extra glow added briefly once the mark finishes assembling.
    public float GlowBoost { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var scale = Math.Min(dirtyRect.Width / ViewW, dirtyRect.Height / ViewH);
        var offsetX = dirtyRect.X + (dirtyRect.Width - ViewW * scale) / 2f;
        var offsetY = dirtyRect.Y + (dirtyRect.Height - ViewH * scale) / 2f;

        canvas.SaveState();
        canvas.Translate(offsetX, offsetY);
        canvas.Scale(scale, scale);

        DrawGlyph(canvas, _eTop, ETopTarget, growHorizontally: true, ETopOutline, ETopFill, White);
        DrawGlyph(canvas, _eBody, EBodyTarget, growHorizontally: false, EBodyOutline, EBodyFill, White);
        DrawGlyph(canvas, _p, PTarget, growHorizontally: true, POutline, PFill, Accent);

        canvas.RestoreState();
    }

    private void DrawGlyph(ICanvas canvas, PathF path, RectF target, bool growHorizontally,
        float outlineProgress, float fillProgress, Color color)
    {
        if (outlineProgress > 0)
        {
            var clip = GrowRect(target, growHorizontally, outlineProgress);
            canvas.SaveState();
            canvas.ClipRectangle(clip);
            DrawOutline(canvas, path, color);
            canvas.RestoreState();
        }

        if (fillProgress > 0)
        {
            var clip = GrowRect(target, growHorizontally, fillProgress);
            canvas.SaveState();
            canvas.ClipRectangle(clip);
            DrawFill(canvas, path, color);
            canvas.RestoreState();
        }
    }

    private static RectF GrowRect(RectF target, bool growHorizontally, float progress)
    {
        progress = Math.Clamp(progress, 0f, 1f);
        return growHorizontally
            ? new RectF(target.X, target.Y, target.Width * progress, target.Height)
            : new RectF(target.X, target.Y, target.Width, target.Height * progress);
    }

    // Soft halo (three widening, fading strokes) standing in for the source
    // SVG's Gaussian-blur glow filter, which MAUI's canvas has no equivalent for.
    private void DrawOutline(ICanvas canvas, PathF path, Color color)
    {
        canvas.SaveState();
        canvas.Translate(0, ViewH);
        canvas.Scale(0.1f, -0.1f);

        var haloBoost = 1f + GlowBoost;
        canvas.StrokeLineJoin = LineJoin.Round;
        canvas.StrokeLineCap = LineCap.Round;

        canvas.StrokeColor = color.WithAlpha(0.10f * haloBoost);
        canvas.StrokeSize = 34f * haloBoost;
        canvas.DrawPath(path);

        canvas.StrokeColor = color.WithAlpha(0.22f * haloBoost);
        canvas.StrokeSize = 18f * haloBoost;
        canvas.DrawPath(path);

        canvas.StrokeColor = color;
        canvas.StrokeSize = 7f;
        canvas.DrawPath(path);

        canvas.RestoreState();
    }

    private void DrawFill(ICanvas canvas, PathF path, Color color)
    {
        canvas.SaveState();
        canvas.Translate(0, ViewH);
        canvas.Scale(0.1f, -0.1f);

        canvas.FillColor = color;
        canvas.FillPath(path);

        canvas.RestoreState();
    }

    // Minimal parser for the M/L/C (+ lowercase relative forms) + Z commands
    // these three paths use - just enough of the SVG path grammar to
    // reproduce them exactly, without pulling in a full SVG library.
    private static PathF ParsePath(string d)
    {
        var tokens = Regex.Matches(d, @"[MmLlCcZz]|-?\d*\.?\d+(?:[eE][-+]?\d+)?")
            .Select(m => m.Value)
            .ToList();

        var path = new PathF();
        float curX = 0, curY = 0, startX = 0, startY = 0;
        char cmd = '\0';
        int i = 0;

        float Next() => float.Parse(tokens[i++], CultureInfo.InvariantCulture);

        while (i < tokens.Count)
        {
            var t = tokens[i];
            if (t.Length == 1 && "MmLlCcZz".Contains(t[0]))
            {
                cmd = t[0];
                i++;
                continue;
            }

            switch (cmd)
            {
                case 'M':
                    curX = Next(); curY = Next();
                    startX = curX; startY = curY;
                    path.MoveTo(curX, curY);
                    cmd = 'L';
                    break;
                case 'm':
                    curX += Next(); curY += Next();
                    startX = curX; startY = curY;
                    path.MoveTo(curX, curY);
                    cmd = 'l';
                    break;
                case 'L':
                    curX = Next(); curY = Next();
                    path.LineTo(curX, curY);
                    break;
                case 'l':
                    curX += Next(); curY += Next();
                    path.LineTo(curX, curY);
                    break;
                case 'C':
                    {
                        float x1 = Next(), y1 = Next(), x2 = Next(), y2 = Next(), x = Next(), y = Next();
                        path.CurveTo(x1, y1, x2, y2, x, y);
                        curX = x; curY = y;
                        break;
                    }
                case 'c':
                    {
                        float x1 = curX + Next(), y1 = curY + Next();
                        float x2 = curX + Next(), y2 = curY + Next();
                        float x = curX + Next(), y = curY + Next();
                        path.CurveTo(x1, y1, x2, y2, x, y);
                        curX = x; curY = y;
                        break;
                    }
                case 'Z':
                case 'z':
                    path.Close();
                    curX = startX; curY = startY;
                    break;
                default:
                    i++;
                    break;
            }
        }

        return path;
    }
}
