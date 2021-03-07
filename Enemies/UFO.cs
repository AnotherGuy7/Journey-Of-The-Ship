using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.Obstacles;
using Journey_Of_The_Ship.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Journey_Of_The_Ship.Enemies
{
    public class UFO : Enemy
    {
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Projectiles };
        public override CollisionType collisionType => CollisionType.Enemies;

        public static Texture2D ufoSpritesheet;
        public static SoundEffect shootSound;
        public static SoundEffect deathSound;

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
        private bool playedSound = false;

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
                shootSound.Play(Main.soundEffectVolume, Main.random.Next(0, 101) / 100f, 0f);
                Laser.NewLaser(position + shootOffset, shootVelocity, false);
                shootCounter = 0;
            }

            if (health <= 0)
            {
                deathTimer++;
                if (deathTimer >= 80)
                {
                    SpawnGore(Main.random.Next(0, 1 + 1), UFOWidth, UFOHeight, 4);
                    Explosion.NewExplosion(position + new Vector2(UFOWidth / 2f, UFOHeight / 2f), Vector2.Zero);
                    PowerUps.PowerUp.NewPowerUp(PowerUps.PowerUp.Attack, position + new Vector2(UFOWidth / 2f, UFOHeight / 2f), new Vector2(0f, 0.3f), 15);
                    Main.StartScreenShake(8, 1);
                    DestroyInstance(this);
                }
                if (!playedSound)
                {
                    deathSound.Play(Main.soundEffectVolume, 0f, 0f);
                    playedSound = true;
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
                Asteroid collidingAsteroid = collider as Asteroid;
                collidingAsteroid.health -= 2;
                Explosion.NewExplosion(collidingAsteroid.position, Vector2.Zero);
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

        private void SpawnDusts()
        {
            if (health == 2)
            {
                smokeSpawnTimer++;
                if (smokeSpawnTimer % 30 == 0)
                {
                    GenerateSmoke(Color.Orange, Color.Gray, UFOWidth, UFOHeight, 2);
                }
            }
            else if (health == 1)
            {
                smokeSpawnTimer++;
                if (smokeSpawnTimer % 15 == 0)
                {
                    GenerateSmoke(Color.Orange, Color.Gray, UFOWidth, UFOHeight, 6);
                    rumbleOffset = new Vector2(Main.random.Next(-1, 2) / 2f, Main.random.Next(-1, 2) / 2f);
                }
            }
            else if (health == 0)
            {
                GenerateSmoke(Color.Orange, Color.Gray, UFOWidth, UFOHeight, 4);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ufoSpritesheet, position + rumbleOffset, animRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
