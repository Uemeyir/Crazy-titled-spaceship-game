using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using sopra05_2223.Controls;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using sopra05_2223.SoundManagement;

namespace sopra05_2223.Screens
{
    internal sealed class YouSureScreen : IScreen
    {
        private List<Button> mComponents;

        // Calling component.Update() or component.Draw() requires a
        // GameTime object to be passed (for whatever reason?!).
        private readonly GameTime mGameTime = Globals.GameTime;

        // Needed for calling the function for saving the game objects.
        private readonly GameScreen mCurrentGameScreen;

        private int mScreenWidth;
        private int mScreenHeight;

        public ScreenManager ScreenManager
        {
            get; set;
        }

        bool IScreen.UpdateLower => false;

        bool IScreen.DrawLower => true;

        public YouSureScreen(int screenWidth, int screenHeight, GameScreen currentGameScreen)
        {
            // Needed for calling the function for saving the game objects.
            mCurrentGameScreen = currentGameScreen;

            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;

            CalculateButtons(new Point(mScreenWidth, mScreenHeight));
        }

        private void CalculateButtons(Point screenSize)
        {
            // buttonwidth should be considered, didnt find a smart way to do that
            var buttonLeftSide = screenSize.X / 2 - 100;
            var buttonTopSide = screenSize.Y / 3;
            var verticalSpace = 50;


            // Create a new button that shows the question.
            var titleButton = new Button(false)
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide),
                Text = "Schließen ohne zu speichern?",
            };
            // Title button needs no functionality.


            var saveButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + 2 * verticalSpace),
                Text = "Nein",
            };
            saveButton.Click += SaveButton_Click;


            var mainMenuButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + verticalSpace),
                Text = "Ja",
            };
            mainMenuButton.Click += MainMenuButton_Click;


            // Store all menu buttons in one list to make updating the screen easier.
            mComponents = new List<Button>() {
                titleButton,
                saveButton,
                mainMenuButton,
            };
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Remove this screen and its background to get back to the pauseMenu before adding the saveGameScreen.
            // Needed so that the back-buttons work properly in the saveGameScreen.
            ScreenManager.RemoveScreens(2);
            ScreenManager.AddScreen(new MenuBackgroundScreen(new Point(mScreenWidth, mScreenHeight)));
            ScreenManager.AddScreen(new SaveGameScreen(mScreenWidth, mScreenHeight, mCurrentGameScreen, true));
        }

        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            mCurrentGameScreen.GetSoundManager().PlayMusic(MusicEnum.MusicMenu);
            ScreenManager.ReturnToMainMenu();
        }


        // returns true if point is in bounds of this screen, else returns false.

        void IScreen.Resize(Point newSize)
        {
            mScreenHeight = newSize.Y;
            mScreenWidth = newSize.X;
            CalculateButtons(newSize);
        }
        void IScreen.Update(Input input)
        {
            if (input.GetKeys().Contains(Keys.Escape))
            {
                ScreenManager.RemoveScreens(2);
            }

            foreach (var component in mComponents)
            {
                // Update all Buttons.
                component.Update(mGameTime);
            }
        }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            // Draw everything in component (all Buttons).
            foreach (var component in mComponents)
            {
                component.Draw(mGameTime, spriteBatch);
            }
        }

    }
}
