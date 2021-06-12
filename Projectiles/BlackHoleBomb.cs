using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Journey_Of_The_Ship.Projectiles
{
    public class BlackHoleBomb : Projectile
    {
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.None };
        public override CollisionType collisionType => CollisionType.None;

        public static Texture2D bombTexture;
        public static Texture2D accretionDiskTexture;
        public static SoundEffect blackHoleOpeningSound;
        public static SoundEffect blackHoleActiveSound;

        private const int MaxLifeTime = 15 * 60;
        private const int BombWidth = 5;
        private const int BombHeight = 5;

        private int lifeTimer = 0;
        private float accretionDiskRotation = 0f;
        private float accretionDiskAlpha = 0f;
        private int frame = 0;
        private int frameCounter = 0;
        private bool playedOpeningSound = false;
        private int activeSoundTimer = 0;
        private Rectangle animRect;

        public static void NewBlackHoleBomb(Vector2 position, Vector2 velocity, bool friendly)
        {
            BlackHoleBomb currentInstance = new BlackHoleBomb();
            currentInstance.position = position;
            currentInstance.velocity = velocity;
            currentInstance.friendly = friendly;
            currentInstance.hitbox = new Rectangle((int)currentInstance.position.X, (int)currentInstance.position.Y, BombWidth, BombHeight);
            Main.activeProjectiles.Add(currentInstance);
        }

        public override void Update()
        {
            lifeTimer++;
            if (lifeTimer > MaxLifeTime)
            {
                Main.StartScreenShake(30, 2);
                DestroyInstance(this);
                Main.shaderManager.DisableScreenShaders();
                return;
            }
            float strength = 0.01f;
            Vector2 center = position + new Vector2(BombWidth / 2f, BombHeight / 2f);
            float distance = 0.13f;
            Main.shaderManager.ActivateBlackHoleShader(strength, center, distance);
            if (lifeTimer > 2 * 60 && !playedOpeningSound)
            {
                playedOpeningSound = true;
                velocity = Vector2.Zero;
                blackHoleOpeningSound.Play();
            }
            if (lifeTimer > 3 * 60)
            {
                activeSoundTimer++;
                if (activeSoundTimer > 200)
                {
                    activeSoundTimer = 0;
                    blackHoleActiveSound.Play();
                }
                CollisionBody[] activeEntitiesClone = Main.activeEntities.ToArray();
                foreach (CollisionBody entity in activeEntitiesClone)
                {
                    Vector2 posAdd = position - entity.position;
                    posAdd.Normalize();
                    entity.position += posAdd * 1.6f;

                    if (Vector2.Distance(position, entity.position) <= 20f)
                    {
                        if (entity is Enemy)
                        {
                            Enemy enemy = entity as Enemy;
                            enemy.health -= 1;
                            if (enemy.health <= 0)
                            {
                                enemy.DestroyInstance(enemy);
                            }
                        }
                        else if (entity is Player)
                        {
                            Main.playerHealth -= 1;
                        }
                    }
                }
                Main.StartScreenShake(1, 4);
                if (accretionDiskAlpha <= 1f && lifeTimer > 5 * 60)
                {
                    accretionDiskAlpha += 0.04f;
                    for (int s = 0; s < Main.random.Next(1, 4); s++)
                    {
                        float radius = 11f;
                        float angle = Main.random.Next(0, 360);
                        float angleInRadians = MathHelper.ToRadians(angle);
                        Vector2 spawnPos = new Vector2((float)Math.Cos(angleInRadians), (float)Math.Sin(angleInRadians)) * radius;
                        Vector2 velocity = new Vector2(Main.random.Next(-5, 5 + 1) / 10f, Main.random.Next(-5, 5 + 1) / 10f);
                        Smoke.NewSmokeParticle(spawnPos, velocity, Color.Orange, Color.Red, (int)(accretionDiskAlpha / 0.2f), 5);
                    }
                }
            }

            frameCounter++;
            if (frameCounter >= 5)
            {
                frame += 1;
                frameCounter = 0;
                if (frame >= 4)
                {
                    frame = 0;
                }
            }
            accretionDiskRotation += 0.58104f;

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            animRect = new Rectangle(0, BombHeight * frame, BombWidth, BombHeight);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bombTexture, position, animRect, Color.White);

            Vector2 accretionDiskOrigin = new Vector2(accretionDiskTexture.Width / 2f, accretionDiskTexture.Height / 2f);
            spriteBatch.Draw(accretionDiskTexture, position + new Vector2(BombWidth / 2f, BombHeight / 2f), null, Color.White * accretionDiskAlpha, accretionDiskRotation, accretionDiskOrigin, 1f, SpriteEffects.None, 0f);
        }
    }
}
