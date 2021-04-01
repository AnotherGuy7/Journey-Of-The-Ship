using Journey_Of_The_Ship.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Journey_Of_The_Ship.PowerUps
{
    public class PowerUp : CollisionBody
    {
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Player };
        public override CollisionType collisionType => CollisionType.PowerUp;

        public static Texture2D[] powerUpTextures = new Texture2D[4];
        public static Texture2D powerUpAura;
        public static Texture2D powerUpRing;

        public const int Attack = 0;
        public const int Health = 1;
        public const int Speed = 2;

        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private Color auraColor;
        private float scale = 1f;
        private float auraRotation = 0f;
        private int lifeTimer = 0;

        public int powerUpType = 0;
        private float auraTimer = 0;

        private Color attackColor = new Color(238, 152, 121);
        private Color healthColor = new Color(128, 231, 121);
        private Color speedColor = new Color(121, 124, 238);
        private Color rapidFireColor = new Color(210, 121, 231);
        private Color[] colorsArray = new Color[4];

        public static void NewPowerUp(int powerUpType, Vector2 position, Vector2 velocity, int lifeTime = 12)
        {
            PowerUp newinstance = new PowerUp();
            newinstance.BuildColorsArray();
            newinstance.texture = powerUpTextures[powerUpType];
            newinstance.auraColor = newinstance.colorsArray[powerUpType];
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
            hitbox.X = (int)position.X - (hitbox.Width / 2);
            hitbox.Y = (int)position.Y - (hitbox.Height / 2);

            if (lifeTimer <= 0)
            {
                DestoryInstance(this);
            }

            auraTimer += 0.045f;
            scale = 0.7f + ((float)Math.Sin(auraTimer) * 0.3f);
            auraRotation = auraTimer;
        }

        public void BuildColorsArray()
        {
            colorsArray[0] = attackColor;
            colorsArray[1] = healthColor;
            colorsArray[2] = speedColor;
            colorsArray[3] = rapidFireColor;
        }

        public void DestoryInstance(PowerUp powerUp)
        {
            Main.activeEntities.Remove(powerUp);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(powerUpAura, position, null, auraColor, auraRotation, new Vector2(powerUpAura.Width / 2f, powerUpAura.Height / 2f), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(powerUpRing, position, null, auraColor, 0f, new Vector2(powerUpRing.Width / 2f, powerUpRing.Height / 2f), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position, null, Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.7f, SpriteEffects.None, 0f);
        }
    }
}
