using System;
using System.IO;
using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;

namespace MyApp
{
    class Program
    {
        // Common Settings
        //--------------------------------------------------------------------------------------
        private const int ScreenWidth = 1280;
        private const int ScreenHeight = 720;
        private const int TitleFontSize = 120;
        private const int SubtitleFontSize = 80;
        private const int CommonFontSize = 40;
        private const int FPS = 60;
        private const int FrameRateFix = 100;

        // Textures
        //--------------------------------------------------------------------------------------
        private static readonly string PlayerImagePath = Path.Combine(Environment.CurrentDirectory, @"resources\Player.png");
        private static readonly string EnemyOneImagePath = Path.Combine(Environment.CurrentDirectory, @"resources/EnemyOne.png");
        private static readonly string EnemyTwoImagePath = Path.Combine(Environment.CurrentDirectory, @"resources/EnemyTwo.png");
        private static readonly string EnemyThreeImagePath = Path.Combine(Environment.CurrentDirectory, @"resources/EnemyThree.png");
        private static readonly string BulletImagePath = Path.Combine(Environment.CurrentDirectory, @"resources/Bullet.png");
        private static Texture2D _playerTexture;
        private static Texture2D _enemyOneTexture;
        private static Texture2D _enemyTwoTexture;
        private static Texture2D _enemyThreeTexture;
        private static Texture2D _bulletTexture;

        // Common Settings
        //--------------------------------------------------------------------------------------
        private static bool _isGameplayRunning = false;

        // Player Settings
        //--------------------------------------------------------------------------------------
        private static Vector2 _playerInitialPosition = new Vector2(550, 610);
        private static Vector2 _playerPosition = _playerInitialPosition;
        private static Vector2 _playerMaxPosition = new Vector2(200, 1100);
        private static float _playerSpeed = 6 * FrameRateFix;
        private static Rectangle _playerCollisionRectangle;
        private static int _playerLives = 3;

        // Player Bullet Settings
        //--------------------------------------------------------------------------------------
        private static Vector2 _playerBulletPosition;
        private static Vector2 _bulletFixedPosition = new Vector2(50, 20);
        private static float _bulletSpeed = 6 * FrameRateFix;
        private static bool _isPlayerBulletActive = false;
        private static Rectangle _playerBulletCollisionRectangle;


        // Enemy Settings
        //--------------------------------------------------------------------------------------
        private static bool _isGameOver = false;

        private static Vector2 _enemiesMaxPosition = new Vector2(140, 950);
        private static bool _isGoingToRight = true;
        private static float _enemiesSpeed = 35 * FrameRateFix;
        private static float _enemyMoveTimer = 0f;
        private static float _enemyMoveInterval = 0.5f;
        private static int _spacing = 128;

        // Enemy Bullet Settings 
        //--------------------------------------------------------------------------------------
        private static Vector2 _enemyBulletPosition;
        private static float _enemyBulletSpeed = 4 * FrameRateFix;
        private static bool _isEnemyBulletActive = false;
        private static float _enemyShootTimer = 0f;
        private static float _enemyShootInterval = 2f;
        private static float _enemyShootProbability = 1f;
        private static Rectangle _enemyBulletCollisionRectangle;

        // Gameplay settings
        //--------------------------------------------------------------------------------------
        private static int _score = 0;
        private static int _highScore ;
        public static Vector2 enemyDebug;

        
        private struct Enemy
        {
            public bool IsAlive;
            public Texture2D Texture;
            public Vector2 Position;
            public Rectangle CollisionRectangle;
            public int Score;
        }


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
        

        private static Random _random = new Random();


        public enum GameScreenEnum
        {
            MainMenu,
            Gameplay,
            GameOver
        }
        
        private static GameScreenEnum _currentScreen = GameScreenEnum.Gameplay; //todo cambiar a main menu cuando este hecho


        public static void Main()
        {
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Space Invader - Final Exam");
            Raylib.SetTargetFPS(FPS);

            LoadTextures();
            InitializeEnemy(_enemies1, _enemyOnePosition, _enemyOneTexture);
            InitializeEnemy(_enemies2, _enemyTwoPosition, _enemyTwoTexture);
            InitializeEnemy(_enemies3, _enemyThreePosition, _enemyThreeTexture);
            
            // Main game loop
            while (!Raylib.WindowShouldClose())
            {
                // Update
                //----------------------------------------------------------------------------------
                if (_currentScreen == GameScreenEnum.Gameplay)
                {
                    //Enemies
                    MoveAllEnemies();
                    EnemyShoot();
                    EnemyBulletHandler();
                    
                    //Player
                    PlayerMovement();
                    PlayerShoot();
                    PlayerBulletHandler();
                    
                    //Collisions
                    UpdatePlayerCollisionRectangle();
                    UpdatePlayerBulletCollisionRectangle();
                    UpdateEnemyBulletCollisionRectangle();
                    HandleCollisions(_enemies1);
                    HandleCollisions(_enemies2);
                    HandleCollisions(_enemies3);
                    CheckEnemyBulletCollisionWithPlayer();
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

        }

        private static void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Blue);

            switch (_currentScreen)
            {
                case GameScreenEnum.MainMenu:
                {
                    MainMenu();
                }
                    break;
                case GameScreenEnum.Gameplay:
                {
                    Gameplay();
                    Ui();
                }
                    break;
                case GameScreenEnum.GameOver:
                {
                    GameOver();
                }
                    break;
                default:
                {
                    Console.WriteLine("Error: No en seleccionar una escena ~ Draw()");
                }
                    break;
                
            }
            

            Raylib.EndDrawing();
        }

        private static void Ui()
        {
            int livesYPadding = 20;
            int scoreYPadding = 60;
            int highscoreYPadding = 100;
            
            Raylib.DrawText("Lives: ", 30, livesYPadding, CommonFontSize, Color.White);
            Raylib.DrawText(_playerLives.ToString("F0"), 300, livesYPadding, CommonFontSize, Color.White);
            
            Raylib.DrawText("Score: ", 30, scoreYPadding, CommonFontSize, Color.White);
            Raylib.DrawText(_score.ToString("F0"), 300, scoreYPadding, CommonFontSize, Color.White);
            
            Raylib.DrawText("Highscore: ", 30, highscoreYPadding, CommonFontSize, Color.White);
            Raylib.DrawText(_highScore.ToString("F0"), 300, highscoreYPadding, CommonFontSize, Color.White);
        }

        private static void Gameplay()
        {
            // GameplayDebug();
            
            //Player -------------------
            Raylib.DrawTexture(_playerTexture, (int)_playerPosition.X, (int)_playerPosition.Y, Color.White);
            DrawPlayerBullet();
            DrawEnemyBullet();


            //Enemy ------------------------
            DrawEnemyPositions(_enemies1);
            DrawEnemyPositions(_enemies2);
            DrawEnemyPositions(_enemies3);
            
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
                    enemies[i].Position.X += _enemiesSpeed * deltaTime;
                    UpdateEnemyCollisionRectangle(ref enemies[i]);
                }

            }
            else
            {
                for (int i = enemies.Length - 1; i >= 0; i--)
                {
                    enemies[i].Position.X -= _enemiesSpeed * deltaTime;
                    UpdateEnemyCollisionRectangle(ref enemies[i]);
                }
            }
        }

        private static void DrawEnemyPositions(Enemy[] enemies)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (!enemies[i].IsAlive) continue;

                Raylib.DrawTexture(enemies[i].Texture, (int)enemies[i].Position.X, (int)enemies[i].Position.Y, Color.White);
            }
        }

        private static void InitializeEnemy(Enemy[] enemies, Vector2 initialEnemyPosition, Texture2D enemyTexture)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].Score = 1;
                enemies[i].Texture = enemyTexture;
                enemies[i].IsAlive = true;
                enemies[i].Position = new Vector2(initialEnemyPosition.X + i * _spacing, initialEnemyPosition.Y); 
                UpdateEnemyCollisionRectangle(ref enemies[i]);
            }
        }
        private static void UpdateEnemyCollisionRectangle(ref Enemy enemy) 
        {
            enemy.CollisionRectangle = new Rectangle(enemy.Position.X, enemy.Position.Y, enemy.Texture.Width, enemy.Texture.Height);
        }
        private static void UpdatePlayerCollisionRectangle()
        {
            _playerCollisionRectangle = new Rectangle(_playerPosition.X, _playerPosition.Y, _playerTexture.Width, _playerTexture.Height);
        }
        private static void UpdatePlayerBulletCollisionRectangle()
        {
            if (_isPlayerBulletActive)
            {
                _playerBulletCollisionRectangle = new Rectangle(_playerBulletPosition.X, _playerBulletPosition.Y, _bulletTexture.Width, _bulletTexture.Height);
            } else
            {
                _playerBulletCollisionRectangle = default;
            }
        }
        private static void UpdateEnemyBulletCollisionRectangle()
        {
            if (_isEnemyBulletActive)
            {
                _enemyBulletCollisionRectangle = new Rectangle(_enemyBulletPosition.X, _enemyBulletPosition.Y, _bulletTexture.Width, _bulletTexture.Height);
            } else
            {
                _enemyBulletCollisionRectangle = default;
            }
        }


        private static void MoveAllEnemies()
        {
            float deltaTime = Raylib.GetFrameTime();
            _enemyMoveTimer += deltaTime;

            float leftmostEnemy = _enemies1[0].Position.X;
            float rightmostEnemy = _enemies1[_enemies1.Length - 1].Position.X;

            foreach (var _ in _enemies2) 
            {
                leftmostEnemy = Math.Min(leftmostEnemy, _enemies1[0].Position.X);
                rightmostEnemy = Math.Max(rightmostEnemy, _enemies2[_enemies2.Length - 1].Position.X);
            }
            foreach (var _ in _enemies3) 
            {
                leftmostEnemy = Math.Min(leftmostEnemy, _enemies3[0].Position.X);
                rightmostEnemy = Math.Max(rightmostEnemy, _enemies3[_enemies3.Length - 1].Position.X);
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
            if (_enemyMoveTimer >= _enemyMoveInterval)
            {
                _enemyMoveTimer = 0f;
                EnemyMovement(_enemies1);
                EnemyMovement(_enemies2);
                EnemyMovement(_enemies3);

            }
        }

        private static void GameplayDebug()
        {
            Raylib.DrawRectangleLines((int)_playerCollisionRectangle.X, (int)_playerCollisionRectangle.Y, (int)_playerCollisionRectangle.Width, (int)_playerCollisionRectangle.Height, Color.Red);
            

            for (int i = 0; i < _enemies1.Length; i++)
            {
                if (!(_enemies1[i].IsAlive)) continue;
                Raylib.DrawRectangleLines((int)_enemies1[i].CollisionRectangle.X, (int)_enemies1[i].CollisionRectangle.Y, (int)_enemies1[i].CollisionRectangle.Width, (int)_enemies1[i].CollisionRectangle.Height, Color.Red);

            }
            for (int i = 0; i < _enemies2.Length; i++)
            {
                if (!(_enemies2[i].IsAlive)) continue;

                Raylib.DrawRectangleLines((int)_enemies2[i].CollisionRectangle.X, (int)_enemies2[i].CollisionRectangle.Y, (int)_enemies2[i].CollisionRectangle.Width, (int)_enemies2[i].CollisionRectangle.Height, Color.Red);

            }
            for (int i = 0; i < _enemies3.Length; i++)
            {
                if (!(_enemies3[i].IsAlive)) continue;
                Raylib.DrawRectangleLines((int)_enemies3[i].CollisionRectangle.X, (int)_enemies3[i].CollisionRectangle.Y, (int)_enemies3[i].CollisionRectangle.Width, (int)_enemies3[i].CollisionRectangle.Height, Color.Red);
            }

            Raylib.DrawRectangleLines((int)_playerBulletCollisionRectangle.X, (int)_playerBulletCollisionRectangle.Y, (int)_playerBulletCollisionRectangle.Width, (int)_playerBulletCollisionRectangle.Height, Color.Red);

            Raylib.DrawRectangleLines((int)_enemyBulletCollisionRectangle.X, (int)_enemyBulletCollisionRectangle.Y, (int)_enemyBulletCollisionRectangle.Width, (int)_enemyBulletCollisionRectangle.Height, Color.Red);


        }
        
        private static void EnemyShoot()
        {
            if (_isEnemyBulletActive) return; 

            _enemyShootTimer += Raylib.GetFrameTime();

            if (_enemyShootTimer >= _enemyShootInterval)
            {
                _enemyShootTimer = 0f;
                if (_random.NextSingle() <= _enemyShootProbability)
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
            
            if (allAliveEnemies.Count == 0) return;
            
            int randomNext = _random.Next(allAliveEnemies.Count);
            Enemy enemy = allAliveEnemies[randomNext];
            
            
            
             _enemyBulletPosition = enemy.Position;
             _enemyBulletPosition.X += enemy.Texture.Width / 2;
             _enemyBulletPosition.Y += enemy.Texture.Height / 2; 
            _isEnemyBulletActive = true;
        }
        

        private static void AddAliveEnemiesToList(List<Enemy> list,Enemy[] enemies)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.IsAlive)
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
                _enemyBulletPosition.Y += _enemyBulletSpeed * deltaTime;

                if (_enemyBulletPosition.Y > ScreenHeight)
                {
                    _isEnemyBulletActive = false;
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

        private static void CheckEnemyBulletCollisionWithPlayer()
        {
            if (!_isEnemyBulletActive) return;
            
            if (Raylib.CheckCollisionRecs(_enemyBulletCollisionRectangle, _playerCollisionRectangle))
            {
                _isEnemyBulletActive = false;
                PlayerHit();
                CheckGameCondition();
            }
        }

        private static void PlayerHit()
        {
            _playerLives--;
        }

        private static void HandleCollisions(Enemy[] enemies)
        {
            if (!_isPlayerBulletActive) return; 


            for (int i = 0; i < enemies.Length; i++) 
            {
                if (enemies[i].IsAlive) 
                {
                    if (Raylib.CheckCollisionRecs(_playerBulletCollisionRectangle, enemies[i].CollisionRectangle))
                    {
                        enemies[i].IsAlive = false; 
                        _isPlayerBulletActive = false; 
                        AddScore(enemies[i]);
                        CheckGameCondition();
                        return;
                    }
                }
            }
        }

        private static void CheckGameCondition()
        {
            if (!AreEnemiesAlive())
            {
                SaveHighScore();
                _isGameOver = true;
                _currentScreen = GameScreenEnum.MainMenu;
            }
            if (_playerLives <= 0)
            {
                SaveHighScore();
                _isGameOver = true;
                _currentScreen = GameScreenEnum.GameOver;
            }
            
        }

        private static void AddScore(Enemy enemy)
        {
            _score += enemy.Score;
        }

        private static void SaveHighScore()
        {
            if (_score > _highScore)
            {
                _highScore = _score;
            }
        }
        
        private static bool AreEnemiesAlive()
        {
            foreach (var enemy in _enemies1)
            {
                if (enemy.IsAlive) return true;
            }

            foreach (var enemy in _enemies2)
            {
                if (enemy.IsAlive) return true;
            }

            foreach (var enemy in _enemies3)
            {
                if (enemy.IsAlive) return true;
            }
            
            return false;
        }

        private static void GameOver()
        {
            Raylib.DrawText("Game Over", ScreenWidth/2, ScreenHeight/2, TitleFontSize, Color.White);
        }
        
        private static void MainMenu()
        {
            Raylib.DrawText("Main Menu", ScreenWidth/2, ScreenHeight/2, TitleFontSize, Color.White);
        }
    }
}