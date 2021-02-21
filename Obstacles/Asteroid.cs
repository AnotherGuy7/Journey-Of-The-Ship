﻿using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Journey_Of_The_Ship.Obstacles
{
    public class Asteroid : CollisionBody
    {
        public static Texture2D[] asteroidTextures;

        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private float rotationToAdd = 0f;
        private int asteroidWidth = 23;
        private int asteroidHeight = 23;

        private int health = 5;
        private int lifeTimer = 30 * 60;

        private float rotation;
        private float scale;

        public static void NewAsteroid(int asteroidType, Vector2 position, Vector2 velocity, float rotationToAdd = 0f, float scale = 1f)
        {
            Asteroid newInstance = new Asteroid();
            newInstance.texture = asteroidTextures[asteroidType];
            newInstance.position = position;
            newInstance.velocity = velocity;
            newInstance.rotationToAdd = rotationToAdd;
            newInstance.scale = scale;
            newInstance.asteroidWidth = newInstance.texture.Width;
            newInstance.asteroidHeight = newInstance.texture.Height;
            newInstance.hitbox = new Rectangle((int)position.X, (int)position.Y, newInstance.asteroidWidth, newInstance.asteroidHeight);
            Main.activeEntities.Add(newInstance);
        }

        public override void Update()
        {
            CollisionBody[] bodiesArray = Main.activeProjectiles.ToArray();
            DetectCollisions(bodiesArray.ToList());

            position += velocity;
            Vector2 originOffset = new Vector2(asteroidWidth / 2f, asteroidHeight / 2f);
            hitbox.X = (int)position.X - (int)originOffset.X;
            hitbox.Y = (int)position.Y - (int)originOffset.Y;

            rotation += rotationToAdd;

            if (health <= 0)
            {
                SpawnGore(5);
                Main.StartScreenShake(8, 1);
                DestroyInstance(this);
            }

            lifeTimer--;
            if (lifeTimer <= 0)
            {
                DestroyInstance(this);
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
                    Explosion.NewExplosion(collidingProjectile.position, Vector2.Zero);
                    collidingProjectile.DestroyInstance(collidingProjectile);
                }
            }
        }

        public void DestroyInstance(Asteroid asteroid)
        {
            Main.gameScore += 1;
            Main.activeEntities.Remove(asteroid);
        }

        private void SpawnGore(int amount)
        {
            for (int g = 0; g < amount; g++)
            {
                int goreType = Main.random.Next(2, 3 + 1);
                Vector2 pos = position + new Vector2(Main.random.Next(0, asteroidWidth), Main.random.Next(0, asteroidHeight));
                Vector2 vel = new Vector2(Main.random.Next(-2, 3) / 5f, Main.random.Next(1, 5) / 5f);
                Gore.NewGoreParticle(goreType, pos, vel, 8f, 0.9f, Main.random.Next(0, 100) / 1000f);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation, new Vector2(asteroidWidth / 2f, asteroidHeight / 2f), scale, SpriteEffects.None, 0f);
        }
    }
}