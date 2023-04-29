using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Core.Entity;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;

namespace sopra05_2223.Screens
{
    // This is a screen that realizes a Parallax background.
    // It is currently not used but could be used e.g. as a menu background.
    // (updateLower and drawLower have to be set true on the menu screen above then)
    // This is NOT the parallax that is used in the game,
    // the game parallax is implemented in GameScreen.cs!

    // To test this parallax background screen, call the following on another screen:
    // ScreenManager.AddScreen(new ParallaxScreen(new Point(mScreenWidth, mScreenHeight), new Point(8000, 8000)));

    internal class ParallaxScreen : IScreen
    {
        private EntityManager mEntityManager;
        private Camera mCamera;
        private Point mScreenSize;
        private readonly Point mWorldSize;

        public ParallaxScreen(Point screenSize, Point worldSize)
        {
            mScreenSize = screenSize;
            mWorldSize = worldSize;
            mEntityManager = new EntityManager(worldSize);
            Initialize();

        }

        private void Initialize()
        {
            mCamera = new Camera(new Vector2(0, 0), mScreenSize, mWorldSize, mEntityManager);
        }

        public ScreenManager ScreenManager { get; set; }

        bool IScreen.UpdateLower => false;

        bool IScreen.DrawLower => false;

        void IScreen.Update(Input input)
        {
            mEntityManager.Update();

            mCamera.Update(input.GetMousePosition());

            var keys = input.GetKeys();

            if (keys.Contains(Keys.Escape))
            {
                // Remove this screen from the screen stack to show the screen beneath.
                ScreenManager.RemoveScreens();

            }
        }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            // Add Background image here if needed.
            /*
            spriteBatch.End();
            spriteBatch.Begin(samplerState: SamplerState.LinearWrap);
            // This does not work with other screen formats / resolutions yet!
            // Try F5 and F7 when on the screen to see what I mean.
            spriteBatch.Draw(Art.MenuBackground1,
                new Vector2(0, 0),
                new Rectangle(0, 0, mScreenSize.X * 2, mScreenSize.Y * 2),
                Color.White,
                0f,
                Vector2.One,
                0.75f,
                SpriteEffects.None,
                0);
            */
            spriteBatch.End();
            spriteBatch.Begin();
            mCamera.mParallax.Draw(spriteBatch, mCamera.Zoom * 2.5f, mWorldSize);
            mEntityManager.Draw(spriteBatch, mCamera.Rectangle, mCamera.TranslationMatrix);
            Art.AnimationManager.DrawAnimations(spriteBatch, mCamera.TranslationMatrix);

        }

        public void Resize(Point newSize)
        {
            mScreenSize = newSize;
            mCamera.Resize(newSize);
        }
    }
}
