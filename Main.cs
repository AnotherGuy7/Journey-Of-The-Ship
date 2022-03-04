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
        public static ShaderManager shaderManager;
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
        public static int highScore = 0;

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
        private RenderTarget2D renderTarget;

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
            shaderManager = new ShaderManager();
            shaderManager.activeScreenShader = null;
            PlayerUI.InitializePlayerUI();
        }

        protected override void LoadContent()
        {
            animationHandler = new AnimationHandler();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            renderTarget = new RenderTarget2D(GraphicsDevice, actualResolutionWidth, actualResolutionHeight);
            mainFont = Content.Load<SpriteFont>("Fonts/MainFont");
            fadeOutTexture = LoadTex("UI/ScreenFadeOut");

            Player.playerSpritesheet = LoadTex("Spritesheets/PlayerShip");
            Player.playerAfterImageTexture = LoadTex("Objects/PlayerAfterImage");
            Bullet.bulletTexture = LoadTex("Objects/Bullet");
            Missile.missileTexture = LoadTex("Spritesheets/Missile");
            Missile.targetLockedIndicator = LoadTex("UI/TargetLockedIndicator");
            Laser.laserTexture = LoadTex("Objects/Laser");
            BlackHoleBomb.bombTexture = LoadTex("Objects/BlackHoleBomb");
            BlackHoleBomb.accretionDiskTexture = LoadTex("Objects/AccretionDisk");

            UFO.ufoSpritesheet = LoadTex("Spritesheets/UFO");
            Slicer.slicerSpritesheet = LoadTex("Spritesheets/Slicer");
            Slicer.slicerAfterImageTexture = LoadTex("Objects/SlicerAfterImage");
            RayEnemy.raySpritesheet = LoadTex("Spritesheets/Ray");
            Stasis.stabilizerSpritesheet = LoadTex("Spritesheets/Stasis");
            ContinuousLaser.laserSpritesheet = LoadTex("Spritesheets/ContinuousLaser");
            StasisBeam.beamSpritesheet = LoadTex("Spritesheets/StasisBeam");

            int amountOfAsteroids = 5;
            Asteroid.asteroidTextures = new Texture2D[amountOfAsteroids];
            for (int a = 0; a < amountOfAsteroids; a++)
            {
                Asteroid.asteroidTextures[a] = LoadTex("Objects/Asteroid_" + (a + 1));
            }

            int amountOfPlanets = 4;
            Planet.planetsTextureArray = new Texture2D[amountOfPlanets];
            for (int p = 0; p < amountOfPlanets; p++)
            {
                Planet.planetsTextureArray[p] = LoadTex("Objects/Planet_" + (p + 1));
            }

            int amountOfGoreTextures = 11;
            Gore.goreTextures = new Texture2D[amountOfGoreTextures];
            for (int g = 0; g < amountOfGoreTextures; g++)
            {
                Gore.goreTextures[g] = LoadTex("Objects/Gore/Gore_" + (g + 1));
            }

            Star.starSpritesheet = LoadTex("Objects/Star");
            Explosion.explosionSpritesheet = LoadTex("Spritesheets/Explosion");
            Smoke.whitePixelTexture = LoadTex("Objects/WhitePixel");
            PowerUp.powerUpAura = LoadTex("Objects/Power-Ups/PowerUpAura");
            PowerUp.powerUpRing = LoadTex("Objects/Power-Ups/PowerUpRing");
            PowerUp.powerUpTextures[PowerUp.Attack] = LoadTex("Objects/Power-Ups/AttackUp");
            PowerUp.powerUpTextures[PowerUp.Health] = LoadTex("Objects/Power-Ups/HealthUp");
            PowerUp.powerUpTextures[PowerUp.Speed] = LoadTex("Objects/Power-Ups/SpeedUp");
            AbilityDrop.abilityRect = LoadTex("Objects/Power-Ups/AbilityRect");
            AbilityDrop.abilityTextures[AbilityDrop.RapidFire] = LoadTex("Objects/Power-Ups/RapidFire");

            ParallaxBackground.spaceSet1 = new Texture2D[2];
            ParallaxBackground.spaceSet1[0] = LoadTex("Backgrounds/SpaceBackgroundSet1/Space1_Part1");
            ParallaxBackground.spaceSet1[1] = LoadTex("Backgrounds/SpaceBackgroundSet1/Space1_Part2");

            PlayerUI.playerHealthMark = LoadTex("UI/HealthBarMark");
            PlayerUI.playerHealthOutlines = LoadTex("UI/HealthBarOutlines");
            PlayerUI.clearEnvironmentIcon = LoadTex("UI/ClearEnvironmentIcon");
            PlayerUI.asteroidEnvironmentIcon = LoadTex("UI/AsteroidEnvironmentIcon");
            PlayerUI.abilityIcons[0] = LoadTex("Objects/Icons/RapidFire");
            PlayerUI.abilityIcons[1] = LoadTex("Objects/Icons/BlackHole");
            PlayerUI.abilityIcons[2] = LoadTex("Objects/Icons/BlackHole");
            PlayerUI.abilityIcons[3] = LoadTex("Objects/Icons/IsotopeReactionContainer");
            PlayerUI.abilityBorder = LoadTex("UI/AbilityBorders");
            WarningOverlay.warningOverlayTexture = LoadTex("UI/AsteroidFieldWarningLines");
            WarningOverlay.warningOverlayMark = LoadTex("UI/AsteroidFieldWarningMark");
            Button.menuStyleTop = LoadTex("UI/ButtonOutlineTop");
            Button.menuStyleBottom = LoadTex("UI/ButtonOutlineBottom");
            Color[] whiteColorArray = new Color[1] { Color.White } ;
            Texture2D whitePixel = new Texture2D(GraphicsDevice, 1, 1);     //Me being too lazy to make a 1 pixel texture and loading it through Content.mgcb
            whitePixel.SetData(whiteColorArray);
            Button.whitePixel = Slider.whitePixel = whitePixel;
            Slider.sliderButtonTexture = LoadTex("UI/SliderButton");
            SettingsScreen.settingsPanel = LoadTex("UI/SettingsPanel");
            ModificationScreen.normalBarrelTexture = LoadTex("Objects/Icons/NormalBarrel");
            ModificationScreen.extendedBarrelTexture = LoadTex("Objects/Icons/ExtendedBarrel");
            ModificationScreen.powerfulBarrelTexture = LoadTex("Objects/Icons/PowerfulBarrel");
            ModificationScreen.normalWingsTexture = LoadTex("Objects/Icons/NormalWings");
            ModificationScreen.thinCutWingsTexture = LoadTex("Objects/Icons/ThinCutWings");
            ModificationScreen.hoverEquippedWingsTexture = LoadTex("Objects/Icons/HoverEquippedWings");
            ModificationScreen.chargedWingsTexture = LoadTex("Objects/Icons/ChargedWings");
            ModificationScreen.shipTexture = LoadTex("Objects/Icons/Ship");
            ModificationScreen.shipBarrels = LoadTex("Objects/Icons/ShipBarrels");
            ModificationScreen.shipPropellers = LoadTex("Objects/Icons/ShipPropellers");
            ModificationScreen.shipWings = LoadTex("Objects/Icons/ShipWings");
            ModificationScreen.bulletTexture = LoadTex("Objects/Icons/Bullet");
            ModificationScreen.missileTexture = LoadTex("Objects/Icons/Missile");
            ModificationScreen.rapidFireTexture = LoadTex("Objects/Icons/RapidFire");
            ModificationScreen.blackHoleTexture = LoadTex("Objects/Icons/BlackHole");
            ModificationScreen.shieldsTexture = LoadTex("Objects/Icons/Shields");
            ModificationScreen.isotopeReactionContainmentTexture = LoadTex("Objects/Icons/IsotopeReactionContainer");
            ModificationScreen.modificationHangarTexture = LoadTex("Backgrounds/ModificationHangar");

            Player.shootSound = LoadSFX("PlayerShoot");
            Player.dashSound = LoadSFX("/Dash");
            Explosion.explosionSound = LoadSFX("Explosion_3");
            UFO.shootSound = LoadSFX("UFOShoot");
            UFO.deathSound = LoadSFX("UFODying");
            Slicer.slicerChargeUpSound = LoadSFX("SlicerChargeUp");
            Slicer.slicerAirCutSound = LoadSFX("SlicerAirCuts");
            BlackHoleBomb.blackHoleOpeningSound = LoadSFX("BlackHoleBombOpen");
            BlackHoleBomb.blackHoleActiveSound = LoadSFX("BlackHoleBombActive");
            Stasis.beamSound = LoadSFX("StasisBeam");
            MusicManager.titleMusic = Content.Load<Song>("Sounds/Music/JotS_Title");
            MusicManager.mainMusic = Content.Load<Song>("Sounds/Music/JotS_Main");

            ShaderManager.blackHoleShader = Content.Load<Effect>("Shaders/BlackHoleShader");
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
            shaderManager.Update();
            switch (gameState)
            {
                case GameStates.GameState_Title:
                    parallax.Update();
                    mainUI.Update();
                    //shaderManager.ActivateBlackHoleShader(0.1f, mousePosition);
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
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, screenMatrix);

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

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, shaderManager.activeScreenShader);
            _spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
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
            if (screenShakeDuration > 0)
                return;

            screenShakeDuration = duration;
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

        private Texture2D LoadTex(string path)
        {
            return Content.Load<Texture2D>("Textures/" + path);
        }

        private SoundEffect LoadSFX(string path)
        {
            return Content.Load<SoundEffect>("Sounds/" + path);
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