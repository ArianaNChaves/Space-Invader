using Raylib_cs;

namespace MyApp
{
    class Program
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        public static void Main()
        {
            Raylib.InitWindow(screenWidth, screenHeight, "raylib [core] example - basic window");

            Raylib.SetTargetFPS(60);               // Set our game to run at 60 frames-per-second
            //--------------------------------------------------------------------------------------

            // Main game loop
            while (!Raylib.WindowShouldClose())    // Detect window close button or ESC key
            {
                // Update
                //----------------------------------------------------------------------------------
                // TODO: Update your variables here
                //----------------------------------------------------------------------------------

                // Draw
                //----------------------------------------------------------------------------------
                Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.Gray);

                Raylib.DrawText("Congrats! You created your first window!", 190, 200, 20, Color.White);

                Raylib.EndDrawing();
                //----------------------------------------------------------------------------------
            }

            // De-Initialization
            //--------------------------------------------------------------------------------------
            Raylib.CloseWindow();        // Close window and OpenGL context
            //--------------------------------------------------------------------------------------

        }
        
    }
}