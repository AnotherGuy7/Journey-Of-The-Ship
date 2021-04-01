using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_Of_The_Ship.UI
{
    public class Button : UIObject
    {
        public static Texture2D menuStyleTop;
        public static Texture2D menuStyleBottom;

        public Rectangle buttonRect;
        public Color drawColor;
        public ButtonStyle buttonStyle = ButtonStyle.MainMenu;
        public bool buttonHoveredOver = false;
        public bool buttonPressed = false;
        public float scale = 0.6f;
        public string stringText;

        private Vector2 textSize;
        private Color inactiveButtonColor;
        private Color activeButtonColor;
        private Vector2 textOutlineOffset;
        private int interactionLayer;

        public Button(string text, Vector2 position, Color inactiveColor, Color activeColor, int buttonInteractionLayer = 1)
        {
            stringText = text;
            textSize = Main.mainFont.MeasureString(text);
            int width = (int)(textSize.X * scale);
            int height = (int)(textSize.Y * scale);
            buttonRect = new Rectangle((int)position.X - (width / 2), (int)position.Y - (height / 2), width, height);
            inactiveButtonColor = inactiveColor;
            activeButtonColor = activeColor;
            interactionLayer = buttonInteractionLayer;
        }

        public enum ButtonStyle
        {
            MainMenu
        }

        public override void Update()
        {
            scale = 0.6f;
            buttonPressed = false;
            buttonHoveredOver = false;
            drawColor = inactiveButtonColor;
            textOutlineOffset = new Vector2(buttonRect.Width / 1.8f, buttonRect.Height / 2f) * scale;

            if (Main.uiInteractionLayer != interactionLayer)
                return;

            if (buttonRect.Contains(Main.mousePosition))
            {
                scale = 0.65f;
                buttonHoveredOver = true;
                drawColor = activeButtonColor;
                textOutlineOffset += new Vector2(0.5f);
                if (Main.mouseState.LeftButton == ButtonState.Pressed)
                {
                    buttonPressed = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (buttonStyle == ButtonStyle.MainMenu)
            {
                Vector2 position = new Vector2(buttonRect.X, buttonRect.Y);
                spriteBatch.DrawString(Main.mainFont, stringText, position, drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                Vector2 textCenter = position + new Vector2(buttonRect.Width / 2f, buttonRect.Height / 2f);
                Vector2 outlineOrigin = new Vector2(menuStyleTop.Width / 2f, menuStyleTop.Height / 2f);
                spriteBatch.Draw(menuStyleTop, textCenter - textOutlineOffset, null, drawColor, 0f, outlineOrigin, scale, SpriteEffects.None, 0f);

                Vector2 bottomOffset = new Vector2(0f, 2f);
                spriteBatch.Draw(menuStyleBottom, textCenter + textOutlineOffset + bottomOffset, null, drawColor, 0f, outlineOrigin, scale, SpriteEffects.None, 0f);
            }

            //Vector2 position2 = new Vector2(buttonRect.X, buttonRect.Y);
            //spriteBatch.DrawString(Main.mainFont, Main.mousePosition + "; " + buttonHoveredOver, position2, drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
