using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.PowerUps
{
    public class AbilityDrop : CollisionBody
    {
        public override CollisionType collisionType => CollisionType.None;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Player };

        public static Texture2D[] abilityTextures = new Texture2D[1];
        public static Texture2D abilityRect;

        public const int RapidFire = 0;

        private Texture2D texture;
        private Vector2 velocity;
        private Color rectColor;
        private float scale = 1f;
        private float[] rectRotations = new float[2];
        private int lifeTimer = 0;

        public int abilityType = 0;
        private Color rapidFireColor = new Color(210, 121, 231);
        private Color[] colorsArray = new Color[1];

        public static void NewAbilityDrop(int abilityType, Vector2 position, Vector2 velocity, int lifeTime = 12)
        {
            AbilityDrop newinstance = new AbilityDrop();
            newinstance.BuildColorsArray();
            newinstance.texture = abilityTextures[abilityType];
            newinstance.rectColor = newinstance.colorsArray[abilityType];
            newinstance.position = position;
            newinstance.velocity = velocity;
            newinstance.hitbox = new Rectangle((int)position.X, (int)position.Y, newinstance.texture.Width, newinstance.texture.Height);
            newinstance.lifeTimer = lifeTime;
            newinstance.abilityType = abilityType;
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

            rectRotations[0] += 0.014f;
            rectRotations[1] -= 0.014f;
        }

        public void BuildColorsArray()
        {
            colorsArray[0] = rapidFireColor;
        }

        public void DestoryInstance(AbilityDrop abilityDrop)
        {
            Main.activeEntities.Remove(abilityDrop);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(abilityRect, position, null, rectColor, rectRotations[0], new Vector2(abilityRect.Width / 2f, abilityRect.Height / 2f), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(abilityRect, position, null, rectColor, rectRotations[1], new Vector2(abilityRect.Width / 2f, abilityRect.Height / 2f), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position, null, Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 0.7f, SpriteEffects.None, 0f);
        }
    }
}
