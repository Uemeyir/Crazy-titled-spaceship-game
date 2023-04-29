using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Controls;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.SoundManagement;
using sopra05_2223.Core;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Screens
{
    internal sealed class MenuScreen : IScreen
    {
        // To save the menu Buttons.
        private List<Button> mComponents;

        // Calling component.Update() or component.Draw() requires a
        // GameTime object to be passed (for whatever reason?!).
        private readonly GameTime mGameTime = Globals.GameTime;

        private int mScreenWidth;
        private int mScreenHeight;

        // Needed so that the quit button knows which game to quit.
        private readonly Game1 mCurrentGame1;
        private readonly SoundManager mSoundManager;

        // Needed to check if Techdemo is open.
        public ScreenManager ScreenManager
        {
            get; set;
        }

        // Set DrawLower and UpdateLower to true to draw and refresh the MenuBackgroundScreen as well.
        bool IScreen.UpdateLower => true;

        bool IScreen.DrawLower => true;


        public MenuScreen(int screenWidth, int screenHeight, Game1 game1, SoundManager soundManger)
        {
            mSoundManager = soundManger;
            mCurrentGame1 = game1;
            // buttonwidth should be considered, didnt find a smart way to do that

            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;

            CalculateButtons(new Point(mScreenWidth, mScreenHeight));
        }

        private void CalculateButtons(Point screenSize)
        {
            var buttonLeftSide = screenSize.X / 2 - 100;

            // Create a new button "New Game"
            var newGameButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, mScreenHeight / 2 - 200),
                Text = "Neues Spiel",
            };

            // NewGameButton_Click provides whatever functionality the button should have when clicked.
            newGameButton.Click += NewGameButton_Click;

            var techdemoButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, mScreenHeight / 2 - 150),
                Text = "Techdemo laden",
            };

            // NewGameButton_Click provides whatever functionality the button should have when clicked.
            techdemoButton.Click += TechdemoButton_Click;

            var loadGameButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, mScreenHeight / 2 - 100),
                Text = "Spiel laden",
            };

            loadGameButton.Click += LoadGameButton_Click;


            var optionsGameButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, mScreenHeight / 2 - 50),
                Text = "Optionen",
            };

            optionsGameButton.Click += OptionsGameButton_Click;

            var statisticsGameButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, mScreenHeight / 2 + 0),
                Text = "Statistiken",
            };

            statisticsGameButton.Click += StatisticsGameButton_Click;

            var achievementsGameButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, mScreenHeight / 2 + 50),
                Text = "Errungenschaften",
            };
            achievementsGameButton.Click += AchievementsGameButton_Click;

            var quitGameButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, mScreenHeight / 2 + 150),
                Text = "Spiel beenden",
            };
            quitGameButton.Click += QuitGameButton_Click;

            var infoButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, mScreenHeight / 2 + 100),
                Text = "Starthilfe",
            };
            infoButton.Click += InfoButton_Click;


            // Store all menu buttons in one list to make updating the screen easier.
            mComponents = new List<Button>() {
                newGameButton,
                techdemoButton,
                loadGameButton,
                optionsGameButton,
                statisticsGameButton,
                achievementsGameButton,
                quitGameButton,
                infoButton,
            };
        }


        private void NewGameButton_Click(object sender, EventArgs e)
        {
            var player = new PlayerPlayer(0, 0);
            var ki = new PlayerKi(0, 0);
            mSoundManager.PlayMusic(MusicEnum.Music1);
            ScreenManager.AddScreen(new GameScreen(new Point(mScreenWidth, mScreenHeight), Globals.sWorldSize, mSoundManager, player, ki, false));
            ScreenManager.AddScreen(new Hud(mScreenWidth, mScreenHeight));
        }

        private void TechdemoButton_Click(object sender, EventArgs e)
        {
            mSoundManager.PlayMusic(MusicEnum.Music2);
            var player = new PlayerPlayer(0, 0);
            var ki = new PlayerKi(0, 0);
            ScreenManager.AddScreen(new TechdemoScreen(new Point(mScreenWidth, mScreenHeight), new Point(80000, 80000), mSoundManager, player, ki));
            ScreenManager.AddScreen(new Hud(mScreenWidth, mScreenHeight));
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new MenuBackgroundScreen(new Point(mScreenWidth, mScreenHeight)));
            ScreenManager.AddScreen(new LoadGameScreen(mScreenWidth, mScreenHeight, mSoundManager, false));
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

        private void AchievementsGameButton_Click(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new MenuBackgroundScreen(new Point(mScreenWidth, mScreenHeight)));
            ScreenManager.AddScreen(new AchievementsScreen(mScreenWidth, mScreenHeight));
        }
        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            mCurrentGame1.Exit();
        }
        private void InfoButton_Click(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new ScreenplayScreen(new Point(mScreenWidth, mScreenHeight)));
        }

        /* --- currently unused ---
        private void TutorialButton_Click(object sender, EventArgs e)
        {
            var player = new PlayerPlayer(0, 0);
            var ki = new PlayerKi(0, 0);
            ScreenManager.AddScreen(new GameScreen(new Point(mScreenWidth, mScreenHeight), new Point(10000, 10000), mSoundManager, player, ki, true));
            ScreenManager.AddScreen(new Hud(mScreenWidth, mScreenHeight));
            var screenSize = new Point(mScreenWidth, mScreenHeight);
            Globals.ScreenManager.AddScreen(new TextOverlayScreen(screenSize, new TextPane(new Rectangle(0, 0, screenSize.X, screenSize.Y)), true));
        }
        */

        void IScreen.Resize(Point newSize)
        {
            mScreenHeight = newSize.Y;
            mScreenWidth = newSize.X;
            CalculateButtons(newSize);
        }

        void IScreen.Update(Input input)
        {
            if (input.GetKey(Keys.Escape))
            {
                Globals.SoundManager.PlaySound(SoundEnum.BaseExplosion);
            }
            foreach (var component in mComponents)
            {
                // Update all Buttons.
                component.Update(mGameTime);
            }
        }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin();
            spriteBatch.Draw(Art.Title, new Vector2(50, 100), new Rectangle(0, 0, 1000, 700), Color.White, -0.15f, Vector2.One, 0.5f, SpriteEffects.None, 1);

            // Draw everything in component (all Buttons).
            foreach (var component in mComponents)
            {
                component.Draw(mGameTime, spriteBatch);
            }
        }

    }
}