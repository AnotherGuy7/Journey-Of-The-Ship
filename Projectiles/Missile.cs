﻿using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Journey_Of_The_Ship.Projectiles
{
    public class Missile : Projectile
    {
        public override CollisionType[] colliderTypes => new CollisionType[2] { CollisionType.Enemies, CollisionType.Obstacles };
        public override CollisionType collisionType => CollisionType.Projectiles;

        public static Texture2D missileTexture;
        public static Texture2D targetLockedIndicator;

        private const int MaxLifeTime = 6 * 60;
        private const int MissileWidth = 5;
        private const int MissileHeight = 11;

        private int lifeTimer = 0;
        private bool enemyDetected = false;
        private float rotation = 0f;
        private Enemy target;
        private Color indicatorColor = Color.White;
        private int indicatorColorChangeTimer = 0;
        private int frame = 0;
        private int frameCounter = 0;

        public static void NewMissile(Vector2 position, Vector2 velocity, bool friendly)
        {
            Missile currentInstance = new Missile();
            currentInstance.position = position;
            currentInstance.velocity = velocity;
            currentInstance.friendly = friendly;
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, MissileWidth, MissileHeight);
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

            ExtraEffects();
            if (lifeTimer > 20)
            {
                if (target == null && !enemyDetected)
                {
                    CollisionBody[] activeEntitiesClone = Main.activeEntities.ToArray();
                    float lowestDistance = 120f;        //Maximum detection range
                    for (int i = 0; i < activeEntitiesClone.Length; i++)
                    {
                        Enemy enemy = activeEntitiesClone[i] as Enemy;
                        if (enemy != null)
                        {
                            float distance = Vector2.Distance(position, enemy.position);
                            if (distance <= lowestDistance)
                            {
                                target = enemy;
                                enemyDetected = true;
                                lowestDistance = distance;
                            }
                        }
                    }
                }
                else
                {
                    if (Main.activeEntities.Contains(target))
                    {
                        Vector2 newVelocity = target.center - position;
                        newVelocity.Normalize();
                        newVelocity *= 0.5f;
                        velocity = newVelocity;
                        rotation = (float)Math.Atan2(newVelocity.Y, newVelocity.X) + MathHelper.ToRadians(90f);

                        indicatorColorChangeTimer++;
                        if (indicatorColorChangeTimer >= 5)
                        {
                            if (indicatorColor == Color.White)
                            {
                                indicatorColor = Color.Red;
                            }
                            else
                            {
                                indicatorColor = Color.White;
                            }
                            indicatorColorChangeTimer = 0;
                        }
                    }
                }
            }

            frameCounter++;
            if (frameCounter >= 12)
            {
                frame++;
                frameCounter = 0;
                if (frame >= 4)
                {
                    frame = 0;
                }
            }


            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
        }

        private void ExtraEffects()
        {
            if (Main.random.Next(0, 2) == 0)
            {
                for (int s = 0; s < Main.random.Next(1, 4); s++)
                {
                    Vector2 angle = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                    float radius = 4f;
                    Vector2 offset = angle * radius;
                    Vector2 smokePosition = position + offset;
                    Vector2 velocity = angle;
                    velocity *= -0.6f;
                    Smoke.NewSmokeParticle(smokePosition, velocity, Color.Orange, Color.Gray, 5, 8, 8);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle animRect = new Rectangle(0, frame * MissileHeight, MissileWidth, MissileHeight);
            Vector2 missileOrigin = new Vector2(MissileWidth / 2f, MissileHeight / 2f);
            spriteBatch.Draw(missileTexture, position, animRect, Color.White, rotation, missileOrigin, 1f, SpriteEffects.None, 0f);
            if (target != null)
            {
                spriteBatch.Draw(targetLockedIndicator, target.center, indicatorColor);
            }
        }
    }
}