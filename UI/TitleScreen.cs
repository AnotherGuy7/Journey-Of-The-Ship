using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Journey_Of_The_Ship.UI
{
    public class TitleScreen : UIObject
    {
        private SettingsScreen settingsScreen = new SettingsScreen();
        private ModificationScreen modificationScreen = new ModificationScreen();

        private Button startButton = new Button("Start", new Vector2(84f, 100f), Color.White, Color.LightYellow);
        private Button settingsButton = new Button("Settings", new Vector2(84f, 130f), Color.White, Color.LightYellow);
        private Button quitButton = new Button("Quit", new Vector2(84f, 160f), Color.White, Color.Red);

        private int gameStartTimer = 0;
        private bool pressedStartButton = false;
        private bool pressedSettingsButton = false;
        private bool settingsScreenLeaving = false;
        private bool settingsScreenShowing = false;

        public static void InitializeTitleScreen()
        {
            TitleScreen newInstance = new TitleScreen();
            Main.mainUI = newInstance;
        }

        public override void Update()
        {
            startButton.Update();
            settingsButton.Update();
            quitButton.Update();
            if (settingsScreenShowing)
            {
                settingsScreen.Update();
            }

            if (startButton.buttonPressed)
            {
                Main.FadeOut(100);
                pressedStartButton = true;
            }
            if (pressedStartButton)
            {
                gameStartTimer++;
                if (gameStartTimer > 100)
                {
                    gameStartTimer = 0;
                    Main.FadeIn(100);
                    ModificationScreen.InitializeModificationScreen();
                    DestroyInstance(this);
                }
            }

            if (settingsButton.buttonPressed && !settingsScreenLeaving)
            {
                settingsScreenShowing = true;
                pressedSettingsButton = true;
            }
            if (pressedSettingsButton)
            {
                settingsScreen.position.X *= 0.6f;
                if (settingsScreen.position.X <= 7f)
                {
                    pressedSettingsButton = false;
                    Main.uiInteractionLayer = 2;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Back))
            {
                settingsScreenLeaving = true;
            }
            if (settingsScreenLeaving)
            {
                settingsScreen.position.X *= 1.4f;
                if (settingsScreen.position.X >= Main.desiredResolutionWidth)
                {
                    settingsScreenLeaving = false;
                    settingsScreenShowing = false;
                    Main.uiInteractionLayer = 1;
                    Main.saveManager.SaveGame();
                }
            }

            if (quitButton.buttonPressed)
            {
                Main.CloseGame();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            startButton.Draw(spriteBatch);
            settingsButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);
            if (settingsScreenShowing)
            {
                settingsScreen.Draw(spriteBatch);
            }
        }
    }
}
