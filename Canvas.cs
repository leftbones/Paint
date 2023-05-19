using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Paint;

class Canvas {
    public Vector2i WindowSize { get; private set; }
    public Vector2i Size { get; private set; }
    public Vector2i Position { get; private set; }
    public int Scale { get; private set; }
    public int Zoom { get; private set;  }

    public Brush Brush { get; private set; }
    public ByteColor[,] Matrix { get; private set; }

    public int BackgroundGridSize { get; private set; }
    public Tuple<ByteColor, ByteColor> BackgroundColors { get; private set; }

    private Texture2D BackgroundTexture;

    private Image MatrixImage;
    private Texture2D MatrixTexture;

    private ByteColor Transparent = new ByteColor();

    public Canvas(Vector2i window_size, Vector2i canvas_size) {
        WindowSize = window_size;
        Size = canvas_size;
        Scale = 2;
        Zoom = 1;

        Brush = new Brush(this);
        Matrix = new ByteColor[Size.X, Size.Y];
        for (int x = 0; x < Size.X - 1; x++) {
            for (int y = 0; y < Size.Y - 1; y++) {
                var Pos = new Vector2i(x, y);
                SetPixel(Pos, new ByteColor());
            }
        }

        BackgroundGridSize = 8;
        BackgroundColors = new Tuple<ByteColor, ByteColor>(new ByteColor(200, 200, 200), new ByteColor(150, 150, 150));
        BackgroundTexture = GenerateBackgroundTexture();

        MatrixImage = GenImageColor(Size.X, Size.Y, Color.WHITE);
        MatrixTexture = LoadTextureFromImage(MatrixImage);
    }

    // Set a pixel in the canvas to the given color
    public void SetPixel(Vector2i pos, ByteColor color) {
        if (InBounds(pos))
            Matrix[pos.X, pos.Y] = color;
    }

    public void AdjustZoom(int amount) {
        Zoom = Math.Clamp(Zoom + amount, 1, 16);
    }

    public void MoveViewport(int x, int y) {
        Position = new Vector2i(Position.X + x, Position.Y + y);
    }

    // Get a pixel from the canvas
    public ByteColor GetPixel(Vector2i pos) {
        return Matrix[pos.X, pos.Y];
    }

    // Generate the background "transparent" texture
    public unsafe Texture2D GenerateBackgroundTexture() {
        Image Buffer = GenImageColor(Size.X / BackgroundGridSize, Size.Y / BackgroundGridSize, Color.BLACK);
        Texture2D Texture = LoadTextureFromImage(Buffer);

        for (int x = 0; x < Size.X; x++) {
            for (int y = 0; y < Size.Y; y++) {
                Color C = (x + y) % 2 == 0 ? BackgroundColors.Item1.ToRL() : BackgroundColors.Item2.ToRL();
                ImageDrawPixel(ref Buffer, x, y, C);
            }
        }

        UpdateTexture(Texture, Buffer.data);
        return Texture;
    }

    // Handle user input
    public void Input() {
        // Update Brush Position
        Brush.LastPosition = Brush.Position;
        Brush.Position = Utility.GetMousePos(Position, Scale, Zoom);

        // Adjust Brush Size
        int MouseWheel = (int)GetMouseWheelMove();
        Brush.Resize(MouseWheel);

        // Brush Painting
        if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
            Brush.Paint(0);

        else if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT))
            Brush.Paint(1);

        // Scale Up/Down
        if (IsKeyPressed(KeyboardKey.KEY_UP))
            AdjustZoom(1);

        if (IsKeyPressed(KeyboardKey.KEY_DOWN))
            AdjustZoom(-1);

        // Move "Viewport"
        if (IsKeyDown(KeyboardKey.KEY_W)) MoveViewport(0, -1);
        if (IsKeyDown(KeyboardKey.KEY_S)) MoveViewport(0, 1);
        if (IsKeyDown(KeyboardKey.KEY_A)) MoveViewport(-1, 0);
        if (IsKeyDown(KeyboardKey.KEY_D)) MoveViewport(1, 0);
    }

    // Update all canvas elements
    public void Update() {

    }

    // Draw all canvas elements
    public unsafe void Draw() {
        // Background
        DrawTexturePro(BackgroundTexture, new Rectangle(0, 0, BackgroundTexture.width, BackgroundTexture.height), new Rectangle(Position.X, Position.Y, (Size.X * Scale) * Zoom, (Size.Y * Scale) * Zoom), Vector2.Zero, 0, Color.WHITE);

        // Matrix
        ImageClearBackground(ref MatrixImage, Color.MAGENTA);

        for (int x = 0; x < Size.X; x++) {
            for (int y = 0; y < Size.Y; y++) {
                var Pos = new Vector2i(x, y);
                var C = GetPixel(Pos);
				ImageDrawPixel(ref MatrixImage, x, y, C.ToRL());
            }
        }

        UpdateTexture(MatrixTexture, MatrixImage.data);

        DrawTexturePro(MatrixTexture, new Rectangle(0, 0, Size.X, Size.Y), new Rectangle(Position.X, Position.Y, (Size.X * Scale) * Zoom, (Size.Y * Scale) * Zoom), Vector2.Zero, 0, Color.WHITE);

        // Brush
	    Brush.Draw(Position, Zoom, Scale);

        // HUD
        DrawText($"Zoom: {Zoom}", 6, 6, 20, Color.BLACK);
        DrawText($"Zoom: {Zoom}", 5, 5, 20, Color.RED);
    }

    // Check if a position is in the bounds of the canvas
    public bool InBounds(Vector2i pos) {
        return pos.X >= 0 && pos.X < Size.X && pos.Y >= 0 && pos.Y < Size.Y;
    }
}