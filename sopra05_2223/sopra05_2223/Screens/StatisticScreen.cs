using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Controls;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using System;
using System.Collections.Generic;
using sopra05_2223.Core;

namespace sopra05_2223.Screens
{
    internal sealed class StatisticScreen : IScreen
    {
        private List<Button> mComponents;

        private readonly GameTime mGameTime = Globals.GameTime;

        private readonly int mScreenWidth;
        private readonly int mScreenHeight;
        private bool mRunStatistics;

        public ScreenManager ScreenManager
        {
            get; set;
        }
        bool IScreen.UpdateLower => true;

        bool IScreen.DrawLower => true;

        public StatisticScreen(int screenWidth, int screenHeight, bool runStatistics = false)
        {
            mRunStatistics = runStatistics;
            CalculateButtons(new Point(screenWidth, screenHeight));
            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;
        }
        private static readonly string[] sStats1 =
        {
            "Spieldauer: ",
            "Gesammlete Ressourcen: ",
            "Ressourcen durch Trümmerteile: ",
            "Gesammelte natürliche Ressourcen: ",
            "Übernommene Basen: ",
            "Schnellster Erfolgreicher \nDurchlauf: ",
            "Geringste Schiffsverluste in \neinem erfolgreichen Durchlauf: ",
            "Meiste Verluste in einem \nerfolgreichen Durchlauf: ",
            "Wenigste zerstörte gegnerische \nSchiffe pro Durchlauf: ",
            "Meiste Verluste in einem \nerfolgreichen Durchlauf: "
        };

        private void CalculateButtons(Point screenSize)
        {
            // buttonwidth should be considered, didnt find a smart way to do that
            var buttonLeftSide = screenSize.X / 2 - 100;

            var backToPreviousScreen = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 2 + 250),
                Text = "Zurück"
            };
            // For the functionality
            backToPreviousScreen.Click += BackToPreviousScreen_Click;

            // Switch between stats and achievements using this button.
            var achsScreenButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 2 + 300),
                Text = "Errungenschaften"
            };
            achsScreenButton.Click += AchsScreenButton_Click;

            var text = "Statistiken aktuelles Spiel";
            if (mRunStatistics)
            {
                text = "Statistiken gesamt";
            }
            var runStatisticsButton = new Button()
            {
                Position = new Vector2(buttonLeftSide + 350, screenSize.Y / 2 + 300),
                Text = text
            };
            runStatisticsButton.Click += RunStatisticsButton_Click;

            // For the seocond stats screen
            var secondStatsScreenButton = new Button()
            {
                Position = new Vector2(buttonLeftSide + 350, screenSize.Y / 2 + 250),
                Text = "Weitere Statistiken"
            };
            secondStatsScreenButton.Click += SecondScreenButton_Click;

            // store button and stats to draw it
            mComponents = new List<Button>() {
                secondStatsScreenButton,
                backToPreviousScreen,
                achsScreenButton,
                runStatisticsButton
            };
        }
        private void BackToPreviousScreen_Click(object sender, EventArgs e)
        {
            // Remove 2 Screens from Screen stack (StatisticScreen and MenuBackgroundScreen).
            ScreenManager.RemoveScreens();
            ScreenManager.RemoveScreens();
        }

        private void AchsScreenButton_Click(object sender, EventArgs e)
        {
            // Remove 1 Screen from Screen stack (StatsScreen).
            ScreenManager.RemoveScreens();
            // Replace it with the achs screen.
            ScreenManager.AddScreen(new AchievementsScreen(mScreenWidth, mScreenHeight));
        }

        private void SecondScreenButton_Click(object sender, EventArgs e)
        {
            ScreenManager.RemoveScreens();
            ScreenManager.AddScreen(new StatisticScreenTwo(mScreenWidth, mScreenHeight, mRunStatistics));
            
        }

        private void RunStatisticsButton_Click(object sender, EventArgs e)
        {
            if (!mRunStatistics)
            {
                mComponents[3].Text = "Statistiken gesamt";
            }
            else
            {
                mComponents[3].Text = "Statistiken aktuelles Spiel";
            }
            mRunStatistics = !mRunStatistics;
        }
        void IScreen.Resize(Point newSize)
        {
            CalculateButtons(newSize);
        }
        void IScreen.Update(Input input)
        {
            if (input.GetKeys().Contains(Keys.Escape))
            {
                ScreenManager.RemoveScreens();
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

            if (!mRunStatistics)
            {
                var timespan = TimeSpan.FromSeconds(Globals.mStatistics["RunningSeconds"]);
                spriteBatch.DrawString(Art.Arial22, sStats1[0] + $"{timespan:hh\\:mm\\:ss}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 - 250), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[1] + $"{Globals.mStatistics["ScrapResources"] + Globals.mStatistics["NaturalResources"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 - 150), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[2] + $"{Globals.mStatistics["ScrapResources"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 - 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[3] + $"{Globals.mStatistics["NaturalResources"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 + 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[4] + $"{Globals.mStatistics["BasesTaken"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 + 150), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[5] + $"{Globals.mStatistics["FastestWin"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 - 250), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[6] + $"{Globals.mStatistics["FewestLosses"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 - 150), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[7] + $"{Globals.mStatistics["MostDestroyed"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 - 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[8] + $"{Globals.mStatistics["LeastDestroyed"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 + 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[9] + $"{Globals.mStatistics["MostLosses"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 + 150), Color.White);
            }
            else
            {
                var timespan = TimeSpan.FromSeconds(Globals.mRunStatistics["RunningSeconds"]);
                spriteBatch.DrawString(Art.Arial22, sStats1[0] + $"{timespan:hh\\:mm\\:ss}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 - 250), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[1] + $"{Globals.mRunStatistics["ScrapResources"] + Globals.mRunStatistics["NaturalResources"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 - 150), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[2] + $"{Globals.mRunStatistics["ScrapResources"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 - 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[3] + $"{Globals.mRunStatistics["NaturalResources"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 + 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats1[4] + $"{Globals.mRunStatistics["BasesTaken"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 + 150), Color.White);
            }
            
        }
    }
}