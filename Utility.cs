using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Paint;

static class Utility {
    // Get the position of the mouse, snapped to an integer grid, adjusted for scale and zoom
    public static Vector2i GetMousePos(Vector2i position, int scale, int zoom) {
        var MousePos = new Vector2i(
            (int)Math.Round((double)GetMouseX() / zoom / scale) * zoom * scale,
            (int)Math.Round((double)GetMouseY() / zoom / scale) * zoom * scale
        );
        var AdjustedPos = MousePos / zoom / scale;
        return AdjustedPos - (position / zoom / scale);
    }

    // Get the points along a congiuous line between two points
    public static List<Vector2i> GetLinePoints(Vector2i a, Vector2i b, int size) {
        List<Vector2i> points = new List<Vector2i>();

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                var start = new Vector2i((a.X - size / 2) + x, (a.Y - size / 2) + y);
                var end = new Vector2i((b.X - size / 2) + x, (b.Y - size / 2) + y);

                int dx = Math.Abs(end.X - start.X);
                int dy = Math.Abs(end.Y - start.Y);
                int sx = start.X < end.X ? 1 : -1;
                int sy = start.Y < end.Y ? 1 : -1;
                int err = dx - dy;

                while (true) {
                    points.Add(new Vector2i(start.X, start.Y));

                    if (start == end) break;

                    int e2 = 2 * err;

                    if (e2 > -dy) {
                        err -= dy;
                        start.X += sx;
                    }

                    if (e2 < dx) {
                        err += dx;
                        start.Y += sy;
                    }
                }
            }
        }

        return points;
    }
}