using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Paint;

class Brush {
    public Canvas Canvas { get; private set; }
    public int Scale { get; private set; }
    public Vector2i Position { get; set; }
    public Vector2i LastPosition { get; set; }

    public byte Size { get; private set; }

    public ByteColor PrimaryColor { get; set; }
    public ByteColor SecondaryColor { get; set; }


    public Brush(Canvas canvas) {
        Canvas = canvas;
        Scale = canvas.Scale;

        Size = 4;

        PrimaryColor = new ByteColor(0, 0, 0, 255);
        SecondaryColor = new ByteColor(255, 255, 255, 255);

        HideCursor();
    }

    public void Paint(int index) {
        var Color = index == 0 ? PrimaryColor : SecondaryColor;

        var LinePoints = Utility.GetLinePoints(Position, LastPosition, Size).Distinct().ToList();
        var PointCache = new List<Vector2i>();

        foreach (var Point in LinePoints) {
            if (PointCache.Contains(Point)) continue;
            PointCache.Add(Point);

		    Canvas.SetPixel(Point, Color);
        }
    }

    public void Resize(int amount) {
        Size = (byte)Math.Clamp(Size - amount, 1, 255);
    }

    public void Draw(int zoom) {
        int X = (Position.X - (Size / 2)) * Scale;
        int Y = (Position.Y - (Size / 2)) * Scale;
        DrawRectangleLines(X * zoom, Y * zoom, (Size * Scale) * zoom, (Size * Scale) * zoom, Color.BLACK);
    }
}