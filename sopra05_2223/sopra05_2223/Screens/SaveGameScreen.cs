using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Controls;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using sopra05_2223.SoundManagement;
using System;
using System.Collections.Generic;

namespace sopra05_2223.Screens
{
    internal sealed class SaveGameScreen : IScreen
    {
        private List<Button> mComponents;

        // To draw the notification 
        private List<Button> mSavedNotification;

        // Calling component.Update() or component.Draw() requires a
        // GameTime object to be passed (for whatever reason?!).
        private readonly GameTime mGameTime = Globals.GameTime;

        // Saving game objects is a method of GameScreen, so we need the current one.
        private readonly GameScreen mCurrentGameScreen;


        // Need this to resize Screen which will be implemented this week
        private int mScreenWidth;
        private int mScreenHeight;

        // To check if the game saved button is clicked or not
        private bool mClicked;

        // See if this screen has been opened from the PauseMenuScreen or from the YouSureScreen.
        private bool mOpenedFromYouSureScreen;


        public ScreenManager ScreenManager
        {
            get; set;
        }

        bool IScreen.UpdateLower => false;

        bool IScreen.DrawLower => true;

        public SaveGameScreen(int screenWidth, int screenHeight, GameScreen currentGameScreen, bool openedFromYouSureScreen = false)
        {
            // Saving game objects is a method of GameScreen, so we need the current one.
            mCurrentGameScreen = currentGameScreen;

            mOpenedFromYouSureScreen = openedFromYouSureScreen;

            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;
            CalculateButtons(new Point(mScreenWidth, mScreenHeight));
        }

        private void CalculateButtons(Point screenSize)
        {
            // Button width should be considered, didn't find a smart way to do that.
            var buttonLeftSide = screenSize.X / 2 - 100;
            var buttonTopSide = screenSize.Y / 4;
            var verticalSpace = 50;

            var save1Button = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + verticalSpace),
                Text = "Speichern in Fach 1",
            };

            save1Button.Click += Save1Button_Click;

            var save2Button = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + 2 * verticalSpace),
                Text = "Speichern in Fach 2",
            };

            save2Button.Click += Save2Button_Click;


            var save3Button = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + 3 * verticalSpace),
                Text = "Speichern in Fach 3",
            };

            save3Button.Click += Save3Button_Click;


            var backButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + 4 * verticalSpace),
                Text = "Zurück",
            };
            backButton.Click += BackButton_Click;


            var savedGameButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide - 150),
                Text = "Gespeichert!",
            };
            savedGameButton.Click += savedGame_Click;
            mClicked = false;
            mSavedNotification = new List<Button>() { savedGameButton };

            // Store all menu buttons in one list to make updating the screen easier.
            mComponents = new List<Button>() {
                save1Button,
                save2Button,
                save3Button,
                backButton
            };
        }

        private void savedGame_Click(object sender, EventArgs e)
        {
        }
        private void Save1Button_Click(object sender, EventArgs e)
        {
            // Currently only saves the EntityManager, nothing else.
            // Leads to wrong mini map and camera position.
            mCurrentGameScreen.SaveEntityManager(1);
            mClicked = true;

            if (mOpenedFromYouSureScreen)
            {
                // Go back to the main menu without further asking.
                ScreenManager.ReturnToMainMenu();
                mCurrentGameScreen.GetSoundManager().PlayMusic(MusicEnum.MusicMenu);
            }
        }

        private void Save2Button_Click(object sender, EventArgs e)
        {
            // Currently only saves the EntityManager, nothing else.
            // Leads to wrong mini map and camera position.
            mCurrentGameScreen.SaveEntityManager(2);
            mClicked = true;

            if (mOpenedFromYouSureScreen)
            {
                // Go back to the main menu without further asking.
                ScreenManager.ReturnToMainMenu();
                mCurrentGameScreen.GetSoundManager().PlayMusic(MusicEnum.MusicMenu);
            }
        }

        private void Save3Button_Click(object sender, EventArgs e)
        {
            // Currently only saves the EntityManager, nothing else.
            // Leads to wrong mini map and camera position.
            mCurrentGameScreen.SaveEntityManager(3);
            mClicked = true;

            if (mOpenedFromYouSureScreen)
            {
                // Go back to the main menu without further asking.
                ScreenManager.ReturnToMainMenu();
                mCurrentGameScreen.GetSoundManager().PlayMusic(MusicEnum.MusicMenu);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            ScreenManager.RemoveScreens(2);
        }


        void IScreen.Resize(Point newSize)
        {
            mScreenHeight = newSize.Y;
            mScreenWidth = newSize.X;
            CalculateButtons(newSize);
        }
        void IScreen.Update(Input input)
        {
            // Menu screens should be closed whenever ESC is pressed.
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

            if (mClicked)
            {
                foreach (var button in mSavedNotification)
                {
                    button.Draw(mGameTime, spriteBatch);
                }

            }
        }


    }
}
