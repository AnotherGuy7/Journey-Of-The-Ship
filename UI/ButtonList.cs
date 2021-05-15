using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_Of_The_Ship.UI
{
    public class ButtonList : UIObject
    {
        public int amountOfButtons;
        public Vector2 position;
        public bool expanded = false;
        public bool[] buttonPressed;
        public int selectedItem = 0;


        private Button[] buttons;
        private Texture2D[] buttonIcons;
        private string[] buttonLabels;
        private int buttonHeights = 0;
        private bool expanding = false;
        private bool detracting = false;
        private float expansionFloat = 0f;

        public ButtonList(int amountOfButtons, Vector2 listPosition, int averageButtonWidth, int averageButtonHeight, Texture2D[] listButtonIcons, string[] listButtonLabels, Color buttonsInactiveColor, Color buttonsActiveColor, int buttonsInteractionLayer)
        {
            amountOfButtons += 1;       //This is because 0 is reserved as the slot to show what in the list is chosen, so we need an extra slot to add buttons that are needed
            this.amountOfButtons = amountOfButtons;
            buttons = new Button[amountOfButtons];
            position = listPosition;
            buttonHeights = averageButtonHeight;
            buttonPressed = new bool[amountOfButtons];

            Texture2D[] newIconsArray = new Texture2D[listButtonIcons.Length + 1];      //Just making a placeholder value for 0, since "0" is considered an empty slot
            for (int i = 0; i < newIconsArray.Length; i++)
            {
                if (i != 0)
                {
                    newIconsArray[i] = listButtonIcons[i - 1];
                }
                else
                {
                    newIconsArray[0] = listButtonIcons[0];
                }
            }

            string[] newLabelsArray = new string[listButtonLabels.Length + 1];
            for (int i = 0; i < newLabelsArray.Length; i++)
            {
                if (i != 0)
                {
                    newLabelsArray[i] = listButtonLabels[i - 1];
                }
                else
                {
                    newLabelsArray[0] = listButtonLabels[0];
                }
            }

            buttonIcons = newIconsArray;
            buttonLabels = newLabelsArray;

            for (int i = 0; i < amountOfButtons; i++)
            {
                if (i == 0)
                {
                    buttons[i] = new Button(buttonIcons[i], buttonLabels[i], averageButtonWidth, averageButtonHeight, listPosition, buttonsInactiveColor, buttonsActiveColor, 1f, 1f, 0);
                    buttons[i].buttonPosition = position;
                    buttons[i].visible = true;
                }
                else
                {
                    buttons[i] = new Button(buttonIcons[i], buttonLabels[i], averageButtonWidth, averageButtonHeight, listPosition, buttonsInactiveColor, buttonsActiveColor, 1f, 1f, buttonsInteractionLayer);
                    buttons[i].buttonPosition = position;
                    buttons[i].visible = false;
                }
            }
        }

        public override void Update()
        {
            for (int i = 0; i < amountOfButtons; i++)
            {
                buttonPressed[i] = false;
                buttons[i].Update();
            }

            if (expanding)
            {
                if (expansionFloat >= buttonHeights)
                {
                    expanded = true;
                    expanding = false;
                    return;
                }

                expansionFloat += (float)buttonHeights / 40f;       //It will take 40 frames to complete, no matter the size

                for (int i = 0; i < amountOfButtons; i++)
                {
                    float xOffset = 0f;
                    if (i != 0)
                    {
                        xOffset = 4f;
                    }
                    Vector2 buttonPosition = position + new Vector2(xOffset, expansionFloat * i);
                    buttons[i].buttonPosition = buttonPosition;
                }
            }

            if (detracting)
            {
                if (expansionFloat <= 0f)
                {
                    expanded = false;
                    detracting = false;
                    for (int i = 0; i < amountOfButtons; i++)
                    {
                        if (i != 0)
                            buttons[i].visible = false;
                        buttons[i].buttonPosition = position;
                    }
                    return;
                }

                expansionFloat -= (float)buttonHeights / 40f;       //It will take 40 frames to complete, no matter the size

                for (int i = 0; i < amountOfButtons; i++)
                {
                    float xOffset = 0f;
                    if (i != 0)
                    {
                        xOffset = 4f;
                    }
                    Vector2 buttonPosition = position + new Vector2(xOffset, expansionFloat * i);
                    buttons[i].buttonPosition = buttonPosition;
                }
            }

            if (expanded)
            {
                for (int i = 1; i < amountOfButtons; i++)
                {
                    buttonPressed[i] = buttons[i].buttonPressed;
                    if (buttons[i].buttonPressed)
                    {
                        buttons[0].buttonIcon = buttonIcons[i];
                        buttons[0].stringText = buttonLabels[i];
                        break;
                    }
                }
            }
        }

        public void Expand()
        {
            if (!expanded)
            {
                expanding = true;
                for (int i = 0; i < amountOfButtons; i++)
                {
                    buttons[i].visible = true;
                }
            }
        }

        public void Detract()
        {
            if (expanded)
            {
                detracting = true;
                for (int i = 1; i < amountOfButtons; i++)
                {
                    buttons[i].visible = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = buttons.Length - 1; i >= 0; i--)
            {
                buttons[i].Draw(spriteBatch);
            }
        }
    }
}
