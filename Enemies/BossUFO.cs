using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Journey_Of_The_Ship.Enemies
{
    public class BossUFO : CollisionBody
    {
        public override CollisionType collisionType => CollisionType.Enemies;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.FriendlyProjectiles };

        public static Texture2D[] bossUFOAnimationArray;

        private int frame = 0;
        private int frameCounter = 0;
        private int shootCounter = 0;
        private Vector2 shootVelocity = new Vector2(0f, 6f);
        private int health = 8;
        private int flyDirection = 1;

        private Vector2 leftTurretPosition = new Vector2(12f, 59f);
        private Vector2 rightTurretPosition = new Vector2(118f, 63f);

        public static void NewBossUFO(Vector2 position)
        {
            BossUFO currentInstance = new BossUFO();
            currentInstance.position = position;
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, bossUFOAnimationArray[0].Width, bossUFOAnimationArray[0].Height);
            Main.activeEntities.Add(currentInstance);
        }

        public override void Update()
        {
            AnimateShip();
            CollisionBody[] bodiesArray = Main.activeProjectiles.ToArray();
            DetectCollisions(bodiesArray.ToList());

            Vector2 velocity = Vector2.Zero;

            if (position.Y < 50f)
            {
                velocity.Y = 1f;
            }

            if (position.X < 0)
            {
                flyDirection = 1;
            }
            else if (position.X > 700 - hitbox.Width)
            {
                flyDirection = -1;
            }
            velocity.X = 4 * flyDirection;

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            shootCounter++;
            if (shootCounter >= 2 * 60)
            {
                if (Main.random.Next(0, 2) == 0)
                {
                    Laser.NewLaser(position + leftTurretPosition, shootVelocity);
                }
                else
                {
                    Laser.NewLaser(position + rightTurretPosition, shootVelocity);
                }
                shootCounter = 0;
            }

            if (health <= 0)
            {
                DestroyInstance(this);
            }
        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            if (collider is Projectile)
            {
                Projectile collidingProjectile = collider as Projectile;
                health -= 1;
                Explosion.NewExplosion(collidingProjectile.position, Vector2.Zero);
                collidingProjectile.DestroyInstance(collidingProjectile);
            }
        }

        private void AnimateShip()
        {
            frameCounter++;
            if (frameCounter >= 7)
            {
                frame++;
                frameCounter = 0;
                if (frame >= bossUFOAnimationArray.Length)
                {
                    frame = 0;
                }
            }
        }

        public void DestroyInstance(BossUFO boss)
        {
            Main.gameScore += 1;
            Main.activeEntities.Remove(boss);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bossUFOAnimationArray[frame], hitbox, Color.White);
        }
    }
}
