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
        //private Button shipBarrelsButton = new Button(shipBarrels, new Vector2(Main.desiredResolutionWidth / 2f, Main.desiredResolutionHeight / 2f), Color.White, Color.Red, 5f, 1);
        //private Button shipPropellersButton = new Button(shipBarrels, new Vector2(Main.desiredResolutionWidth / 2f, Main.desiredResolutionHeight / 2f), Color.White, Color.Red, 5f, 1);
        //private Button shipWingsButton = new Button(shipBarrels, new Vector2(Main.desiredResolutionWidth / 2f, Main.desiredResolutionHeight / 2f), Color.White, Color.Red, 5f, 1);
        private TextureCollisionButton shipBarrelsButton = new TextureCollisionButton(shipBarrels, new Vector2(Main.desiredResolutionWidth / 2f, Main.desiredResolutionHeight / 2f), Color.Red, Color.White, 2);
        private TextureCollisionButton shipPropellersButton = new TextureCollisionButton(shipPropellers, new Vector2(Main.desiredResolutionWidth / 2f, Main.desiredResolutionHeight / 2f), Color.Red, Color.White, 2);
        private TextureCollisionButton shipWingsButton = new TextureCollisionButton(shipWings, new Vector2(Main.desiredResolutionWidth / 2f, Main.desiredResolutionHeight / 2f), Color.Red, Color.White, 2);


        public static Texture2D normalBarrelTexture;
        public static Texture2D extendedBarrelTexture;
        public static Texture2D powerfulBarrelTexture;
        private Texture2D[] turretModTextures = new Texture2D[3];
        public string[] turretModLabels = new string[3] { "Normal", "Longer Cannon Barrels", "More Powerful Barrels" };
        private ButtonList turretModList;

        private bool pressedBackToTitle = false;
        private int titleScreenTransitionTimer = 0;

        public static void InitializeModificationScreen()
        {
            ModificationScreen newInstance = new ModificationScreen();
            newInstance.Initialize();
            Main.mainUI = newInstance;
        }

        public void Initialize()
        {
            turretModTextures[0] = normalBarrelTexture;
            turretModTextures[1] = extendedBarrelTexture;
            turretModTextures[2] = powerfulBarrelTexture;
            turretModList = new ButtonList(3, new Vector2(40, 30), 50, 25, turretModTextures, turretModLabels);
        }

        public override void Update()
        {
            turretModList.Update();
            shipBarrelsButton.Update();
            shipPropellersButton.Update();
            shipWingsButton.Update();

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

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawLayeredShip(spriteBatch);
            turretModList.Draw(spriteBatch);
        }

        private void DrawLayeredShip(SpriteBatch spriteBatch)       //Probably not the best way to do it, but whatever
        {
            Vector2 shipPos = new Vector2(Main.desiredResolutionWidth / 2f, Main.desiredResolutionHeight / 2f);
            Vector2 shipOrigin = new Vector2(shipTexture.Width / 2f, shipTexture.Height / 2f);
            float shipScale = 2f;

            spriteBatch.Draw(shipTexture, shipPos, null, Color.White, 0f, Vector2.Zero, shipScale, SpriteEffects.None, 0f);
            shipBarrelsButton.Draw(spriteBatch);
            shipPropellersButton.Draw(spriteBatch);
            shipWingsButton.Draw(spriteBatch);
        }
    }
}
