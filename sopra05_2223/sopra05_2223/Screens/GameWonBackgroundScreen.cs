using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using Microsoft.Xna.Framework;
using sopra05_2223.Core.Animation;

namespace sopra05_2223.Screens
{
    // This Screen can be used as background for menu screens by putting it on 
    // the ScreenStack before putting the menu screen on.
    // (DrawLower and UpdateLower of the menu screen overlaying this screen have to be set true).
    // It allows easier reuse of the same background in different menu screens.
    internal class GameWonBackgroundScreen : IScreen
    {
        // Hauptmenü-Hintergrund passt sich nicht an Auflösung an. #267
        // Das Grafik-Rectangle wird in jedem Draw-Durchlauf in Abhängigkeit von der ScreenSize gesetzt.
        // Die Screen Size ändert sich aber nur, wenn F11 gedrückt wird oder das Fenster manuell verzogen wird.
        // Daher müsste vermutlich das Seitenverhältnis noch miteinberechnet werden.
        private Point mScreenSize;
        private int mElapsed;

        public GameWonBackgroundScreen(Point screenSize)
        {
            mScreenSize = screenSize;
        }


        public ScreenManager ScreenManager { get; set; }

        bool IScreen.UpdateLower => false;

        bool IScreen.DrawLower => false;

        void IScreen.Update(Input input)
        {

            var keys = input.GetKeys();
            mElapsed += Globals.GameTime.ElapsedGameTime.Milliseconds;
            if (mElapsed >= 300)
            {
                mElapsed -= 300;
                Art.AnimationManager.AddAnimation(new Explosion1(new Vector2(Globals.RandomNumber() % mScreenSize.X, Globals.RandomNumber() % mScreenSize.Y), 0.5f));
            }

            Art.AnimationManager.UpdateAnimations(input);
            if (keys.Contains(Keys.Escape))
            {
                // Remove this screen from the screen stack to show the screen beneath.
                // If UpdateLower ist set true in the menuScreen overlaying this screen,
                // then this screen will close directly with the menu whenever ESC is pressed.
                ScreenManager.RemoveScreens();

            }
        }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(samplerState: SamplerState.LinearWrap);

            spriteBatch.Draw(Art.GameWonBackground, new Rectangle(0, 0, mScreenSize.X, mScreenSize.Y), new Rectangle(0, 0,Art.GameWonBackground.Width,Art.GameWonBackground.Height  ), Color.LightGray);
            Art.AnimationManager.DrawAnimations(spriteBatch, Matrix.Identity);
        }

        public void Resize(Point newSize)
        {
            mScreenSize = newSize;
        }
    }
}
