using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Journey_Of_The_Ship.PowerUps
{
    public class PowerUp : CollisionBody
    {
        public static Texture2D[] powerUpTextures = new Texture2D[5];
        public static Texture2D[] powerUpAuras = new Texture2D[5];

        public const int Attack = 0;
        public const int Health = 1;
        public const int Speed = 2;

        private Texture2D texture;
        private Texture2D auraTexture;
        private Vector2 position;
        private Vector2 velocity;
        private float scale = 1f;
        private float auraRotation = 0f;
        private int lifeTimer = 0;

        public int powerUpType = 0;
        private float auraTimer = 0;

        public static void NewPowerUp(int powerUpType, Vector2 position, Vector2 velocity, int lifeTime = 12)
        {
            PowerUp newinstance = new PowerUp();
            newinstance.texture = powerUpTextures[powerUpType];
            newinstance.auraTexture = powerUpAuras[powerUpType];
            newinstance.position = position;
            newinstance.velocity = velocity;
            newinstance.hitbox = new Rectangle((int)position.X, (int)position.Y, newinstance.texture.Width, newinstance.texture.Height);
            newinstance.lifeTimer = lifeTime;
            newinstance.powerUpType = powerUpType;
            Main.activeEntities.Add(newinstance);
        }

        public override void Update()
        {
            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            if (lifeTimer <= 0)
            {
                DestoryInstance(this);
            }

            auraTimer += 0.0045f;
            scale = 0.7f + ((float)Math.Sin(auraTimer) * 0.3f);
            auraRotation = auraTimer;
        }

        private void DestoryInstance(PowerUp powerUp)
        {
            Main.activeEntities.Remove(powerUp);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.7f, SpriteEffects.None, 0f);
            spriteBatch.Draw(auraTexture, position, null, Color.White, auraRotation, new Vector2(auraTexture.Width / 2f, auraTexture.Height / 2f), scale, SpriteEffects.None, 0f);
        }
    }
}
