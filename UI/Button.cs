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
        public Vector2 buttonPosition;
        public Color drawColor;
        public ButtonStyle buttonStyle = ButtonStyle.MainMenu;
        public bool buttonHoveredOver = false;
        public bool buttonPressed = false;
        public bool focused = false;
        public float scale = 1f;
        public string stringText;
        public bool visible = true;

        private Vector2 textSize;
        private Color inactiveButtonColor;
        private Color activeButtonColor;
        private Vector2 textOutlineOffset;
        private int interactionLayer;
        private float defaultScale = 1f;
        private float activeScale = 1f;

        /// <summary>
        /// A button with partial borders around it.
        /// </summary>
        public Button(string text, Vector2 position, Color inactiveColor, Color activeColor, float inactiveScale = 0.6f, float activeScale = 0.65f, int buttonInteractionLayer = 1)
        {
            stringText = text;
            textSize = Main.mainFont.MeasureString(text);
            int width = (int)(textSize.X * inactiveScale);
            int height = (int)(textSize.Y * inactiveScale);
            buttonRect = new Rectangle((int)position.X - (width / 2), (int)position.Y - (height / 2), width, height);
            buttonPosition = new Vector2(buttonRect.X, buttonRect.Y);
            inactiveButtonColor = inactiveColor;
            activeButtonColor = activeColor;
            buttonStyle = ButtonStyle.MainMenu;
            scale = inactiveScale;
            defaultScale = inactiveScale;
            this.activeScale = activeScale;
            interactionLayer = buttonInteractionLayer;
        }

        /// <summary>
        /// A button with text and an icon. Outlined with white borders.
        /// </summary>
        public Button(Texture2D texture, string text, int width, int height, Vector2 position, Color inactiveColor, Color activeColor, float inactiveScale = 1f, float activeScale = 1f, int buttonInteractionLayer = 1)
        {
            buttonIcon = texture;
            stringText = text;
            textSize = Main.mainFont.MeasureString(text);
            buttonRect = new Rectangle((int)position.X - (width / 2), (int)position.Y - (height / 2), width, height);
            buttonPosition = new Vector2(buttonRect.X, buttonRect.Y);
            inactiveButtonColor = inactiveColor;
            activeButtonColor = activeColor;
            buttonStyle = ButtonStyle.ButtonWithIcon;
            scale = inactiveScale;
            defaultScale = inactiveScale;
            this.activeScale = activeScale;
            interactionLayer = buttonInteractionLayer;
        }

        /// <summary>
        /// A button with only an icon. Outlined with white borders.
        /// </summary>
        public Button(Texture2D texture, Vector2 position, Color inactiveColor, Color activeColor, float inactiveScale = 1f, float activeScale = 1f, int buttonInteractionLayer = 1)
        {
            buttonIcon = texture;
            buttonRect = new Rectangle((int)position.X, (int)position.Y, (int)(texture.Width * inactiveScale), (int)(texture.Height * inactiveScale));
            buttonPosition = new Vector2(buttonRect.X, buttonRect.Y);
            inactiveButtonColor = inactiveColor;
            activeButtonColor = activeColor;
            buttonStyle = ButtonStyle.IconOnly;
            scale = inactiveScale;
            defaultScale = inactiveScale;
            this.activeScale = activeScale;
            interactionLayer = buttonInteractionLayer;
        }

        public enum ButtonStyle
        {
            MainMenu,
            ButtonWithIcon,
            IconOnly
        }

        public override void Update()
        {
            if (!visible)
                return;

            scale = defaultScale;
            buttonPressed = false;
            buttonHoveredOver = false;
            drawColor = inactiveButtonColor;
            buttonRect.X = (int)buttonPosition.X;
            buttonRect.Y = (int)buttonPosition.Y;
            textOutlineOffset = new Vector2(buttonRect.Width / 1.8f, buttonRect.Height / 2f) * scale;

            if (Main.uiInteractionLayer != interactionLayer)
                return;

            if (focused)
            {
                scale = activeScale;
                drawColor = activeButtonColor;
                textOutlineOffset += new Vector2(0.5f);
            }

            if (buttonRect.Contains(Main.mousePosition))
            {
                scale = activeScale;
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
            if (!visible)
                return;

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

                DrawBorderLines(spriteBatch, buttonPosition);

                Vector2 iconPosition = new Vector2(buttonRect.X + 3f, buttonRect.Y + 3f);
                float iconScaleX = ((buttonRect.Width - 2f) * 0.4f) / buttonRect.Width;
                float iconScaleY = (buttonRect.Height - 6f) / buttonIcon.Height;
                Vector2 iconScale = new Vector2(iconScaleX, iconScaleY);
                spriteBatch.Draw(buttonIcon, iconPosition, null, Color.White, 0f, Vector2.Zero, iconScale, SpriteEffects.None, 0f);

                Vector2 textPosition = iconPosition + new Vector2((buttonIcon.Width * iconScaleX) + 3f, 0f);
                float textScaleX = ((buttonRect.Width - 2f) * 0.6f) / buttonRect.Width;
                float textScaleY = (buttonRect.Height - 8f) / textSize.Y;
                Vector2 textScale = new Vector2(textScaleX, textScaleY); ;
                spriteBatch.DrawString(Main.mainFont, stringText, textPosition, drawColor, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                //Main.debugValue = iconScale.ToString();
            }
            else if (buttonStyle == ButtonStyle.IconOnly)
            {
                Vector2 buttonPosition = new Vector2(buttonRect.X, buttonRect.Y);
                Vector2 iconPosition = buttonPosition + new Vector2(buttonRect.Width / 2f, buttonRect.Height / 2f);
                Vector2 iconOrigin = new Vector2(buttonIcon.Width / 2f, buttonIcon.Height / 2f);

                DrawBorderLines(spriteBatch, buttonPosition);

                spriteBatch.Draw(buttonIcon, iconPosition, null, drawColor, 0f, iconOrigin, scale, SpriteEffects.None, 0f);
            }

            //Vector2 position2 = new Vector2(buttonRect.X, buttonRect.Y);
            //spriteBatch.DrawString(Main.mainFont, Main.mousePosition + "; " + buttonHoveredOver, position2, drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private void DrawBorderLines(SpriteBatch spriteBatch, Vector2 position)
        {
            //Top line
            Vector2 topLinePosition = position + new Vector2(1f, 0f);
            Vector2 topLineScale = new Vector2(buttonRect.Width - 2f, 1f);
            //topLineScale.X *= scale;
            spriteBatch.Draw(whitePixel, topLinePosition, null, drawColor, 0f, Vector2.Zero, topLineScale, SpriteEffects.None, 0f);

            //Left line
            Vector2 leftLinePosition = position + new Vector2(0f, 1f);
            Vector2 leftLineScale = new Vector2(1f, buttonRect.Height - 2f);
            //leftLineScale.Y *= scale;
            spriteBatch.Draw(whitePixel, leftLinePosition, null, drawColor, 0f, Vector2.Zero, leftLineScale, SpriteEffects.None, 0f);

            //Right line
            Vector2 rightLinePosition = position + new Vector2(buttonRect.Width - 1f, 1f);
            Vector2 rightLineScale = new Vector2(1f, buttonRect.Height - 2f);
            //rightLineScale.Y *= scale;
            spriteBatch.Draw(whitePixel, rightLinePosition, null, drawColor, 0f, Vector2.Zero, rightLineScale, SpriteEffects.None, 0f);

            //Left line
            Vector2 bottomLinePosition = position + new Vector2(1f, buttonRect.Height - 1f);
            Vector2 bottomLineScale = new Vector2(buttonRect.Width - 2f, 1f);
            //bottomLineScale.X *= scale;
            spriteBatch.Draw(whitePixel, bottomLinePosition, null, drawColor, 0f, Vector2.Zero, bottomLineScale, SpriteEffects.None, 0f);
        }
    }
}
