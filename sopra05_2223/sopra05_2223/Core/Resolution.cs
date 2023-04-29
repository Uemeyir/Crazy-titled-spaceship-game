using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using sopra05_2223.ScreenManagement;

namespace sopra05_2223.Core
{
    internal sealed class Resolution
    {
        internal ScreenManager mScreenManager;
        internal GraphicsDeviceManager mGraphics;
        [JsonRequired]
        internal Point mScreenSize;
        [JsonRequired]
        private Point mWindowSize;
        [JsonRequired]
        private bool mCustomSize;
        internal GameWindow mWindow;
        // needed for serialization because we cannot save mGraphics:
        [JsonRequired]
        internal bool mIsFullScreen;
        [JsonRequired]
        internal bool mIsBorderless;

        public Resolution(ScreenManager screenManager, GraphicsDeviceManager graphics, Point screenSize, Point windowSize, GameWindow window)
        {
            mScreenManager = screenManager;
            mScreenSize = screenSize;
            mWindowSize = windowSize;
            mGraphics = graphics;
            mWindow = window;
            mWindow.ClientSizeChanged += (Window_ClientSizeChanged);
            mIsFullScreen = mGraphics.IsFullScreen;
        }

        [JsonConstructor]
        public Resolution(Point screenSize, Point windowSize, bool isFullScreen, bool isBorderLess)
        {
            mScreenSize = screenSize;
            mWindowSize = windowSize;
            mIsFullScreen = isFullScreen;
            mIsBorderless = isBorderLess;
        }

        private void ToggleFullscreen()
        {
            if (mGraphics.IsFullScreen)
            {
                mGraphics.PreferredBackBufferHeight = mWindowSize.Y;
                mGraphics.PreferredBackBufferWidth = mWindowSize.X;
                mScreenSize = mWindowSize;
            }

            mGraphics.IsFullScreen = !mGraphics.IsFullScreen;
            mGraphics.ApplyChanges();

            mIsFullScreen = mGraphics.IsFullScreen;
        }

        internal void ToggleBorderless()
        {
            mGraphics.HardwareModeSwitch = !mGraphics.HardwareModeSwitch;
            mIsBorderless = !mIsBorderless;
            mGraphics.ApplyChanges();
        }

        internal void FullscreenCall()
        {
            if (mCustomSize)
            {
                mWindowSize = Globals.sResolutions[1];
                UseResolution(false, Globals.sResolutions[1]);
                mCustomSize = false;
            }
            if (!mGraphics.IsFullScreen)
            {
                mWindow.ClientSizeChanged -= (Window_ClientSizeChanged);
                UseResolution(true, Point.Zero);
                ToggleFullscreen();
            }
            else
            {
                ToggleFullscreen();
                mWindow.AllowUserResizing = true;
                mWindow.ClientSizeChanged += (Window_ClientSizeChanged);
            }

            ResizeScreenManager();
            mIsFullScreen = mGraphics.IsFullScreen;
        }

        internal void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            mCustomSize = true;
            UseResolution(false, new Point(mGraphics.GraphicsDevice.PresentationParameters.BackBufferWidth, mGraphics.GraphicsDevice.PresentationParameters.BackBufferHeight));
            ResizeScreenManager();
            mIsFullScreen = mGraphics.IsFullScreen;
        }

        internal void UseResolution(bool native, Point newRes)
        {
            if (native)
            {
                var newHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                var newWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                mGraphics.PreferredBackBufferHeight = newHeight;
                mGraphics.PreferredBackBufferWidth = newWidth;
                mScreenSize = new Point(newWidth, newHeight);
            }
            else
            {
                mScreenSize = newRes;
                mWindowSize = newRes;
                mGraphics.PreferredBackBufferHeight = mWindowSize.Y;
                mGraphics.PreferredBackBufferWidth = mWindowSize.X;
            }
            mGraphics.ApplyChanges();
            mIsFullScreen = mGraphics.IsFullScreen;
        }

        internal void ResizeScreenManager()
        {
            mScreenManager.ResizeScreens(mScreenSize);
            mIsFullScreen = mGraphics.IsFullScreen;
        }
    }
}
