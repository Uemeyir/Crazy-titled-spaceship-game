using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Controls;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using System;
using System.Collections.Generic;
using sopra05_2223.Core;
using sopra05_2223.Serializer;
using sopra05_2223.SoundManagement;

namespace sopra05_2223.Screens
{
    internal sealed class LoadGameScreen : IScreen
    {
        private List<Button> mComponents;

        // Calling component.Update() or component.Draw() requires a
        // GameTime object to be passed (for whatever reason?!).
        private readonly GameTime mGameTime = Globals.GameTime;

        // Loading game objects is a method of GameScreen, so we need the current one.
        private readonly SoundManager mSoundManager;

        // Need this to resize Screen which will be implemented this week
        private int mScreenWidth;
        private int mScreenHeight;

        private readonly bool mInGameMenu;

        private Button mNotification;
        private bool mNotify;

        public ScreenManager ScreenManager
        {
            get; set;
        }

        bool IScreen.UpdateLower => false;

        bool IScreen.DrawLower => true;

        public LoadGameScreen(int screenWidth, int screenHeight, SoundManager soundManager, bool inGameMenu)
        {
            // Loading game objects is a method of GameScreen, so we need the current one.
            mSoundManager = soundManager;

            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;
            CalculateButtons(new Point(mScreenWidth, mScreenHeight));
            mInGameMenu = inGameMenu;
        }

        private void CalculateButtons(Point screenSize)
        {
            // Button width should be considered, didn't find a smart way to do that.
            var buttonLeftSide = screenSize.X / 2 - 100;
            var buttonTopSide = screenSize.Y / 4;
            const int verticalSpace = 50;

            var button1ClickAble = System.IO.File.Exists(Globals.SaveGamePath(1));
            var load1Button = new Button(button1ClickAble)
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + verticalSpace),
                Text = "Laden aus Fach 1",
            };

            if (button1ClickAble)
            {
                load1Button.Click += Load1Button_Click;
            }
            else
            {
                load1Button.Click += ButtonNotifyEmpty;
            }

            var button2ClickAble = System.IO.File.Exists(Globals.SaveGamePath(2));
            var load2Button = new Button(button2ClickAble)
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + 2 * verticalSpace),
                Text = "Laden aus Fach 2",
            };

            if (button2ClickAble)
            {
                load2Button.Click += Load2Button_Click;
            }
            else
            {
                load2Button.Click += ButtonNotifyEmpty;
            }

            var button3ClickAble = System.IO.File.Exists(Globals.SaveGamePath(3));
            var load3Button = new Button(button3ClickAble)
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + 3 * verticalSpace),
                Text = "Laden aus Fach 3",
            };

            if (button3ClickAble)
            {
                load3Button.Click += Load3Button_Click;
            }
            else
            {
                load3Button.Click += ButtonNotifyEmpty;
            }

            var backButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + 4 * verticalSpace),
                Text = "Zurück",
            };
            backButton.Click += BackButton_Click;

            mNotification = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide - 150),
            };

            // Store all menu buttons in one list to make updating the screen easier.
            mComponents = new List<Button>() {
                load1Button,
                load2Button,
                load3Button,
                backButton
            };
        }

        private void ButtonNotifyEmpty(object sender, EventArgs e)
        {
            mNotification.Text = "Spielstand ist leer";
            mNotify = true;
        }

        private void LoadGame(int slot)
        {
            var res = SopraSerializer.DeserializeGameScreen(slot);

            if (res != null && res.mPlayer != null && res.mEntityManager is { } && res.mKi != null && res.mRunStatistics != null)
            {
                // start the loaded game
                var gs = new GameScreen(
                    res.mEntityManager,
                    mSoundManager,
                    res.mPlayer,
                    res.mKi,
                    res.mCamPos,
                    res.mCamZoom,
                    res.mRunStatistics);
                var hud = new Hud(Globals.Resolution.mScreenSize.X, Globals.Resolution.mScreenSize.Y);
                if (mInGameMenu)
                {
                    // LoadMenu, Hud, GameScreen
                    ScreenManager.RemoveScreens(3);
                    // Game music already playing because we opened from pause menu.
                }
                else
                {
                    // If game is loaded from main menu, start the game music.
                    mSoundManager.PlayMusic(MusicEnum.Music1);
                }
                // LoadMenu, Background
                ScreenManager.RemoveScreens(2);
                ScreenManager.AddScreen(gs);
                ScreenManager.AddScreen(hud);

            }
            else
            {
                mNotification.Text = "Spieldateien sind fehlerhaft";
                mNotify = true;
            }

        }

        private void Load1Button_Click(object sender, EventArgs e)
        {
            // Currently only loads the EntityManager, nothing else.
            // Leads to wrong mini map and camera position.
            LoadGame(1);
        }

        private void Load2Button_Click(object sender, EventArgs e)
        {
            // Currently only loads the EntityManager, nothing else.
            // Leads to wrong mini map and camera position.
            LoadGame(2);
        }

        private void Load3Button_Click(object sender, EventArgs e)
        {
            // Currently only loads the EntityManager, nothing else.
            // Leads to wrong mini map and camera position.
            LoadGame(3);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            ScreenManager.RemoveScreens(2);
        }


        void IScreen.Resize(Point newSize)
        {
            mScreenWidth = newSize.X;
            mScreenHeight = newSize.Y;
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

            if (mNotify)
            {
                mNotification.Draw(mGameTime, spriteBatch);
            }
        }

    }
}
