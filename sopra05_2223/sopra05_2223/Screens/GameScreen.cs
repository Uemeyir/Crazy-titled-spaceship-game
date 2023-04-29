using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Core;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity;
using sopra05_2223.Core.Entity.Planet;
using sopra05_2223.Core.Entity.Resource;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.InputSystem;
using sopra05_2223.Pathfinding;
using sopra05_2223.Protagonists;
using sopra05_2223.ScreenManagement;
using sopra05_2223.Serializer;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using sopra05_2223.Background;
using sopra05_2223.Core.Animation;
using sopra05_2223.Core.Entity.Base;
using sopra05_2223.SoundManagement;
using sopra05_2223.Core.Entity.Base.PlanetBase;
using sopra05_2223.Core.Entity.Base.SpaceBase;
using sopra05_2223.NotificationSystem;

namespace sopra05_2223.Screens
{
    internal class GameScreen : IScreen
    {
        [JsonRequired]
        protected readonly EntityManager mEntityManager;

        private static Rectangle sLastRectangle;
        [JsonRequired]
        internal Camera mCamera;
        [JsonRequired]
        private Point mScreenSize;
        private readonly Point mWorldSize;
        private MoveEntities mMoveEntities;
        private Grid mGrid;
        private Grid mGridExtended;
        private const int GridTileDimension = 150;
        private const int GridTileDimensionExtended = 400;

        private Input mLastInput;
        private bool mLeftMouseButton;
        private bool mClipToScreen;
        private bool mClipToMiniMap;
        private bool mDebug;
        private int mElapsed;
        private Vector2 mLastBase;
        private int mAfterWin;
        private static readonly int sDelay = 2000;
        private bool mLockPause;

        [JsonRequired]
        private readonly Player mPlayer;
        [JsonRequired]
        private readonly Player mKi;

        private readonly SoundManager mSoundManager;

        protected FogOfWar mFogOfWar;

        private RenderTarget2D mMainRt = new (Globals.GraphicsDevice.GraphicsDevice, Globals.GraphicsDevice.GraphicsDevice.PresentationParameters.BackBufferWidth,
            Globals.GraphicsDevice.GraphicsDevice.PresentationParameters.BackBufferHeight);

        private int mLastTimeSavedAchievementsAndStatistics;
        private const int SaveIntervalAchievementsAndStatistics = 1;
        [JsonRequired]
        private Dictionary<string, int> mStatistics = new (Globals.sRunStatisticsDefault);

        public GameScreen(Point screenSize, Point worldSize, SoundManager soundManager, PlayerPlayer player, PlayerKi ki, bool tutorial)
        {
            mSoundManager = soundManager;
            mScreenSize = screenSize;
            mWorldSize = worldSize;
            mEntityManager = new EntityManager(worldSize);

            mPlayer = player;
            mKi = ki;

            if (!tutorial)
            {
                Initialize();
            }
            else
            {
                Tutorial1();
            }
            Globals.sLastNotifiedStats["DestroyedEnemyShips"] = Globals.mStatistics["DestroyedEnemyShips"];
            Globals.sLastNotifiedStats["LostShips"] = Globals.mStatistics["LostShips"];
            Globals.mRunStatistics = mStatistics;
            Globals.NotificationManager = new NotificationManager();
            Globals.NotificationManager.AddNotification(new Notification("Erobere alle Basen um das Universum zu befreien!"));
            Globals.NotificationManager.AddNotification(new Notification("Drücke H für Hilfe."));
            Globals.Player = mPlayer;
        }

        // Used for loading from saveFile.
        internal GameScreen(EntityManager entityManager, SoundManager soundManager,
            Player player, Player ki, Vector2 camPosition, float camZoom, Dictionary<string, int> runStatistics)
        {
            mScreenSize = Globals.Resolution.mScreenSize;
            mEntityManager = entityManager;
            mSoundManager = soundManager;
            mWorldSize = entityManager.mWorldSize;
            mPlayer = player;
            mKi = ki;
            mCamera = new Camera(camPosition, mScreenSize, mWorldSize, mEntityManager, zoom: camZoom);
            mFogOfWar = new FogOfWar(mEntityManager, mCamera);

            mGrid = new Grid(mWorldSize.X, mWorldSize.Y, GridTileDimension);
            mGrid.InsertPlanetsIntoGrid(mEntityManager.Entities.Where(e => e is EPlanet).ToList());
            mGrid.InsertPlanetBasesIntoGrid(mEntityManager.Entities.Where(e => e is EPlanetBase).ToList());
            Globals.mGridStandard = mGrid;

            mGridExtended = new Grid(mWorldSize.X, mWorldSize.Y, GridTileDimensionExtended);
            mGridExtended.InsertPlanetsIntoGrid(mEntityManager.Entities.Where(e => e is EPlanet).ToList());
            mGridExtended.InsertPlanetBasesIntoGrid(mEntityManager.Entities.Where(e => e is EPlanetBase).ToList());
            Globals.mGridExtended = mGridExtended;

            mMoveEntities = new MoveEntities(mWorldSize);
            Globals.mMoveEntities = mMoveEntities;
            Globals.Camera = mCamera;

            // Use Savegame specific statistics instead, however shouldn't make a difference.
            Globals.sLastNotifiedStats["DestroyedEnemyShips"] = Globals.mStatistics["DestroyedEnemyShips"];
            Globals.sLastNotifiedStats["LostShips"] = Globals.mStatistics["LostShips"];
            Globals.mRunStatistics = runStatistics;
            mStatistics = Globals.mRunStatistics;
            Globals.NotificationManager = new NotificationManager();
            Globals.Player = mPlayer;
        }

        private void Initialize()
        {
            //idea behind the player class was that we have to create only one class for the ki (PlayerKi) and its assets/entities
            //and only one for the player (PlayerPlayer), for every entity which is created (or removed) in this game, the players
            //assets should be updated to
            mPlayer.SetOpponent(mKi);
            mKi.SetOpponent(mPlayer);
            
            mKi.SetEntityManager(mEntityManager);

            // ------------------------------------------------------ Camera ---------------------------------------------------------------------

            const float varZoom = 0.2f;
            mCamera = new Camera(new Vector2(mWorldSize.X - 1 / varZoom * mScreenSize.X, mWorldSize.Y - 1 / varZoom * mScreenSize.Y) / 2f, mScreenSize, mWorldSize, mEntityManager, zoom: varZoom);
            mFogOfWar = new FogOfWar(mEntityManager, mCamera)
            {
                mEnabled = true
            };

            mGrid = new Grid(mWorldSize.X, mWorldSize.Y, GridTileDimension);
            mGridExtended = new Grid(mWorldSize.X, mWorldSize.Y, GridTileDimensionExtended);

            // ---------------------------------------------------------------- Player -ships ----------------------------------------------------------------------------------------

            var attacker1Player = new EAttacker(new Vector2(15500, 15500), mPlayer);
            attacker1Player.GetComponent<CGun>()?.SetTarget(null);

            var eTransporterPlayer = new ETransport(new Vector2(16000, 16100), mPlayer);
            mEntityManager.Add(eTransporterPlayer);
            eTransporterPlayer.AddCollectors(6);

            var attacker2Player = new EAttacker(new Vector2(16500, 15500), mPlayer);
            attacker2Player.GetComponent<CGun>()?.SetTarget(null);

            var moerserPlayer = new EMoerser(new Vector2(16000, 17000), mPlayer);
            moerserPlayer.GetComponent<CGun>()?.SetTarget(null);


            Globals.mStatistics["BuiltAttack"] += 2;
            Globals.mRunStatistics["BuiltAttack"] += 2;
            Globals.mStatistics["BuiltMoerser"] += 1;
            Globals.mRunStatistics["BuiltMoerser"] += 1;
            Globals.mStatistics["BuiltTransport"] += 1;
            Globals.mRunStatistics["BuiltTransport"] += 1;


            mEntityManager.Add(attacker1Player);
            mEntityManager.Add(attacker2Player);
            mEntityManager.Add(moerserPlayer);
            mPlayer.AddEntity(attacker1Player);
            mPlayer.AddEntity(eTransporterPlayer);
            mPlayer.AddEntity(attacker2Player);
            mPlayer.AddEntity(moerserPlayer);

            // ---------------------------------------------------- Resources -------------------------------------------------------------------
           
            var newResource1 = new EResource(new Vector2(3000, 12000), 100, 200);
            mEntityManager.Add(newResource1);
            newResource1.GetComponent<CTeam>().SetNeutral();

            var newResource2 = new EResource(new Vector2(18000, 6000), 100, 200);
            mEntityManager.Add(newResource2);
            newResource2.GetComponent<CTeam>().SetNeutral();

            var newResource3 = new EResource(new Vector2(20100, 25000), 150, 300);
            mEntityManager.Add(newResource3);
            newResource3.GetComponent<CTeam>().SetNeutral();

            var newResource4 = new EResource(new Vector2(21000, 11000), 150, 300);
            mEntityManager.Add(newResource4);
            newResource4.GetComponent<CTeam>().SetNeutral();

            var newResource5 = new EResource(new Vector2(18000, 18200), 150, 450);
            mEntityManager.Add(newResource5);
            newResource5.GetComponent<CTeam>().SetNeutral();

            var newResource6 = new EResource(new Vector2(10800, 14600), 350, 100);
            mEntityManager.Add(newResource6);
            newResource6.GetComponent<CTeam>().SetNeutral();

            var newResource7 = new EResource(new Vector2(14800, 14120), 350, 850);
            mEntityManager.Add(newResource7);
            newResource7.GetComponent<CTeam>().SetNeutral();

            var newResource8 = new EResource(new Vector2(13600, 16600), 750, 800);
            mEntityManager.Add(newResource8);
            newResource8.GetComponent<CTeam>().SetNeutral();

            var newResource9 = new EResource(new Vector2(19400, 14600), 450, 600);
            mEntityManager.Add(newResource9);
            newResource9.GetComponent<CTeam>().SetNeutral();

            var newResource10 = new EResource(new Vector2(17700, 16040), 650, 950);
            mEntityManager.Add(newResource10);
            newResource10.GetComponent<CTeam>().SetNeutral();


            // ----------------------------------------------------------- Planets -------------------------------------------------------------------------------------
            var planet1 = new EPlanet(new Vector2(7000, 9500), 0, mGrid);
            mEntityManager.Add(planet1);
            mGridExtended.InsertPlanetIntoGrid(planet1);
            var planet2 = new EPlanet(new Vector2(10000, 11000), 1, mGrid);
            mEntityManager.Add(planet2);
            mGridExtended.InsertPlanetIntoGrid(planet2);
            var planet3 = new EPlanet(new Vector2(25000, 17000), 2, mGrid);
            mEntityManager.Add(planet3);
            mGridExtended.InsertPlanetIntoGrid(planet3);
            var planet4 = new EPlanet(new Vector2(12000, 20000), 3, mGrid);
            mEntityManager.Add(planet4);
            mGridExtended.InsertPlanetIntoGrid(planet4);
            var planet5 = new EPlanet(new Vector2(25000, 31000), 4, mGrid);
            mEntityManager.Add(planet5);
            mGridExtended.InsertPlanetIntoGrid(planet5);
            var planet6 = new EPlanet(new Vector2(30000, 26500), 5, mGrid);
            mEntityManager.Add(planet6);
            mGridExtended.InsertPlanetIntoGrid(planet6);
            var planet7 = new EPlanet(new Vector2(25000, 7520), 6, mGrid);
            mEntityManager.Add(planet7);
            mGridExtended.InsertPlanetIntoGrid(planet7);
            var planet8 = new EPlanet(new Vector2(6500, 4500), 7, mGrid);
            mEntityManager.Add(planet8);
            mGridExtended.InsertPlanetIntoGrid(planet8);
            var planet9 = new EPlanet(new Vector2(3800, 29000), 8, mGrid);
            mEntityManager.Add(planet9);
            mGridExtended.InsertPlanetIntoGrid(planet9);
            var planet10 = new EPlanet(new Vector2(31000, 14650), 9, mGrid);
            mEntityManager.Add(planet10);
            mGridExtended.InsertPlanetIntoGrid(planet10);


            // ------------------------------------------------------------ Planetbases --------------------------------------------------------------------------------
            var planetBase1 = new EPlanetBase1(new Vector2(1000, 4400), mKi, mGrid);
            var planetBase2 = new EPlanetBase1(new Vector2(1000, 30600), mKi, mGrid);
            var planetBase3 = new EPlanetBase1(new Vector2(6000, 16000), mKi, mGrid);
            var planetBase4 = new EPlanetBase1(new Vector2(31100, 24000), mKi, mGrid);
            var planetBase5 = new EPlanetBase1(new Vector2(1080, 2400), mKi, mGrid);
            var planetBase6 = new EPlanetBase1(new Vector2(24000, 10000), mKi, mGrid);
            var planetBase7 = new EPlanetBase1(new Vector2(14070, 28070), mKi, mGrid);
            var planetBase8 = new EPlanetBase1(new Vector2(16110, 29100), mKi, mGrid);
            var planetBase9 = new EPlanetBase1(new Vector2(15900, 20800), mKi, mGrid);

            mEntityManager.Add(planetBase1);
            mEntityManager.Add(planetBase2);
            mEntityManager.Add(planetBase3);
            mEntityManager.Add(planetBase4);
            mEntityManager.Add(planetBase5);
            mEntityManager.Add(planetBase6);
            mEntityManager.Add(planetBase7);
            mEntityManager.Add(planetBase8);
            mEntityManager.Add(planetBase9);

            mGridExtended.InsertBaseIntoGrid(planetBase1);
            mGridExtended.InsertBaseIntoGrid(planetBase2);
            mGridExtended.InsertBaseIntoGrid(planetBase3);
            mGridExtended.InsertBaseIntoGrid(planetBase4);
            mGridExtended.InsertBaseIntoGrid(planetBase5);
            mGridExtended.InsertBaseIntoGrid(planetBase6);
            mGridExtended.InsertBaseIntoGrid(planetBase7);
            mGridExtended.InsertBaseIntoGrid(planetBase8);
            mGridExtended.InsertBaseIntoGrid(planetBase9);


            // --------------------------------------------------------- Space Bases --------------------------------------------------------------
            var spaceBase1 = new ESpaceBase1(new Vector2(3500, 3200), mKi, mGrid);
            mEntityManager.Add(spaceBase1);
            mGridExtended.InsertBaseIntoGrid(spaceBase1);

            var spaceBase2 = new ESpaceBase1(new Vector2(7600, 1200), mKi, mGrid);
            mEntityManager.Add(spaceBase2);
            mGridExtended.InsertBaseIntoGrid(spaceBase2);

            var spaceBase3 = new ESpaceBase1(new Vector2(5600, 27000), mKi, mGrid);
            mEntityManager.Add(spaceBase3);
            mGridExtended.InsertBaseIntoGrid(spaceBase3);

            var spaceBase4 = new ESpaceBase1(new Vector2(18000, 12000), mKi, mGrid);
            mEntityManager.Add(spaceBase4);
            mGridExtended.InsertBaseIntoGrid(spaceBase4);

            var spaceBase5 = new ESpaceBase1(new Vector2(28500, 29000), mKi, mGrid);
            mEntityManager.Add(spaceBase5);
            mGridExtended.InsertBaseIntoGrid(spaceBase5);

            var spaceBase6 = new ESpaceBase1(new Vector2(29000, 2500), mKi, mGrid);
            mEntityManager.Add(spaceBase6);
            mGridExtended.InsertBaseIntoGrid(spaceBase6);

            var spaceBase7 = new ESpaceBase1(new Vector2(19500, 22000), mKi, mGrid);
            mEntityManager.Add(spaceBase7);
            mGridExtended.InsertBaseIntoGrid(spaceBase7);

            var spaceBase8 = new ESpaceBase1(new Vector2(22000, 28000), mKi, mGrid);
            mEntityManager.Add(spaceBase8);
            mGridExtended.InsertBaseIntoGrid(spaceBase8);

            var spaceBase9 = new ESpaceBase1(new Vector2(14000,30000), mKi, mGrid);
            mEntityManager.Add(spaceBase9);
            mGridExtended.InsertBaseIntoGrid(spaceBase9);

            // add all guns
            foreach (var audible in mEntityManager.Entities.Where(entity => entity.GetComponent<CGun>() != null))
            {
                var temp = audible.GetComponent<CGun>();
                mSoundManager.AddIAudible(temp);
            }

            var ai = (PlayerKi)mKi;
            ai.ResetQueues();

            Globals.mGridStandard = mGrid;
            Globals.mGridExtended = mGridExtended;
            mMoveEntities = new MoveEntities(mWorldSize);
            Globals.mMoveEntities = mMoveEntities;
            Globals.Camera = mCamera;
            Globals.mBuilt = Globals.sBuiltDefault;
            Globals.mExplored = Globals.sExploredDefault;
        }

        [JsonIgnore]
        public ScreenManager ScreenManager
        {
            get; set;
        }

        bool IScreen.UpdateLower => false;

        bool IScreen.DrawLower => false;

        void IScreen.Update(Input input)
        {
            Update(input);
        }

        protected virtual void Update(Input input)
        {
            mEntityManager.Update();

            Art.AnimationManager.UpdateAnimations(input);

            mCamera.Update(input.GetMousePosition());
            if (input.GetScrollAmount() != 0)
            {
                mCamera.ChangeZoom(input.GetScrollAmount());
            }

            HandleMouseInput(input);
            HandleKeyInput(input);

            mLastInput = input;

            var keys = input.GetKeys();

            if ((keys.Contains(Keys.Escape) || keys.Contains(Globals.mKeys["Pause"])) && !mLockPause)
            {
                // Open Pause menu.
                ScreenManager.AddScreen(new PauseMenuScreen(Globals.Resolution.mScreenSize.X, Globals.Resolution.mScreenSize.Y, this, mSoundManager));

                if (ScreenManager.GetTopScreen() is DebugScreen)
                {
                    mDebug = false;
                    ScreenManager.RemoveScreens();
                }
            }

            if (keys.Contains(Keys.F3))
            {
                if (!mDebug)
                {
                    mDebug = true;
                    ScreenManager.AddScreen(new DebugScreen(mPlayer, mKi, mEntityManager, mCamera, Art.AnimationManager, mFogOfWar, this));
                }
                else
                {
                    mDebug = false;
                }
            }

            if (keys.Contains(Globals.mKeys["Help"]))
            {
                // Open ShipsInfo screen.
                ScreenManager.AddScreen(new ShipsInfoScreen(mScreenSize));

                if (mDebug)
                {
                    mDebug = false;
                    ScreenManager.RemoveScreens();
                }
            }

            // For demonstration purposes, the number of space bases is not taken into account
            // Add "&& mPlayer.mSpaceBases == 0" later!
            if (mPlayer.mShips.Count == 0 && mPlayer.mSpaceBases.Count == 0)
            {
                ScreenManager.AddScreen(new GameOverBackgroundScreen(new Point(mScreenSize.X, mScreenSize.Y)));
                ScreenManager.AddScreen(new GameOverScreen(mScreenSize.X, mScreenSize.Y, this));
            }

            // AI has no more bases -> player has taken all bases (win condition)
            if (mKi.mSpaceBases.Count + mKi.mPlanetBases.Count == 0)
            {
                if (!mLockPause)
                {
                    Art.AnimationManager.AddAnimation(new Explosion2(mLastBase, 6f));
                }
                mLockPause = true;
                Globals.mGameSpeed = 0.3f;
                mAfterWin += Globals.GameTime.ElapsedGameTime.Milliseconds;
                if (mAfterWin >= sDelay)
                {
                    mLockPause = false;
                    ScreenManager.AddScreen(new GameWonBackgroundScreen(new Point(mScreenSize.X, mScreenSize.Y)));
                    ScreenManager.AddScreen(new GameWonScreen(mScreenSize.X, mScreenSize.Y, this));
                }
            }
            else if (mKi.mSpaceBases.Count + mKi.mPlanetBases.Count == 1)
            {
                foreach (var k in mKi.mPlanetBases)
                {
                    mLastBase = k.GetPos();
                }
                foreach (var k in mKi.mSpaceBases)
                {
                    mLastBase = k.GetPos();
                }
            }

            mKi.Update();
            mPlayer.Update();

            mElapsed += Globals.GameTime.ElapsedGameTime.Milliseconds;
            if (mElapsed >= 1000)
            {
                Globals.mStatistics["RunningSeconds"]++; 
                Globals.mRunStatistics["RunningSeconds"]++;
                mStatistics = Globals.mRunStatistics;
                mElapsed -= 1000;
            }
            
            Globals.mStatistics["TotalResources"] = Globals.mStatistics["NaturalResources"] + Globals.mStatistics["ScrapResources"];
            Globals.mRunStatistics["TotalResources"] = Globals.mRunStatistics["NaturalResources"] + Globals.mRunStatistics["ScrapResources"];
            if (Globals.mAchievements["Resource1"] == false && Globals.mStatistics["TotalResources"] >= 1000)
            {
                Globals.mAchievements["Resource1"] = true;
                Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Sucher", Globals.Resolution.mScreenSize));
            }
            else if (Globals.mAchievements["Resource2"] == false && Globals.mStatistics["TotalResources"] >= 5000)
            {
                Globals.mAchievements["Resource2"] = true;
                Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Schürfer", Globals.Resolution.mScreenSize));
            }
            else if (Globals.mAchievements["Resource3"] == false && Globals.mStatistics["TotalResources"] >= 25000)
            {
                Globals.mAchievements["Resource3"] = true;
                Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Sammler", Globals.Resolution.mScreenSize));
            }
            else if (Globals.mAchievements["Resource4"] == false && Globals.mStatistics["TotalResources"] >= 100000)
            {
                Globals.mAchievements["Resource4"] = true;
                Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Arbeiter", Globals.Resolution.mScreenSize));
            }
            else if (Globals.mAchievements["Resource5"] == false && Globals.mStatistics["TotalResources"] >= 500000)
            {
                Globals.mAchievements["Resource5"] = true;
                Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Krösus", Globals.Resolution.mScreenSize));
            }

            if (Globals.mAchievements["AllCorners"] == false && Globals.mExplored["TopLeft"] && Globals.mExplored["TopRight"] && Globals.mExplored["BottomLeft"] && Globals.mExplored["BottomRight"])
            {
                Globals.mAchievements["AllCorners"] = true;
                Globals.ScreenManager.AddScreen(new AchievementGetScreen(3000, Globals.SoundManager, "Ist hier noch was?", Globals.Resolution.mScreenSize));
            }

            // Save Achievements and Statistics every Second.
            if ( (Globals.GameTime.TotalGameTime.Seconds - mLastTimeSavedAchievementsAndStatistics) >= SaveIntervalAchievementsAndStatistics)
            {
                SopraSerializer.SerializeAchievements();
                SopraSerializer.Serialize(Globals.sStatisticsPath,
                    Globals.mStatistics,
                    typeof(Dictionary<string, int>));

                mLastTimeSavedAchievementsAndStatistics = Globals.GameTime.TotalGameTime.Seconds;
            }
        }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch);
        }

        protected virtual void Draw(SpriteBatch spriteBatch)
        {
            // update FoW LightMask
            spriteBatch.End();
            mFogOfWar.Draw(spriteBatch);

            Globals.GraphicsDevice.GraphicsDevice.SetRenderTarget(mMainRt);
            spriteBatch.Begin(samplerState: SamplerState.LinearWrap);
            spriteBatch.Draw(Art.PurpleNebula,
                new Vector2(0, 0),
                new Rectangle(0, 0, mScreenSize.X * 2, mScreenSize.Y * 2),
                Color.White,
                0f,
                Vector2.One,
                0.75f,
                SpriteEffects.None,
                0);
            spriteBatch.End();
            spriteBatch.Begin();
            mCamera.mParallax.Draw(spriteBatch, mCamera.Zoom * 2.5f, mWorldSize);
            mEntityManager.Draw(spriteBatch, mCamera.Rectangle, mCamera.TranslationMatrix);

            spriteBatch.End();
            Globals.GraphicsDevice.GraphicsDevice.SetRenderTarget(null);

            // Draw Main Game with applied FogOfWar
            if (mFogOfWar.mEnabled)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Art.LightingEffect.Parameters["lightMask"].SetValue(mFogOfWar.mLightsTarget);
                Art.LightingEffect.CurrentTechnique.Passes[0].Apply();
            }
            else
            {
                spriteBatch.Begin();
            }
            spriteBatch.Draw(mMainRt, Vector2.Zero, Color.White);

            spriteBatch.End();
            spriteBatch.Begin();

            // draw red rect
            // this is ugly, if possible change later
            if (mLeftMouseButton)
            {
                SelectRectangle.Draw(spriteBatch, mLastInput.GetSelectRect());
                mLeftMouseButton = false;
            }

            if (Globals.ScreenManager.GetScreen(typeof(ShipBaseScreen)) != null)
            {
                ShipBaseScreen shipbasescreen = (ShipBaseScreen)Globals.ScreenManager.GetScreen(typeof(ShipBaseScreen));
                var shipbaseentity = shipbasescreen.GetEntity();
                var topleftvec2 = shipbaseentity.GetPos() - new Vector2(Globals.sViewRadius["SpaceBase"], Globals.sViewRadius["SpaceBase"]);
                var circletopleft = new Point((int)topleftvec2.X, (int)topleftvec2.Y);
                spriteBatch.End();
                spriteBatch.Begin(transformMatrix:mCamera.TranslationMatrix);
                spriteBatch.Draw(Art.Circle, new Rectangle(circletopleft.X, circletopleft.Y, Globals.sViewRadius["SpaceBase"] * 2, Globals.sViewRadius["SpaceBase"] * 2), Color.White * 0.6f);
                spriteBatch.End();
                spriteBatch.Begin();
            }
            else if (Globals.ScreenManager.GetScreen(typeof(PlanetBaseScreen)) != null)
            {
                PlanetBaseScreen planetbasescreen = (PlanetBaseScreen)Globals.ScreenManager.GetScreen(typeof(PlanetBaseScreen));
                var planetbaseentity = planetbasescreen.GetEntity();
                var topleftvec2 = planetbaseentity.GetPos() - new Vector2(Globals.sViewRadius["SpaceBase"], Globals.sViewRadius["SpaceBase"]);
                var circletopleft = new Point((int)topleftvec2.X, (int)topleftvec2.Y);
                spriteBatch.End();
                spriteBatch.Begin(transformMatrix: mCamera.TranslationMatrix);
                spriteBatch.Draw(Art.Circle, new Rectangle(circletopleft.X, circletopleft.Y, Globals.sViewRadius["SpaceBase"] * 2, Globals.sViewRadius["SpaceBase"] * 2), Color.White * 0.6f);
                spriteBatch.End();
                spriteBatch.Begin();
            }


            Art.AnimationManager.DrawAnimations(spriteBatch, mCamera.TranslationMatrix);
            if (!mCamera.mMinimapDisabled)
            {
                mCamera.DrawMiniMap(spriteBatch);
            }
            
        }
        internal void SaveEntityManager(int slotNumber)
        {
            // Save the object to the SaveGame directory.
            SopraSerializer.Serialize(Globals.SaveGamePath(slotNumber), this, typeof(GameScreen));
        }

        protected Point GetWorldSize()
        {
            return mWorldSize;
        }

        protected Player GetPlayer()
        {
            return mPlayer;
        }

        protected Player GetKi()
        {
            return mKi;
        }

        protected EntityManager GetEntityManager()
        {
            return mEntityManager;
        }

        private void SelectEntities(Input input)
        {
            // Unselect previous Selection.
            foreach (var e in mEntityManager.mSelectedEntities.Where(x => x.GetComponent<CSelect>() != null))
            {
                e.GetComponent<CSelect>().mSelected = false;
                if (e is ETransport et)
                {
                    foreach (var c in et.GetCollectors())
                    {
                        c.GetComponent<CSelect>().mSelected = false;
                    }
                }
            }

            // Select Entities using the Selection Rectangle.
            if (input.GetSelectRect().Height != 0 || input.GetSelectRect().Width != 0)
            {
                var delta = mCamera.ScreenToWorld(input.GetSelectRect().Location.ToVector2()).ToPoint();
                var width = (int)(input.GetSelectRect().Width * (1 / mCamera.Zoom));
                var height = (int)(input.GetSelectRect().Height * (1 / mCamera.Zoom));
                sLastRectangle = new Rectangle(delta.X, delta.Y, width, height);
                mEntityManager.mSelectedEntities = mEntityManager.Entities
                    .Where(x => x.GetComponent<CSelect>() != null
                                && x.GetComponent<CSelect>().AllowGroupSelection && x is not ECollector
                                && x.Rectangle.Intersects(sLastRectangle)).ToList();

                List<ECollector> toAdd = new();
                foreach (var e in mEntityManager.mSelectedEntities)
                {
                    e.GetComponent<CSelect>().mSelected = true;
                    if (e is ETransport et)
                    {

                        foreach (var c in et.GetCollectors())
                        {
                            c.GetComponent<CSelect>().mSelected = true;
                            toAdd.Add(c);
                        }
                    }
                }

                foreach (var c in toAdd)
                {
                    mEntityManager.mSelectedEntities.Add(c);
                }

            }
            else
            {
                // Select one Entity using the current mousePosition.
                mEntityManager.mSelectedEntities = mEntityManager.Entities
                    .Where(x => x.GetComponent<CSelect>() != null && x is not ECollector
                                && x.Rectangle.Contains(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()))
                                && x.GetComponent<CTeam>()?.mTeam is Team.Player or Team.Neutral)
                    .ToList();
                List<ECollector> toAdd = new();
                foreach (var e in mEntityManager.mSelectedEntities)
                {
                    e.GetComponent<CSelect>().mSelected = true;

                    if (e is EBase)
                    {
                        var cTakeBase = e.GetComponent<CTakeBase>();
                        if (cTakeBase != null && e.GetComponent<CTeam>().mTeam is Team.Neutral
                                              && (cTakeBase.IsShipInActionRadius(mPlayer) && !cTakeBase.IsShipInActionRadius(mPlayer.GetOpponent())))
                        {
                            // Basis einnehmen.
                            e.GetComponent<CTeam>().ChangeTeam(mPlayer);
                            e.GetComponent<CHealth>()?.ChangeHealth(50);
                            e.AddComponent(new CView(e is ESpaceBase
                                ? Globals.sViewRadius["SpaceBase"]
                                : Globals.sViewRadius["PlanetBase"]));
                            Globals.mStatistics["BasesTaken"] += 1;
                            Globals.mRunStatistics["BasesTaken"] += 1;
                            // It seems like we get wrong numbers here:
                            var kiBaseCount = mKi.mPlanetBases.Count + mKi.mSpaceBases.Count;
                            var playerBaseCount = mPlayer.mPlanetBases.Count + mPlayer.mSpaceBases.Count;
                            Globals.NotificationManager.AddNotification(new Notification("Basis eingenommen (" + playerBaseCount + "/" + (playerBaseCount + kiBaseCount)+ ")"));
                        }
                    }

                    if (e is ETransport et)
                    {
                        foreach (var c in et.GetCollectors())
                        {
                            c.GetComponent<CSelect>().mSelected = true;
                            toAdd.Add(c);
                        }
                    }

                }

                foreach (var c in toAdd)
                {
                    mEntityManager.mSelectedEntities.Add(c);
                }
            }
        }

        // function has been restructured (mechanism in MoveEntities)
        private void MoveSelection(Point pos)
        {
            
            if (Globals.mMoveEntities.SetTargetOfGroup(mEntityManager, pos))
            {
                // not optimal to loop over the selection again here, however it's important to reset the targets.
                foreach (var e in mEntityManager.mSelectedEntities)
                {
                    e.GetComponent<CGun>()?.RemoveFixedTarget();
                    e.GetComponent<CStorage>()?.RemoveTarget();
                }

                var rand = Globals.RandomNumber() % 3;
                switch (rand)
                {
                    case 0:
                        Globals.SoundManager.PlaySound(SoundEnum.Bombership);
                        break;
                    case 1:
                        Globals.SoundManager.PlaySound(SoundEnum.Shootership);
                        break;
                    default:
                        Globals.SoundManager.PlaySound(SoundEnum.Transportship);
                        break;
                }
                Art.AnimationManager.AddAnimation(new ArrowAnimation(new Vector2(pos.X, pos.Y), 0.7f));
            }
        }

        private void ActionSelection(Entity target)
        {
            var selection = mEntityManager.mSelectedEntities;
            var targetTeam = target.GetComponent<CTeam>()?.mTeam;
            var actionPossible = false;

            switch (target)
            {
                case EResource:
                    // Pickup Resource.
                    actionPossible = true;
                    break;
                case EBase when targetTeam == Team.Player:
                    foreach (var e in selection.Where(e => e.GetComponent<CStorage>() != null))
                    {
                        e.GetComponent<CStorage>().mTarget = target;
                        actionPossible = true;
                    }
                    break;
                case EBase when targetTeam == Team.Neutral:
                    actionPossible = true;
                    break;
                default:
                {
                    // Set Target for the entities that support the given Action.
                    foreach (var e in selection)
                    {
                        // The selected Entities can't apply Actions on themselves.
                        if (target == e)
                        {
                            return;
                        }

                        if ((targetTeam == Team.Ki && e is EAttacker or EMoerser)
                            || (targetTeam == Team.Player && e is EMedic))
                        {
                            e.GetComponent<CGun>()?.SetFixedTarget(target);
                            actionPossible = true;
                        }
                    }

                    break;
                }
            }

            if (!actionPossible)
            {
                // can't apply action, so don't move to target.
                return;
            }

            // Move entire selection to target.
            if (target is EBase)
            {
                var xMid = 0f;
                var yMid = 0f;
                foreach (var e in selection)
                {
                    xMid += e.GetPos().X;
                    yMid += e.GetPos().Y;
                }
                xMid /= selection.Count;
                yMid /= selection.Count;

                var delta = new Vector2(xMid - target.GetPos().X, yMid - target.GetPos().Y);
                // Can't normalize a Zero Vector + no need to change position in that case.
                if (delta.Length() == 0)
                {
                    return;
                }
                delta.Normalize();
                var wayPoint = (target.GetPos() + delta * 1200).ToPoint();

                Globals.mMoveEntities.Move(selection, wayPoint, false);
            }
            else
            {
                Globals.mMoveEntities.Move(selection, target.GetPos().ToPoint(), false);
            }

            // Tell the player his order will be carried out
            switch (Globals.RandomNumber() % 3)
            {
                case 0:
                    mSoundManager.PlaySound(SoundEnum.Shootership);
                    break;
                case 1:
                    mSoundManager.PlaySound(SoundEnum.Bombership);
                    break;
                case 2:
                    mSoundManager.PlaySound(SoundEnum.Transportship);
                    break;
            }

            Art.AnimationManager.AddAnimation(new ArrowAnimation(target.Rectangle.Center.ToVector2(), 0.4f));
        }
        
        public void Resize(Point newSize)
        {
            mScreenSize = newSize;
            mCamera.Resize(newSize);

            mMainRt = new RenderTarget2D (Globals.GraphicsDevice.GraphicsDevice, Globals.GraphicsDevice.GraphicsDevice.PresentationParameters.BackBufferWidth,
            Globals.GraphicsDevice.GraphicsDevice.PresentationParameters.BackBufferHeight);
            mFogOfWar.mLightsTarget = new RenderTarget2D(Globals.GraphicsDevice.GraphicsDevice,
                Globals.GraphicsDevice.GraphicsDevice.PresentationParameters.BackBufferWidth,
                Globals.GraphicsDevice.GraphicsDevice.PresentationParameters.BackBufferHeight);
        }

        private void HandleMouseInput(Input input)
        {
            var pos = input.GetMousePosition();
            var leftButton = input.GetMouseButton(0);
            var rightButton = input.GetMouseButton(1);

            // Click on the MiniMap.
            if (mCamera.mMiniMapRectangle.Contains(pos) && !mCamera.mMinimapDisabled)
            {
                switch (leftButton)
                {
                    default:
                    case MouseActionEnum.Free:
                    case MouseActionEnum.Released:
                        mClipToMiniMap = false;
                        mClipToScreen = false;
                        break;
                    case MouseActionEnum.Pressed:
                    case MouseActionEnum.Held:
                        if (mClipToScreen)
                        {
                            mLeftMouseButton = true;
                            SelectEntities(input);
                            break;
                        }
                        mClipToMiniMap = true;
                        mCamera.MiniMapMove(new Vector2(
                            (int)((1.0 * pos.X) / mCamera.mMiniMapRectangle.Width * mWorldSize.X),
                            (int)(1.0 * (pos.Y - (mScreenSize.Y - mCamera.mMiniMapRectangle.Height)) / mCamera.mMiniMapRectangle.Height * mWorldSize.Y)));
                        break;
                }

                switch (rightButton)
                {
                    default:
                    case MouseActionEnum.Free:
                    case MouseActionEnum.Held:
                    case MouseActionEnum.Released:
                        break;
                    case MouseActionEnum.Pressed:
                        MoveSelection(mCamera.ScreenToWorldClickInMiniMap(pos.ToVector2()).ToPoint());
                        break;
                }
                return;
            }

            // Click on Screen.
            var mouseMapPosition = mCamera.ScreenToWorld(pos.ToVector2());
            switch (leftButton)
            {
                default:
                case MouseActionEnum.Free:
                case MouseActionEnum.Released:
                    mClipToMiniMap = false;
                    mClipToScreen = false;
                    break;
                case MouseActionEnum.Pressed:
                case MouseActionEnum.Held:
                    if (mClipToMiniMap)
                    {
                        mCamera.MiniMapMove(new Vector2(
                            (int)((1.0 * pos.X) / mCamera.mMiniMapRectangle.Size.X * mWorldSize.X),
                            (int)(1.0 * (pos.Y - (mScreenSize.Y - mCamera.mMiniMapRectangle.Size.Y)) / mCamera.mMiniMapRectangle.Size.Y * mWorldSize.Y)));
                        break;
                    }

                    mLeftMouseButton = true;
                    mClipToScreen = true;

                    SelectEntities(input);
                    break;
            }
            switch (rightButton)
            {
                default:
                case MouseActionEnum.Free:
                case MouseActionEnum.Released:
                case MouseActionEnum.Held:
                    break;
                case MouseActionEnum.Pressed:
                    // Might wanna precompute all visible entities at start of EntityManager.Update instead
                    var e = mEntityManager.Entities.FirstOrDefault(e => e.Rectangle.Contains(mouseMapPosition));
                    // Clicked on
                    if (e != null) // Entity
                    {
                        ActionSelection(e);
                    }
                    else // Map
                    {

                        MoveSelection(mouseMapPosition.ToPoint());
                        
                    }
                    break;
            }

        }

        private void HandleKeyInput(Input input)
        {
            if (input.GetKey(Keys.B))
            {
                foreach (var e in mEntityManager.mSelectedEntities)
                {
                    e.GetComponent<CBuoyPlacer>()?.PlaceBuoy();
                }
            } else if (input.GetKey(Globals.mKeys["ToggleMiniMap"]))
            {
                mCamera.mMinimapDisabled = !mCamera.mMinimapDisabled;
            }
        }
        public SoundManager GetSoundManager()
        {
            return mSoundManager;
        }

        private void Tutorial1()
        {
            mPlayer.SetOpponent(mKi);
            mKi.SetOpponent(mPlayer);

            mCamera = new Camera(new Vector2(0, 0), mScreenSize, mWorldSize, mEntityManager);
            mFogOfWar = new FogOfWar(mEntityManager, mCamera)
            {
                // Assuming fow is not supposed to exist in tutorial
                mEnabled = false
            };
            mGrid = new Grid(mWorldSize.X, mWorldSize.Y, GridTileDimension);

            var bomberPlayer = new EAttacker(mCamera.ScreenToWorld(new Vector2(mScreenSize.X * 0.5f, mScreenSize.Y * 0.5f)), mPlayer);
            bomberPlayer.GetComponent<CTeam>().ChangeTeam(mPlayer);
            bomberPlayer.GetComponent<CSelect>().mSelected = true;

            mEntityManager.Add(bomberPlayer);
            mPlayer.AddEntity(bomberPlayer);
        }
    }
}
