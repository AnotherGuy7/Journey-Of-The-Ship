using Journey_Of_The_Ship.Effects;
using Journey_Of_The_Ship.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_Of_The_Ship
{
    public class AnimationHandler      //This class is just for in-game animations like transitions
    {
        public int animationTimer = 0;
        public int secondaryAnimationTimer = 0;

        public const int GameEntry = 0;
        public const int PlayerEntry = 1;

        public bool[] isAnimationPlaying = new bool[2];

        public void Update()
        {
            if (isAnimationPlaying[GameEntry])
            {
                GameEntryAnimation();
            }
            if (isAnimationPlaying[PlayerEntry])
            {
                PlayerEntryAnimation();
            }
        }

        public static void PlayGameEntryAnimation()
        {
            Main.animationHandler.isAnimationPlaying[GameEntry] = true;
        }

        private void GameEntryAnimation()
        {
            animationTimer++;

            if (Main.fadeProgress <= 0f)
            {
                Main.FadeOut(100);
            }
            if (Main.fadeProgress >= 1f)
            {
                Main.FadeIn(100);
                Main.ReInitializeGame();
                EndAnimation(GameEntry);
                PlayPlayerEntryAnimation();
                Main.mainUI.DestroyInstance(Main.mainUI);
                Main.mainUI = null;
            }
        }

        public static void PlayPlayerEntryAnimation()
        {
            Main.animationHandler.isAnimationPlaying[PlayerEntry] = true;
        }

        private void PlayerEntryAnimation()
        {
            animationTimer++;
            float flightStopCoord = 160f;

            if (animationTimer % 2 == 0)
            {
                Main.player.AddAfterImage(0.8f, Main.player.position);
            }

            if (animationTimer == 1)
            {
                Main.player.canMove = false;
                Main.player.position.Y = 260f;
            }
            if (animationTimer >= 120 && Main.player.position.Y > flightStopCoord)
            {
                Main.player.position.Y -= animationTimer / 32f;
            }

            if (Main.player.position.Y <= flightStopCoord)
            {
                Main.player.canMove = true;
                for (int s = 0; s < 60; s++)
                {
                    Vector2 position = Main.player.position;
                    float angle = s * (360f / 60f);
                    Vector2 velocity = new Vector2(0.6f, 0.6f) * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    Smoke.NewSmokeParticle(position + new Vector2(Main.player.hitbox.Width / 2f, Main.player.hitbox.Height / 2f), velocity, Color.LightBlue, Color.White, 30, 60);
                }
            }

            if (Main.player.canMove)
            {
                secondaryAnimationTimer++;
                if (secondaryAnimationTimer % 5 == 0)
                {
                    if (PlayerUI.uiAlpha == 0f)
                    {
                        PlayerUI.uiAlpha = 1f;
                    }
                    else if (PlayerUI.uiAlpha == 1f)
                    {
                        PlayerUI.uiAlpha = 0f;
                    }
                }
                if (secondaryAnimationTimer >= 25)
                {
                    PlayerUI.uiAlpha = 1f;
                    EndAnimation(PlayerEntry);
                }
            }
        }

        public void EndAnimation(int animationIndex)
        {
            animationTimer = 0;
            secondaryAnimationTimer = 0;
            isAnimationPlaying[animationIndex] = false;
        }
    }
}
