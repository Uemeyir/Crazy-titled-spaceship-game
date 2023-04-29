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

namespace sopra05_2223.Screens
{
    internal sealed class StatisticScreenTwo: IScreen
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

        public StatisticScreenTwo(int screenWidth, int screenHeight, bool runStatistics = false)
        {
            mRunStatistics = runStatistics;
            CalculateButtons(new Point(screenWidth, screenHeight));
            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;
            SopraSerializer.DeserializeStatistics();
        }

        private static readonly string[] sStats2 =
        {
            "Anzahl an Schiffe: ",
            "Anzahl an Mörser: ",
            "Anzahl an Angriffsschiffen: ",
            "Anzahl an Transportschiffen: ",
            "Anzahl an Medicschiffen: ",
            "Anzahl an Spionageschiffen: ",
            "Zerstörte Schiffe: ",
            "Zerstörte feindliche Schiffe: ",
            "Zerstörte Schiffe des Spielers: ",
        };

        private void CalculateButtons(Point screenSize)
        {
            // buttonwidth should be considered, didnt find a smart way to do that
            var buttonLeftSide = screenSize.X / 2 - 100;

            var backToPreviousScreen = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 2 + 250),
                Text = "Zurück zum Hauptmenü"
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
            if (mRunStatistics)
            {
                runStatisticsButton.Text = "Statistiken gesamt";
            }
            runStatisticsButton.Click += RunStatisticsButton_Click;

            // For the seocond stats screen
            var backToFirstStatsScreenButton = new Button()
            {
                Position = new Vector2(buttonLeftSide + 350, screenSize.Y / 2 + 250),
                Text = "Zurück"
            };
            backToFirstStatsScreenButton.Click+= BackToFirstStatsScreen_Click;

            // store button and stats to draw it
            mComponents = new List<Button>() {
                backToFirstStatsScreenButton,
                backToPreviousScreen,
                achsScreenButton,
                runStatisticsButton,
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

        private void BackToFirstStatsScreen_Click(object sender, EventArgs e)
        {
            // Remove 1 Screen from Screen stack
            ScreenManager.RemoveScreens();
            // Replace it with the secondStatScreen.
            ScreenManager.AddScreen(new StatisticScreen(mScreenWidth,mScreenHeight, mRunStatistics));
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
                spriteBatch.DrawString(Art.Arial22, sStats2[0] + $"{Globals.mStatistics["BuiltMoerser"] + Globals.mStatistics["BuiltAttack"] + Globals.mStatistics["BuiltTransport"] + Globals.mStatistics["BuiltMedic"] + Globals.mStatistics["BuiltSpy"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 - 250), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[1] + $"{Globals.mStatistics["BuiltMoerser"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 - 150), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[2] + $"{Globals.mStatistics["BuiltAttack"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 - 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[3] + $"{Globals.mStatistics["BuiltTransport"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 + 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[4] + $"{Globals.mStatistics["BuiltMedic"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 - 250), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[5] + $"{Globals.mStatistics["BuiltSpy"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 - 150), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[6] + $"{Globals.mStatistics["DestroyedEnemyShips"] + Globals.mStatistics["LostShips"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 - 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[7] + $"{Globals.mStatistics["DestroyedEnemyShips"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 + 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[8] + $"{Globals.mStatistics["LostShips"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 + 150), Color.White);
            }
            else
            {
                spriteBatch.DrawString(Art.Arial22, sStats2[0] + $"{Globals.mRunStatistics["BuiltMoerser"] + Globals.mRunStatistics["BuiltAttack"] + Globals.mRunStatistics["BuiltTransport"] + Globals.mRunStatistics["BuiltMedic"] + Globals.mRunStatistics["BuiltSpy"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 - 250), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[1] + $"{Globals.mRunStatistics["BuiltMoerser"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 - 150), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[2] + $"{Globals.mRunStatistics["BuiltAttack"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 - 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[3] + $"{Globals.mRunStatistics["BuiltTransport"]}", new Vector2((mScreenWidth / 2 - 200) - 350, mScreenHeight / 2 + 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[4] + $"{Globals.mRunStatistics["BuiltMedic"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 - 250), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[5] + $"{Globals.mRunStatistics["BuiltSpy"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 - 150), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[6] + $"{Globals.mRunStatistics["DestroyedEnemyShips"] + Globals.mRunStatistics["LostShips"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 - 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[7] + $"{Globals.mRunStatistics["DestroyedEnemyShips"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 + 50), Color.White);
                spriteBatch.DrawString(Art.Arial22, sStats2[8] + $"{Globals.mRunStatistics["LostShips"]}", new Vector2((mScreenWidth / 2 - 200) + 350, mScreenHeight / 2 + 150), Color.White);
            }
        }
    }
}