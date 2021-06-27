using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.Obstacles;
using Journey_Of_The_Ship.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Journey_Of_The_Ship.Enemies
{
    public class RayEnemy : Enemy
    {
        public override CollisionType collisionType => CollisionType.Enemies;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.FriendlyProjectiles };
        public override int AmountOfHealth => 1;

        public static Texture2D raySpritesheet;
        public static SoundEffect laserSound;

        public override int Width => 7;
        public override int Height => 11;

        private int frame = 0;
        private int frameCounter = 0;
        private Vector2 shootOffset = new Vector2(0f, 11f);
        private int direction = 1;
        private int laserDurationTimer = 0;
        private int laserChargeUpTimer = 0;
        private bool shootingLaser = false;
        private Rectangle animRect;
        private ContinuousLaser laser;

        public static void NewRay(Vector2 position)
        {
            RayEnemy currentInstance = new RayEnemy();
            currentInstance.position = position;
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, currentInstance.Width, currentInstance.Height);
            currentInstance.health = currentInstance.AmountOfHealth;
            Main.activeEntities.Add(currentInstance);
        }

        public override void Update()
        {
            AnimateShip();
            CollisionBody[] bodiesArray = Main.activeProjectiles.ToArray();
            DetectCollisions(bodiesArray.ToList());

            Vector2 velocity = Vector2.Zero;

            if (position.Y < 40f)
            {
                velocity.Y = 0.3f;
            }
            else
            {
                if (position.X + Width > Main.desiredResolutionWidth)
                {
                    direction = -1;
                }
                if (position.X <= 1)
                {
                    direction = 1;
                }

                velocity.X = 0.4f * direction;

                if (shootingLaser)
                {
                    velocity *= 0.5f;
                }
            }

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            center = position + new Vector2(Width / 2f, Height / 2f);

            if (!shootingLaser)
            {
                laserChargeUpTimer++;
                if (laserChargeUpTimer >= 4 * 60)
                {
                    shootingLaser = true;
                    laser = ContinuousLaser.NewLaser(position, true, Color.White);
                }
            }
            else
            {
                laserDurationTimer++;
                if (laserDurationTimer >= 5 * 60)
                {
                    frame = 0;
                    shootingLaser = false;
                    laserDurationTimer = 0;
                    laserChargeUpTimer = 0;

                    laser.DestroyInstance(laser);
                    laser = null;
                }

                if (laser != null)
                {
                    laser.position = position + shootOffset;
                }
            }
        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            if (collider is Projectile)
            {
                Projectile collidingProjectile = collider as Projectile;
                health -= 1;
                if (health <= 0)
                {
                    DropPowerUp(1, new Vector2(Width / 2f, Height / 2f));
                    SpawnGore(Main.random.Next(6, 7 + 1), Width, Height, 1);
                    GenerateSmoke(Color.Orange, Color.Gray, Width, Height, 16);
                    if (laser != null)
                    {
                        laser.DestroyInstance(laser);
                        laser = null;
                    }
                    DestroyInstance(this);
                }
                Explosion.NewExplosion(collidingProjectile.position, Vector2.Zero);
                collidingProjectile.DestroyInstance(collidingProjectile);
            }
            if (collider is Asteroid)
            {
                Asteroid collidingAsteroid = collider as Asteroid;
                collidingAsteroid.health -= 2;
                Explosion.NewExplosion(collidingAsteroid.position, Vector2.Zero);
                DropPowerUp(1, new Vector2(Width / 2f, Height / 2f));
                SpawnGore(Main.random.Next(6, 7 + 1), Width, Height, 1);
                GenerateSmoke(Color.Orange, Color.Gray, Width, Height, 16);
                if (laser != null)
                {
                    laser.DestroyInstance(laser);
                    laser = null;
                }
                DestroyInstance(this);
            }
        }


        private void AnimateShip()
        {
            frameCounter += laserChargeUpTimer / 60;
            if (frameCounter >= 15)
            {
                frame++;
                frameCounter = 0;
                if (frame >= 4)
                {
                    frame = 0;
                }
                animRect = new Rectangle(0, frame * Height, Width, Height);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(raySpritesheet, position, animRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
