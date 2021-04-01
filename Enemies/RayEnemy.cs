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
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Projectiles };
        public override CollisionType collisionType => CollisionType.Enemies;

        public static Texture2D raySpritesheet;
        public static SoundEffect laserSound;

        private const int RayWidth = 7;
        private const int RayHeight = 11;

        private int frame = 0;
        private int frameCounter = 0;
        private Vector2 shootOffset = new Vector2(0f, 11f);
        private int health = 2;
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
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, RayWidth, RayHeight);
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
                if (position.X + RayWidth > Main.desiredResolutionWidth)
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
                if (collidingProjectile.friendly)
                {
                    health -= 1;
                    if (health <= 0)
                    {
                        DropPowerUp(1, new Vector2(RayWidth / 2f, RayHeight / 2f));
                        SpawnGore(Main.random.Next(6, 7 + 1), RayWidth, RayHeight, 1);
                        GenerateSmoke(Color.Orange, Color.Gray, RayWidth, RayHeight, 16);
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
            }
            if (collider is Asteroid)
            {
                Asteroid collidingAsteroid = collider as Asteroid;
                collidingAsteroid.health -= 2;
                Explosion.NewExplosion(collidingAsteroid.position, Vector2.Zero);
                DropPowerUp(1, new Vector2(RayWidth / 2f, RayHeight / 2f));
                SpawnGore(Main.random.Next(6, 7 + 1), RayWidth, RayHeight, 1);
                GenerateSmoke(Color.Orange, Color.Gray, RayWidth, RayHeight, 16);
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
                animRect = new Rectangle(0, frame * RayHeight, RayWidth, RayHeight);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(raySpritesheet, position, animRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
