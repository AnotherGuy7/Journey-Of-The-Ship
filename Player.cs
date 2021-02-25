using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.PowerUps;
using Journey_Of_The_Ship.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Journey_Of_The_Ship
{
    public class Player : CollisionBody
    {
        public static Texture2D playerSpritesheet;
        public static Texture2D playerAfterImageTexture;

        private const int PlayerWidth = 21;
        private const int PlayerHeight = 19;
        private const int MaxFrames = 4;
        private const float MoveSpeed = 1.5f;
        private const int DashDetectionTime = 5;
        private const int DashDurationTime = 10;
        private const int DashCooldownTime = 120;

        public Vector2 position;

        private float shootSpeed = 2f;
        private int shootTimer = 0;
        private int shootLevel = 1;
        private int frame = 0;
        private int frameCounter = 0;
        private int explosionCounter = 0;
        private bool canMove = false;
        private bool dying = false;
        private Rectangle animationRect;

        private int dashTimer = 0;
        private int dashCooldown = 0;
        private int[] dashKeyPressTimers = new int[2];
        private bool[] dashKeyCanPressAgain = new bool[2];
        private int dashDirection = 1;

        private Vector2 leftTurretOffset = new Vector2(2f, 5f);
        private Vector2 centerTurretOffset = new Vector2(8.5f, -1f);
        private Vector2 rightTurretOffset = new Vector2(18f, 5f);

        private List<Vector2> afterImagePositions = new List<Vector2>();
        private List<float> afterImageAlpha = new List<float>();

        public override void Initialize()
        {
            dying = false;
            explosionCounter = 0;
            hitbox = new Rectangle((int)position.X, (int)position.Y, PlayerWidth, PlayerHeight);
        }

        public override void Update()
        {
            if (shootTimer > 0)
                shootTimer--;

            AnimateShip();
            HandleAfterImages();
            CollisionBody[] bodiesArray = Main.activeProjectiles.ToArray();
            DetectCollisions(bodiesArray.ToList());
            Vector2 velocity = Vector2.Zero;
            canMove = !dying;

            if (canMove)
            {
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.W) && position.Y > 0)
                {
                    velocity.Y -= MoveSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.A) && position.X > 0)
                {
                    velocity.X -= MoveSpeed;
                    dashKeyPressTimers[0] = DashDetectionTime;
                }
                if (keyboardState.IsKeyDown(Keys.S) && position.Y < Main.desiredResolutionHeight - hitbox.Height)
                {
                    velocity.Y += MoveSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.D) && position.X < Main.desiredResolutionWidth - hitbox.Width)
                {
                    velocity.X += MoveSpeed;
                    dashKeyPressTimers[1] = DashDetectionTime;
                }
                if (keyboardState.IsKeyDown(Keys.Space) && shootTimer <= 0)
                {
                    Shoot();
                }
                UpdatePlayerDash(keyboardState, velocity);
            }
            if (dashTimer > 0)
            {
                dashTimer--;
                velocity = new Vector2(MoveSpeed * 3f * dashDirection, 0f);
            }

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            if (dying)
            {
                explosionCounter++;
                if (explosionCounter % 10 == 0)
                {
                    if (explosionCounter >= 5 * 60)
                    {
                        Main.gameState = Main.GameStates.GameState_GameOver;
                    }
                    float explosionOffsetX = Main.random.Next(0, PlayerWidth);
                    float explosionOffsetY = Main.random.Next(0, PlayerHeight);
                    Explosion.NewExplosion(position + new Vector2(explosionOffsetX, explosionOffsetY), Vector2.Zero);
                }
            }
        }

        private void AnimateShip()
        {
            frameCounter++;
            if (frameCounter >= 5)
            {
                frame++;
                frameCounter = 0;
                if (frame >= MaxFrames)
                {
                    frame = 0;
                }
                animationRect = new Rectangle(0, frame * PlayerHeight, PlayerWidth, PlayerHeight);
            }
        }

        private void Shoot()
        {
            switch (shootLevel)
            {
                case 1:
                    shootTimer += 30;
                    Bullet.NewBullet(position + centerTurretOffset, new Vector2(0f, -shootSpeed), true);
                    break;
                case 2:
                    shootTimer += 10;
                    Bullet.NewBullet(position + centerTurretOffset, new Vector2(0f, -shootSpeed), true);
                    break;
            }
        }

        private void UpdatePlayerDash(KeyboardState keyboardState, Vector2 velocity)
        {
            if (dashCooldown > 0)
                dashCooldown--;

            for (int timerIndex = 0; timerIndex < dashKeyPressTimers.Length; timerIndex++)
            {
                if (dashKeyPressTimers[timerIndex] > 0)
                {
                    dashKeyPressTimers[timerIndex]--;
                }
                else
                {
                    dashKeyCanPressAgain[timerIndex] = false;
                }
            }

            if (dashKeyPressTimers[0] > 0 && keyboardState.IsKeyUp(Keys.A))
            {
                dashKeyCanPressAgain[0] = true;
            }
            if (dashKeyPressTimers[1] > 0 && keyboardState.IsKeyUp(Keys.D))
            {
                dashKeyCanPressAgain[1] = true;
            }
            if (dashTimer <= 0 && dashCooldown <= 0 && dashKeyCanPressAgain[0] && dashKeyPressTimers[0] > 0 && keyboardState.IsKeyDown(Keys.A))
            {
                dashTimer += DashDurationTime;
                dashDirection = -1;
                dashKeyPressTimers[0] = 0;
                dashKeyCanPressAgain[0] = false;
                dashCooldown = DashCooldownTime;
            }
            if (dashTimer <= 0 && dashCooldown <= 0 && dashKeyCanPressAgain[1] && dashKeyPressTimers[1] > 0 && keyboardState.IsKeyDown(Keys.D))
            {
                dashTimer += DashDurationTime;
                dashDirection = 1;
                dashKeyPressTimers[1] = 0;
                dashKeyCanPressAgain[1] = false;
                dashCooldown = DashCooldownTime;
            }
        }

        private void HandleAfterImages()
        {
            if (dashTimer > 0 && dashTimer % 2 == 0)
            {
                afterImageAlpha.Add(0.8f);
                afterImagePositions.Add(position);
            }

            if (afterImageAlpha.Count > 0)
            {
                int currentAfterImageCount = afterImageAlpha.Count;
                for (int a = 0; a < currentAfterImageCount; a++)
                {
                    if (a >= afterImageAlpha.Count)
                    {
                        break;
                    }
                    if (afterImageAlpha[a] > 0)
                    {
                        afterImageAlpha[a] -= 0.8f / DashDurationTime;
                        if (afterImageAlpha[a] <= 0f)
                        {
                            afterImageAlpha.RemoveAt(a);
                            afterImagePositions.RemoveAt(a);
                            a--;
                        }
                    }
                }
            }
        }

        public override void HandleCollisions(CollisionBody collider)
        {
            if (collider is Projectile)
            {
                Projectile collidingProjectile = collider as Projectile;
                if (!collidingProjectile.friendly)
                {
                    Main.playerHealth -= 1;
                    if (Main.playerHealth <= 0)
                    {
                        dying = true;
                    }
                    Explosion.NewExplosion(collidingProjectile.position, Vector2.Zero);
                    collidingProjectile.DestroyInstance(collidingProjectile);
                }
            }
            else if (collider is PowerUp)
            {
                PowerUp powerUp = collider as PowerUp;
                if (powerUp.powerUpType == PowerUp.Attack)
                {
                    shootLevel += 1;
                }
            }
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int a = 0; a < afterImageAlpha.Count; a++)
            {
                if (afterImageAlpha[a] > 0f)
                {
                    spriteBatch.Draw(playerAfterImageTexture, afterImagePositions[a], null, Color.White * afterImageAlpha[a]);
                }
            }

            spriteBatch.Draw(playerSpritesheet, hitbox, animationRect, Color.White);
        }
    }
}
