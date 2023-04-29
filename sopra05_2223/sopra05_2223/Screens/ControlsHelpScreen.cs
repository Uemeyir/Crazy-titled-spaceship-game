using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Controls;
using System.Collections.Generic;
using System;

namespace sopra05_2223.Screens
{
    internal sealed class ControlsHelpScreen : IScreen
    {
        private Point mScreenSize;

        // To save the menu Buttons.
        private List<Button> mComponents;

        public ControlsHelpScreen(Point screenSize)
        {
            mScreenSize = screenSize;
            CalculateButtons(screenSize);
        }

        private void CalculateButtons(Point screenSize)
        {
            var buttonLeftSide = screenSize.X / 2 - 100;

            var nextScreenButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, mScreenSize.Y - 150),
                Text = "Mehr Infos",
            };

            nextScreenButton.Click += NextScreenButton_Click;

            var backButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, mScreenSize.Y - 100),
                Text = "Zurück",
            };

            backButton.Click += BackButton_Click;

            // Store all menu buttons in one list to make updating the screen easier.
            mComponents = new List<Button>() {
                nextScreenButton,
                backButton
            };
        }
        private void NextScreenButton_Click(object sender, EventArgs e)
        {
            ScreenManager.RemoveScreens(1);
            ScreenManager.AddScreen(new ScreenplayScreen(mScreenSize));
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            ScreenManager.RemoveScreens(1);
        }


        public ScreenManager ScreenManager { get; set; }

        bool IScreen.UpdateLower => false;

        bool IScreen.DrawLower => false;

        void IScreen.Update(Input input)
        {
            if (input.GetKeys().Contains(Keys.Escape) || input.GetKeys().Contains(Globals.mKeys["Help"]))
            {
                ScreenManager.RemoveScreens(1);
            }
            foreach (var component in mComponents)
            {
                // Update all Buttons.
                component.Update(Globals.GameTime);
            }
        }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(samplerState: SamplerState.LinearWrap);
            spriteBatch.Draw(Art.ControlsHelpScreenBackground, new Rectangle(0, 0, mScreenSize.X, mScreenSize.Y), new Rectangle(0, 0, Art.ControlsHelpScreenBackground.Width, Art.ControlsHelpScreenBackground.Height), Color.White);
            // Draw everything in component (all Buttons).
            foreach (var component in mComponents)
            {
                component.Draw(Globals.GameTime, spriteBatch);
            }
        }

        public void Resize(Point newSize)
        {
            mScreenSize = newSize;
        }
    }
}
