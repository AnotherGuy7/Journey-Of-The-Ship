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

        public static Player player;
        public static AnimationHandler animationHandler;
        public static SaveManager saveManager;
        public static MusicManager musicManager;
        public static UIObject mainUI;     //The UI that is currently being shown to the player (that they can interact with. Title Screen, Modification Screen, etc.)
        public PlayerUI playerUI;

        public static int playerHealth = 6;
        public static float playerSpeed = 1.5f;
        public static int gameDifficulty = 1;
        public static int gameScore = 1;
        public static int screenShakeDuration = 0;
        public static int screenShakeIntensity = 0;
        public static float fadeProgress = 0f;
        public static int fadeOutTimer = 0;
        public static int fadeInTimer = 0;
        public static int fadeOutStartTime = 0;
        public static int fadeInStartTime = 0;
        public static int uiInteractionLayer = 1;

        public static GameStates gameState = GameStates.GameState_Title;
        public static Events activeEvent = Events.None;

        public static int desiredResolutionWidth = 167;     //The resolution that the game will actually use, or "screen-space"
        public static int desiredResolutionHeight = 200;
        public static int actualResolutionWidth = 500;      //The actual resolution the game will be drawn at (Window size)
        public static int actualResolutionHeight = 600;

        public static float soundEffectVolume = 0.8f;
        public static float musicVolume = 0.8f;

        public static Vector2 mousePosition;
        public static MouseState mouseState;
        public static bool closeGame = false;

        private int eventTimer = 0;

        private Texture2D fadeOutTexture;
        private ParallaxBackground parallax;
        private Vector2 screenOffset;

        public static string debugValue = "";

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

            saveManager = new SaveManager();
            saveManager.LoadGame();

            mainUI = new TitleScreen();
            parallax = new ParallaxBackground();
            musicManager = new MusicManager();
            PlayerUI.InitializePlayerUI();
        }

        protected override void LoadContent()
        {
            animationHandler = new AnimationHandler();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            mainFont = Content.Load<SpriteFont>("Fonts/MainFont");
            fadeOutTexture = Content.Load<Texture2D>("Textures/UI/ScreenFadeOut");

            Player.playerSpritesheet = Content.Load<Texture2D>("Textures/Spritesheets/PlayerShip");
            Player.playerAfterImageTexture = Content.Load<Texture2D>("Textures/Objects/PlayerAfterImage");
            Bullet.bulletTexture = Content.Load<Texture2D>("Textures/Objects/Bullet");
            Missile.missileTexture = Content.Load<Texture2D>("Textures/Spritesheets/Missile");
            Missile.targetLockedIndicator = Content.Load<Texture2D>("Textures/UI/TargetLockedIndicator");
            Laser.laserTexture = Content.Load<Texture2D>("Textures/Objects/Laser");
            UFO.ufoSpritesheet = Content.Load<Texture2D>("Textures/Spritesheets/UFO");
            Slicer.slicerSpritesheet = Content.Load<Texture2D>("Textures/Spritesheets/Slicer");
            Slicer.slicerAfterImageTexture = Content.Load<Texture2D>("Textures/Objects/SlicerAfterImage");
            RayEnemy.raySpritesheet = Content.Load<Texture2D>("Textures/Spritesheets/Ray");
            Stasis.stabilizerSpritesheet = Content.Load<Texture2D>("Textures/Spritesheets/Stasis");
            ContinuousLaser.laserSpritesheet = Content.Load<Texture2D>("Textures/Spritesheets/ContinuousLaser");
            StasisBeam.beamSpritesheet = Content.Load<Texture2D>("Textures/Spritesheets/StasisBeam");

            int amountOfAsteroids = 5;
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

            int amountOfGoreTextures = 11;
            Gore.goreTextures = new Texture2D[amountOfGoreTextures];
            for (int g = 0; g < amountOfGoreTextures; g++)
            {
                Gore.goreTextures[g] = Content.Load<Texture2D>("Textures/Objects/Gore/Gore_" + (g + 1));
            }

            Star.starSpritesheet = Content.Load<Texture2D>("Textures/Objects/Star");
            Explosion.explosionSpritesheet = Content.Load<Texture2D>("Textures/Spritesheets/Explosion");
            Smoke.whitePixelTexture = Content.Load<Texture2D>("Textures/Objects/WhitePixel");
            PowerUp.powerUpAura = Content.Load<Texture2D>("Textures/Objects/Power-Ups/PowerUpAura");
            PowerUp.powerUpRing = Content.Load<Texture2D>("Textures/Objects/Power-Ups/PowerUpRing");
            PowerUp.powerUpTextures[PowerUp.Attack] = Content.Load<Texture2D>("Textures/Objects/Power-Ups/AttackUp");
            PowerUp.powerUpTextures[PowerUp.Health] = Content.Load<Texture2D>("Textures/Objects/Power-Ups/HealthUp");
            PowerUp.powerUpTextures[PowerUp.Speed] = Content.Load<Texture2D>("Textures/Objects/Power-Ups/SpeedUp");
            AbilityDrop.abilityRect = Content.Load<Texture2D>("Textures/Objects/Power-Ups/AbilityRect");
            AbilityDrop.abilityTextures[AbilityDrop.RapidFire] = Content.Load<Texture2D>("Textures/Objects/Power-Ups/RapidFire");

            ParallaxBackground.spaceSet1 = new Texture2D[2];
            ParallaxBackground.spaceSet1[0] = Content.Load<Texture2D>("Textures/Backgrounds/SpaceBackgroundSet1/Space1_Part1");
            ParallaxBackground.spaceSet1[1] = Content.Load<Texture2D>("Textures/Backgrounds/SpaceBackgroundSet1/Space1_Part2");

            PlayerUI.playerHealthMark = Content.Load<Texture2D>("Textures/UI/HealthBarMark");
            PlayerUI.playerHealthOutlines = Content.Load<Texture2D>("Textures/UI/HealthBarOutlines");
            PlayerUI.clearEnvironmentIcon = Content.Load<Texture2D>("Textures/UI/ClearEnvironmentIcon");
            PlayerUI.asteroidEnvironmentIcon = Content.Load<Texture2D>("Textures/UI/AsteroidEnvironmentIcon");
            WarningOverlay.warningOverlayTexture = Content.Load<Texture2D>("Textures/UI/AsteroidFieldWarningLines");
            WarningOverlay.warningOverlayMark = Content.Load<Texture2D>("Textures/UI/AsteroidFieldWarningMark");
            Button.menuStyleTop = Content.Load<Texture2D>("Textures/UI/ButtonOutlineTop");
            Button.menuStyleBottom = Content.Load<Texture2D>("Textures/UI/ButtonOutlineBottom");
            Color[] whiteColorArray = new Color[1] { Color.White } ;
            Texture2D whitePixel = new Texture2D(GraphicsDevice, 1, 1);     //Me being too lazy to make a 1 pixel texture and loading it through Content.mgcb
            whitePixel.SetData(whiteColorArray);
            Button.whitePixel = Slider.whitePixel = whitePixel;
            Slider.sliderButtonTexture = Content.Load<Texture2D>("Textures/UI/SliderButton");
            SettingsScreen.settingsPanel = Content.Load<Texture2D>("Textures/UI/SettingsPanel");
            ModificationScreen.normalBarrelTexture = Content.Load<Texture2D>("Textures/Objects/Icons/NormalBarrel");
            ModificationScreen.extendedBarrelTexture = Content.Load<Texture2D>("Textures/Objects/Icons/ExtendedBarrel");
            ModificationScreen.powerfulBarrelTexture = Content.Load<Texture2D>("Textures/Objects/Icons/PowerfulBarrel");
            ModificationScreen.normalWingsTexture = Content.Load<Texture2D>("Textures/Objects/Icons/NormalWings");
            ModificationScreen.thinCutWingsTexture = Content.Load<Texture2D>("Textures/Objects/Icons/ThinCutWings");
            ModificationScreen.hoverEquippedWingsTexture = Content.Load<Texture2D>("Textures/Objects/Icons/HoverEquippedWings");
            ModificationScreen.chargedWingsTexture = Content.Load<Texture2D>("Textures/Objects/Icons/ChargedWings");
            ModificationScreen.shipTexture = Content.Load<Texture2D>("Textures/Objects/Icons/Ship");
            ModificationScreen.shipBarrels = Content.Load<Texture2D>("Textures/Objects/Icons/ShipBarrels");
            ModificationScreen.shipPropellers = Content.Load<Texture2D>("Textures/Objects/Icons/ShipPropellers");
            ModificationScreen.shipWings = Content.Load<Texture2D>("Textures/Objects/Icons/ShipWings");
            ModificationScreen.bulletTexture = Content.Load<Texture2D>("Textures/Objects/Icons/Bullet");
            ModificationScreen.missileTexture = Content.Load<Texture2D>("Textures/Objects/Icons/Missile");
            ModificationScreen.modificationHangarTexture = Content.Load<Texture2D>("Textures/Backgrounds/ModificationHangar");

            Player.shootSound = Content.Load<SoundEffect>("Sounds/PlayerShoot");
            Player.dashSound = Content.Load<SoundEffect>("Sounds//Dash");
            Explosion.explosionSound = Content.Load<SoundEffect>("Sounds/Explosion_3");
            UFO.shootSound = Content.Load<SoundEffect>("Sounds/UFOShoot");
            UFO.deathSound = Content.Load<SoundEffect>("Sounds/UFODying");
            Stasis.beamSound = Content.Load<SoundEffect>("Sounds/StasisBeam");
            MusicManager.titleMusic = Content.Load<Song>("Sounds/Music/JotS_Title");
            MusicManager.mainMusic = Content.Load<Song>("Sounds/Music/JotS_Main");
        }

        protected override void Update(GameTime gameTime)
        {
            debugValue = "";
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || closeGame)
            {
                Exit();
            }

            mouseState = Mouse.GetState();
            mousePosition = mouseState.Position.ToVector2() / 3f;       //Divided by 3 cause it has to be converted into screen-space coordinates

            if (fadeOutTimer > 0)
            {
                fadeOutTimer--;
                fadeProgress = 1f - ((float)fadeOutTimer / (float)fadeOutStartTime);
            }
            if (fadeInTimer > 0)
            {
                fadeInTimer--;
                fadeProgress = (float)fadeInTimer / (float)fadeInStartTime;
            }

            animationHandler.Update();
            musicManager.UpdateMusic();
            switch (gameState)
            {
                case GameStates.GameState_Title:
                    parallax.Update();
                    mainUI.Update();
                    break;
                case GameStates.GameState_Playing:
                    parallax.Update();

                    VisualEffect[] backgroundEffectsClone = backgroundEffects.ToArray();        //This is so that when the active list changes, the loop isn't affected
                    Projectile[] activeProjectilesClone = activeProjectiles.ToArray();
                    CollisionBody[] activeEntitiesClone = activeEntities.ToArray();
                    VisualEffect[] activeEffectsClone = activeEffects.ToArray();
                    UIObject[] activeUIClone = activeUI.ToArray();
                    foreach (VisualEffect backgroundEffect in backgroundEffectsClone)
                    {
                        backgroundEffect.Update();
                    }
                    foreach (Projectile projectile in activeProjectilesClone)
                    {
                        projectile.Update();
                    }
                    foreach (CollisionBody entity in activeEntitiesClone)
                    {
                        entity.Update();
                    }
                    foreach (VisualEffect effect in activeEffectsClone)
                    {
                        effect.Update();
                    }
                    foreach (UIObject ui in activeUIClone)
                    {
                        ui.Update();
                    }

                    HandleEvents();
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
                    parallax.Draw(_spriteBatch);
                    mainUI.Draw(_spriteBatch);
                    break;
                case GameStates.GameState_Playing:
                    parallax.Draw(_spriteBatch);

                    VisualEffect[] backgroundEffectsClone = backgroundEffects.ToArray();
                    Projectile[] activeProjectilesClone = activeProjectiles.ToArray();
                    CollisionBody[] activeEntitiesClone = activeEntities.ToArray();
                    VisualEffect[] activeEffectsClone = activeEffects.ToArray();
                    UIObject[] activeUIClone = activeUI.ToArray();
                    foreach (VisualEffect backgroundEffect in backgroundEffectsClone)
                    {
                        backgroundEffect.Draw(_spriteBatch);
                    }
                    foreach (Projectile projectile in activeProjectilesClone)
                    {
                        projectile.Draw(_spriteBatch);
                    }
                    foreach (CollisionBody entity in activeEntitiesClone)
                    {
                        entity.Draw(_spriteBatch);
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

            if (fadeProgress > 0)
            {
                _spriteBatch.Draw(fadeOutTexture, new Rectangle(0, 0, desiredResolutionWidth, desiredResolutionHeight), Color.White * fadeProgress);
            }
            if (debugValue != "")
            {
                Vector2 position = new Vector2(desiredResolutionWidth, desiredResolutionHeight) - (mainFont.MeasureString(debugValue) * 0.4f);
                _spriteBatch.DrawString(mainFont, debugValue, position, Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            }
            _spriteBatch.End();

        }

        public static void FadeIn(int duration)
        {
            fadeInStartTime = duration;
            fadeInTimer = duration;
        }

        public static void FadeOut(int duration)
        {
            fadeOutStartTime = duration;
            fadeOutTimer = duration;
        }

        public static void ReInitializeGame()
        {
            player.Initialize();
            player.position.X = desiredResolutionWidth / 2f;
            player.position.Y = 180f;
            activeEntities.Add(player);

            playerHealth = 6;
            gameDifficulty = 1;
            gameScore = 1;
            gameState = GameStates.GameState_Playing;


            for (int star = 0; star < 20; star++)
            {
                float starSpawnPosX = random.Next(9, desiredResolutionWidth - 9);
                float starSpawnPosY = random.Next(0, desiredResolutionHeight);
                float starFallSpeed = (float)random.NextDouble() * 0.5f;
                Star.NewStarEffect(new Vector2(starSpawnPosX, starSpawnPosY), new Vector2(0f, starFallSpeed), 0f, random.Next(0, 2));
            }
        }

        public static void CloseGame()
        {
            closeGame = true;
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
                if (random.Next(1, 25000) == 1)
                {
                    activeEvent = Events.AsteroidField;
                    WarningOverlay.ShowWarning(3 * 60);
                }
            }
        }

        private void HandleEvents()
        {
            if (activeEvent != Events.None)
            {
                eventTimer++;
                if (activeEvent == Events.AsteroidField && eventTimer >= 40 * 60)
                {
                    activeEvent = Events.None;
                }
            }
        }

        private void SpawnEnemies()
        {
            switch (activeEvent)
            {
                case Events.None:
                    if (random.Next(1, 300) == 1)
                    {
                        float spawnPosX = random.Next(25, desiredResolutionWidth - 25);
                        float spawnPosY = -50;
                        UFO.NewUFO(new Vector2(spawnPosX, spawnPosY));
                    }
                    if (random.Next(1, 380) == 1)
                    {
                        float spawnPosX = random.Next(25, desiredResolutionWidth - 25);
                        float spawnPosY = -50;
                        Slicer.NewSlicer(new Vector2(spawnPosX, spawnPosY));
                    }
                    if (random.Next(1, 720) == 1)
                    {
                        float spawnPosX = random.Next(8, desiredResolutionWidth - 8);
                        float spawnPosY = -50;
                        RayEnemy.NewRay(new Vector2(spawnPosX, spawnPosY));
                    }
                    if (random.Next(1, 720) == 1)
                    {
                        float spawnPosX = random.Next(16, desiredResolutionWidth - 16);
                        float spawnPosY = -50;
                        Stasis.NewStasis(new Vector2(spawnPosX, spawnPosY));
                    }
                    break;
                case Events.AsteroidField:
                    if (random.Next(1, 120) == 1)
                    {
                        int asteroidType = random.Next(0, 4 + 1);
                        float asteroidSpawnPosX = random.Next(24, desiredResolutionWidth - 24);
                        float asteroidSpawnPosY = -50;
                        float asteroidFallSpeed = (float)random.NextDouble() * 0.2f;
                        float asteroidScale = (float)(random.Next(80, 100 + 1)) / 100f;
                        float asteroidRotation = (float)(random.Next(10, 100 + 1) / 2000f);
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

        /*public static void DrawDebugRect(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Rectangle rect)
        {
            Texture2D rectTexture = new Texture2D(graphicsDevice, rect.Width, rect.Height);

            Color[] colorsArray = new Color[rect.Width * rect.Height];
            for (int c = 0; c < colorsArray.Length; c++)
            {
                colorsArray[c] = Color.Orange;
            }
            rectTexture.SetData(colorsArray);
            debugValue = rect.Center.ToString();

            spriteBatch.Draw(rectTexture, rect, Color.White);
        }*/
    }
}