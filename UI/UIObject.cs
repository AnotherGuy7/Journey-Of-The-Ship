using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.UI
{
    public abstract class UIObject
    {
        public virtual void Update()
        { }

        public void DestroyInstance(UIObject uiObject)
        {
            Main.activeUI.Remove(uiObject);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        { }
    }
}
