using System.Numerics;
using Raylib_cs;

namespace MyApp
{
    class Program
    {
        // Common Settings
        //--------------------------------------------------------------------------------------
        private const int ScreenWidth = 1280;
        private const int ScreenHeight = 720;
        private const int TittleFontSize = 130;
        private const int SubtittleFontSize = 80;
        private const int CommonFontSize = 50;
        private const int FPS = 60;
        private const int FrameRateFix = 100;
        
        // Textures
        //--------------------------------------------------------------------------------------
        private static readonly string PlayerImagePath = Path.Combine(Environment.CurrentDirectory, @"resources\Player.png");
        private static readonly string EnemyOneImagePath = Path.Combine(Environment.CurrentDirectory, @"resources/EnemyOne.png");
        private static readonly string EnemyTwoImagePath = Path.Combine(Environment.CurrentDirectory, @"resources/EnemyTwo.png");
        private static readonly string EnemyThreeImagePath = Path.Combine(Environment.CurrentDirectory, @"resources/EnemyThree.png");
        private static readonly string BulletImagePath = Path.Combine(Environment.CurrentDirectory, @"resources/Bullet.png");
        private static readonly string BoomImagePath = Path.Combine(Environment.CurrentDirectory, @"resources/Boom.png");
        private static Texture2D _playerTexture;
        private static Texture2D _enemyOneTexture;
        private static Texture2D _enemyTwoTexture;
        private static Texture2D _enemyThreeTexture;
        private static Texture2D _bulletTexture;
        private static Texture2D _boomTexture;
        
        // Common Settings
        //--------------------------------------------------------------------------------------
        private static bool _isGameOver = false;
        
        // Player Settings
        //--------------------------------------------------------------------------------------
        private static Vector2 _playerInitialPosition = new Vector2(550, 610);
        private static Vector2 _playerPosition = _playerInitialPosition;
        private static Vector2 _playerMaxPosition = new Vector2(200, 1100);
        private static float _playerSpeed = 6 * FrameRateFix;
        
        // Player Bullet Settings
        //--------------------------------------------------------------------------------------
        private static Vector2 _playerBulletPosition;
        private static Vector2 _bulletFixedPosition = new Vector2(57, 20);
        private static float _bulletSpeed = 6 * FrameRateFix;
        private static bool _isPlayerBulletActive = false;

        // Enemy Settings
        //--------------------------------------------------------------------------------------
        private static Vector2 _enemiesMaxPosition = new Vector2(210, 1110);
        private static bool _isGoingToRight = true;
        private static float _enemiesSpeed = 10 * FrameRateFix;
        private static Vector2 _enemyOneInitialPosition = new Vector2(210, 350);
        private static Vector2 _enemyOnePosition = _enemyOneInitialPosition;

        
        // Boom Settings
        //--------------------------------------------------------------------------------------
        
        
        private static Random _random = new Random();

        public static void Main()
        {
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Space Invader - Final Exam");
            Raylib.SetTargetFPS(FPS);
            
            LoadTextures();
            while (!Raylib.WindowShouldClose())
            {
                // Update
                //----------------------------------------------------------------------------------
                if (!_isGameOver)
                {
                    //Enemy movement Handler
                    EnemyMovement();
                    //Player movement Handler
                    PlayerMovement();
                    //Enemy Shoot?
                    //Player Shoot?
                    PlayerShoot();
                    PlayerBulletHandler();
                }
                //----------------------------------------------------------------------------------
                Draw();
            }

            // De-Initialization
            //--------------------------------------------------------------------------------------
            Raylib.CloseWindow();
            //--------------------------------------------------------------------------------------

        }
        
        private static void LoadTextures()
        {
            //Player
            _playerTexture = Raylib.LoadTexture(PlayerImagePath);
            
            //Enemies
            _enemyOneTexture = Raylib.LoadTexture(EnemyOneImagePath);
            _enemyTwoTexture = Raylib.LoadTexture(EnemyTwoImagePath);
            _enemyThreeTexture = Raylib.LoadTexture(EnemyThreeImagePath);
            
            //Bullet
            _bulletTexture = Raylib.LoadTexture(BulletImagePath);

            //Boom
            _boomTexture = Raylib.LoadTexture(BoomImagePath);
        }
        
        private static void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Blue);
            
            //Hacer un enum con las diferentes escenas o pantallas y luego hacer un switch para dibujar la pantalla que queres ver
            
            Gameplay();
            
            // UI
            //--------------------------------------------------------------------------------------
            Ui();
            
            Raylib.EndDrawing();
        }

        private static void Ui()
        {
            // Raylib.DrawText("Congrats! You created your first window!", 190, 200, 20, Color.White);  
        }

        private static void Gameplay()
        {
            //Player
            Raylib.DrawTexture(_playerTexture, (int)_playerPosition.X, (int)_playerPosition.Y, Color.White);
            DrawPlayerBullet();
            
            //Primer enemigo
            
            Raylib.DrawTexture(_enemyOneTexture, (int)_enemyOnePosition.X, (int)_enemyOnePosition.Y, Color.White); //Esto tiene que ser un array


        }

        private static void PlayerMovement()
        {
            float deltaTime = Raylib.GetFrameTime();
            bool isPlayerOutsideLeft = _playerPosition.X <= _playerMaxPosition.X;
            bool isPlayerOutsideRight = _playerPosition.X >= _playerMaxPosition.Y;
            if (Raylib.IsKeyDown(KeyboardKey.D) && !isPlayerOutsideRight)
            {
                _playerPosition.X += _playerSpeed * deltaTime;
            }
            if (Raylib.IsKeyDown(KeyboardKey.A) && !isPlayerOutsideLeft)
            {
                _playerPosition.X -= _playerSpeed * deltaTime;
            }
        }

        private static void PlayerShoot()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Space) && !_isPlayerBulletActive)
            {
                _playerBulletPosition = _playerPosition;
                _playerBulletPosition.X += _bulletFixedPosition.X;
                _playerBulletPosition.Y -= _bulletFixedPosition.Y;
                _isPlayerBulletActive = true;
            }
        }

        private static void PlayerBulletHandler()
        {
            float deltaTime = Raylib.GetFrameTime();
            bool isBulletOutsideTop = _playerBulletPosition.Y <= 0;
            if (_isPlayerBulletActive)
            {
                _playerBulletPosition.Y -= _bulletSpeed * deltaTime;
            }
            if (isBulletOutsideTop)
            {
                _isPlayerBulletActive = false;
            }
        }

        private static void DrawPlayerBullet()
        {
            if (_isPlayerBulletActive)
            {
                Raylib.DrawTexture(_bulletTexture, (int)_playerBulletPosition.X, (int)_playerBulletPosition.Y, Color.White);
            }
        }

        private static void EnemyMovement()
        {
            float deltaTime = Raylib.GetFrameTime();
            bool isEnemyOutsideLeft = _enemyOnePosition.X <= _enemiesMaxPosition.X;
            bool isEnemyOutsideRight = _enemyOnePosition.X >= _enemiesMaxPosition.Y;
            if (!isEnemyOutsideRight && _isGoingToRight)
            {
                _enemyOnePosition.X += _enemiesSpeed * deltaTime;
            }
            if (!isEnemyOutsideLeft && !_isGoingToRight)
            {
                _enemyOnePosition.X -= _enemiesSpeed * deltaTime;
            }
            
            if (isEnemyOutsideRight)
            {
                _isGoingToRight = false;
            }
            
            if (isEnemyOutsideLeft)
            {
                _isGoingToRight = true;
            }
            
        }
        
        
        private static void Debug()
        {
           
            
        }
    }
}