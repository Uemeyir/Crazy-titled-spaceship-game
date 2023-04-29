using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Core;
using sopra05_2223.Core.Entity;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.ShipBase;
using sopra05_2223.NotificationSystem;

namespace sopra05_2223.Screens
{
    internal sealed class Hud : IScreen
    {
        // Counts quantity of resources
        private static int sAmountOxygen;
        private static int sAmountMetal;

        // Notifications
        private readonly NotificationManager mNotificationManager = Globals.NotificationManager;

        // Indicates whether notification menu is open
        private bool mNotificationMenuOpen;

        private int mScreenWidth;
        private int mScreenHeight;

        // Options menu items
        private static int sSelectedAttacker;
        private static int sSelectedSpy;
        private static int sSelectedTransport;
        private static int sSelectedMoerser;
        private static int sSelectedMedic;
        private static List<Entity> sSelectedEntities;
        private static string sShowOptions;
        private int mOxygenAttacker;
        private int mMetalAttacker;
        private int mOxygenSpy;
        private int mMetalSpy;
        private int mOxygenTransport;
        private int mMetalTransport;
        private int mOxygenMoerser;
        private int mMetalMoerser;
        private int mOxygenMedic;
        private int mMetalMedic;

        public Hud(int screenWidth, int screenHeight)
        {

            sAmountOxygen = 0;
            sAmountMetal = 0;

            mOxygenAttacker = 0;
            mMetalAttacker = 0;
            mOxygenSpy = 0;
            mMetalSpy = 0;
            mOxygenTransport = 0;
            mMetalTransport = 0;
            mOxygenMoerser = 0;
            mMetalMoerser = 0;
            mOxygenMedic = 0;
            mMetalMedic = 0;

            sSelectedAttacker = 0;
            sSelectedSpy = 0;
            sSelectedTransport = 0;
            sSelectedMoerser = 0;
            sSelectedMedic = 0;
            sSelectedEntities = new List<Entity>();
            sShowOptions = "Attack Ship";

            mNotificationMenuOpen = false;

            this.mScreenWidth = screenWidth;
            this.mScreenHeight = screenHeight;
        }

        public ScreenManager ScreenManager
        {
            get;
            set;
        }

        bool IScreen.UpdateLower => true;

        bool IScreen.DrawLower => true;

        void IScreen.Resize(Point newSize)
        {
            mScreenHeight = newSize.Y;
            mScreenWidth = newSize.X;
        }
        void IScreen.Update(Input input)
        {
            if (mNotificationMenuOpen == false)
            {
            }
            if (new Rectangle(this.mScreenWidth - 75, this.mScreenHeight - 75, 60, 60).Contains(input.GetMousePosition()) && input.GetMouseButton(0) == MouseActionEnum.Pressed)
            {
                Click(ButtonEnum.Left);
            }

            // Checks which spaceship is selected in the options menu
            if (new Rectangle(this.mScreenWidth - 300, 20, 50, 50).Contains(input.GetMousePosition()) && input.GetMouseButton(0) == MouseActionEnum.Pressed)
            {
                sShowOptions = "Attack Ship";
            }
            else if (new Rectangle(this.mScreenWidth - 240, 20, 50, 50).Contains(input.GetMousePosition()) && input.GetMouseButton(0) == MouseActionEnum.Pressed)
            {
                sShowOptions = "Spy";
            }
            else if (new Rectangle(this.mScreenWidth - 180, 20, 50, 50).Contains(input.GetMousePosition()) && input.GetMouseButton(0) == MouseActionEnum.Pressed)
            {
                sShowOptions = "Transport";
            }
            else if (new Rectangle(this.mScreenWidth - 120, 20, 50, 50).Contains(input.GetMousePosition()) && input.GetMouseButton(0) == MouseActionEnum.Pressed)
            {
                sShowOptions = "Moerser";
            }
            else if (new Rectangle(this.mScreenWidth - 60, 20, 50, 50).Contains(input.GetMousePosition()) && input.GetMouseButton(0) == MouseActionEnum.Pressed)
            {
                sShowOptions = "Medic";
            }

            // Starts Option depending on which spaceship is selected
            var keys = input.GetKeys();
            if (sSelectedSpy == 1 && sShowOptions == "Spy" && keys.Contains(Keys.O))
            {
                foreach (var k in sSelectedEntities.Cast<ESpy>())
                {
                    k.GetComponent<CBuoyPlacer>().mShouldPlaceBuoy = true;
                }
            }

            mNotificationManager.Update();

            DeleteUnselectedShips();
        }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            ShowResourcesOfSelectedUnits();

            spriteBatch.Draw(Art.Sauerstoffflasche, new Rectangle(5, 5, 35, 40), Color.White);
            spriteBatch.Draw(Art.Metall, new Rectangle(5, 45, 35, 40), Color.White);
            spriteBatch.Draw(Art.Ausrufezeichen, new Rectangle(this.mScreenWidth - 75, this.mScreenHeight - 75, 60, 60), Color.White);
            spriteBatch.DrawString(Art.Arial22, sAmountOxygen.ToString(), new Vector2(45, 5), Color.White);
            spriteBatch.DrawString(Art.Arial22, sAmountMetal.ToString(), new Vector2(45, 45), Color.White);
            if (ScreenManager.GetStack().OfType<ShipBaseScreen>().Any())
            {
                ShipBaseScreen shipscreen = (ShipBaseScreen)ScreenManager.GetScreen(typeof(ShipBaseScreen));
                if (shipscreen != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        spriteBatch.Draw(Art.OptionsMenu, new Rectangle(this.mScreenWidth - 305, 5, 300, 200), Color.White * 0.3f);
                        spriteBatch.Draw(Art.Healthbar, new Rectangle(mScreenWidth - 280, 80 + 20 * i, 250, 15), Color.White);
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, Art.Health);
                        if (shipscreen.GetEntity().GetComponent<BuildSlotManager>() != null && shipscreen.GetEntity().GetComponent<CTeam>().mTeam is Team.Player or Team.Ki)
                        {
                            var slottotal = shipscreen.GetEntity().GetComponent<BuildSlotManager>().GetBuildSlots()[i].GetTotalDuration();
                            var slotcurrent = shipscreen.GetEntity().GetComponent<BuildSlotManager>().GetBuildSlots()[i].GetElapsedTime();
                            Art.Health.Parameters["HealthPercent"].SetValue(slotcurrent / slottotal);
                            spriteBatch.Draw(Art.Healthbar, new Rectangle(mScreenWidth - 280, 80 + 20 * i, 250, 15), Color.White);
                        }
                        spriteBatch.End();
                        spriteBatch.Begin();
                    }
                }
            }
            else if (sSelectedTransport + sSelectedAttacker + sSelectedSpy + sSelectedMoerser + sSelectedMedic > 0)
            {
                spriteBatch.Draw(Art.OptionsMenu, new Rectangle(this.mScreenWidth - 305, 5, 300, 200), Color.White);
                spriteBatch.DrawString(Art.Arial12,
                    sSelectedAttacker.ToString("N0"),
                    new(this.mScreenWidth - 280, 80),
                    Color.White);
                spriteBatch.DrawString(Art.Arial12,
                    sSelectedSpy.ToString("N0"),
                    new(this.mScreenWidth - 220, 80),
                    Color.White);
                spriteBatch.DrawString(Art.Arial12,
                    sSelectedTransport.ToString("N0"),
                    new(this.mScreenWidth - 160, 80),
                    Color.White);
                spriteBatch.DrawString(Art.Arial12,
                    sSelectedMoerser.ToString("N0"),
                    new(this.mScreenWidth - 100, 80),
                    Color.White);
                spriteBatch.DrawString(Art.Arial12,
                    sSelectedMedic.ToString("N0"),
                    new(this.mScreenWidth - 40, 80),
                    Color.White);
                spriteBatch.Draw(Art.Bomber, new Rectangle(this.mScreenWidth - 300, 20, 50, 50), Color.White);
                spriteBatch.Draw(Art.Spy, new Rectangle(this.mScreenWidth - 240, 20, 50, 50), Color.White);
                spriteBatch.Draw(Art.Transport, new Rectangle(this.mScreenWidth - 180, 20, 50, 50), Color.White);
                spriteBatch.Draw(Art.Moerser, new Rectangle(this.mScreenWidth - 120, 20, 50, 50), Color.White);
                spriteBatch.Draw(Art.Medic, new Rectangle(this.mScreenWidth - 60, 20, 50, 50), Color.White);
            }

            Vector2 textPosition = new(this.mScreenWidth * 0.71f, this.mScreenHeight * 0.9f);

            // Draws notification when condition is met

            // Draws Notification Menu
            if (mNotificationMenuOpen)
            {
                mNotificationManager.DrawNotificationsMenu(spriteBatch, textPosition);
                /*
                spriteBatch.Draw(Art.NotificationMenu, new Rectangle(175, 10, 450, 460), Color.White);
                for (var i = 0; i < mNotifications.Count; ++i)
                {
                    //TODO: change position to be relative to screenSize
                    spriteBatch.DrawString(Art.Arial12, mNotifications[i], new Vector2(195, 50) + (i * new Vector2(0, 30)), Color.White);
                }
                */
            }
            else
            {
                mNotificationManager.DrawNewNotifications(spriteBatch, textPosition);
            }

            // Outputs options of the selected spaceship
            if (sSelectedTransport + sSelectedAttacker + sSelectedSpy + sSelectedMoerser + sSelectedMedic > 0)
            {
                if (sShowOptions == "Attack Ship")
                {
                    spriteBatch.Draw(Art.ActionMenuBomber, new Rectangle(this.mScreenWidth - 300, 20, 50, 50), Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Sauerstoff: " + mOxygenAttacker + " / " + Globals.sStorage["Attacker"].X + "  -  Metall: " + mMetalAttacker + " / " + Globals.sStorage["Attacker"].Y,
                        new Vector2(this.mScreenWidth - 300, 110),
                        Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Fähigkeiten Attacker:",
                        new Vector2(this.mScreenWidth - 300, 140),
                        Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Gegner gezielt angreifen: Rechtsklick.",
                        new Vector2(this.mScreenWidth - 300, 170),
                        Color.White);
                }

                else if (sShowOptions == "Spy")
                {
                    spriteBatch.Draw(Art.ActionMenuSpy,
                        new Rectangle(this.mScreenWidth - 240, 20, 50, 50),
                        Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Sauerstoff: " + mOxygenSpy.ToString() + " / " + Globals.sStorage["Spy"].X + "  -  Metall: " + mMetalSpy.ToString() + " / " + Globals.sStorage["Spy"].Y,
                        new Vector2(this.mScreenWidth - 300, 110),
                        Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Fähigkeiten Spionageschiff:",
                        new Vector2(this.mScreenWidth - 300, 140),
                        Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Boje setzen: Drücke 'B', danach Rechtsklick auf gewünschte Stelle",
                        new Vector2(this.mScreenWidth - 300, 170),
                        Color.White);
                    if (sSelectedSpy == 1)
                    {
                        List<ESpy> spy = new();
                        foreach (var k in sSelectedEntities)
                        {
                            if (k is ESpy spy1)
                            {
                                spy.Add(spy1);
                            }
                        }
                        foreach (var k in spy)
                        {
                            spriteBatch.DrawString(Art.Arial12,
                                $"{k.GetComponent<CBuoyPlacer>().mBuoyCount}",
                                new Vector2(this.mScreenWidth - 100, 170),
                                Color.White);
                            if (k.GetComponent<CBuoyPlacer>().mBuoyList.Count == 1 && k.GetComponent<CBuoyPlacer>().mBuoyList[0].GetComponent<CLifespan>()?.GetRemaining() > 0)
                            {
                                spriteBatch.DrawString(Art.Arial12,
                                    $"{k.GetComponent<CBuoyPlacer>().mBuoyList[0].GetComponent<CLifespan>()?.GetRemaining()}",
                                    new Vector2(this.mScreenWidth - 80, 110),
                                    Color.White);
                            }
                            if (k.GetComponent<CBuoyPlacer>().mBuoyList.Count == 2)
                            {
                                if (k.GetComponent<CBuoyPlacer>().mBuoyList[0].GetComponent<CLifespan>()
                                        ?.GetRemaining() > 0)
                                {
                                    spriteBatch.DrawString(Art.Arial12,
                                        $"{k.GetComponent<CBuoyPlacer>().mBuoyList[0].GetComponent<CLifespan>()?.GetRemaining()}",
                                        new Vector2(this.mScreenWidth - 80, 110),
                                        Color.White);
                                }

                                if (k.GetComponent<CBuoyPlacer>().mBuoyList[1].GetComponent<CLifespan>()
                                        ?.GetRemaining() > 0)
                                {
                                    spriteBatch.DrawString(Art.Arial12,
                                        $"{k.GetComponent<CBuoyPlacer>().mBuoyList[1].GetComponent<CLifespan>()?.GetRemaining()}",
                                        new Vector2(this.mScreenWidth - 80, 140),
                                        Color.White);
                                }
                            }
                        }
                    }
                }
                else if (sShowOptions == "Transport")
                {
                    spriteBatch.Draw(Art.ActionMenuTransport, new Rectangle(this.mScreenWidth - 180, 20, 50, 50), Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Sauerstoff: " + mOxygenTransport.ToString() + " / " + Globals.sStorage["Transport"].X + "  -  Metall: " + mMetalTransport.ToString() + " / " + Globals.sStorage["Transport"].Y,
                        new Vector2(this.mScreenWidth - 300, 110),
                        Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Fähigkeiten Transportschiff:",
                        new Vector2(this.mScreenWidth - 300, 140),
                        Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Transportiert und sammelt Ressourcen.",
                        new Vector2(this.mScreenWidth - 300, 170),
                        Color.White);
                }

                else if (sShowOptions == "Moerser")
                {
                    spriteBatch.Draw(Art.ActionMenuMoerser, new Rectangle(this.mScreenWidth - 120, 20, 50, 50), Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Sauerstoff: " + mOxygenMoerser.ToString() + " / " + Globals.sStorage["Moerser"].X + "  -  Metall: " + mMetalMoerser.ToString() + " / " + Globals.sStorage["Moerser"].Y,
                        new Vector2(this.mScreenWidth - 300, 110),
                        Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Fähigkeiten Mörserschiff:",
                        new Vector2(this.mScreenWidth - 300, 140),
                        Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Gegner gezielt angreifen: Rechtsklick.",
                        new Vector2(this.mScreenWidth - 300, 170),
                        Color.White);
                }

                else if (sShowOptions == "Medic")
                {
                    spriteBatch.Draw(Art.ActionMenuMedic, new Rectangle(this.mScreenWidth - 60, 20, 50, 50), Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Sauerstoff: " + mOxygenMedic.ToString() + " / " + Globals.sStorage["Medic"].X + "  -  Metall: " + mMetalMedic.ToString() + " / " + Globals.sStorage["Medic"].Y,
                        new Vector2(this.mScreenWidth - 300, 110),
                        Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Fähigkeiten Medicschiff:",
                        new Vector2(this.mScreenWidth - 300, 140),
                        Color.White);
                    spriteBatch.DrawString(Art.Arial12,
                        "Einheiten gezielt heilen: Rechtsklick.",
                        new Vector2(this.mScreenWidth - 300, 170),
                        Color.White);
                }
            }
        }


        private void Click(ButtonEnum button)
        {
            if (button == ButtonEnum.Left)
            {
                mNotificationMenuOpen = !mNotificationMenuOpen;
            }
        }

        private void DeleteUnselectedShips()
        {
            if (sSelectedEntities is null)
            {
                return;
            }

            for (int i = 0; i < sSelectedEntities.Count; i++)
            {
                Entity entity = sSelectedEntities[i];

                if (entity.GetComponent<CSelect>().mSelected is false)
                {
                    sSelectedEntities.Remove(entity);
                    switch (entity)
                    {
                        case EAttacker:
                            sSelectedAttacker -= 1;
                            break;
                        case ESpy:
                            sSelectedSpy -= 1;
                            break;
                        case ETransport:
                            sSelectedTransport -= 1;
                            break;
                        case EMedic:
                            sSelectedMedic -= 1;
                            break;
                        case EMoerser:
                            sSelectedMoerser -= 1;
                            break;
                    }
                }
            }
        }


        // Counts selected ship types
        public static void ShipCounter(int amount, Entity other)
        {
            if (sSelectedEntities is null)
            {
                return;
            }

            if (sSelectedEntities.Contains(other) is false)
            {
                if (amount == 1)
                {
                    sSelectedEntities.Add(other);
                    switch (other)
                    {
                        case EAttacker:
                            sSelectedAttacker += amount;
                            break;
                        case ESpy:
                            sSelectedSpy += amount;
                            break;
                        case ETransport:
                            sSelectedTransport += amount;
                            break;
                        case EMedic:
                            sSelectedMedic += amount;
                            break;
                        case EMoerser:
                            sSelectedMoerser += amount;
                            break;
                    }
                }
            }
            else
            {
                if (amount == -1)
                {
                    sSelectedEntities.Remove(other);
                    switch (other)
                    {
                        case EAttacker:
                            sSelectedAttacker += amount;
                            break;
                        case ESpy:
                            sSelectedSpy += amount;
                            break;
                        case ETransport:
                            sSelectedTransport += amount;
                            break;
                        case EMedic:
                            sSelectedMedic += amount;
                            break;
                        case EMoerser:
                            sSelectedMoerser += amount;
                            break;
                    }
                }
            }
        }

        private void ShowResourcesOfSelectedUnits()
        {
            sAmountMetal = 0;
            sAmountOxygen = 0;
            mOxygenAttacker = 0;
            mMetalAttacker = 0;
            mOxygenSpy = 0;
            mMetalSpy = 0;
            mOxygenTransport = 0;
            mMetalTransport = 0;
            mOxygenMoerser = 0;
            mMetalMoerser = 0;
            mOxygenMedic = 0;
            mMetalMedic = 0;
            foreach (var entity in sSelectedEntities)
            {
                if (entity.GetComponent<CStorage>() != null)
                {
                    sAmountMetal += entity.GetComponent<CStorage>().GetMetal();
                    sAmountOxygen += entity.GetComponent<CStorage>().GetOxygen();

                    switch (entity)
                    {
                        case EAttacker:
                            mOxygenAttacker += entity.GetComponent<CStorage>().GetOxygen();
                            mMetalAttacker += entity.GetComponent<CStorage>().GetMetal();
                            break;
                        case ESpy:
                            mOxygenSpy += entity.GetComponent<CStorage>().GetOxygen();
                            mMetalSpy += entity.GetComponent<CStorage>().GetMetal();
                            break;
                        case ETransport:
                            mOxygenTransport += entity.GetComponent<CStorage>().GetOxygen();
                            mMetalTransport += entity.GetComponent<CStorage>().GetMetal();
                            break;
                        case EMoerser:
                            mOxygenMoerser += entity.GetComponent<CStorage>().GetOxygen();
                            mMetalMoerser += entity.GetComponent<CStorage>().GetMetal();
                            break;
                        case EMedic:
                            mOxygenMedic += entity.GetComponent<CStorage>().GetOxygen();
                            mMetalMedic += entity.GetComponent<CStorage>().GetMetal();
                            break;
                    }
                }
            }
        }
    }
}
