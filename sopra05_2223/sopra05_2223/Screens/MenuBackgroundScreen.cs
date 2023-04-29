using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using Microsoft.Xna.Framework;

namespace sopra05_2223.Screens
{
    // This Screen can be used as background for menu screens by putting it on 
    // the ScreenStack before putting the menu screen on.
    // (DrawLower and UpdateLower of the menu screen overlaying this screen have to be set true).
    // It allows easier reuse of the same background in different menu screens.
    internal sealed class MenuBackgroundScreen : IScreen
    {
        private Point mScreenSize;

        public MenuBackgroundScreen(Point screenSize)
        {
            mScreenSize = screenSize;
        }


        public ScreenManager ScreenManager { get; set; }

        bool IScreen.UpdateLower => false;

        bool IScreen.DrawLower => false;

        void IScreen.Update(Input input)
        {
            if (ScreenManager.GetTopScreen() is MenuBackgroundScreen)
            {
            }
        }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(samplerState: SamplerState.LinearWrap);
            spriteBatch.Draw(Art.MenuBackground1, new Rectangle(0, 0, mScreenSize.X, mScreenSize.Y), new Rectangle(0, 0,Art.MenuBackground1.Width,Art.MenuBackground1.Height  ), Color.White);
        }

        public void Resize(Point newSize)
        {
            mScreenSize = newSize;
        }
    }
}