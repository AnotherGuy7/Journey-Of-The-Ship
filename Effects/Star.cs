using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Journey_Of_The_Ship.Effects
{
    public class Star : VisualEffect
    {
        public static Texture2D starSpritesheet;

        public override bool backgroundEffect => true;

        private const int MaxFrames = 3;
        private const int StarWidth = 7;
        private const int StarHeight = 7;

        private Rectangle animRect;
        private Vector2 position;
        private Vector2 velocity = Vector2.Zero;
        private float rotation;
        private float drawAlpha = 1f;
        private float glowTimer = 0f;
        private int animationStyle = 0;


        public static void NewStarEffect(Vector2 position, Vector2 velocity, float rotation = 0f, int animationStyle = 0)
        {
            Star newInstance = new Star();
            newInstance.position = position;
            newInstance.velocity = velocity;
            newInstance.rotation = rotation;
            newInstance.animationStyle = animationStyle;
            Main.backgroundEffects.Add(newInstance);
        }

        public override void Update()
        {
            position += velocity;
            if (position.Y + StarHeight > Main.desiredResolutionHeight)
            {
                DestroyInstance(this);
            }

            if (animationStyle == 0)
            {
                rotation += 0.0008581f;
                animRect = new Rectangle(0, StarHeight, StarWidth, StarHeight);
            }
            else if (animationStyle == 1)
            {
                frameCounter++;
                if (frameCounter > 20)
                {
                    frame += 1;
                    frameCounter = 0;
                    if (frame >= MaxFrames)
                    {
                        frame = 0;
                    }
                    //drawAlpha = 0.5f + (frame / 10f);
                    animRect = new Rectangle(0, frame * StarHeight, StarWidth, StarHeight);
                }
            }

            glowTimer += 0.0045f;
            drawAlpha = Math.Abs((float)Math.Sin(glowTimer));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(starSpritesheet, position, animRect, Color.White * drawAlpha, rotation, new Vector2(StarWidth / 2f, StarHeight / 2f), 0.6f, SpriteEffects.None, 0f);
        }
    }
}