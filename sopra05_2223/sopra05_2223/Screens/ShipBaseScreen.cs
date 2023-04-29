using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Core;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.Core.ShipBase;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;

namespace sopra05_2223.Screens
{
    internal sealed class ShipBaseScreen : IScreen
    {
        private Point mScreenSize;
        private readonly Entity mEntity;
        private bool mResourceScreen;
        public bool mLeftMouse;

        public ShipBaseScreen(Point screenSize, Entity entity)
        {
            mEntity = entity;
            mScreenSize = screenSize;
            mEntity.GetComponent<CMoveResource>().GetInRange(Globals.sViewRadius["SpaceBase"]);
            ScreenManager = Globals.ScreenManager;
            mEntity.GetComponent<CSelect>().mSelected = false;
        }

        public ScreenManager ScreenManager
        {
            get; set;
        }
        void IScreen.Update(Input input)
        {
            if (mEntity.GetComponent<CMoveResource>().GetEntitesInRange().Count > 0 && !mResourceScreen)
            {
                ScreenManager.AddScreen(new ResourceScreen(mEntity, mScreenSize));
                mResourceScreen = true;
            }
            if (input.GetKeys().Contains(Keys.Escape))
            {
                input.GetKeys().Remove(Keys.Escape);
                CloseWindow();
            }

            if (input.GetMouseButton(0) == MouseActionEnum.Released || mLeftMouse)
            {
                var mousepos = input.GetMousePosition();
                var mousetoworld = Globals.Camera.ScreenToWorld(new Vector2(mousepos.X, mousepos.Y));
                if (mousepos.X > 0.4f * mScreenSize.X && mousepos.X <= mScreenSize.X * 0.6 && !new Rectangle((int)mousetoworld.X, (int)mousetoworld.Y,1,1).Intersects(new Rectangle(mEntity.GetX(), mEntity.GetY(), mEntity.GetWidth(), mEntity.GetHeight())))
                {
                    CloseWindow();
                }

                if (mousepos.X >= 0.22 * mScreenSize.X && mousepos.X <= 0.22 * mScreenSize.X + 0.1 * mScreenSize.Y)
                {
                    if (mousepos.Y >= 0.21 * mScreenSize.Y &&
                        mousepos.Y <= 0.31 * mScreenSize.Y)
                    {
                        RequestBuildOfShipType(typeof(EAttacker));
                    }
                    else if (mousepos.Y >= 0.36 * mScreenSize.Y &&
                             mousepos.Y <= 0.46 * mScreenSize.Y)
                    {
                        RequestBuildOfShipType(typeof(ETransport));
                    }
                    else if (mousepos.Y >= 0.51 * mScreenSize.Y &&
                             mousepos.Y <= 0.61 * mScreenSize.Y)
                    {
                        RequestBuildOfShipType(typeof(ESpy));
                    }
                    else if (mousepos.Y >= 0.66 * mScreenSize.Y &&
                             mousepos.Y <= 0.76 * mScreenSize.Y)
                    {
                        RequestBuildOfShipType(typeof(EMoerser));
                    }
                    else if (mousepos.Y >= 0.81 * mScreenSize.Y &&
                             mousepos.Y <= 0.91 * mScreenSize.Y)
                    {
                        RequestBuildOfShipType(typeof(EMedic));
                    }
                }
            }

            mLeftMouse = false;
        }

        // Builds the given ShipType if enough Resources left.
        private void RequestBuildOfShipType(Type shipType)
        {
            if (mEntity.GetComponent<CStorage>()?.GetMetal() >= Globals.sCosts[shipType].X 
                && mEntity.GetComponent<CStorage>()?.GetOxygen() >= Globals.sCosts[shipType].Y
                && mEntity.GetComponent<BuildSlotManager>().AddTask(Globals.sBuildTime[shipType], shipType))
            {
                mEntity.GetComponent<CStorage>()?.RemoveFromStorage(false, Globals.sCosts[shipType].X);
                mEntity.GetComponent<CStorage>()?.RemoveFromStorage(true, Globals.sCosts[shipType].Y);
            }
        }

        private void CloseWindow()
        {
            var x = mEntity.GetComponent<CSelect>();
            if (x != null)
            {
                x.mSelected = false;
            }

            if (ScreenManager.GetTopScreen() is not Hud)
            {
                Globals.ScreenManager.ReturnToGameScreen();
            }
        }

        
        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Art.ShipBaseBg, new Rectangle(0, 0, (int)(mScreenSize.X * 0.40), mScreenSize.Y), new Rectangle(0, 0, Art.ShipBaseBg.Width, Art.ShipBaseBg.Height), Color.AliceBlue);

            DrawResources(spriteBatch);
            DrawSlots(spriteBatch);

            spriteBatch.Draw(Art.Bomber, new Rectangle((int)(mScreenSize.X * 0.22), (int)(mScreenSize.Y * 0.21), (int)(mScreenSize.Y * 0.1), (int)(mScreenSize.Y * 0.1)), Color.White);
            spriteBatch.Draw(Art.Transport, new Rectangle((int)(mScreenSize.X * 0.22), (int)(mScreenSize.Y * 0.36), (int)(mScreenSize.Y * 0.1), (int)(mScreenSize.Y * 0.1)), Color.White);
            spriteBatch.Draw(Art.Spy, new Rectangle((int)(mScreenSize.X * 0.22), (int)(mScreenSize.Y * 0.51), (int)(mScreenSize.Y * 0.1), (int)(mScreenSize.Y * 0.1)), Color.White);
            spriteBatch.Draw(Art.Moerser, new Rectangle((int)(mScreenSize.X * 0.22), (int)(mScreenSize.Y * 0.66), (int)(mScreenSize.Y * 0.1), (int)(mScreenSize.Y * 0.1)), Color.White);
            spriteBatch.Draw(Art.Medic, new Rectangle((int)(mScreenSize.X * 0.22), (int)(mScreenSize.Y * 0.81), (int)(mScreenSize.Y * 0.1), (int)(mScreenSize.Y * 0.1)), Color.White);

            spriteBatch.DrawString(Art.Arial12, $"{Globals.sCosts[typeof(EAttacker)].X} Metall", new Vector2(mScreenSize.X * 0.22f + mScreenSize.Y * 0.11f, mScreenSize.Y * 0.23f), Color.White);
            spriteBatch.DrawString(Art.Arial12, $"{Globals.sCosts[typeof(ETransport)].X} Metall", new Vector2(mScreenSize.X * 0.22f + mScreenSize.Y * 0.11f, mScreenSize.Y * 0.38f), Color.White);
            spriteBatch.DrawString(Art.Arial12, $"{Globals.sCosts[typeof(ESpy)].X} Metall", new Vector2(mScreenSize.X * 0.22f + mScreenSize.Y * 0.11f, mScreenSize.Y * 0.53f), Color.White);
            spriteBatch.DrawString(Art.Arial12, $"{Globals.sCosts[typeof(EMoerser)].X} Metall", new Vector2(mScreenSize.X * 0.22f + mScreenSize.Y * 0.11f, mScreenSize.Y * 0.68f), Color.White);
            spriteBatch.DrawString(Art.Arial12, $"{Globals.sCosts[typeof(EMedic)].X} Metall", new Vector2(mScreenSize.X * 0.22f + mScreenSize.Y * 0.11f, mScreenSize.Y * 0.83f), Color.White);
            
            spriteBatch.DrawString(Art.Arial12, $"{Globals.sCosts[typeof(EAttacker)].Y} Sauerstoff", new Vector2(mScreenSize.X * 0.22f + mScreenSize.Y * 0.11f, mScreenSize.Y * 0.28f), Color.White);
            spriteBatch.DrawString(Art.Arial12, $"{Globals.sCosts[typeof(ETransport)].Y} Sauerstoff", new Vector2(mScreenSize.X * 0.22f + mScreenSize.Y * 0.11f, mScreenSize.Y * 0.43f), Color.White);
            spriteBatch.DrawString(Art.Arial12, $"{Globals.sCosts[typeof(ESpy)].Y} Sauerstoff", new Vector2(mScreenSize.X * 0.22f + mScreenSize.Y * 0.11f, mScreenSize.Y * 0.58f), Color.White);
            spriteBatch.DrawString(Art.Arial12, $"{Globals.sCosts[typeof(EMoerser)].Y} Sauerstoff", new Vector2(mScreenSize.X * 0.22f + mScreenSize.Y * 0.11f, mScreenSize.Y * 0.73f), Color.White);
            spriteBatch.DrawString(Art.Arial12, $"{Globals.sCosts[typeof(EMedic)].Y} Sauerstoff", new Vector2(mScreenSize.X * 0.22f + mScreenSize.Y * 0.11f, mScreenSize.Y * 0.88f), Color.White);
        }

        private void DrawSlots(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < mEntity.GetComponent<BuildSlotManager>().GetSlots(); i++)
            {
                spriteBatch.Draw(Art.Healthbar,
                    new Rectangle((int)(mScreenSize.X * 0.05), (int)(mScreenSize.Y * 0.2 + mScreenSize.Y * 0.15 * i), (int)(mScreenSize.X * 0.15), (int)(mScreenSize.Y * 0.12)),
                    new Rectangle(0, 0, Art.Healthbar.Width, Art.Healthbar.Height),
                    Color.White * 0.5f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, Art.Health);
                Art.Health.Parameters["HealthPercent"].SetValue(mEntity.GetComponent<BuildSlotManager>().GetBuildSlots()[i].GetElapsedTime() / mEntity.GetComponent<BuildSlotManager>().GetBuildSlots()[i].GetTotalDuration());
                spriteBatch.Draw(Art.Healthbar,
                    new Rectangle((int)(mScreenSize.X * 0.05), (int)(mScreenSize.Y * 0.2 + mScreenSize.Y * 0.15 * i), (int)(mScreenSize.X * 0.15), (int)(mScreenSize.Y * 0.12)),
                    new Rectangle(0, 0, Art.Healthbar.Width, Art.Healthbar.Height),
                    Color.White * 0.5f);
                spriteBatch.End();
                spriteBatch.Begin();
            }
            mEntity.GetComponent<BuildSlotManager>().Draw(spriteBatch, mScreenSize);
        }

        private void DrawResources(SpriteBatch spriteBatch)
        {
            var x = mEntity.GetComponent<CStorage>();
            var currentMetal = x.GetMetal() /
                               (float)x.GetMaxMetal();
            var currentOxygen = x.GetOxygen() /
                                (float)x.GetMaxOxygen();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, Art.MetalEffect);
            Art.MetalEffect.Parameters["MetalPercent"].SetValue(currentMetal);
            spriteBatch.Draw(Art.Healthbar, new Rectangle((int)(mScreenSize.X * 0.05), (int)(mScreenSize.Y * 0.12), (int)(mScreenSize.X * 0.15f), (int)(mScreenSize.Y * 0.05)), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, Art.OxygenEffect);
            Art.OxygenEffect.Parameters["OxygenPercent"].SetValue(currentOxygen);
            spriteBatch.Draw(Art.Healthbar, new Rectangle((int)(mScreenSize.X * 0.05), (int)(mScreenSize.Y * 0.05), (int)(mScreenSize.X * 0.15f), (int)(mScreenSize.Y * 0.05)), Color.White);
            spriteBatch.End();
            spriteBatch.Begin();

            if (currentMetal <= 0.5f)
            {
                spriteBatch.DrawString(Art.MenuButtonFont, x.GetMetal().ToString(), new Vector2(mScreenSize.X * 0.12f, mScreenSize.Y * 0.12f), Color.White);
            }
            else
            {
                spriteBatch.DrawString(Art.MenuButtonFont, x.GetMetal().ToString(), new Vector2(mScreenSize.X * 0.12f, mScreenSize.Y * 0.12f), Color.Black);
            }

            if (currentOxygen <= 0.5f)
            {
                spriteBatch.DrawString(Art.MenuButtonFont, x.GetOxygen().ToString(), new Vector2(mScreenSize.X * 0.12f, mScreenSize.Y * 0.05f), Color.White);
            }
            else
            {
                spriteBatch.DrawString(Art.MenuButtonFont, x.GetOxygen().ToString(), new Vector2(mScreenSize.X * 0.12f, mScreenSize.Y * 0.05f), Color.Black);
            }
        }

        void IScreen.Resize(Point newRes)
        {
            mScreenSize = newRes;
        }

        public Entity GetEntity()
        {
            return mEntity;
        }

        bool IScreen.UpdateLower => true;
        bool IScreen.DrawLower => true;
    }
}
