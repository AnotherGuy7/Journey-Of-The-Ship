using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.Projectiles
{
    public class ContinuousLaser : Projectile
    {
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Player };
        public override CollisionType collisionType => CollisionType.Projectiles;
        public override bool continuous => true;

        public static Texture2D laserSpritesheet;

        private const int LaserWidth = 7;
        private const int LaserHeight = 3;

        private bool shootingDown = false;
        private int frame = 0;
        private int frameCounter = 0;
        private Rectangle animRect;
        private Color laserColor;

        public static ContinuousLaser NewLaser(Vector2 position, bool down, Color color)
        {
            ContinuousLaser currentInstance = new ContinuousLaser();
            currentInstance.position = position;
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, LaserWidth, LaserHeight);
            currentInstance.shootingDown = down;
            currentInstance.laserColor = color;
            Main.activeProjectiles.Add(currentInstance);
            return currentInstance;
        }

        public override void Update()
        {
            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            if (shootingDown)
                hitbox.Height = (int)((Main.desiredResolutionHeight + 20f) - position.Y);
            else
                hitbox.Height = (int)((Main.desiredResolutionHeight + 20f) - position.Y);

            frameCounter++;
            if (frameCounter >= 3)
            {
                frame++;
                frameCounter = 0;
                if (frame >= 4)
                {
                    frame = 0;
                }
                animRect = new Rectangle(0, frame * LaserHeight, LaserWidth, LaserHeight);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < hitbox.Height / LaserHeight; i++)
            {
                Vector2 pos = position + new Vector2(0f, i * LaserHeight);
                spriteBatch.Draw(laserSpritesheet, pos, animRect, laserColor);
            }
        }
    }
}
