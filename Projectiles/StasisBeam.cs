using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.Projectiles
{
    public class StasisBeam : Projectile
    {
        public override CollisionType collisionType => CollisionType.EnemyProjectiles;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Player };
        public override bool continuous => true;


        public static Texture2D beamSpritesheet;

        private const int beamWidth = 5;
        private const int beamHeight = 5;

        private int frame = 0;
        private int frameCounter = 0;
        private Rectangle animRect;

        public static StasisBeam NewBeam(Vector2 position)
        {
            StasisBeam currentInstance = new StasisBeam();
            currentInstance.position = position;
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, beamWidth, beamHeight);
            Main.activeProjectiles.Add(currentInstance);
            return currentInstance;
        }

        public override void Update()
        {
            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            hitbox.Height = (int)((Main.desiredResolutionHeight + 20f) - position.Y);

            frameCounter++;
            if (frameCounter >= 3)
            {
                frame++;
                frameCounter = 0;
                if (frame >= 3)
                {
                    frame = 0;
                }
                animRect = new Rectangle(0, frame * beamHeight, beamWidth, beamHeight);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < hitbox.Height / beamHeight; i++)
            {
                Vector2 pos = position + new Vector2(0f, i * beamHeight);
                spriteBatch.Draw(beamSpritesheet, pos, animRect, Color.White);
            }
        }
    }
}
