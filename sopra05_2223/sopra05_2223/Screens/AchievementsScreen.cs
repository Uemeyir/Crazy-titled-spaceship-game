using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using sopra05_2223.Controls;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using System;
using System.Collections.Generic;
using sopra05_2223.Serializer;

namespace sopra05_2223.Screens
{
    internal sealed class AchievementsScreen : IScreen
    {
        private List<Button> mComponents;

        private readonly GameTime mGameTime = Globals.GameTime;

        private readonly int mScreenWidth;
        private readonly int mScreenHeight;

        public ScreenManager ScreenManager
        {
            get; set;
        }
        bool IScreen.UpdateLower => true;

        bool IScreen.DrawLower => true;

        private static readonly string[] sTextEnemy =
            {
                "/100 Zerstörte gegnerische Einheiten",
                "/500 Zerstörte gegnerische Einheiten",
                "/2000 Zerstörte gegnerische Einheiten",
                "/5000 Zerstörte gegnerische Einheiten",
                "/15000 Zerstörte gegnerische Einheiten",
                " Zerstörte gegnerische Einheiten"
            };
        private static readonly string[] sTextResource =
            {
                "/1000 Gesammelte Ressourcen",
                "/5000 Gesammelte Ressourcen",
                "/25000 Gesammelte Ressourcen",
                "/100000 Gesammelte Ressourcen",
                "/500000 Gesammelte Ressourcen",
                " Gesammelte Ressourcen"
            };

        private static readonly  string[] sTextBases =
            {
                "/1 Mal alle Basen erobert",
                "/5 Mal alle Basen erobert",
                "/15 Mal alle Basen erobert",
                " Mal alle Basen erobert",
            };
        private static readonly string[] sTextOtherAchievements =
            {
                "Erkunde alle vier Ecken der Karte",
                "Verliere alle Raumschiffe und Basen",
                "Baue nur einen Schiffstypen (außer Transport) und gewinne",
                "Zerstöre mind. 5000 gegnerische Schiffe in einem Durchlauf",
                "Zerstöre max. 500 gegnerische Schiffe in einem Durchlauf",
                "Gewinne mit mind. 1000 Schiffsverlusten"
            };
        private static readonly string[] sEnemyShipAchievements =
            {
                "EnemyShips1",
                "EnemyShips2",
                "EnemyShips3",
                "EnemyShips4",
                "EnemyShips5",
            };
        private static readonly string[] sResourceAchievements =
            {
                "Resource1",
                "Resource2",
                "Resource3",
                "Resource4",
                "Resource5"
            };
        private static readonly string[] sBasesAchievement =
            {
                "AllBases1",
                "AllBases2",
                "AllBases3"
            };
        private static readonly string[] sOtherAchievements =
            {
                "AllCorners",
                "GameOver",
                "OneShipType",
                "5000EnemyShips",
                "500EnemyShips",
                "1000LostShips"
            };

        private static readonly string[] sNameEnemy =
            {
                "Feldwebel: ",
                "Leutnant: ",
                "Hauptmann: ",
                "Major: ",
                "General: ",
                "General: "
            };
        private static readonly string[] sNameResource =
            {
                "Sucher: ",
                "Schürfer: ",
                "Sammler: ",
                "Arbeiter: ",
                "Krösus: ",
                "Krösus: "
            };

        private static readonly string[] sNameBases =
            {
                "Fürst der Sterne: ",
                "König der Galaxien: ",
                "Herrscher über den Kosmos: ",
                "Herrscher über den Kosmos: "
            };
        private static readonly string[] sNameOtherAchievements =
            {
                "Ist hier noch was?: ",
                "Game Over: ",
                "Keep It Very Simple: ",
                "Zerstörer: ",
                "Pazifist: ",
                "Pyrrhussieg: "
            };

        public AchievementsScreen(int screenWidth, int screenHeight)
        {
            CalculateButtons(new Point(screenWidth, screenHeight));
            mScreenWidth = screenWidth;
            mScreenHeight = screenHeight;
            SopraSerializer.DeserializeAchievements();
        }

        private void CalculateButtons(Point screenSize)
        {
            // buttonwidth should be considered, didnt find a smart way to do that
            var buttonLeftSide = screenSize.X * 0.77f;

            var backToPreviousScreen = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 2 - 50),
                Text = "Zurück"
            };
            // For the functionality
            backToPreviousScreen.Click += BackToPreviousScreen_Click;

            var statsScreenButton = new Button()
            {
                Position = new Vector2(buttonLeftSide, screenSize.Y / 2f),
                Text = "Statistiken"
            };
            statsScreenButton.Click += StatsScreenButton_Click;

            // store button and stats to draw it
            mComponents = new List<Button>() {
                backToPreviousScreen,
                statsScreenButton,
            };
        }

        private void BackToPreviousScreen_Click(object sender, EventArgs e)
        {
            // Remove 2 Screens from Screen stack (AchScreen and MenuBackgroundScreen).
            ScreenManager.RemoveScreens(2);
        }

        private void StatsScreenButton_Click(object sender, EventArgs e)
        {
            // Remove 1 Screen from Screen stack (AchScreen).
            ScreenManager.RemoveScreens();
            // Replace it with the stats screen.
            ScreenManager.AddScreen(new StatisticScreen(mScreenWidth, mScreenHeight));
        }

        void IScreen.Resize(Point newSize)
        {
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

        private void DrawTextGroup(SpriteBatch spriteBatch, IReadOnlyList<string> lines, Vector2 start, Vector2 delta)
        {
            for (var i=0; i < lines.Count; i++)
            {
                spriteBatch.DrawString(Art.Arial22, lines[i], new Vector2(mScreenWidth * (start.X + i * delta.X), mScreenHeight * (start.Y + i * delta.Y)), Color.White);
            }
        }

        private int DrawAchievementGroup(SpriteBatch spriteBatch, IReadOnlyList<string> achievements, Vector2 start, Vector2 delta)
        {
            var lvl = 0;
            for (var i=0; i < achievements.Count; i++)
            {
                var color = Color.Gray;
                if (Globals.mAchievements[achievements[i]])
                {
                    color = Color.White;
                    lvl++;
                }
                spriteBatch.Draw(Art.Achievement, new Rectangle((int)(mScreenWidth * (start.X + i * delta.X)), (int)(mScreenHeight * (start.Y + i * delta.Y)), (int)(mScreenWidth * 0.05f), (int)(mScreenWidth * 0.05f)), color);
            }
            return lvl;
        }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            // Draw everything in component (all Buttons).
            foreach (var component in mComponents)
            {
                component.Draw(mGameTime, spriteBatch);
            }

            var enemyShipsLvl = DrawAchievementGroup(spriteBatch, sEnemyShipAchievements, new Vector2(0.05f, 0.05f), new Vector2(0.05f, 0f));
            var resourceLvl = DrawAchievementGroup(spriteBatch, sResourceAchievements, new Vector2(0.05f, 0.15f), new Vector2(0.05f, 0f));
            var allBasesLvl = DrawAchievementGroup(spriteBatch, sBasesAchievement, new Vector2(0.10f, 0.25f), new Vector2(0.05f, 0f));

            spriteBatch.DrawString(Art.Arial22, sNameEnemy[enemyShipsLvl] + $"{Globals.mStatistics["DestroyedEnemyShips"]}" + sTextEnemy[enemyShipsLvl], new Vector2(mScreenWidth * 0.35f, mScreenHeight * 0.07f), Color.White);
            spriteBatch.DrawString(Art.Arial22, sNameResource[resourceLvl] + $"{Globals.mStatistics["TotalResources"]}" + sTextResource[resourceLvl], new Vector2(mScreenWidth * 0.35f, mScreenHeight * 0.17f), Color.White);
            spriteBatch.DrawString(Art.Arial22, sNameBases[allBasesLvl] + $"{Globals.mStatistics["TimesWon"]}" + sTextBases[allBasesLvl],  new Vector2(mScreenWidth * 0.35f, mScreenHeight * 0.27f), Color.White);
                        
            DrawAchievementGroup(spriteBatch, sOtherAchievements, new Vector2(0.15f, 0.35f), new Vector2(0f, 0.1f));
            DrawTextGroup(spriteBatch, sTextOtherAchievements, new Vector2(0.35f, 0.39f), new Vector2(0f, 0.1f));
            DrawTextGroup(spriteBatch, sNameOtherAchievements, new Vector2(0.35f, 0.35f), new Vector2(0f, 0.1f));
        }
    }
}
