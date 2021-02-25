using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Journey_Of_The_Ship.Effects
{
    public class Smoke : VisualEffect
    {
        public static Texture2D whitePixelTexture;

        private const int SmokeWidth = 4;
        private const int SmokeHeight = 4;

        private Vector2 position;
        private Vector2 velocity = Vector2.Zero;
        private Color smokeColor = Color.White;
        private float rotation;
        private float scale;
        private float drawAlpha = 1f;

        private int colorChangeTimerStart = 0;
        private int colorChangeTimer = 0;
        private int lifeTimer = 0;
        private int fadeTime = 0;
        private Color startColor;
        private Color endColor;


        public static void NewSmokeParticle(Vector2 position, Vector2 velocity, Color startColor, Color endColor, int colorChangeTime, int lifeTime, int fadeTime = 60, float scale = 0.4f, float rotation = 0f)
        {
            Smoke newInstance = new Smoke();
            newInstance.position = position;
            newInstance.velocity = velocity;
            newInstance.rotation = rotation;
            newInstance.scale = scale;
            newInstance.smokeColor = startColor;
            newInstance.startColor = startColor;
            newInstance.endColor = endColor;
            newInstance.lifeTimer = lifeTime;
            newInstance.fadeTime = fadeTime;
            newInstance.colorChangeTimer = colorChangeTime;
            newInstance.colorChangeTimerStart = colorChangeTime;
            Main.activeEffects.Add(newInstance);
        }

        public override void Update()
        {
            lifeTimer--;
            if (lifeTimer <= fadeTime)
            {
                drawAlpha = (float)lifeTimer / (float)fadeTime;
            }
            if (lifeTimer <= 0)
            {
                DestroyInstance(this);
            }

            position += velocity;

            if (colorChangeTimer > 0)
            {
                colorChangeTimer--;
                smokeColor.R = (byte)MathHelper.Lerp(startColor.R, endColor.R, 1f - ((float)colorChangeTimer / (float)colorChangeTimerStart));
                smokeColor.G = (byte)MathHelper.Lerp(startColor.G, endColor.G, 1f - ((float)colorChangeTimer / (float)colorChangeTimerStart));
                smokeColor.B = (byte)MathHelper.Lerp(startColor.B, endColor.B, 1f - ((float)colorChangeTimer / (float)colorChangeTimerStart));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(whitePixelTexture, position, null, smokeColor * drawAlpha, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}