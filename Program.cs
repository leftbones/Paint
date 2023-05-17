using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Paint;

class Program {
    static void Main(string[] args) {
        ////
        // Init
        var WindowSize = new Vector2i(1200, 800);
        var CanvasSize = new Vector2i(600, 400);
        var OutsideCanvasColor = new ByteColor(100, 100, 100);
        InitWindow(WindowSize.X, WindowSize.Y, "Paint");

        ////
        // Setup
        var Canvas = new Canvas(WindowSize, CanvasSize);


        ////
        // Main Loop
        while (!WindowShouldClose()) {
            // Input
            Canvas.Input();

            // Update
            Canvas.Update();

            // Draw
            BeginDrawing();
            ClearBackground(OutsideCanvasColor.ToRL());
            Canvas.Draw();
            EndDrawing();
        }

        ////
        // Exit
        CloseWindow();
    }
}
