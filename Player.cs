﻿using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.Enemies;
using Journey_Of_The_Ship.PowerUps;
using Journey_Of_The_Ship.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Journey_Of_The_Ship
{
    public class Player : CollisionBody
    {
        public override CollisionType collisionType => CollisionType.Player;
        public override CollisionType[] colliderTypes => new CollisionType[3] { CollisionType.Enemies, CollisionType.Obstacles, CollisionType.EnemyProjectiles };

        public static Texture2D playerSpritesheet;
        public static Texture2D playerAfterImageTexture;
        public static SoundEffect shootSound;
        public static SoundEffect dashSound;
        public static TurretTypes turretType = TurretTypes.Normal;
        public static WingTypes wingType = WingTypes.Normal;
        public static PropellerType propellerType = PropellerType.Normal;
        public static AmmoType ammoType = AmmoType.Bullets;
        public static AbilityType abilityType = AbilityType.None;

        private const int PlayerWidth = 21;
        private const int PlayerHeight = 19;
        private const int MaxFrames = 4;
        private const int DashDetectionTime = 5;
        private const int DashDurationTime = 10;
        private const int DashCooldownTime = 120;

        public bool canMove = false;
        public int killsNeededForAbility = 0;
        public int killsNeededRequirement = 0;
        public int abilityDuration = 0;

        private float shootSpeed = 2f;
        private int shootTimer = 0;
        private int shootLevel = 1;
        private int frame = 0;
        private int frameCounter = 0;
        private int explosionCounter = 0;
        private int turretNumber = 0;
        private int immunityCounter = 0;
        private int stunTimer = 0;
        private bool dying = false;
        private Rectangle animationRect;
        private bool abilityActive = false;
        private int abilityActiveTime = 0;
        private bool abilityShotOnce = false;

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

        public enum TurretTypes
        {
            Normal,
            Longer,
            Powerful
        }

        public enum WingTypes
        {
            Normal,

        }

        public enum PropellerType
        {
            Normal,
        }

        public enum AmmoType
        {
            Bullets,
            Missiles
        }

        public enum AbilityType
        {
            None,
            RapidFire,
            BlackHole,
            Shields,
            IsotopeReactionContainment
        }

        public override void Initialize()
        {
            dying = false;
            explosionCounter = 0;
            hitbox = new Rectangle((int)position.X, (int)position.Y, PlayerWidth, PlayerHeight);

            if (turretType == TurretTypes.Normal)
            {
                shootSpeed = 2f;
            }
            else if (turretType == TurretTypes.Longer)
            {
                shootSpeed = 2.6f;
            }
            else if (turretType == TurretTypes.Powerful)
            {

            }

            if (abilityType == AbilityType.RapidFire)
            {
                killsNeededRequirement = 20;
                abilityActiveTime = 25 * 60;
            }
            else if (abilityType == AbilityType.BlackHole)
            {
                killsNeededRequirement = 35;
                abilityActiveTime = (15 * 60) - 2;
            }
            else if (abilityType == AbilityType.Shields)
            {
                killsNeededRequirement = 15;
                abilityActiveTime = 30 * 60;
            }
            else if (abilityType == AbilityType.IsotopeReactionContainment)
            {
                killsNeededRequirement = 15;
                abilityActiveTime = 40 * 60;
            }
        }

        public override void Update()
        {
            if (shootTimer > 0)
                shootTimer--;
            if (immunityCounter > 0)
                immunityCounter--;
            if (stunTimer > 0)
                stunTimer--;

            AnimateShip();
            HandleAbilities();
            HandleAfterImages();
            ExtraEffects();
            CollisionBody[] bodiesArray = Main.activeEntities.ToArray();
            DetectCollisions(bodiesArray.ToList());
            CollisionBody[] projectilesArray = Main.activeProjectiles.ToArray();
            DetectCollisions(projectilesArray.ToList());
            Vector2 velocity = Vector2.Zero;

            if (canMove && stunTimer <= 0)
            {
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.W) && position.Y > 0)
                {
                    velocity.Y -= Main.playerSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.A) && position.X > 0)
                {
                    velocity.X -= Main.playerSpeed;
                    dashKeyPressTimers[0] = DashDetectionTime;
                }
                if (keyboardState.IsKeyDown(Keys.S) && position.Y < Main.desiredResolutionHeight - hitbox.Height)
                {
                    velocity.Y += Main.playerSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.D) && position.X < Main.desiredResolutionWidth - hitbox.Width)
                {
                    velocity.X += Main.playerSpeed;
                    dashKeyPressTimers[1] = DashDetectionTime;
                }
                if (keyboardState.IsKeyDown(Keys.Space) && shootTimer <= 0)
                {
                    Shoot();
                    shootSound.Play(Main.soundEffectVolume, Main.random.Next(0, 101) / 100f, 0f);
                }
                if (keyboardState.IsKeyDown(Keys.E) && killsNeededForAbility <= 0)
                {
                    abilityActive = true;
                    abilityDuration = abilityActiveTime;
                    killsNeededForAbility = killsNeededRequirement;
                }
                UpdatePlayerDash(keyboardState, velocity);
            }
            if (dashTimer > 0)
            {
                dashTimer--;
                velocity = new Vector2(Main.playerSpeed * 3f * dashDirection, 0f);
            }

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            if (dying)
            {
                explosionCounter++;
                canMove = false;
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
            Vector2 shootPos = position;

            if (abilityType == AbilityType.RapidFire && abilityActive)
            {
                shootTimer += 3;
                Main.StartScreenShake(1, 1);
                SpawnProjectile(shootPos + centerTurretOffset, new Vector2(0f, -shootSpeed));
                return;
            }
            if (ammoType == AmmoType.Bullets)
            {
                switch (shootLevel)
                {
                    case 1:
                        shootTimer += 30;
                        SpawnProjectile(shootPos + centerTurretOffset, new Vector2(0f, -shootSpeed));
                        break;
                    case 2:
                        turretNumber++;
                        if (turretNumber > 1)
                            turretNumber = 0;

                        shootTimer += 25;
                        if (turretNumber == 0)
                        {
                            shootPos += leftTurretOffset;
                        }
                        else
                        {
                            shootPos += rightTurretOffset;
                        }
                        SpawnProjectile(shootPos, new Vector2(0f, -shootSpeed));
                        break;
                    case 3:
                        turretNumber++;
                        if (turretNumber > 2)
                            turretNumber = 0;

                        shootTimer += 20;
                        if (turretNumber == 0)
                        {
                            shootPos += leftTurretOffset;
                        }
                        else if (turretNumber == 1)
                        {
                            shootPos += centerTurretOffset;
                        }
                        else if (turretNumber == 2)
                        {
                            shootPos += rightTurretOffset;
                        }
                        SpawnProjectile(shootPos, new Vector2(0f, -shootSpeed));
                        break;
                }
            }
            else if (ammoType == AmmoType.Missiles)
            {
                turretNumber++;
                if (turretNumber > 1)
                    turretNumber = 0;

                shootPos.X -= 2f;
                if (turretNumber == 0)
                {
                    shootPos += leftTurretOffset;
                }
                else
                {
                    shootPos += rightTurretOffset;
                }

                switch (shootLevel)
                {
                    case 1:
                        shootTimer += 80;
                        SpawnProjectile(shootPos, new Vector2(0f, -shootSpeed));
                        break;
                    case 2:
                        shootTimer += 75;
                        SpawnProjectile(shootPos, new Vector2(0f, -shootSpeed));
                        break;
                    case 3:
                        shootTimer += 70;
                        SpawnProjectile(shootPos, new Vector2(0f, -shootSpeed));
                        break;
                }
            }
        }

        private void SpawnProjectile(Vector2 position, Vector2 velocity)
        {
            if (ammoType == AmmoType.Bullets)
            {
                Bullet.NewBullet(position, velocity);
            }
            else if (ammoType == AmmoType.Missiles)
            {
                Missile.NewMissile(position, velocity);
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
                immunityCounter += DashDurationTime;
                dashTimer += DashDurationTime;
                dashDirection = -1;
                dashKeyPressTimers[0] = 0;
                dashKeyCanPressAgain[0] = false;
                dashCooldown = DashCooldownTime;
                dashSound.Play();
            }
            if (dashTimer <= 0 && dashCooldown <= 0 && dashKeyCanPressAgain[1] && dashKeyPressTimers[1] > 0 && keyboardState.IsKeyDown(Keys.D))
            {
                immunityCounter += DashDurationTime;
                dashTimer += DashDurationTime;
                dashDirection = 1;
                dashKeyPressTimers[1] = 0;
                dashKeyCanPressAgain[1] = false;
                dashCooldown = DashCooldownTime;
                dashSound.Play();
            }
        }

        private void ExtraEffects()
        {
            if (Main.random.Next(0, 5) == 0)
            {
                for (int s = 0; s < Main.random.Next(1, 4); s++)
                {
                    Vector2 offset = new Vector2(4f, 15f);
                    if (Main.random.Next(0, 2) == 1)
                    {
                        offset.X += 12f;
                    }
                    Vector2 position = Main.player.position + offset;
                    float angle = 270f + Main.random.Next(-15, 15 + 1);
                    float angleInRadians = MathHelper.ToRadians(angle);
                    Vector2 velocity = new Vector2((float)Math.Cos(angleInRadians), (float)Math.Sin(angleInRadians));
                    velocity.Normalize();
                    velocity *= -0.6f;
                    Smoke.NewSmokeParticle(position, velocity, Color.Orange, Color.Gray, 5, 8, 8);
                }
            }
        }

        public void AddAfterImage(float startingAlpha, Vector2 position)
        {
            afterImageAlpha.Add(startingAlpha);
            afterImagePositions.Add(position);
        }

        private void HandleAfterImages()
        {
            if (dashTimer > 0 && dashTimer % 2 == 0)
            {
                AddAfterImage(0.8f, position);
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

        private void HandleAbilities()
        {
            if (killsNeededForAbility <= 0)
            {
                killsNeededForAbility = 0;
            }

            if (!abilityActive)
                return;

            if (abilityDuration > 0)
            {
                abilityDuration--;
            }
            else
            {
                abilityActive = false;
                abilityShotOnce = false;
            }

            switch (abilityType)
            {
                case AbilityType.BlackHole:
                    if (!abilityShotOnce)
                    {
                        BlackHoleBomb.NewBlackHoleBomb(position, new Vector2(0f, -0.15f));
                        abilityShotOnce = true;
                    }
                    break;
            }

        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            if (collider is PowerUp)
            {
                PowerUp powerUp = collider as PowerUp;
                switch (powerUp.powerUpType)
                {
                    case PowerUp.Attack:
                        shootLevel += 1;
                        break;
                    case PowerUp.Health:
                        if (Main.playerHealth < 6)
                        {
                            Main.playerHealth += 1;
                        }
                        break;
                    case PowerUp.Speed:
                        Main.playerSpeed += 0.2f;
                        break;
                }
                powerUp.DestoryInstance(powerUp);
            }
            /*if (collider is AbilityDrop)
            {
                AbilityDrop abilityDrop = collider as AbilityDrop;
                switch (abilityDrop.abilityType)
                {
                    case AbilityDrop.RapidFire:
                        abilityActive[RapidFireAbility] = true;
                        abilityDurations[RapidFireAbility] = 30 * 60;
                        break;
                }
                abilityDrop.DestoryInstance(abilityDrop);
            }*/

            if (immunityCounter > 0)
                return;

            if (abilityActive && abilityType == AbilityType.Shields)
                return;

            if (collider is Projectile)
            {
                Projectile collidingProjectile = collider as Projectile;

                if (!collidingProjectile.continuous)
                {
                    immunityCounter += 30;
                    Main.playerHealth -= 1;
                    if (Main.playerHealth <= 0)
                    {
                        dying = true;
                    }
                    Explosion.NewExplosion(collidingProjectile.position, Vector2.Zero);
                    collidingProjectile.DestroyInstance(collidingProjectile);
                }
                if (collidingProjectile.continuous)
                {
                    if (collidingProjectile is ContinuousLaser)
                    {
                        immunityCounter += 30;
                        Main.playerHealth -= 1;
                        if (Main.playerHealth <= 0)
                        {
                            dying = true;
                        }
                        Explosion.NewExplosion(position, Vector2.Zero);
                    }
                    if (collidingProjectile is StasisBeam)
                    {
                        stunTimer = 2;
                    }
                }
            }
            if (collider is Slicer)
            {
                immunityCounter += 30;
                Main.playerHealth -= 1;
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
