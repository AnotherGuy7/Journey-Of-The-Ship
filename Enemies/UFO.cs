using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.Obstacles;
using Journey_Of_The_Ship.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Journey_Of_The_Ship.Enemies
{
    public class UFO : CollisionBody
    {
        public static Texture2D ufoSpritesheet;
        public Vector2 position;

        private const int UFOWidth = 27;
        private const int UFOHeight = 27;
        //private const int MaxFrames = 6;
        private const int FlyFrames = 5;
        private const int DeathFrame = 5;

        private int frame = 0;
        private int frameCounter = 0;
        private int shootCounter = 0;
        private int smokeSpawnTimer = 0;
        private int deathTimer = 0;
        private Vector2 shootVelocity = new Vector2(0f, 3f);
        private Vector2 shootOffset = new Vector2(12f, 24f);
        private Vector2 rumbleOffset;
        private Vector2 deathVelocity;
        private int health = 3;
        private Rectangle animRect;

        public static void NewUFO(Vector2 position)
        {
            UFO currentInstance = new UFO();
            currentInstance.position = position;
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, UFOWidth, UFOHeight);
            Main.activeEntities.Add(currentInstance);
        }

        public override void Update()
        {
            SpawnDusts();
            AnimateShip();
            CollisionBody[] bodiesArray = Main.activeProjectiles.ToArray();
            DetectCollisions(bodiesArray.ToList());

            Vector2 velocity = Vector2.Zero;

            if (position.Y < 33f)
            {
                velocity.Y = 0.3f;
            }
            if (deathTimer != 0)
            {
                if (deathVelocity == Vector2.Zero)
                {
                    deathVelocity = new Vector2(Main.random.Next(-2, 3), Main.random.Next(-2, 3));
                }
                velocity = deathVelocity * ((float)deathTimer / 80f);
            }

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            shootCounter++;
            if (shootCounter >= 4 * 60 && health > 0)
            {
                Laser.NewLaser(position + shootOffset, shootVelocity, false);
                shootCounter = 0;
            }

            if (health <= 0)
            {
                deathTimer++;
                if (deathTimer >= 80)
                {
                    SpawnGore(4);
                    Explosion.NewExplosion(position + new Vector2(UFOWidth / 2f, UFOHeight / 2f), Vector2.Zero);
                    Main.StartScreenShake(8, 1);
                    DestroyInstance(this);
                }
            }
        }

        public override void HandleCollisions(CollisionBody collider)
        {
            if (collider is Projectile)
            {
                Projectile collidingProjectile = collider as Projectile;
                if (collidingProjectile.friendly)
                {
                    health -= 1;
                    if (deathTimer > 0)
                    {
                        deathTimer = 80;
                    }
                    Explosion.NewExplosion(collidingProjectile.position, Vector2.Zero);
                    collidingProjectile.DestroyInstance(collidingProjectile);
                }
            }
            if (collider is Asteroid)
            {
                health = 0;
            }
        }

        private void AnimateShip()
        {
            frameCounter++;
            if (frameCounter >= 7)
            {
                frame++;
                frameCounter = 0;
                if (frame >= FlyFrames)
                {
                    frame = 0;
                }
                animRect = new Rectangle(0, frame * UFOHeight, UFOWidth, UFOHeight);
            }
            if (health <= 0 && deathTimer >= 70)
            {
                animRect = new Rectangle(0, DeathFrame * UFOHeight, UFOWidth, UFOHeight);
            }
        }

        public void DestroyInstance(UFO ufo)
        {
            Main.gameScore += 1;
            Main.activeEntities.Remove(ufo);
        }

        private void GenerateSmoke(int amount)
        {
            for (int s = 0; s < amount; s++)
            {
                Vector2 pos = position + new Vector2(Main.random.Next(0, UFOWidth), Main.random.Next(0, UFOHeight));
                Vector2 vel = new Vector2(Main.random.Next(-2, 2) / 20.5f, -0.03f);
                Smoke.NewSmokeParticle(pos, vel, Color.Orange, Color.Gray, 30, 90);
            }
        }

        private void SpawnDusts()
        {
            if (health == 2)
            {
                smokeSpawnTimer++;
                if (smokeSpawnTimer % 30 == 0)
                {
                    GenerateSmoke(2);
                }
            }
            else if (health == 1)
            {
                smokeSpawnTimer++;
                if (smokeSpawnTimer % 15 == 0)
                {
                    GenerateSmoke(6);
                    rumbleOffset = new Vector2(Main.random.Next(-1, 2) / 2f, Main.random.Next(-1, 2) / 2f);
                }
            }
            else if (health == 0)
            {
                GenerateSmoke(4);
            }
        }

        private void SpawnGore(int amount)
        {
            for (int g = 0; g < amount; g++)
            {
                int goreType = Main.random.Next(0, 1 + 1);
                Vector2 pos = position + new Vector2(Main.random.Next(0, UFOWidth), Main.random.Next(0, UFOHeight));
                Vector2 vel = new Vector2(Main.random.Next(-2, 3) / 5f, Main.random.Next(1, 5) / 5f);
                Gore.NewGoreParticle(goreType, pos, vel, 8f, 0.8f, Main.random.Next(0, 100) / 1000f);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ufoSpritesheet, position + rumbleOffset, animRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
