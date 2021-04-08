using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_Of_The_Ship.UI
{
    /// <summary>
    /// A button that only responds when being hovered over active parts of the image. (Not invisible parts)
    /// </summary>
    public class TextureCollisionButton : UIObject
    {
        private Texture2D buttonTexture;

        public bool buttonHoveredOver = false;
        public bool buttonPressed = false;
        public Vector2 buttonPosition;
        public Rectangle buttonRect;
        public Color drawColor;

        private List<Point> collisionPoints = new List<Point>();        //The whole number values of Points are useful in this case
        private Color buttonActiveColor;
        private Color buttonInactiveColor;
        private float scale = 1f;

        public TextureCollisionButton(Texture2D texture, Vector2 position, Color activeColor, Color inactiveColor, int buttonScale)
        {
            buttonPosition = position;
            buttonTexture = texture;
            buttonRect = new Rectangle((int)position.X, (int)position.Y, buttonTexture.Width * buttonScale, buttonTexture.Height * buttonScale);
            scale = buttonScale;
            buttonActiveColor = activeColor;
            buttonInactiveColor = inactiveColor;

            Color[] colorArray = new Color[buttonTexture.Width * buttonTexture.Height];
            buttonTexture.GetData(colorArray);

            int rowPos = 0;     //The current X coordinate of the scan
            int columnPos = 0;      //The current Y coordiante of the scan
            for (int i = 0; i < colorArray.Length; i++)     //Getting the colors in the array and returning their positions IN THE IMAGE!!!
            {
                if (rowPos >= buttonTexture.Width)      //Resets the x and y if you reach the border of the image
                {
                    rowPos = 0;
                    columnPos++;
                }

                if (colorArray[i].A != 0)
                {
                    Point pixelPoint = new Point(rowPos, columnPos);       //This is the coordiante of the visible picture IN THE IMAGE!!!
                    collisionPoints.Add(pixelPoint);
                }

                rowPos++;
            }
        }

        public override void Update()
        {
            buttonPressed = false;
            buttonHoveredOver = false;
            drawColor = buttonInactiveColor;
            if (buttonRect.Contains(Main.mousePosition))
            {
                //Vector2 normalizedMousePosition = (Main.mousePosition - buttonPosition) / scale;      //Only works if the origin is at 0. Gets the coordinate of the mouse as if it were hovering over the texture in an image editor
                int pointX = (int)(Main.mousePosition.X - buttonPosition.X) / (int)scale;
                int pointY = (int)(Main.mousePosition.Y - buttonPosition.Y) / (int)scale;
                Point normalizedMousePosition = new Point(pointX, pointY);
                for (int point = 0; point < collisionPoints.Count; point++)
                {
                    if (normalizedMousePosition == collisionPoints[point])      //Detects if the mouse is in a colored area that was added to the list
                    {
                        buttonHoveredOver = true;
                        break;
                    }
                }

                Main.debugValue = normalizedMousePosition.ToString();
                if (buttonHoveredOver)
                {
                    drawColor = buttonActiveColor;
                    if (Main.mouseState.LeftButton == ButtonState.Pressed)
                    {
                        buttonPressed = true;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonTexture, buttonPosition, null, drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
