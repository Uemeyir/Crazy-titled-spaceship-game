using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Controls;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using sopra05_2223.SoundManagement;
using System;
using System.Collections.Generic;

namespace sopra05_2223.Screens
{
    internal sealed class GameWonScreen : IScreen
    {
        private List<Button> mComponents;

        // Calling component.Update() or component.Draw() requires a
        // GameTime object to be passed (for whatever reason?!).
        private readonly GameTime mGameTime = Globals.GameTime;

        private int mScreenWidth;
        private int mScreenHeight;
        private bool mCheckAchievement = true;

        private readonly GameScreen mCurrentGameScreen;

        public ScreenManager ScreenManager
        {
            get; set;
        }

        bool IScreen.UpdateLower => true;

        bool IScreen.DrawLower => true;

        public GameWonScreen(int screenWidth, int screenHeight, GameScreen currentGameScreen)
        {
            Globals.mGameSpeed = 1;
            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;
            mCurrentGameScreen = currentGameScreen;

            var seconds = Globals.mRunStatistics["RunningSeconds"];

            if (Globals.mStatistics["FastestWin"] == 0 || seconds < Globals.mStatistics["FastestWin"])
            {
                Globals.mStatistics["FastestWin"] = seconds;
            }
            if (Globals.mStatistics["FewestLosses"] == 0 || Globals.mStatistics["LostShips"] < Globals.mStatistics["FewestLosses"])
            {
                Globals.mStatistics["FewestLosses"] = Globals.mStatistics["LostShips"];
            }
            if (Globals.mStatistics["MostLosses"] == 0 || Globals.mStatistics["LostShips"] > Globals.mStatistics["MostLosses"])
            {
                Globals.mStatistics["MostLosses"] = Globals.mStatistics["LostShips"];
            }
            if (Globals.mStatistics["MostDestroyed"] == 0 || Globals.mStatistics["DestroyedEnemyShips"] > Globals.mStatistics["MostDestroyed"])
            {
                Globals.mStatistics["MostDestroyed"] = Globals.mStatistics["DestroyedEnemyShips"];
            }
            if (Globals.mStatistics["LeastDestroyed"] == 0 || Globals.mStatistics["DestroyedEnemyShips"] < Globals.mStatistics["LeastDestroyed"])
            {
                Globals.mStatistics["LeastDestroyed"] = Globals.mStatistics["DestroyedEnemyShips"];
            }

            Globals.mStatistics["TimesWon"] += 1;

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
                Text = "Du hast gewonnen!",
            };

            var mainMenuButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, buttonTopSide + verticalSpace),
                Text = "Zurück zum Hauptmenü",
            };
            mainMenuButton.Click += MainMenuButton_Click;


            // Store all menu buttons in one list to make updating the screen easier.
            mComponents = new List<Button>() {
                titleButton,
                mainMenuButton,
            };
        }

        private static readonly string[] sStats =
        {
            "Spieldauer: ",
            "Gesammlete Ressourcen: ",
            "Ressourcen durch Trümmerteile: ",
            "Gesammelte natürliche Ressourcen: ",
            "Übernommene Basen: ",
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

        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            ScreenManager.ReturnToMainMenu();
            mCurrentGameScreen.GetSoundManager().PlayMusic(MusicEnum.MusicMenu);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw everything in component (all Buttons).
            foreach (var component in mComponents)
            {
                component.Draw(mGameTime, spriteBatch);
            }

            var textleftside = mScreenWidth * 0.05f;
            var texttopside = mScreenHeight * 0.3f;
            var verticalspace = mScreenHeight * 0.04f;

            var timespan = TimeSpan.FromSeconds(Globals.mRunStatistics["RunningSeconds"]);
            spriteBatch.DrawString(Art.Arial12, sStats[0] + $"{timespan:hh\\:mm\\:ss}", new Vector2(textleftside, texttopside), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[1] + $"{Globals.mRunStatistics["ScrapResources"] + Globals.mRunStatistics["NaturalResources"]}", new Vector2(textleftside, texttopside + verticalspace), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[2] + $"{Globals.mRunStatistics["ScrapResources"]}", new Vector2(textleftside, texttopside + 2*verticalspace), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[3] + $"{Globals.mRunStatistics["NaturalResources"]}", new Vector2(textleftside, texttopside + 3*verticalspace), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[4] + $"{Globals.mRunStatistics["BasesTaken"]}", new Vector2(textleftside, texttopside + 4*verticalspace), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[5] + $"{Globals.mRunStatistics["BuiltMoerser"] + Globals.mRunStatistics["BuiltAttack"] + Globals.mRunStatistics["BuiltTransport"] + Globals.mRunStatistics["BuiltMedic"] + Globals.mRunStatistics["BuiltSpy"]}", new Vector2(textleftside, texttopside + 5*verticalspace), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[6] + $"{Globals.mRunStatistics["BuiltMoerser"]}", new Vector2(textleftside, texttopside + 6*verticalspace), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[7] + $"{Globals.mRunStatistics["BuiltAttack"]}", new Vector2(textleftside, texttopside + 7*verticalspace), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[8] + $"{Globals.mRunStatistics["BuiltTransport"]}", new Vector2(textleftside, texttopside + 8*verticalspace), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[9] + $"{Globals.mRunStatistics["BuiltMedic"]}", new Vector2(textleftside, texttopside + 9*verticalspace), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[10] + $"{Globals.mRunStatistics["BuiltSpy"]}", new Vector2(textleftside, texttopside + 10*verticalspace), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[11] + $"{Globals.mRunStatistics["DestroyedEnemyShips"] + Globals.mRunStatistics["LostShips"]}", new Vector2(textleftside, texttopside + 11*verticalspace), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[12] + $"{Globals.mRunStatistics["DestroyedEnemyShips"]}", new Vector2(textleftside, texttopside + 12*verticalspace), Color.Black);
            spriteBatch.DrawString(Art.Arial12, sStats[13] + $"{Globals.mRunStatistics["LostShips"]}", new Vector2(textleftside, texttopside + 13*verticalspace), Color.Black);
        }

        public void Resize(Point newSize)
        {
            mScreenHeight = newSize.Y;
            mScreenWidth = newSize.X;
            CalculateButtons(newSize);
        }

        public void Update(Input input)
        {
            if (mCheckAchievement)
            {
                CheckAchievement();
                mCheckAchievement = false;
            }
            foreach (var component in mComponents)
            {
                // Update all Buttons.
                component.Update(mGameTime);
            }
        }

        private void CheckAchievement()
        {
            if (Globals.mStatistics["TimesWon"] == 1)
            {
                Globals.mAchievements["AllBases1"] = true;
                Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Fürst der Sterne", new Point(mScreenWidth, mScreenHeight)));
            }
            else if (Globals.mStatistics["TimesWon"] == 5)
            {
                Globals.mAchievements["AllBases2"] = true;
                Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "König der Galaxien", new Point(mScreenWidth, mScreenHeight)));
            }
            else if (Globals.mStatistics["TimesWon"] == 15)
            {
                Globals.mAchievements["AllBases3"] = true;
                Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Herrscher über den Kosmos", new Point(mScreenWidth, mScreenHeight)));
            }

            if (Globals.mAchievements["5000EnemyShips"] == false && Globals.mRunStatistics["DestroyedEnemyShips"] >= 5000)
            {
                Globals.mAchievements["5000EnemyShips"] = true;
                Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Zerstörer", new Point(mScreenWidth, mScreenHeight)));
            }
            if (Globals.mAchievements["500EnemyShips"] == false && Globals.mRunStatistics["DestroyedEnemyShips"] <= 500)
            {
                Globals.mAchievements["500EnemyShips"] = true;
                Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Pazifist", new Point(mScreenWidth, mScreenHeight)));
            }
            if (Globals.mAchievements["1000LostShips"] == false && Globals.mRunStatistics["LostShips"] >= 1000)
            {
                Globals.mAchievements["1000LostShips"] = true;
                Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Pyrrhussieg", new Point(mScreenWidth, mScreenHeight)));
            }

            if (Globals.mAchievements["OneShipType"] == false)
            {
                var usedtypes = 0;
                foreach (var k in Globals.mBuilt)
                {
                    if (k.Value)
                    {
                        usedtypes++;
                    }
                }
                if (usedtypes == 1)
                {
                    Globals.mAchievements["OneShipType"] = true;
                    Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Keep It Very Simple", new Point(mScreenWidth, mScreenHeight)));
                }
            }
        }
    }
}
