using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.PowerUps;
using Microsoft.Xna.Framework;

namespace Journey_Of_The_Ship.Enemies
{
    public abstract class Enemy : CollisionBody
    {
        public Vector2 position;


        public void GenerateSmoke(Color startColor, Color endColor, int width, int height, int amount)
        {
            for (int s = 0; s < amount; s++)
            {
                Vector2 pos = position + new Vector2(Main.random.Next(0, width), Main.random.Next(0, height));
                Vector2 vel = new Vector2(Main.random.Next(-2, 2) / 20.5f, -0.03f);
                Smoke.NewSmokeParticle(pos, vel, startColor, endColor, 30, 90);
            }
        }

        public void SpawnGore(int goreType, int width, int height, int amount)
        {
            for (int g = 0; g < amount; g++)
            {
                Vector2 pos = position + new Vector2(Main.random.Next(0, width), Main.random.Next(0, height));
                Vector2 vel = new Vector2(Main.random.Next(-2, 3) / 5f, Main.random.Next(1, 5) / 5f);
                Gore.NewGoreParticle(goreType, pos, vel, 8f, 0.8f, Main.random.Next(0, 100) / 1000f);
            }
        }

        public void DropAbilities(int baseChance, Vector2 offset)
        {
            if (Main.random.Next(0, 101) <= baseChance)
            {
                AbilityDrop.NewAbilityDrop(Main.random.Next(0, 1), position + offset, new Vector2(0f, 0.3f), 15);
            }
        }

        public void DropPowerUp(int baseChance, Vector2 offset)
        {
            if (Main.random.Next(0, 101) <= baseChance)
            {
                PowerUp.NewPowerUp(Main.random.Next(0, 2 + 1), position + offset, new Vector2(0f, 0.3f), 15);
            }
        }

        public void DestroyInstance(Enemy enemy, bool addPoint = true)
        {
            if (addPoint)
                Main.gameScore += 1;

            Main.activeEntities.Remove(enemy);
        }
    }
}
