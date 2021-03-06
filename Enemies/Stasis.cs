﻿using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.Obstacles;
using Journey_Of_The_Ship.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Journey_Of_The_Ship.Enemies
{
    public class Stasis : Enemy
    {
        public override CollisionType collisionType => CollisionType.Enemies;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.FriendlyProjectiles };
        public override int AmountOfHealth => 1;

        public static Texture2D stabilizerSpritesheet;
        public static SoundEffect beamSound;

        public override int Width => 15;
        public override int Height => 9;

        private int frame = 0;
        private int frameCounter = 0;
        private Vector2 shootOffset = new Vector2(5f, 9f);
        private int beamDurationTimer = 0;
        private int beamChargeUpTimer = 0;
        private bool shootingBeam = false;
        private Rectangle animRect;
        private StasisBeam beam;
        private SoundEffectInstance beamSoundInstance;

        public static void NewStasis(Vector2 position)
        {
            Stasis currentInstance = new Stasis();
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

            if (beamSoundInstance == null)
            {
                beamSoundInstance = beamSound.CreateInstance();
            }

            Vector2 velocity = Vector2.Zero;

            if (position.Y < 40f)
            {
                velocity.Y = 0.3f;
            }

            if (!shootingBeam)
            {
                if (Main.player.position.X > position.X + 8f)
                {
                    velocity.X = 0.1f;
                }
                else if (Main.player.position.X < position.X - 8f)
                {
                    velocity.X = -0.1f;
                }
                else
                {
                    beamChargeUpTimer++;
                    if (beamChargeUpTimer >= 4 * 60)
                    {
                        shootingBeam = true;
                        beam = StasisBeam.NewBeam(position + shootOffset);
                        beamChargeUpTimer = 0;
                    }
                }
            }
            else
            {
                velocity.X = 0;
            }


            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            center = position + new Vector2(Width / 2f, Height / 2f);

            if (shootingBeam)
            {
                beamDurationTimer++;
                if (beamDurationTimer >= 9 * 60)
                {
                    shootingBeam = false;
                    beamDurationTimer = 0;

                    if (beam != null)
                    {
                        beam.DestroyInstance(beam);
                        beam = null;
                    }
                }
                if (beamSoundInstance.State != SoundState.Playing)
                {
                    beamSoundInstance.Play();
                }

                if (beam != null)
                {
                    beam.position = position + shootOffset;
                }
            }
            else
            {
                if (beamSoundInstance.State == SoundState.Playing)
                    beamSoundInstance.Stop();
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
                    SpawnGore(Main.random.Next(8, 10 + 1), Width, Height, Main.random.Next(1, 2 + 1));
                    GenerateSmoke(Color.Orange, Color.Gray, Width, Height, 16);
                    if (beam != null)
                    {
                        beam.DestroyInstance(beam);
                        beam = null;
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
                SpawnGore(Main.random.Next(8, 10 + 1), Width, Height, Main.random.Next(1, 2 + 1));
                GenerateSmoke(Color.Orange, Color.Gray, Width, Height, 16);
                if (beam != null)
                {
                    beam.DestroyInstance(beam);
                    beam = null;
                }
                DestroyInstance(this);
            }
        }


        private void AnimateShip()
        {
            frameCounter++;
            if (frameCounter >= 16)
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
            spriteBatch.Draw(stabilizerSpritesheet, position, animRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
