using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.Obstacles;
using Journey_Of_The_Ship.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Journey_Of_The_Ship.Enemies
{
    public class Slicer : Enemy
    {
        public override CollisionType[] colliderTypes => new CollisionType[2] { CollisionType.Player, CollisionType.Projectiles };
        public override CollisionType collisionType => CollisionType.Enemies;
        public override int AmountOfHealth => 1;

        public static Texture2D slicerSpritesheet;
        public static Texture2D slicerAfterImageTexture;
        public static SoundEffect slicerAirCutSound;
        public static SoundEffect slicerChargeUpSound;

        public override int Width => 23;
        public override int Height => 23;

        private const float SlicerScale = 0.8f;
        private const int DashChargeUpTime = 180;
        private const float SlicerDetectionRange = 50f;

        private int frame = 0;
        private Rectangle animRect;
        private Vector2 velocity;
        private float rotation = 0f;

        private int dashTimer = 0;
        private int dashChargeTimer = 0;
        private bool playerDetected = false;
        private bool dashing = false;
        private int slicerAirCutTimer = 0;
        private Vector2 dashVelocity;
        private List<Vector2> afterImagePositions = new List<Vector2>();
        private List<float> afterImageAlpha = new List<float>();
        private List<float> afterImageRotations = new List<float>();

        public static void NewSlicer(Vector2 position)
        {
            Slicer currentInstance = new Slicer();
            currentInstance.position = position;
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, currentInstance.Width, currentInstance.Height);
            currentInstance.velocity = new Vector2(0f, 0.3f);
            currentInstance.health = currentInstance.AmountOfHealth;
            Main.activeEntities.Add(currentInstance);
        }

        public override void Update()
        {
            AnimateShip();
            HandleAfterImages();
            CollisionBody[] bodiesArray = Main.activeProjectiles.ToArray();
            DetectCollisions(bodiesArray.ToList());

            float rotationToAdd = 0f;
            Vector2 playerPos = Main.player.position;
            if (Vector2.Distance(position, playerPos) <= SlicerDetectionRange && !playerDetected)
            {
                playerDetected = true;
                velocity = Vector2.Zero;
            }

            if (playerDetected && !dashing)
            {
                dashChargeTimer++;
                rotationToAdd = dashChargeTimer / 680f;
                if (dashChargeTimer >= DashChargeUpTime)
                {
                    dashing = true;
                    dashVelocity = playerPos - position;
                    dashVelocity.Normalize();
                    dashVelocity *= 1.3f;
                }
                if (dashChargeTimer == 1)
                {
                    slicerChargeUpSound.Play();
                }
            }
            else
            {
                rotationToAdd = 0.02f;
            }
            if (dashing)
            {
                dashTimer++;
                rotationToAdd = dashChargeTimer / 680f;
                velocity = dashVelocity;

                slicerAirCutTimer++;
                if (slicerAirCutTimer >= 11)
                {
                    slicerAirCutSound.Play();
                }
            }

            position += velocity;
            hitbox.X = (int)position.X - (Width / 2);
            hitbox.Y = (int)position.Y - (Height / 2);
            center = position;
            rotation += rotationToAdd;

            if (health <= 0)
            {
                DropPowerUp(1, new Vector2(Width / 2f, Height / 2f));
                SpawnGore(Main.random.Next(4, 5 + 1), Width, Height, Main.random.Next(1, 2 + 1));
                GenerateSmoke(Color.Orange, Color.Gray, Width, Height, 16);
                DestroyInstance(this);
            }
            if (position.Y > Main.desiredResolutionHeight + 50)
            {
                DestroyInstance(this, false);
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
                    Explosion.NewExplosion(collidingProjectile.position, Vector2.Zero);
                    collidingProjectile.DestroyInstance(collidingProjectile);
                }
            }
            if (collider is Asteroid)
            {
                Asteroid collidingAsteroid = collider as Asteroid;
                collidingAsteroid.health -= 1;
                Explosion.NewExplosion(collidingAsteroid.position, Vector2.Zero);
                health = 0;
            }
        }

        private void HandleAfterImages()
        {
            if (dashTimer > 0 && dashTimer % 2 == 0)
            {
                afterImageAlpha.Add(0.8f);
                afterImagePositions.Add(position);
                afterImageRotations.Add(rotation);
            }

            if (afterImageAlpha.Count > 0)
            {
                int currentAfterImageCount = afterImageAlpha.Count;
                for (int a = 0; a < currentAfterImageCount; a++)
                {
                    if (a >= afterImageAlpha.Count)
                    {
                        break;
                    }
                    if (afterImageAlpha[a] > 0)
                    {
                        afterImageAlpha[a] -= 0.05f;
                        if (afterImageAlpha[a] <= 0f)
                        {
                            afterImageAlpha.RemoveAt(a);
                            afterImagePositions.RemoveAt(a);
                            afterImageRotations.RemoveAt(a);
                            a--;
                        }
                    }
                }
            }
        }

        private void AnimateShip()
        {
            frame = dashChargeTimer / (DashChargeUpTime / 5);
            if (frame >= 5)
            {
                frame = 4;
            }
            animRect = new Rectangle(0, frame * Height, Width, Height);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(Width / 2f, Height / 2f);
            for (int a = 0; a < afterImageAlpha.Count; a++)
            {
                if (afterImageAlpha[a] > 0f)
                {
                    Vector2 position = afterImagePositions[a];
                    float rotation = afterImageRotations[a];
                    spriteBatch.Draw(slicerAfterImageTexture, position, null, Color.White * afterImageAlpha[a], rotation, origin, SlicerScale, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.Draw(slicerSpritesheet, position, animRect, Color.White, rotation, origin, SlicerScale, SpriteEffects.None, 0f);
        }
    }
}
