using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Core;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;

namespace sopra05_2223.Screens
{
    internal sealed class PlanetBaseScreen : IScreen
    {
        private readonly Entity mEntity;
        private Point mScreenSize;
        private bool mResourceScreen;
        public PlanetBaseScreen(Entity entity, Point screenSize)
        {
            mEntity = entity;
            mScreenSize = screenSize;
            ScreenManager = Globals.ScreenManager;
            mEntity.GetComponent<CMoveResource>().GetInRange(Globals.sViewRadius["PlanetBase"]);
            mEntity.GetComponent<CSelect>().mSelected = false;
        }
        bool IScreen.UpdateLower => true;
        bool IScreen.DrawLower => true;

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Art.ShipBaseBg, new Rectangle(0, (int)(mScreenSize.Y * 0.2f), (int)(mScreenSize.X * 0.25f), (int)(mScreenSize.Y * 0.3f)), new Rectangle(0, 0, Art.ShipBaseBg.Width, Art.ShipBaseBg.Height), Color.AliceBlue);

            DrawResources(spriteBatch);
        }

        private void DrawResources(SpriteBatch spriteBatch)
        {
            var x = mEntity.GetComponent<CStorage>();
            var currentMetal = x.GetMetal() / (float)x.GetMaxMetal();
            var currentOxygen = x.GetOxygen() / (float)x.GetMaxOxygen();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, Art.MetalEffect);
            Art.MetalEffect.Parameters["MetalPercent"].SetValue(currentMetal);
            spriteBatch.Draw(Art.Healthbar, new Rectangle((int)(mScreenSize.X * 0.05), (int)(mScreenSize.Y * 0.32), (int)(mScreenSize.X * 0.15f), (int)(mScreenSize.Y * 0.05)), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, Art.OxygenEffect);
            Art.OxygenEffect.Parameters["OxygenPercent"].SetValue(currentOxygen);
            spriteBatch.Draw(Art.Healthbar, new Rectangle((int)(mScreenSize.X * 0.05), (int)(mScreenSize.Y * 0.25), (int)(mScreenSize.X * 0.15f), (int)(mScreenSize.Y * 0.05)), Color.White);
            spriteBatch.End();
            spriteBatch.Begin();

            if (currentMetal <= 0.5f)
            {
                spriteBatch.DrawString(Art.MenuButtonFont, x.GetMetal().ToString(), new Vector2(mScreenSize.X * 0.12f, mScreenSize.Y * 0.32f), Color.White);
            }
            else
            {
                spriteBatch.DrawString(Art.MenuButtonFont, x.GetMetal().ToString(), new Vector2(mScreenSize.X * 0.12f, mScreenSize.Y * 0.32f), Color.Black);
            }

            if (currentOxygen <= 0.5f)
            {
                spriteBatch.DrawString(Art.MenuButtonFont, x.GetOxygen().ToString(), new Vector2(mScreenSize.X * 0.12f, mScreenSize.Y * 0.25f), Color.White);
            }
            else
            {
                spriteBatch.DrawString(Art.MenuButtonFont, x.GetOxygen().ToString(), new Vector2(mScreenSize.X * 0.12f, mScreenSize.Y * 0.25f), Color.Black);
            }
            spriteBatch.DrawString(Art.Arial22, mEntity.GetComponent<CResourceGenerator>().GetResources().X + " Metall/sec", new Vector2(mScreenSize.X * 0.06f, mScreenSize.Y * 0.45f), Color.White);
            spriteBatch.DrawString(Art.Arial22, mEntity.GetComponent<CResourceGenerator>().GetResources().Y + " Sauerstoff/sec", new Vector2(mScreenSize.X * 0.06f, mScreenSize.Y * 0.4f), Color.White);
        }

        public ScreenManager ScreenManager
        {
            get; set;
        }
        public Entity GetEntity()
        {
            return mEntity;
        }

        void IScreen.Resize(Point newRes)
        {
            mScreenSize = newRes;
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

            var mousepos = input.GetMousePosition();
            if (input.GetMouseButton(0) == MouseActionEnum.Released)
            {
                var mousetoworld = Globals.Camera.ScreenToWorld(new Vector2(mousepos.X, mousepos.Y));
                if ((mousepos.X > 0.35 * mScreenSize.X && mousepos.X <= mScreenSize.X * 0.6 || (mousepos.Y > mScreenSize.Y * 0.5f || mousepos.Y < mScreenSize.Y * 0.2f) && mousepos.X <= mScreenSize.X * 0.25f) && !new Rectangle((int)mousetoworld.X, (int)mousetoworld.Y, 1, 1).Intersects(new Rectangle(mEntity.GetX(), mEntity.GetY(), mEntity.GetWidth(), mEntity.GetHeight())))
                {
                    CloseWindow();
                }
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
    }
}
