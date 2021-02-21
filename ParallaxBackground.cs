using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship
{
    public class ParallaxBackground
    {
        public const float parallaxStrength = 0.03f;

        public static Texture2D[] spaceSet1;

        public int backGroundType = 0;

        private Texture2D[] parallaxBackgrounds = new Texture2D[2];
        private Vector2 backgroundPosition = new Vector2(-5f, -5f);
        private Vector2 secondBackgroundPosition = new Vector2(-5f, -5f);

        public ParallaxBackground()
        {
            parallaxBackgrounds = spaceSet1;
            backgroundPosition = new Vector2(-5f, -5f);
            secondBackgroundPosition = new Vector2(-5f, 1f + parallaxBackgrounds[1].Height);
        }

        public void Update()
        {
            backgroundPosition.Y += parallaxStrength;
            secondBackgroundPosition.Y += parallaxStrength;

            if (backgroundPosition.Y > parallaxBackgrounds[0].Height)
            {
                backgroundPosition = new Vector2(-5f, -5f - parallaxBackgrounds[0].Height);
            }
            if (secondBackgroundPosition.Y > parallaxBackgrounds[1].Height)
            {
                secondBackgroundPosition = new Vector2(-5f, -5f - parallaxBackgrounds[1].Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(parallaxBackgrounds[0], backgroundPosition, Color.White);
            spriteBatch.Draw(parallaxBackgrounds[1], secondBackgroundPosition, Color.White);
        }
    }
}
