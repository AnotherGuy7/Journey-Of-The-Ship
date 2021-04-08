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
        public static Texture2D whitePixel;

        public Texture2D buttonIcon;
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
            buttonStyle = ButtonStyle.MainMenu;
            interactionLayer = buttonInteractionLayer;
        }

        public Button(Texture2D texture, string text, int width, int height, Vector2 position, Color inactiveColor, Color activeColor, int buttonInteractionLayer = 1)
        {
            buttonIcon = texture;
            stringText = text;
            textSize = Main.mainFont.MeasureString(text);
            buttonRect = new Rectangle((int)position.X - (width / 2), (int)position.Y - (height / 2), width, height);
            inactiveButtonColor = inactiveColor;
            activeButtonColor = activeColor;
            buttonStyle = ButtonStyle.ButtonWithIcon;
            interactionLayer = buttonInteractionLayer;
        }

        public Button(Texture2D texture, Vector2 position, Color inactiveColor, Color activeColor, float buttonScale = 1f, int buttonInteractionLayer = 1)
        {
            buttonIcon = texture;
            buttonRect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            inactiveButtonColor = inactiveColor;
            activeButtonColor = activeColor;
            buttonStyle = ButtonStyle.IconOnly;
            interactionLayer = buttonInteractionLayer;
            scale = buttonScale;
        }

        public enum ButtonStyle
        {
            MainMenu,
            ButtonWithIcon,
            IconOnly
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
            else if (buttonStyle == ButtonStyle.ButtonWithIcon)
            {
                Vector2 buttonPosition = new Vector2(buttonRect.X, buttonRect.Y);

                //Top line
                Vector2 topLinePosition = buttonPosition + new Vector2(1f, 0f);
                Vector2 topLineScale = new Vector2(buttonRect.Width - 2f, 1f);
                spriteBatch.Draw(whitePixel, topLinePosition, null, drawColor, 0f, Vector2.Zero, topLineScale, SpriteEffects.None, 0f);

                //Left line
                Vector2 leftLinePosition = buttonPosition + new Vector2(0f, 1f);
                Vector2 leftLineScale = new Vector2(1f, buttonRect.Height - 2f);
                spriteBatch.Draw(whitePixel, leftLinePosition, null, drawColor, 0f, Vector2.Zero, leftLineScale, SpriteEffects.None, 0f);

                //Right line
                Vector2 rightLinePosition = buttonPosition + new Vector2(buttonRect.Width - 1f, 1f);
                Vector2 rightLineScale = new Vector2(1f, buttonRect.Height - 2f);
                spriteBatch.Draw(whitePixel, rightLinePosition, null, drawColor, 0f, Vector2.Zero, rightLineScale, SpriteEffects.None, 0f);

                //Left line
                Vector2 bottomLinePosition = buttonPosition + new Vector2(1f, buttonRect.Height - 1f);
                Vector2 bottomLineScale = new Vector2(buttonRect.Width - 2f, 1f);
                spriteBatch.Draw(whitePixel, bottomLinePosition, null, drawColor, 0f, Vector2.Zero, bottomLineScale, SpriteEffects.None, 0f);

                Vector2 iconPosition = new Vector2(buttonRect.X + 3f + 1f, buttonRect.Y + buttonRect.Height / 2f);
                spriteBatch.Draw(buttonIcon, iconPosition, null, Color.White);

                Vector2 textPosition = iconPosition + new Vector2(buttonIcon.Width + 3f, 0f);
                spriteBatch.DrawString(Main.mainFont, stringText, textPosition, drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else if (buttonStyle == ButtonStyle.IconOnly)
            {
                Vector2 buttonPosition = new Vector2(buttonRect.X, buttonRect.Y);
                Vector2 buttonOrigin = new Vector2(buttonIcon.Width / 2f, buttonIcon.Height / 2f);
                spriteBatch.Draw(buttonIcon, buttonPosition, null, drawColor, 0f, buttonOrigin, scale, SpriteEffects.None, 0f);
            }

            //Vector2 position2 = new Vector2(buttonRect.X, buttonRect.Y);
            //spriteBatch.DrawString(Main.mainFont, Main.mousePosition + "; " + buttonHoveredOver, position2, drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
