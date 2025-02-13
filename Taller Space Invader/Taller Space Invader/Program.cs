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
        private static Vector2 _enemiesMaxPosition = new Vector2(140, 870);
        private static bool _isGoingToRight = true;
        private static float _enemiesSpeed = 35 * FrameRateFix;
        private static float _enemyMoveTimer = 0f;
        private static float _enemyMoveInterval = 0.5f;
            //Enemy One
        private static Vector2 _enemyOneInitialPosition = new Vector2(80, 350);
        private static Vector2 _enemyOnePosition = _enemyOneInitialPosition;
        private static Vector2[] _enemyOnePositionsArray = new Vector2[5];
            //Enemy Two
        private static Vector2 _enemyTwoInitialPosition = new Vector2(80, 250);
        private static Vector2 _enemyTwoPosition = _enemyTwoInitialPosition;
        private static Vector2[] _enemyTwoPositionsArray = new Vector2[5];
            //Enemy Three
        private static Vector2 _enemyThreeInitialPosition = new Vector2(80, 150);
        private static Vector2 _enemyThreePosition = _enemyThreeInitialPosition;
        private static Vector2[] _enemyThreePositionsArray = new Vector2[5];

        
        // Boom Settings
        //--------------------------------------------------------------------------------------
        
        
        private static Random _random = new Random();
        

        public static void Main()
        {
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Space Invader - Final Exam");
            Raylib.SetTargetFPS(FPS);
            
            LoadTextures();
            InitializeEnemyPositions(_enemyOnePositionsArray, _enemyOnePosition, _enemyOneTexture);
            InitializeEnemyPositions(_enemyTwoPositionsArray, _enemyTwoPosition, _enemyTwoTexture);
            InitializeEnemyPositions(_enemyThreePositionsArray, _enemyThreePosition, _enemyThreeTexture);
            while (!Raylib.WindowShouldClose())
            {
                // Update
                //----------------------------------------------------------------------------------
                if (!_isGameOver)
                {
                    //Enemy movement Handler
                    MoveAllEnemies();

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
            
            DrawEnemyPositions(_enemyOnePositionsArray, _enemyOneTexture);
            DrawEnemyPositions(_enemyTwoPositionsArray, _enemyTwoTexture);
            DrawEnemyPositions(_enemyThreePositionsArray, _enemyThreeTexture);

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

        private static void EnemyMovement(Vector2[] enemyPosition)
        {
            float deltaTime = Raylib.GetFrameTime();
            bool isEnemyOutsideLeft = enemyPosition[0].X <= _enemiesMaxPosition.X;
            bool isEnemyOutsideRight = enemyPosition[enemyPosition.Length - 1].X >= _enemiesMaxPosition.Y;
            
            if (_isGoingToRight)
            {
                for (int i = 0; i < enemyPosition.Length; i++)

                {
                    enemyPosition[i].X += _enemiesSpeed * deltaTime;
                }
                if (isEnemyOutsideRight)
                {
                    _isGoingToRight = false;
                }
            }
            else
            {
                for (int i = enemyPosition.Length - 1; i >= 0; i--)

                {
                    enemyPosition[i].X -= _enemiesSpeed * deltaTime;
                }
                if (isEnemyOutsideLeft)
                {
                    _isGoingToRight = true;
                }
            }
            
        }
        
        private static void DrawEnemyPositions(Vector2[] enemyPosition, Texture2D enemyTexture)
        {
            int spacing = enemyTexture.Width + 10;
            for (int i = 0; i < enemyPosition.Length; i++)
            {
                Raylib.DrawTexture(enemyTexture, (int)enemyPosition[i].X + spacing, (int)enemyPosition[i].Y, Color.White);
            }
        }
        
        private static void InitializeEnemyPositions(Vector2[] enemyPosition, Vector2 initialEnemyPosition, Texture2D enemyTexture)
        {
            int spacing = enemyTexture.Width + 10;
            for (int i = 0; i < enemyPosition.Length; i++)
            {
                enemyPosition[i] = new Vector2(initialEnemyPosition.X + i * spacing, initialEnemyPosition.Y);
            }
        }

        private static void MoveAllEnemies()
        {
            float deltaTime = Raylib.GetFrameTime();
            _enemyMoveTimer += deltaTime;

            if (_enemyMoveTimer >= _enemyMoveInterval)
            {
                _enemyMoveTimer = 0f;
                EnemyMovement(_enemyOnePositionsArray);
                EnemyMovement(_enemyTwoPositionsArray);
                EnemyMovement(_enemyThreePositionsArray);

            }
        }
        private static void Debug()
        {
           
            
        }
    }
}