using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Controls;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using System;
using System.Collections.Generic;
using sopra05_2223.Core;
using sopra05_2223.SoundManagement;

namespace sopra05_2223.Screens
{

    internal sealed class PauseMenuScreen : IScreen
    {
        private List<Button> mComponents;

        // Calling component.Update() or component.Draw() requires a
        // GameTime object to be passed (for whatever reason?!).
        private readonly GameTime mGameTime = Globals.GameTime;

        // Needed for calling the function for saving the game objects.
        private readonly GameScreen mCurrentGameScreen;

        private int mScreenWidth;
        private int mScreenHeight;
        private readonly SoundManager mSoundManager;

        public ScreenManager ScreenManager
        {
            get; set;
        }

        bool IScreen.UpdateLower => false;

        bool IScreen.DrawLower => true;


        public PauseMenuScreen(int screenWidth, int screenHeight, GameScreen currentGameScreen, SoundManager soundManager)
        {
            // Needed for calling the function for saving the game objects.
            mCurrentGameScreen = currentGameScreen;
            mSoundManager = soundManager;

            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;

            CalculateButtons(new Point(mScreenWidth, mScreenHeight));
        }

        private void CalculateButtons(Point screenSize)
        {
            // buttonwidth should be considered, didnt find a smart way to do that
            var buttonLeftSide = screenSize.X / 2 - 100;
            var buttonTopSide = screenSize.Y / 4;
            var verticalSpace = 50;


            // Create a new button "Save"
            var saveButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + verticalSpace),
                Text = "Speichern",
            };
            // SaveButton_Click provides whatever functionality the button should have when clicked.
            saveButton.Click += SaveButton_Click;

            // Load the last saved state of the EntityManager.
            var loadButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + 2 * verticalSpace),
                Text = "Laden",
            };
            loadButton.Click += LoadButton_Click;


            var statisticsGameButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + 3 * verticalSpace),
                Text = "Statistiken und Errungenschaften",
            };
            statisticsGameButton.Click += StatisticsGameButton_Click;


            var optionsGameButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + 4 * verticalSpace),
                Text = "Optionen",
            };
            optionsGameButton.Click += OptionsGameButton_Click;


            var mainMenuGameButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + 5 * verticalSpace),
                Text = "Hauptmenü",
            };
            mainMenuGameButton.Click += MainMenuButton_Click;


            var backToGameButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide),
                Text = "Zurück zum Spiel",
            };

            backToGameButton.Click += BackToGameButton_Click;


            // Store all menu buttons in one list to make updating the screen easier.
            if (mCurrentGameScreen is not TechdemoScreen)
            {
                mComponents = new List<Button>() {
                    saveButton,
                    loadButton,
                    backToGameButton,
                    statisticsGameButton,
                    optionsGameButton,
                    mainMenuGameButton,
                };
            }
            else
            {
                statisticsGameButton = new Button()
                {
                    Position = new Vector2(buttonLeftSide, buttonTopSide + verticalSpace),
                    Text = "Statistiken und Errungenschaften",
                };
                statisticsGameButton.Click += StatisticsGameButton_Click;

                optionsGameButton = new Button()
                {
                    Position = new Vector2(buttonLeftSide, buttonTopSide + 2 * verticalSpace),
                    Text = "Optionen",
                };
                optionsGameButton.Click += OptionsGameButton_Click;

                mainMenuGameButton = new Button()
                {
                    Position = new Vector2(buttonLeftSide, buttonTopSide + 3 * verticalSpace),
                    Text = "Hauptmenü",
                };
                mainMenuGameButton.Click += MainMenuButton_Click;

                mComponents = new List<Button>() {
                    backToGameButton,
                    statisticsGameButton,
                    optionsGameButton,
                    mainMenuGameButton,
                };
            }
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Currently only saves the EntityManager, nothing else.
            // Leads to wrong mini map and camera position when loading the saved game.
            ScreenManager.AddScreen(new MenuBackgroundScreen(new Point(mScreenWidth, mScreenHeight)));
            ScreenManager.AddScreen(new SaveGameScreen(mScreenWidth, mScreenHeight, mCurrentGameScreen, false));
        }
        private void LoadButton_Click(object sender, EventArgs e)
        {
            // Currently only loads the saved EntityManager, nothing else.
            // Leads to wrong mini map and camera position.
            ScreenManager.AddScreen(new MenuBackgroundScreen(new Point(mScreenWidth, mScreenHeight)));
            ScreenManager.AddScreen(new LoadGameScreen(mScreenWidth, mScreenHeight, mSoundManager, true));
        }

        private void BackToGameButton_Click(object sender, EventArgs e)
        {
            ScreenManager.RemoveScreens();
        }

        private void OptionsGameButton_Click(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new MenuBackgroundScreen(new Point(mScreenWidth, mScreenHeight)));
            ScreenManager.AddScreen(new OptionsMenuScreen(mScreenWidth, mScreenHeight, mSoundManager));
        }

        private void StatisticsGameButton_Click(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new MenuBackgroundScreen(new Point(mScreenWidth, mScreenHeight)));
            ScreenManager.AddScreen(new StatisticScreen(mScreenWidth, mScreenHeight));
        }

        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            if (mCurrentGameScreen is not TechdemoScreen)
            {
                ScreenManager.AddScreen(new MenuBackgroundScreen(new Point(mScreenWidth, mScreenHeight)));
                ScreenManager.AddScreen(new YouSureScreen(mScreenWidth, mScreenHeight, mCurrentGameScreen));
            }
            else
            {
                mCurrentGameScreen.GetSoundManager().PlayMusic(MusicEnum.MusicMenu);
                ScreenManager.RemoveScreens(3);
            }
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
            if (input.GetKeys().Contains(Keys.Escape) || input.GetKeys().Contains(Globals.mKeys["Pause"]))
            {
                ScreenManager.RemoveScreens();
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