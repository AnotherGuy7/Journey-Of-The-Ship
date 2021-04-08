using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_Of_The_Ship.UI
{
    public class ButtonList : UIObject
    {
        private int amountOfButtons;
        private Vector2 position;
        private Button[] buttons;
        private Texture2D[] buttonIcons;
        private string[] buttonLabels;

        public ButtonList(int amountOfButtons, Vector2 listPosition, int averageButtonWidth, int averageButtonHeight, Texture2D[] listButtonIcons, string[] listButtonLabels)
        {
            this.amountOfButtons = amountOfButtons;
            buttons = new Button[amountOfButtons];
            position = listPosition;
            buttonIcons = listButtonIcons;
            buttonLabels = listButtonLabels;

            for (int i = 0; i < amountOfButtons; i++)
            {
                Vector2 buttonPosition = listPosition + new Vector2(0f, averageButtonHeight * i);
                buttons[i] = new Button(listButtonIcons[i], listButtonLabels[i], averageButtonWidth, averageButtonHeight, buttonPosition, Color.White, Color.Orange, 2);

            }
        }

        public override void Update()
        {
            foreach (Button button in buttons)
            {
                button.Update();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Button button in buttons)
            {
                button.Draw(spriteBatch);
            }
        }
    }
}
