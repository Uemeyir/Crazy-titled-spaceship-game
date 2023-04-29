using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Controls;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using System;
using System.Collections.Generic;
using sopra05_2223.SoundManagement;

namespace sopra05_2223.Screens
{
    internal sealed class GameOverScreen : IScreen
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

        public GameOverScreen(int screenWidth, int screenHeight, GameScreen currentGameScreen)
        {
            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;
            mCurrentGameScreen = currentGameScreen;
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
                Text = "Du hast verloren!",
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

        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            ScreenManager.ReturnToMainMenu();
            mCurrentGameScreen.GetSoundManager().PlayMusic(MusicEnum.MusicMenu);
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
            spriteBatch.DrawString(Art.Arial12, sStats[0] + $"{timespan:hh\\:mm\\:ss}", new Vector2(textleftside, texttopside), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[1] + $"{Globals.mRunStatistics["ScrapResources"] + Globals.mRunStatistics["NaturalResources"]}", new Vector2(textleftside, texttopside + verticalspace), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[2] + $"{Globals.mRunStatistics["ScrapResources"]}", new Vector2(textleftside, texttopside + 2 * verticalspace), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[3] + $"{Globals.mRunStatistics["NaturalResources"]}", new Vector2(textleftside, texttopside + 3 * verticalspace), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[4] + $"{Globals.mRunStatistics["BasesTaken"]}", new Vector2(textleftside, texttopside + 4 * verticalspace), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[5] + $"{Globals.mRunStatistics["BuiltMoerser"] + Globals.mRunStatistics["BuiltAttack"] + Globals.mRunStatistics["BuiltTransport"] + Globals.mRunStatistics["BuiltMedic"] + Globals.mRunStatistics["BuiltSpy"]}", new Vector2(textleftside, texttopside + 5 * verticalspace), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[6] + $"{Globals.mRunStatistics["BuiltMoerser"]}", new Vector2(textleftside, texttopside + 6 * verticalspace), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[7] + $"{Globals.mRunStatistics["BuiltAttack"]}", new Vector2(textleftside, texttopside + 7 * verticalspace), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[8] + $"{Globals.mRunStatistics["BuiltTransport"]}", new Vector2(textleftside, texttopside + 8 * verticalspace), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[9] + $"{Globals.mRunStatistics["BuiltMedic"]}", new Vector2(textleftside, texttopside + 9 * verticalspace), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[10] + $"{Globals.mRunStatistics["BuiltSpy"]}", new Vector2(textleftside, texttopside + 10 * verticalspace), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[11] + $"{Globals.mRunStatistics["DestroyedEnemyShips"] + Globals.mRunStatistics["LostShips"]}", new Vector2(textleftside, texttopside + 11 * verticalspace), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[12] + $"{Globals.mRunStatistics["DestroyedEnemyShips"]}", new Vector2(textleftside, texttopside + 12 * verticalspace), Color.White);
            spriteBatch.DrawString(Art.Arial12, sStats[13] + $"{Globals.mRunStatistics["LostShips"]}", new Vector2(textleftside, texttopside + 13 * verticalspace), Color.White);
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
                if (Globals.mAchievements["GameOver"] == false)
                {
                    Globals.mAchievements["GameOver"] = true;
                    Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Game Over", new Point(mScreenWidth, mScreenHeight)));
                }

                mCheckAchievement = false;
            }
            foreach (var component in mComponents)
            {
                // Update all Buttons.
                component.Update(mGameTime);
            }
        }
    }
}