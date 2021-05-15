using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Journey_Of_The_Ship.UI
{
    public class ModificationScreen : UIObject
    {
        public static Texture2D shipTexture;
        public static Texture2D shipBarrels;
        public static Texture2D shipPropellers;
        public static Texture2D shipWings;
        public static Texture2D bulletTexture;
        public static Texture2D missileTexture;
        public static Texture2D modificationHangarTexture;

        private TextureCollisionButton shipBarrelsButton;
        private TextureCollisionButton shipPropellersButton;
        private TextureCollisionButton shipWingsButton;


        public static Texture2D normalBarrelTexture;
        public static Texture2D extendedBarrelTexture;
        public static Texture2D powerfulBarrelTexture;
        private Texture2D[] turretModTextures = new Texture2D[3];
        public string[] turretModLabels = new string[3] { "Normal", "Longer Cannon Barrels", "More Powerful Barrels" };
        private ButtonList turretModList;

        public static Texture2D normalWingsTexture;
        public static Texture2D thinCutWingsTexture;
        public static Texture2D hoverEquippedWingsTexture;
        public static Texture2D chargedWingsTexture;
        private Texture2D[] wingModTextures = new Texture2D[4];
        public string[] wingModLabels = new string[4] { "Normal", "Thin Cut Wings", "Hover Equipped Wings", "ChargedWings" };
        private ButtonList wingModList;

        private Button startButton;
        private Button bulletsButton;
        private Button missilesButton;

        private bool pressedBackToTitle = false;
        private bool pressedStartButton = false;
        private int titleScreenTransitionTimer = 0;
        private int mainGameTransitionTimer = 0;

        public static void InitializeModificationScreen()
        {
            ModificationScreen newInstance = new ModificationScreen();
            newInstance.Initialize();
            Main.mainUI = newInstance;
        }

        public void Initialize()
        {
            Vector2 shipOffset = new Vector2(shipTexture.Width, shipTexture.Height);
            Vector2 shipPos = new Vector2(Main.desiredResolutionWidth / 2f, Main.desiredResolutionHeight / 2f) - shipOffset;
            shipBarrelsButton = new TextureCollisionButton(shipBarrels, shipPos, Color.Red, Color.White, 2, 1);
            shipPropellersButton = new TextureCollisionButton(shipPropellers, shipPos, Color.Red, Color.White, 2, 1);
            shipWingsButton = new TextureCollisionButton(shipWings, shipPos, Color.Red, Color.White, 2, 1);

            turretModTextures[0] = normalBarrelTexture;
            turretModTextures[1] = extendedBarrelTexture;
            turretModTextures[2] = powerfulBarrelTexture;
            turretModList = new ButtonList(3, new Vector2(5, 20), 70, 20, turretModTextures, turretModLabels, Color.White, Color.Orange, 2);
            turretModList.selectedItem = (int)Player.turretType;

            wingModTextures[0] = normalWingsTexture;
            wingModTextures[1] = thinCutWingsTexture;
            wingModTextures[2] = hoverEquippedWingsTexture;
            wingModTextures[3] = chargedWingsTexture;
            wingModList = new ButtonList(4, new Vector2(80, 20), 70, 20, wingModTextures, wingModLabels, Color.White, Color.Orange, 2);
            wingModList.selectedItem = (int)Player.wingType;

            Vector2 startButtonPos = new Vector2(Main.desiredResolutionWidth, Main.desiredResolutionHeight);
            startButton = new Button("Set Out", startButtonPos, Color.White, Color.LightYellow, buttonInteractionLayer: 1);
            startButton.buttonPosition -= new Vector2(startButton.buttonRect.Width, startButton.buttonRect.Height);

            bulletsButton = new Button(bulletTexture, new Vector2(30f, Main.desiredResolutionHeight - 18f), Color.White, Color.Orange, 0.6f, 0.65f, 1);
            missilesButton = new Button(missileTexture, bulletsButton.buttonPosition + new Vector2(16f, 0f), Color.White, Color.Orange, 0.6f, 0.65f, 1);
        }

        public override void Update()
        {
            turretModList.Update();
            wingModList.Update();
            shipBarrelsButton.Update();
            shipPropellersButton.Update();
            shipWingsButton.Update();
            startButton.Update();
            bulletsButton.Update();
            missilesButton.Update();

            HandleButtonInputs();

            if (Keyboard.GetState().IsKeyDown(Keys.Back))
            {
                pressedBackToTitle = true;
                Main.FadeOut(100);
            }
            if (pressedBackToTitle)
            {
                titleScreenTransitionTimer++;
                if (titleScreenTransitionTimer >= 100)
                {
                    Main.FadeIn(100);
                    titleScreenTransitionTimer = 0;
                    TitleScreen.InitializeTitleScreen();
                    DestroyInstance(this);
                }
            }
        }

        private void HandleButtonInputs()
        {
            if (shipBarrelsButton.buttonPressed)
            {
                if (!turretModList.expanded)
                {
                    Main.uiInteractionLayer = 2;
                    turretModList.Expand();
                }
            }
            if (shipWingsButton.buttonPressed)
            {
                if (!wingModList.expanded)
                {
                    Main.uiInteractionLayer = 2;
                    wingModList.Expand();
                }
            }

            if (startButton.buttonPressed)
            {
                Main.FadeOut(100);
                MusicManager.FadeOutInto(MusicManager.mainMusic, 100, 100);
                pressedStartButton = true;
            }
            if (pressedStartButton)
            {
                mainGameTransitionTimer++;
                if (mainGameTransitionTimer >= 100)
                {
                    mainGameTransitionTimer = 0;
                    AnimationHandler.PlayGameEntryAnimation();
                }
            }
            if (bulletsButton.buttonPressed)
            {
                Player.ammoType = Player.AmmoType.Bullets;
                bulletsButton.focused = true;
                missilesButton.focused = false;
            }
            if (missilesButton.buttonPressed)
            {
                Player.ammoType = Player.AmmoType.Missiles;
                bulletsButton.focused = false;
                missilesButton.focused = true;
            }

            if (turretModList.expanded)
            {
                for (int i = 0; i < turretModList.amountOfButtons; i++)
                {
                    if (turretModList.buttonPressed[i])
                    {
                        turretModList.Detract();
                        Main.uiInteractionLayer = 1;
                        Player.turretType = (Player.TurretTypes)i - 1;
                        break;
                    }
                }
            }
            if (wingModList.expanded)
            {
                for (int i = 0; i < wingModList.amountOfButtons; i++)
                {
                    if (wingModList.buttonPressed[i])
                    {
                        wingModList.Detract();
                        Main.uiInteractionLayer = 1;
                        Player.wingType = (Player.WingTypes)i - 1;
                        break;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawLayeredShip(spriteBatch);
            turretModList.Draw(spriteBatch);
            wingModList.Draw(spriteBatch);
            startButton.Draw(spriteBatch);
            bulletsButton.Draw(spriteBatch);
            missilesButton.Draw(spriteBatch);

            spriteBatch.DrawString(Main.mainFont, "Ammo:", new Vector2(3f, Main.desiredResolutionHeight - 20f), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }

        private void DrawLayeredShip(SpriteBatch spriteBatch)       //Probably not the best way to do it, but whatever
        {
            float shipScale = 2f;
            Vector2 shipOffset = new Vector2((shipTexture.Width * shipScale) / 2f, (shipTexture.Height * shipScale) / 2f);
            Vector2 shipPos = new Vector2(Main.desiredResolutionWidth / 2f, Main.desiredResolutionHeight / 2f) - shipOffset;

            spriteBatch.Draw(modificationHangarTexture, Vector2.Zero, Color.Gray);
            spriteBatch.Draw(shipTexture, shipPos, null, Color.White, 0f, Vector2.Zero, shipScale, SpriteEffects.None, 0f);
            shipBarrelsButton.Draw(spriteBatch);
            shipPropellersButton.Draw(spriteBatch);
            shipWingsButton.Draw(spriteBatch);
        }
    }
}
