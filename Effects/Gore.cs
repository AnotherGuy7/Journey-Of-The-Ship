using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.Effects
{
    public class Gore : VisualEffect
    {
        public static Texture2D[] goreTextures;

        public const int UFOGorePiece = 0;
        public const int UFOGorePiece2 = 1;
        public const int AsteroidChunkPiece1 = 2;
        public const int AsteroidChunkPiece2 = 3;
        public const int SlicerGorePiece = 4;
        public const int SlicerGorePiece2 = 5;

        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity = Vector2.Zero;
        private float drawRotation;
        private float scale;
        private float drawAlpha = 1f;

        private float rotationToAdd = 0f;
        private int lifeTimer = 0;

        public static void NewGoreParticle(int goreTextureID, Vector2 position, Vector2 velocity, float lifeTime = 1.2f, float scale = 1f, float rotation = 0f)
        {
            Gore newInstance = new Gore();
            newInstance.texture = goreTextures[goreTextureID];
            newInstance.position = position;
            newInstance.velocity = velocity;
            newInstance.lifeTimer = (int)(lifeTime * 60f);
            newInstance.scale = scale;
            newInstance.rotationToAdd = rotation;
            Main.activeEffects.Add(newInstance);
        }

        public override void Update()
        {
            lifeTimer--;
            if (lifeTimer <= 0)
            {
                DestroyInstance(this);
            }

            position += velocity;

            drawRotation += rotationToAdd;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White * drawAlpha, drawRotation, new Vector2(texture.Width / 2f, texture.Height / 2f), scale, SpriteEffects.None, 0f);
        }
    }
}