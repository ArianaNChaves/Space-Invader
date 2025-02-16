using System;
using System.IO;
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
        private static Vector2 _bulletFixedPosition = new Vector2(50, 20);
        private static float _bulletSpeed = 6 * FrameRateFix;
        private static bool _isPlayerBulletActive = false;

        // Enemy Settings
        //--------------------------------------------------------------------------------------
        private static Vector2 _enemiesMaxPosition = new Vector2(140, 950);
        private static bool _isGoingToRight = true;
        private static float _enemiesSpeed = 35 * FrameRateFix;
        private static float _enemyMoveTimer = 0f;
        private static float _enemyMoveInterval = 0.5f;
        private static int _spacing = 128;

        // Enemy Shooting Settings  <----------------------- ADDED ENEMY SHOOTING SETTINGS HERE
        //--------------------------------------------------------------------------------------
        private static Vector2 _enemyBulletPosition;
        private static float _enemyBulletSpeed = 4 * FrameRateFix; //Enemy bullet speed, can be different from player's
        private static bool _isEnemyBulletActive = false;
        private static float _enemyShootTimer = 0f;
        private static float _enemyShootInterval = 0.5f; // Time interval between enemy shooting attempts (in seconds)
        private static float _enemyShootProbability = 0.6f; // Probability of an enemy shooting during an interval (0.0 to 1.0)


        private struct Enemy
        {
            public bool isAlive;
            public Texture2D texture;
            public Vector2 position;
        }

        private static List<Enemy> allEnemiesAlive = new List<Enemy>();
        
        private const float MinXPosition = 40;
        //Enemy One (abajo)
        private static Vector2 _enemyOneInitialPosition = new Vector2(MinXPosition, 360);
        private static Vector2 _enemyOnePosition = _enemyOneInitialPosition;
        private static Enemy[] _enemies1 = new Enemy[5];
        //Enemy Two (medio)
        private static Vector2 _enemyTwoInitialPosition = new Vector2(MinXPosition, 250);
        private static Vector2 _enemyTwoPosition = _enemyTwoInitialPosition;
        private static Enemy[] _enemies2 = new Enemy[5];
        //Enemy Three (arriba)
        private static Vector2 _enemyThreeInitialPosition = new Vector2(MinXPosition, 140);
        private static Vector2 _enemyThreePosition = _enemyThreeInitialPosition;
        private static Enemy[] _enemies3 = new Enemy[5];


        // Boom Settings
        //--------------------------------------------------------------------------------------


        private static Random _random = new Random();


        public static void Main()
        {
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Space Invader - Final Exam");
            Raylib.SetTargetFPS(FPS);

            LoadTextures();
            InitializeEnemyPositions(_enemies1, _enemyOnePosition, _enemyOneTexture);
            InitializeEnemyPositions(_enemies2, _enemyTwoPosition, _enemyTwoTexture);
            InitializeEnemyPositions(_enemies3, _enemyThreePosition, _enemyThreeTexture);

            // Main game loop
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
                    //Enemy Shoot? <----------------------- ENEMY SHOOT CALL
                    EnemyShoot();
                    EnemyBulletHandler();
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

            Debug();

            Raylib.EndDrawing();
        }

        private static void Ui()
        {
            // Raylib.DrawText("Congrats! You created your first window!", 190, 200, 20, Color.White);
        }

        private static void Gameplay()
        {
            //Player NO TOCAR
            Raylib.DrawTexture(_playerTexture, (int)_playerPosition.X, (int)_playerPosition.Y, Color.White);
            DrawPlayerBullet();
            


            //Enemy ------------------------
            DrawEnemyPositions(_enemies1, _enemyOneTexture);
            DrawEnemyPositions(_enemies2, _enemyTwoTexture);
            DrawEnemyPositions(_enemies3, _enemyThreeTexture);
            DrawEnemyBullet();

            //Collisions ------------------------

            CheckEnemyBulletCollisionWithPlayer();

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

        private static void EnemyMovement(Enemy[] enemies)
        {
            float deltaTime = Raylib.GetFrameTime();

            if (_isGoingToRight)
            {
                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i].position.X += _enemiesSpeed * deltaTime;
                }

            }
            else
            {
                for (int i = enemies.Length - 1; i >= 0; i--)
                {
                    enemies[i].position.X -= _enemiesSpeed * deltaTime;
                }

            }

        }

        private static void DrawEnemyPositions(Enemy[] enemies, Texture2D enemyTexture)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (!enemies[i].isAlive) continue;

                Raylib.DrawTexture(enemies[i].texture, (int)enemies[i].position.X + _spacing, (int)enemies[i].position.Y, Color.White);
            }
        }

        private static void InitializeEnemyPositions(Enemy[] enemies, Vector2 initialEnemyPosition, Texture2D enemyTexture)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].texture = enemyTexture;
                enemies[i].isAlive = true;
                enemies[i].position = new Vector2(initialEnemyPosition.X + i * _spacing, initialEnemyPosition.Y);
            }
        }

        private static void MoveAllEnemies()
        {
            float deltaTime = Raylib.GetFrameTime();
            _enemyMoveTimer += deltaTime;

            //Determine boundaries BEFORE movement.
            float leftmostEnemy = _enemies1[0].position.X;
            float rightmostEnemy = _enemies1[_enemies1.Length - 1].position.X;

            foreach (var _ in _enemies2) //Find the limits using all arrays
            {
                leftmostEnemy = Math.Min(leftmostEnemy, _enemies1[0].position.X);
                rightmostEnemy = Math.Max(rightmostEnemy, _enemies2[_enemies2.Length - 1].position.X);
            }
            foreach (var _ in _enemies3) //Find the limits using all arrays
            {
                leftmostEnemy = Math.Min(leftmostEnemy, _enemies3[0].position.X);
                rightmostEnemy = Math.Max(rightmostEnemy, _enemies3[_enemies3.Length - 1].position.X);
            }

            bool isEnemyOutsideLeft = leftmostEnemy <= _enemiesMaxPosition.X;
            bool isEnemyOutsideRight = rightmostEnemy >= _enemiesMaxPosition.Y;

            if (isEnemyOutsideRight)
            {
                _isGoingToRight = false;
            }
            if (isEnemyOutsideLeft)
            {
                _isGoingToRight = true;
            }
            //Now move enemies only after the direction has been determined.
            if (_enemyMoveTimer >= _enemyMoveInterval)
            {
                _enemyMoveTimer = 0f;
                EnemyMovement(_enemies1);
                EnemyMovement(_enemies2);
                EnemyMovement(_enemies3);

            }
        }

        private static void Debug()
        {
            Rectangle playerBounds = new Rectangle(_playerPosition.X, _playerPosition.Y, _playerTexture.Width, _playerTexture.Height);
            Raylib.DrawRectangleLines((int)playerBounds.X, (int)playerBounds.Y, (int)playerBounds.Width, (int)playerBounds.Height, Color.Red);
            
            
            for (int i = 0; i < _enemies1.Length; i++)
            {
                if (!(_enemies1[i].isAlive)) continue;
                Rectangle enemy1Bounds = new Rectangle(_enemies1[i].position.X + _spacing, _enemies1[i].position.Y, _enemies1[i].texture.Width, _enemies1[i].texture.Height);
                Raylib.DrawRectangleLines((int)enemy1Bounds.X, (int)enemy1Bounds.Y, (int)enemy1Bounds.Width, (int)enemy1Bounds.Height, Color.Red);

            }
            for (int i = 0; i < _enemies2.Length; i++)
            {
                if (!(_enemies2[i].isAlive)) continue;

                Rectangle enemy2Bounds = new Rectangle(_enemies2[i].position.X + _spacing, _enemies2[i].position.Y, _enemies2[i].texture.Width, _enemies2[i].texture.Height);
                Raylib.DrawRectangleLines((int)enemy2Bounds.X, (int)enemy2Bounds.Y, (int)enemy2Bounds.Width, (int)enemy2Bounds.Height, Color.Red);

            }
            for (int i = 0; i < _enemies3.Length; i++)
            {
                if (!(_enemies3[i].isAlive)) continue;
                Rectangle enemy3Bounds = new Rectangle(_enemies3[i].position.X + _spacing, _enemies3[i].position.Y, _enemies3[i].texture.Width, _enemies3[i].texture.Height);
                Raylib.DrawRectangleLines((int)enemy3Bounds.X, (int)enemy3Bounds.Y, (int)enemy3Bounds.Width, (int)enemy3Bounds.Height, Color.Red);
            }
            
            Rectangle playerBulletBounds = new Rectangle(_playerBulletPosition.X, _playerBulletPosition.Y, _bulletTexture.Width, _bulletTexture.Height);
            Raylib.DrawRectangleLines((int)playerBulletBounds.X, (int)playerBulletBounds.Y, (int)playerBulletBounds.Width, (int)playerBulletBounds.Height, Color.Red);
            
            Rectangle enemyBulletBounds = new Rectangle(_enemyBulletPosition.X, _enemyBulletPosition.Y, _bulletTexture.Width, _bulletTexture.Height);
            Raylib.DrawRectangleLines((int)enemyBulletBounds.X, (int)enemyBulletBounds.Y, (int)enemyBulletBounds.Width, (int)enemyBulletBounds.Height, Color.Red);

        }

        //----------------------------------------------------------------------------------
        // ENEMY SHOOTING IMPLEMENTATION
        //----------------------------------------------------------------------------------

        private static void EnemyShoot()
        {
            if (_isEnemyBulletActive) return; // Only shoot if no bullet is active

            _enemyShootTimer += Raylib.GetFrameTime();

            if (_enemyShootTimer >= _enemyShootInterval)
            {
                _enemyShootTimer = 0f;
                if (_random.NextSingle() < _enemyShootProbability) //Probability check
                {
                    ShootRandomEnemy();
                }
            }
        }

        private static void ShootRandomEnemy()
        {
            List<Enemy> allAliveEnemies = new List<Enemy>();
            AddAliveEnemiesToList(allAliveEnemies, _enemies1);
            AddAliveEnemiesToList(allAliveEnemies, _enemies2);
            AddAliveEnemiesToList(allAliveEnemies, _enemies3);

            if (allAliveEnemies.Count > 0)
            {
                int randomIndex = _random.Next(allAliveEnemies.Count);
                Enemy shootingEnemy = allAliveEnemies[randomIndex];

                _enemyBulletPosition = shootingEnemy.position;
                _enemyBulletPosition.X += shootingEnemy.texture.Width / 2 + _spacing; // Center bullet on enemy X
                _enemyBulletPosition.Y += shootingEnemy.texture.Height; // Spawn bullet at bottom of enemy
                _isEnemyBulletActive = true;
            }
        }

        private static void AddAliveEnemiesToList(List<Enemy> list, Enemy[] enemies)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.isAlive)
                {
                    list.Add(enemy);
                }
            }
        }


        private static void EnemyBulletHandler()
        {
            if (_isEnemyBulletActive)
            {
                float deltaTime = Raylib.GetFrameTime();
                _enemyBulletPosition.Y += _enemyBulletSpeed * deltaTime; // Move bullet down

                if (_enemyBulletPosition.Y > ScreenHeight) //Bullet reached bottom of screen
                {
                    _isEnemyBulletActive = false; //Deactivate bullet
                }
            }
        }

        private static void DrawEnemyBullet()
        {
            if (_isEnemyBulletActive)
            {
                Raylib.DrawTexture(_bulletTexture, (int)_enemyBulletPosition.X, (int)_enemyBulletPosition.Y, Color.White);
            }
        }
        
        
        //----------------------------------------------------------------------------------
        // COLLISION IMPLEMENTATION
        //----------------------------------------------------------------------------------
        
        private static void CheckEnemyBulletCollisionWithPlayer()
        {
            if (!_isEnemyBulletActive) return;

            Rectangle enemyBulletCollision = new Rectangle(_enemyBulletPosition.X, _enemyBulletPosition.Y, _bulletTexture.Width, _bulletTexture.Height);
            Rectangle playerCollision = new Rectangle(_playerPosition.X, _playerPosition.Y, _playerTexture.Width, _playerTexture.Height);

            if (Raylib.CheckCollisionRecs(enemyBulletCollision, playerCollision))
            {
              Raylib.DrawText("Player baleado!", 20,20,50,Color.White);  
            }
        }

        private static void AllEnemies()
        {
            foreach (var enemy in _enemies1)
            {
                allEnemiesAlive.Add(enemy);
            }
            foreach (var enemy in _enemies2)
            {
                allEnemiesAlive.Add(enemy);
            }
            foreach (var enemy in _enemies3)
            {
                allEnemiesAlive.Add(enemy);
            }
        }
        
        private static void CheckPlayerBulletCollisionWithEnemy()
        {
            if (!_isPlayerBulletActive) return;
            bool isBulletHit = false;
            int enemyHitted;

            Rectangle playerBulletCollision = new Rectangle(_playerBulletPosition.X, _playerBulletPosition.Y, _bulletTexture.Width, _bulletTexture.Height);
            
            
            foreach (var enemy in _enemies1)
            {
                Rectangle enemyCollision = new Rectangle(enemy.position.X, enemy.position.Y, enemy.texture.Width, enemy.texture.Height);
                if (Raylib.CheckCollisionRecs(playerBulletCollision, enemyCollision))
                {
                    Raylib.DrawText("Enemigo baleado!", 20,50,50,Color.Red);
                    _isPlayerBulletActive = false;
                    enemy.isAlive = false;
                }
            }

            // for (int i = 0; i < allEnemiesAlive.Count; i++)
            // {
            //     Rectangle enemyCollision = new Rectangle(allEnemiesAlive[i].position.X, allEnemiesAlive[i].position.Y, allEnemiesAlive[i].texture.Width, allEnemiesAlive[i].texture.Height);
            //     if (Raylib.CheckCollisionRecs(playerBulletCollision, enemyCollision))
            //     {
            //         Raylib.DrawText("Enemigo baleado!", 20,50,50,Color.Red);
            //         _isPlayerBulletActive = false;
            //         isBulletHit = true;
            //         enemyHitted = i;
            //         allEnemiesAlive[enemyHitted].isAlive = false;
            //     }
            // }
            

        }
    }
}