using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Journey_Of_The_Ship.UI
{
    public class Slider : UIObject
    {
        public static Texture2D whitePixel;
        public static Texture2D sliderButtonTexture;

        private float value = 0f;
        private bool clickedOn = true;
        private int interactionLayer;

        public string sliderName = "";
        public Color drawColor;
        public Vector2 sliderPosition;      //Position of the actual slider button
        public Vector2 screenPosition;      //Position of the whole bar
        public Vector2 relativePosition;    //Position of the whole bar in relation to it's parent
        public Rectangle rect;

        public Slider(Vector2 relativePos, int width, int height, Color color, string label = "", float defaultValue = -1f, int sliderInteractionLayer = 1)
        {
            relativePosition = relativePos;
            rect = new Rectangle((int)relativePos.X, (int)relativePos.Y, width, height);
            drawColor = color;
            sliderPosition.Y = relativePos.Y + (height / 2f);
            sliderName = label + ":";
            interactionLayer = sliderInteractionLayer;
            if (defaultValue != -1f)
                SetValue(defaultValue);
        }

        public override void Update()
        {
            rect.X = (int)screenPosition.X;
            rect.Y = (int)screenPosition.Y;

            if (Main.uiInteractionLayer != interactionLayer)
                return;

            if (rect.Contains(Main.mousePosition))
            {
                if (Main.mouseState.LeftButton == ButtonState.Pressed)
                {
                    clickedOn = true;
                }
            }
            if (clickedOn)
            {
                if (Main.mouseState.LeftButton == ButtonState.Released)
                {
                    clickedOn = false;
                    return;
                }

                sliderPosition.X = MathHelper.Clamp(Main.mousePosition.X, screenPosition.X, screenPosition.X + rect.Width);
                value = (sliderPosition.X - screenPosition.X) / (float)rect.Width;
            }
        }

        public void SetValue(float sliderValue)
        {
            sliderPosition.X = relativePosition.X + (rect.Width * sliderValue);
            value = sliderValue;
        }

        public float GetValue()
        {
            return value;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(sliderButtonTexture.Width / 2f, sliderButtonTexture.Height / 2f);
            spriteBatch.Draw(sliderButtonTexture, sliderPosition, null, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);

            //Top line
            Vector2 topLinePosition = screenPosition + new Vector2(1f, 0f);
            Vector2 topLineScale = new Vector2(rect.Width - 2f, 1f);
            spriteBatch.Draw(whitePixel, topLinePosition, null, drawColor, 0f, Vector2.Zero, topLineScale, SpriteEffects.None, 0f);

            //Left line
            Vector2 leftLinePosition = screenPosition + new Vector2(0f, 1f);
            Vector2 leftLineScale = new Vector2(1f, rect.Height - 2f);
            spriteBatch.Draw(whitePixel, leftLinePosition, null, drawColor, 0f, Vector2.Zero, leftLineScale, SpriteEffects.None, 0f);

            //Right line
            Vector2 rightLinePosition = screenPosition + new Vector2(rect.Width - 1f, 1f);
            Vector2 rightLineScale = new Vector2(1f, rect.Height - 2f);
            spriteBatch.Draw(whitePixel, rightLinePosition, null, drawColor, 0f, Vector2.Zero, rightLineScale, SpriteEffects.None, 0f);

            //Left line
            Vector2 bottomLinePosition = screenPosition + new Vector2(1f, rect.Height - 1f);
            Vector2 bottomLineScale = new Vector2(rect.Width - 2f, 1f);
            spriteBatch.Draw(whitePixel, bottomLinePosition, null, drawColor, 0f, Vector2.Zero, bottomLineScale, SpriteEffects.None, 0f);

            if (sliderName != "")
            {
                float scale = 0.4f;
                Vector2 textSize = Main.mainFont.MeasureString(sliderName) * scale;
                spriteBatch.DrawString(Main.mainFont, sliderName, screenPosition - new Vector2(textSize.X + 1f, 0f), drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
