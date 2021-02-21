using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.Projectiles
{
    public class Laser : Projectile
    {
        public static Texture2D laserTexture;

        private const int MaxLifeTime = 3 * 60;
        private const int LaserWidth = 3;
        private const int LaserHeight = 7;

        private int lifeTimer = 0;

        public static void NewLaser(Vector2 position, Vector2 velocity, bool friendly)
        {
            Laser currentInstance = new Laser();
            currentInstance.position = position;
            currentInstance.velocity = velocity;
            currentInstance.friendly = friendly;
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, LaserWidth, LaserHeight);
            Main.activeProjectiles.Add(currentInstance);
        }

        public override void Update()
        {
            lifeTimer++;
            if (lifeTimer > MaxLifeTime)
            {
                DestroyInstance(this);
                return;
            }

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(laserTexture, position, Color.White);
        }
    }
}
