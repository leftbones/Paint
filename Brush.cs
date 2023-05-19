using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Paint;

class Brush {
    public Canvas Canvas { get; private set; }
    public Vector2i Position { get; set; }
    public Vector2i LastPosition { get; set; }

    public byte Size { get; private set; }

    public ByteColor PrimaryColor { get; set; }
    public ByteColor SecondaryColor { get; set; }


    public Brush(Canvas canvas) {
        Canvas = canvas;

        Size = 3;

        PrimaryColor = new ByteColor(0, 0, 0, 255);
        SecondaryColor = new ByteColor(255, 255, 255, 255);

        //HideCursor();
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

    // Grid snap: Round(Pos.X / Width) * Width
    public void Draw(Vector2i position, int zoom, int scale) {
        var MousePos = Utility.GetMousePos(position, scale, zoom);

        var BrushPos = new Vector2i(
            (int)Math.Round((double)MousePos.X - Size / 2) * scale * zoom,
            (int)Math.Round((double)MousePos.Y - Size / 2) * scale * zoom
        );

        int S = Size * scale * zoom;
        DrawRectangleLines(BrushPos.X, BrushPos.Y, S, S, Color.BLACK);
    }
}