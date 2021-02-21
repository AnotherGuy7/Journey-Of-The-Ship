using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Journey_Of_The_Ship.Effects;
using System;
using System.Collections.Generic;
using Journey_Of_The_Ship.Projectiles;
using Journey_Of_The_Ship.Enemies;
using Journey_Of_The_Ship.PowerUps;
using Journey_Of_The_Ship.Obstacles;
using Journey_Of_The_Ship.UI;

namespace Journey_Of_The_Ship
{
    public class Main : Game
    {
        public const int MaxTotalEntities = 100;
        public const int MaxProjectiles = 20;
        public const int MaxStars = 70;
        public const int MaxScreenShakeIntensity = 5;

        public static Random random = new Random();
        public static List<CollisionBody> activeEntities = new List<CollisionBody>();
        public static List<Projectile> activeProjectiles = new List<Projectile>();
        public static List<VisualEffect> backgroundEffects = new List<VisualEffect>();
        public static List<VisualEffect> activeEffects = new List<VisualEffect>();
        public static List<UIObject> activeUI = new List<UIObject>();
        public static SpriteFont mainFont;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Matrix screenMatrix;

        public Player player;

        public static int playerHealth = 6;
        public static int gameDifficulty = 1;
        public static int gameScore = 1;
        public static int screenShakeDuration = 0;
        public static int screenShakeIntensity = 0;

        public static GameStates gameState = GameStates.GameState_Playing;
        public static Events activeEvent = Events.AsteroidField;

        public static int desiredResolutionWidth = 167;     //The resolution that the game will actually use, or "screen-space"
        public static int desiredResolutionHeight = 200;
        public static int actualResolutionWidth = 500;      //The actual resolution the game will be drawn at (Window size)
        public static int actualResolutionHeight = 600;

        private ParallaxBackground parallax;
        private Vector2 screenOffset;

        public enum GameStates
        {
            GameState_Title,
            GameState_Playing,
            GameState_Paused,
            GameState_GameOver,
        }

        public enum Events
        {
            None,
            AsteroidField,
            Boss,
        }

        public Main()
        {
            Window.Title = "Journey of the Ship";
            Content.RootDirectory = "Content";
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            player = new Player();

            _graphics.PreferredBackBufferWidth = (int)actualResolutionWidth;
            _graphics.PreferredBackBufferHeight = (int)actualResolutionHeight;
            _graphics.ApplyChanges();

            float matrixX = (float)actualResolutionWidth / desiredResolutionWidth;
            float matrixY = (float)actualResolutionHeight / desiredResolutionHeight;
            screenMatrix = Matrix.CreateScale(matrixX, matrixY, 1.0f);

            LoadContent();
            ReInitializeGame();

            parallax = new ParallaxBackground();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            mainFont = Content.Load<SpriteFont>("Fonts/MainFont");

            Player.playerSpritesheet = Content.Load<Texture2D>("Textures/Spritesheets/PlayerShip");
            Player.playerAfterImageTexture = Content.Load<Texture2D>("Textures/Objects/PlayerAfterImage");
            Bullet.bulletTexture = Content.Load<Texture2D>("Textures/Objects/Bullet");
            Laser.laserTexture = Content.Load<Texture2D>("Textures/Objects/Laser");
            UFO.ufoSpritesheet = Content.Load<Texture2D>("Textures/Spritesheets/UFO");

            int amountOfAsteroids = 2;
            Asteroid.asteroidTextures = new Texture2D[amountOfAsteroids];
            for (int a = 0; a < amountOfAsteroids; a++)
            {
                Asteroid.asteroidTextures[a] = Content.Load<Texture2D>("Textures/Objects/Asteroid_" + (a + 1));
            }

            int amountOfPlanets = 4;
            Planet.planetsTextureArray = new Texture2D[amountOfPlanets];
            for (int p = 0; p < amountOfPlanets; p++)
            {
                Planet.planetsTextureArray[p] = Content.Load<Texture2D>("Textures/Objects/Planet_" + (p + 1));
            }

            int amountOfGoreTextures = 4;
            Gore.goreTextures = new Texture2D[amountOfGoreTextures];
            for (int g = 0; g < amountOfGoreTextures; g++)
            {
                Gore.goreTextures[g] = Content.Load<Texture2D>("Textures/Objects/Gore/Gore_" + (g + 1));
            }

            Star.starSpritesheet = Content.Load<Texture2D>("Textures/Objects/Star");
            Explosion.explosionSpritesheet = Content.Load<Texture2D>("Textures/Spritesheets/Explosion");
            Smoke.whitePixelTexture = Content.Load<Texture2D>("Textures/Objects/WhitePixel");
            PowerUp.powerUpTextures[0] = Content.Load<Texture2D>("Textures/Objects/Power-Ups/AttackUp");
            PowerUp.powerUpAuras[0] = Content.Load<Texture2D>("Textures/Objects/Power-Ups/RedAura");

            ParallaxBackground.spaceSet1 = new Texture2D[2];
            ParallaxBackground.spaceSet1[0] = Content.Load<Texture2D>("Textures/Backgrounds/SpaceBackgroundSet1/Space1_Part1");
            ParallaxBackground.spaceSet1[1] = Content.Load<Texture2D>("Textures/Backgrounds/SpaceBackgroundSet1/Space1_Part2");

            WarningOverlay.warningOverlayTexture = Content.Load<Texture2D>("Textures/UI/AsteroidFieldWarningLines");
            WarningOverlay.warningOverlayMark = Content.Load<Texture2D>("Textures/UI/AsteroidFieldWarningMark");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            switch (gameState)
            {
                case GameStates.GameState_Title:
                    break;
                case GameStates.GameState_Playing:
                    parallax.Update();

                    VisualEffect[] backgroundEffectsClone = backgroundEffects.ToArray();        //This is so that when the active list changes, the loop isn't affected
                    CollisionBody[] activeEntitiesClone = activeEntities.ToArray();
                    Projectile[] activeProjectilesClone = activeProjectiles.ToArray();
                    VisualEffect[] activeEffectsClone = activeEffects.ToArray();
                    UIObject[] activeUIClone = activeUI.ToArray();
                    foreach (VisualEffect backgroundEffect in backgroundEffectsClone)
                    {
                        backgroundEffect.Update();
                    }
                    foreach (CollisionBody entity in activeEntitiesClone)
                    {
                        entity.Update();
                    }
                    foreach (Projectile projectile in activeProjectilesClone)
                    {
                        projectile.Update();
                    }
                    foreach (VisualEffect effect in activeEffectsClone)
                    {
                        effect.Update();
                    }
                    foreach (UIObject ui in activeUIClone)
                    {
                        ui.Update();
                    }

                    CheckForEvents();
                    SpawnBackgroundEffects();
                    SpawnEnemies();

                    break;
                case GameStates.GameState_Paused:
                    break;
                case GameStates.GameState_GameOver:

                    if (activeEntities.Count != 0)
                    {
                        backgroundEffects.Clear();
                        activeEntities.Clear();
                        activeProjectiles.Clear();
                        activeEffects.Clear();
                        activeUI.Clear();
                    }
                    break;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            screenMatrix.Translation = new Vector3(0f);
            if (screenShakeDuration > 0)
            {
                screenShakeDuration--;
                ShakeScreen();
            }
            //BlendState Alpha Blend: Any transparent pixels in the image will blend with whatever's under them (As well as partially-transparent pixels)
            //SamplerState Point Clamp: No blur effects are added, things drawn in this batch will keep their pixelated effect
            //DepthStencilState None: Doesn't use the depth stencil (Which keeps track of the depth of pixels in the screen, useful for 3D)
            //RasterizerState Cull Counter Clockwise: As far as I know, gets 3D points and converts them into shapes then pixels for easy image representation
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, transformMatrix: screenMatrix);

            switch (gameState)
            {
                case GameStates.GameState_Title:
                    break;
                case GameStates.GameState_Playing:
                    parallax.Draw(_spriteBatch);

                    VisualEffect[] backgroundEffectsClone = backgroundEffects.ToArray();
                    CollisionBody[] activeEntitiesClone = activeEntities.ToArray();
                    Projectile[] activeProjectilesClone = activeProjectiles.ToArray();
                    VisualEffect[] activeEffectsClone = activeEffects.ToArray();
                    UIObject[] activeUIClone = activeUI.ToArray();
                    foreach (VisualEffect backgroundEffect in backgroundEffectsClone)
                    {
                        backgroundEffect.Draw(_spriteBatch);
                    }
                    foreach (CollisionBody entity in activeEntitiesClone)
                    {
                        entity.Draw(_spriteBatch);
                    }
                    foreach (Projectile projectile in activeProjectilesClone)
                    {
                        projectile.Draw(_spriteBatch);
                    }
                    foreach (VisualEffect effect in activeEffectsClone)
                    {
                        effect.Draw(_spriteBatch);
                    }
                    foreach (UIObject ui in activeUIClone)
                    {
                        ui.Draw(_spriteBatch);
                    }

                    break;
                case GameStates.GameState_Paused:
                    break;
                case GameStates.GameState_GameOver:

                    _spriteBatch.DrawString(mainFont, "Score: " + gameScore, new Vector2(270f, 680f), Color.White);
                    break;
            }

            _spriteBatch.End();

        }

        private void ReInitializeGame()
        {
            player.Initialize();
            player.position.X = 167f / 2f;
            player.position.Y = 180f;
            activeEntities.Add(player);

            playerHealth = 5;
            gameDifficulty = 1;
            gameScore = 1;

            for (int star = 0; star < 20; star++)
            {
                float starSpawnPosX = random.Next(9, desiredResolutionWidth - 9);
                float starSpawnPosY = random.Next(0, desiredResolutionHeight);
                float starFallSpeed = (float)random.NextDouble() * 0.5f;
                Star.NewStarEffect(new Vector2(starSpawnPosX, starSpawnPosY), new Vector2(0f, starFallSpeed), 0f, random.Next(0, 2));
            }
        }

        public static void StartScreenShake(int duration, int intensity)
        {
            screenShakeDuration += duration;
            screenShakeIntensity = intensity;
        }

        private void ShakeScreen()
        {
            screenOffset = new Vector2(random.Next(-screenShakeIntensity, screenShakeIntensity + 1), random.Next(-screenShakeIntensity, screenShakeIntensity + 1));
            screenMatrix.Translation = new Vector3(screenOffset, 0f);
        }

        private void CheckForEvents()
        {
            if (activeEvent == Events.None)
            {
                if (random.Next(1, 200) == 1)
                {
                    activeEvent = Events.AsteroidField;
                    WarningOverlay.ShowWarning(5 * 60);
                }
            }
        }

        private void SpawnEnemies()
        {
            switch (activeEvent)
            {
                case Events.None:
                    if (random.Next(1, 250) == 1)
                    {
                        float spawnPosX = random.Next(25, desiredResolutionWidth - 25);
                        float spawnPosY = -50;
                        UFO.NewUFO(new Vector2(spawnPosX, spawnPosY));
                    }
                    break;
                case Events.AsteroidField:
                    if (random.Next(1, 250) == 1)
                    {
                        int asteroidType = random.Next(0, 1 + 1);
                        float asteroidSpawnPosX = random.Next(40, desiredResolutionWidth - 40);
                        float asteroidSpawnPosY = -50;
                        float asteroidFallSpeed = (float)random.NextDouble() * 0.2f;
                        float asteroidScale = (float)(random.Next(40, 100 + 1)) / 100f;
                        float asteroidRotation = (float)(random.Next(10, 200 + 1) / 2000f);
                        Asteroid.NewAsteroid(asteroidType, new Vector2(asteroidSpawnPosX, asteroidSpawnPosY), new Vector2(0f, asteroidFallSpeed / (1.1f - (asteroidScale / 100f))), asteroidRotation, asteroidScale);
                    }
                    break;
            }
        }

        private void SpawnBackgroundEffects()
        {
            if (random.Next(1, 1200) == 1)
            {
                float planetSpawnPosX = random.Next(40, desiredResolutionWidth - 40);
                float planetSpawnPosY = -50;
                float planetFallSpeed = (float)random.NextDouble() * 0.2f;
                float planetScale = (float)(random.Next(30, 50 + 1)) / 100f;
                Planet.NewPlanetEffect(new Vector2(planetSpawnPosX, planetSpawnPosY), new Vector2(0f, planetFallSpeed / (2f * planetScale)), scale: planetScale);
            }
            if (random.Next(1, 50) == 1)
            {
                float starSpawnPosX = random.Next(9, desiredResolutionWidth - 9);
                float starSpawnPosY = -50;
                float starFallSpeed = (float)random.NextDouble() * 0.5f;
                Star.NewStarEffect(new Vector2(starSpawnPosX, starSpawnPosY), new Vector2(0f, starFallSpeed), 0f, random.Next(0, 2));
            }
        }
    }
}