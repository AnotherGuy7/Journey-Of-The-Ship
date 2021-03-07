using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Journey_Of_The_Ship
{
    public abstract class CollisionBody
    {
        public Rectangle hitbox;

        /// <summary>
        /// An array of what this object can collide with.
        /// </summary>
        public virtual CollisionType[] colliderTypes { get; }

        /// <summary>
        /// The type of the collision of this object.
        /// </summary>
        public virtual CollisionType collisionType { get; }

        public enum CollisionType
        {
            Player,
            Enemies,
            Obstacles,
            Projectiles,
            PowerUp
        }

        public virtual void Initialize()
        {}

        public virtual void Update()
        {}

        public virtual void Draw(SpriteBatch spriteBatch)
        {}

        public void DetectCollisions(List<CollisionBody> possibleIntersectors)
        {
            foreach (CollisionBody intersector in possibleIntersectors)
            {
                if (hitbox.Intersects(intersector.hitbox))
                {
                    for (int c = 0; c < intersector.colliderTypes.Length; c++)
                    {
                        if (intersector.colliderTypes[c] == collisionType)
                        {
                            HandleCollisions(intersector, intersector.collisionType);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A method that gets called whenever a collision happens.
        /// </summary>
        /// <param name="collider"> The collider. </param>
        /// <param name="colliderType"> The collision type of the collider. </param>
        public virtual void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {}
    }
}
