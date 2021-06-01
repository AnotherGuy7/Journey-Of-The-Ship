using Microsoft.Xna.Framework;

namespace Journey_Of_The_Ship.Projectiles
{
    public class Projectile : CollisionBody     //Not marked abstract cause it can be instantiated
    {
        public virtual bool continuous { get; }

        public bool friendly = false;

        public Vector2 velocity;

        public void DestroyInstance(Projectile projectile)
        {
            Main.activeProjectiles.Remove(projectile);
        }
    }
}
