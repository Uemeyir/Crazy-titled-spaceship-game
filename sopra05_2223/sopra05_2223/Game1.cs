using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Core;
using sopra05_2223.InputSystem;
using sopra05_2223.ScreenManagement;
using sopra05_2223.Screens;
using sopra05_2223.SoundManagement;
using System;
using System.IO;
using System.Threading;
using sopra05_2223.Serializer;

namespace sopra05_2223
{
    internal sealed class Game1 : Game
    {
        private readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private MouseCatcher mMouseCatcher;

        private Point mScreenSize = new(1280, 720);
        private readonly Point mWindowSize = new(1280, 720);
        private SoundManager mSoundManager;
        // created to not trigger sounds every frame

        private readonly ScreenManager mScreenManager = new();
        private readonly InputManager mInputManager = new();

        private double mElapsed;
        private double mLast;
        private double mNow;

        internal Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            IsFixedTimeStep = false;
            Deactivated += Game_Deactivated;
        }

        protected override void Initialize()
        {
            // ==================== graphics and window settings ================================
            Globals.ScreenManager = mScreenManager;

            // Load previous screen settings or standard settings if the game has not been played yet.
            var deserializedResolution = SopraSerializer.DeserializeScreenResolution();
            if (deserializedResolution is not null)
            {
                Globals.Resolution = deserializedResolution;
                Globals.Resolution.mGraphics = mGraphics;
                Globals.Resolution.mWindow = Window;
                Globals.Resolution.mGraphics.IsFullScreen = Globals.Resolution.mIsFullScreen;
                Globals.Resolution.mScreenManager = mScreenManager;
                if (!Globals.Resolution.mIsFullScreen)
                {
                    Globals.Resolution.mWindow.ClientSizeChanged += (Globals.Resolution.Window_ClientSizeChanged);
                }
            }
            else
            {
                Globals.Resolution = new Resolution(mScreenManager, mGraphics, mScreenSize, mWindowSize, Window);
                mGraphics.IsFullScreen = false;
            }

            Globals.mRunStatistics = Globals.sRunStatisticsDefault;
            Globals.GraphicsDevice = mGraphics;
            mScreenSize = Globals.Resolution.mScreenSize;
            mGraphics.PreferredBackBufferWidth = mScreenSize.X;
            mGraphics.PreferredBackBufferHeight = mScreenSize.Y;
            mGraphics.ApplyChanges();

            Window.Title = "crazy untitled space strategy game";
            Window.AllowUserResizing = true;

            // ========================= sound settings ====================================

            mSoundManager = new SoundManager(Content);

            // Load previous sound settings or standard settings if the game has not been played yet.
            var deserializedSoundManager = SopraSerializer.DeserializeSoundManager();
            if (deserializedSoundManager is not null)
            {
                mSoundManager.SetMusicVolume(deserializedSoundManager.GetMusicVolume());
                mSoundManager.SetSoundVolume(deserializedSoundManager.GetSoundVolume());
                mSoundManager.mSoundMuted = deserializedSoundManager.mSoundMuted;
            }
            else
            {
                mSoundManager.SetSoundVolume(0.5f);
                mSoundManager.SetMusicVolume(0.5f);
            }
            
            mSoundManager.AddIAudible(mInputManager);
            Globals.SoundManager = mSoundManager;

            // Load KeyBindings
            SopraSerializer.DeserializeKeys();

            // Load Achievements and Statistics
            SopraSerializer.DeserializeAchievements();
            SopraSerializer.DeserializeStatistics();

            // Start the game with the Main Menu Screen (and its background beneath it on the screen stack).
            mScreenManager.AddScreen(new MenuBackgroundScreen(mScreenSize));
            mScreenManager.AddScreen(new MenuScreen(this.mScreenSize.X, this.mScreenSize.Y, this, mSoundManager));

            try
            {
                mMouseCatcher = new MouseCatcher();
            }
            catch (Exception)
            {
                mMouseCatcher = null;
            }

            ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);
            base.Initialize();
        }

        private void Game_Deactivated(object sender, EventArgs e)
        {
            if (Globals.ScreenManager.GetTopScreen() is Hud or GameScreen or ShipBaseScreen or PlanetBaseScreen or ResourceScreen)
            {
                Globals.ScreenManager.AddScreen(new PauseMenuScreen(Globals.Resolution.mScreenSize.X, Globals.Resolution.mScreenSize.Y, (GameScreen)Globals.ScreenManager.GetScreen(typeof(GameScreen)), mSoundManager));
                mScreenManager.Update(mInputManager.GetInput());
            }
        }

        protected override void LoadContent()
        {
            SelectRectangle.LoadContent(Content);

            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            Art.Load(Content, this.GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            // only update whenever the game window is not minimized or overlapped.
            if (!IsActive)
            {
                return;
            }

            // this code is not perfect, but it updates the gametime every frame, so its fine
            Globals.GameTime = gameTime;
            Globals.GameTime.ElapsedGameTime *= Globals.mGameSpeed;

            mNow = gameTime.TotalGameTime.TotalSeconds;
            mElapsed = mNow - mLast;
            if (mElapsed > 1.0f)
            {
                mElapsed = 0;
                mLast = mNow;
            }

            if (mInputManager.GetInput().GetKeys().Contains(Keys.F2))
            {
                TakeScreenshot(GraphicsDevice);
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            mInputManager.Update(IsActive);
            mSoundManager.Update(mInputManager.GetInput());
            mScreenManager.Update(mInputManager.GetInput());

            mMouseCatcher?.Update(IsActive, Window, mScreenManager.IsTopScreenMenu());

            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            mSpriteBatch.Begin();
            mScreenManager.Draw(mSpriteBatch);
            mSpriteBatch.Draw(Art.Cursor, new Vector2(mInputManager.GetInput().GetMousePosition().X, mInputManager.GetInput().GetMousePosition().Y), Color.White);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }

        private void TakeScreenshot(GraphicsDevice graphicsDevice)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CUSSG", "Screenshots");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var screenData = new byte[graphicsDevice.PresentationParameters.BackBufferWidth * graphicsDevice.PresentationParameters.BackBufferHeight * 4];
            graphicsDevice.GetBackBufferData(screenData);
            var screenshot = new Texture2D(graphicsDevice, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight);
            screenshot.SetData(screenData);
            int counter = 1;
            var name = "screenshot" + counter + ".png";
            while (File.Exists(Path.Combine(path, name)))
            {
                counter += 1;
                name = "screenshot" + counter + ".png";
            }

            Stream stream = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CUSSG", "Screenshots", name), FileMode.Create);
            screenshot.SaveAsPng(stream, screenshot.Width, screenshot.Height);
            stream.Close();
            screenshot.Dispose();
        }
    }
}
