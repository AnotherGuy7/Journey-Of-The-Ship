﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.Effects
{
    public class Planet : VisualEffect
    {
        public static Texture2D[] planetsTextureArray;

        public override bool backgroundEffect => true;

        private Texture2D appliedTexture;
        private Vector2 position;
        private Vector2 velocity = Vector2.Zero;
        private float scale;
        private Color drawColor;
        private float rotation;

        public static void NewPlanetEffect(Vector2 position, Vector2 velocity, float scale = 0.5f, float rotation = 0f)
        {
            Planet newInstance = new Planet();
            newInstance.position = position;
            newInstance.velocity = velocity;
            newInstance.scale = scale;
            float depthDarnkess = (255f - (40f * (0.5f / scale))) / 255f;
            newInstance.drawColor = new Color(depthDarnkess, depthDarnkess, depthDarnkess);
            newInstance.rotation = rotation;
            newInstance.appliedTexture = planetsTextureArray[Main.random.Next(0, planetsTextureArray.Length)];
            Main.backgroundEffects.Add(newInstance);
        }

        public override void Update()
        {
            position += velocity;
            if (position.Y > Main.desiredResolutionHeight)
            {
                DestroyInstance(this);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(appliedTexture, position, null, drawColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}