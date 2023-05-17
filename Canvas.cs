using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Paint;

class Canvas {
    public Vector2i WindowSize { get; private set; }
    public Vector2i Size { get; private set; }
    public int Scale { get; private set; } // TODO: Change name to 'Zoom'(?)

    public Brush Brush { get; private set; }
    public ByteColor[,] Matrix { get; private set; }

    public Tuple<ByteColor, ByteColor> BackgroundColors { get; private set; }
    public int BackgroundGridSize { get; private set; }

    private Texture2D BackgroundTexture;

    private Image MatrixImage;
    private Texture2D MatrixTexture;

    private ByteColor Transparent = new ByteColor();

    public Canvas(Vector2i window_size, Vector2i canvas_size) {
        WindowSize = window_size;
        Size = canvas_size;
        Scale = 1;

        Brush = new Brush(this);
        Matrix = new ByteColor[Size.X, Size.Y];
        for (int x = 0; x < Size.X - 1; x++) {
            for (int y = 0; y < Size.Y - 1; y++) {
                var Pos = new Vector2i(x, y);
                SetPixel(Pos, new ByteColor());
            }
        }

        BackgroundColors = new Tuple<ByteColor, ByteColor>(new ByteColor(200, 200, 200), new ByteColor(150, 150, 150));
        BackgroundGridSize = 16;
        BackgroundTexture = GenerateBackgroundTexture();

        MatrixImage = GenImageColor(Size.X / Scale, Size.Y / Scale, Color.WHITE);
        MatrixTexture = LoadTextureFromImage(MatrixImage);
    }

    // Set a pixel in the canvas to the given color
    public void SetPixel(Vector2i pos, ByteColor color) {
        if (InBounds(pos))
            Matrix[pos.X, pos.Y] = color;
    }

    public void AdjustScale(int amount) {
        Scale = Math.Clamp(Scale + amount, 1, 4);
    }

    // Get a pixel from the canvas
    public ByteColor GetPixel(Vector2i pos) {
        return Matrix[pos.X, pos.Y];
    }

    // Generate the background "transparent" texture
    public unsafe Texture2D GenerateBackgroundTexture() {
        Image Buffer = GenImageColor(((Size.X / BackgroundGridSize) / Scale) + BackgroundGridSize, ((Size.Y / BackgroundGridSize) / Scale) + BackgroundGridSize, Color.BLACK);
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
        Brush.Position = Utility.GetMousePos(Scale);

        // Adjust Brush Size
        int MouseWheel = (int)GetMouseWheelMove();
        Brush.Resize(MouseWheel);

        // Brush Painting
        if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
            Brush.Paint();

        // Scale Up/Down
        if (IsKeyPressed(KeyboardKey.KEY_UP))
            AdjustScale(1);

        if (IsKeyPressed(KeyboardKey.KEY_DOWN))
            AdjustScale(-1);
    }

    // Update all canvas elements
    public void Update() {

    }

    // Draw all canvas elements
    public unsafe void Draw() {
        // Background
        //DrawTexturePro(BackgroundTexture, new Rectangle(0, 0, (Size.X / BackgroundGridSize) / Scale, (Size.Y / BackgroundGridSize) / Scale), new Rectangle(0, 0, (BackgroundTexture.width * BackgroundGridSize) * Scale, (BackgroundTexture.height * BackgroundGridSize) * Scale), Vector2.Zero, 0, Color.WHITE);
        DrawTexturePro(BackgroundTexture, new Rectangle(0, 0, (Size.X / BackgroundGridSize) / Scale, (Size.Y / BackgroundGridSize) / Scale), new Rectangle(0, 0, Size.X * Scale, Size.Y * Scale), Vector2.Zero, 0, Color.WHITE);

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

        DrawTexturePro(MatrixTexture, new Rectangle(0, 0, Size.X, Size.Y), new Rectangle(0, 0, Size.X * Scale, Size.Y * Scale), Vector2.Zero, 0, Color.WHITE);

        // Brush
        Brush.Draw();
    }

    // Check if a position is in the bounds of the canvas
    public bool InBounds(Vector2i pos) {
        return pos.X >= 0 && pos.X < Size.X && pos.Y >= 0 && pos.Y < Size.Y;
    }
}