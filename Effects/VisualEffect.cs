using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.Effects
{
    public abstract class VisualEffect
    {
        public virtual bool backgroundEffect { get; } = false;


        public int frame = 0;
        public int frameCounter = 0;

        public virtual void Update()
        {}

        public virtual void Draw(SpriteBatch spriteBatch)
        {}

        public void DestroyInstance(VisualEffect effectInstance)
        {
            if (!backgroundEffect)
            {
                Main.activeEffects.Remove(effectInstance);
            }
            else
            {
                Main.backgroundEffects.Remove(effectInstance);
            }
        }
    }
}
