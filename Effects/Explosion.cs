using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.Effects
{
    public class Explosion : VisualEffect
    {
        public static Texture2D explosionSpritesheet;
        public static SoundEffect explosionSound;

        private const int MaxFrames = 5;
        private const int ExplosionWidth = 16;
        private const int ExplosionHeight = 16;

        private int frameAdd = 0;
        private Vector2 position;
        private Vector2 velocity = Vector2.Zero;
        private Rectangle animRect;
        private float rotation;

        private bool playedSound = false;

        public static void NewExplosion(Vector2 position, Vector2 velocity, float rotation = 0f)
        {
            Explosion newInstance = new Explosion();
            newInstance.position = position;
            newInstance.velocity = velocity;
            newInstance.rotation = rotation;
            newInstance.frameAdd = Main.random.Next(0, 1 + 1) * 4;
            newInstance.animRect = new Rectangle(0, 0, ExplosionWidth, ExplosionHeight);
            Main.activeEffects.Add(newInstance);
        }

        public override void Update()
        {
            frameCounter++;
            if (frameCounter >= 4)
            {
                frame++;
                frameCounter = 0;
                animRect = new Rectangle(0, (frame + frameAdd) * ExplosionHeight, ExplosionWidth, ExplosionHeight);
                if (frame >= MaxFrames)
                {
                    frame = 0;
                    DestroyInstance(this);
                }
            }

            if (velocity != Vector2.Zero)
            {
                position += velocity;
            }

            if (!playedSound)
            {
                explosionSound.Play();
                playedSound = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(explosionSpritesheet, position, animRect, Color.White, rotation, new Vector2(ExplosionWidth / 2f, ExplosionHeight / 2f),1f, SpriteEffects.None, 0f);
        }
    }
}
