using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sopra05_2223.Background;
using sopra05_2223.Core;
using sopra05_2223.Core.Animation;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity;
using sopra05_2223.Core.Entity.Planet;
using sopra05_2223.Core.Entity.Resource;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.InputSystem;
using sopra05_2223.Protagonists;
using sopra05_2223.ScreenManagement;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace sopra05_2223.Screens
{
    internal sealed class DebugScreen : IScreen
    {
        // Saves the current EntityManager for easier Access to Entities
        private readonly EntityManager mEntityManager;
        // Saves current Camera
        private readonly Camera mCamera;
        // Saves current List of Animations
        private readonly AnimationManager mAnimations;
        // Sets Borderwidth of Hitbox Rectangle
        private const int Thickness = 5;
        // Saves, if SpawnMode is active or not
        private bool mSpawnMode;
        // Player PlayerPlayer
        private readonly Player mPlayer;
        // Player PlayerKi;
        private readonly Player mKi;
        private Player mSpawnPlayer;
        private readonly GameScreen mGameScreen;

        private const float FpsUpdateTimer = 1;
        private double mLastFpsUpdate;
        private double mFps;
        private double mFpsLeast = 60;
        private double mFpsMost;
        private double mFpsAvg;
        private int mFpsCount;
        private readonly Queue<double> mFpsQueue = new();
        private const int MaxFpsSamples = 100;
        private FogOfWar mFogOfWar;

        public DebugScreen(Player player, Player ki, EntityManager entityManager, Camera camera, AnimationManager animations, FogOfWar fogOfWar, GameScreen gamescreen)
        {
            mEntityManager = entityManager;
            mCamera = camera;
            mAnimations = animations;
            mPlayer = player;
            mKi = ki;
            mSpawnPlayer = player;
            mFogOfWar = fogOfWar;
            mGameScreen = gamescreen;
        }

        bool IScreen.UpdateLower => true;
        bool IScreen.DrawLower => true;
        public ScreenManager ScreenManager
        {
            get; set;
        }

        void IScreen.Draw(SpriteBatch spriteBatch)
        {
            // Text "SpawnMode"
            if (mSpawnMode)
            {
                spriteBatch.DrawString(Art.MenuButtonFont, "Spawn Mode", new Vector2(100, 500), Color.AliceBlue);
            }

            int gsx = Globals.Camera.Rectangle.X / Globals.mGridStandard.GetGridTileDimension();
            int gsy = Globals.Camera.Rectangle.Y / Globals.mGridStandard.GetGridTileDimension();
            int gsxd = (int)Math.Ceiling(Globals.Camera.Rectangle.Width / (float)Globals.mGridStandard.GetGridTileDimension());
            int gsyd = (int)Math.Ceiling(Globals.Camera.Rectangle.Height / (float)Globals.mGridStandard.GetGridTileDimension());
            for (var x = gsx; x < gsx + gsxd; x++)
            {
                for (var y = gsy; y < gsy + gsyd; y++)
                {
                    if (Globals.mGridStandard.IsObstacle(x, y))
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, mCamera.TranslationMatrix);
                        var tileDim = Globals.mGridStandard.GetGridTileDimension();
                        spriteBatch.Draw(Art.Healthbar, new Rectangle(x * tileDim + tileDim / 2 - Thickness * 3, y * tileDim + tileDim / 2 - Thickness * 3, Thickness * 6, Thickness * 6), Color.Yellow);
                    }
                    spriteBatch.End();
                    spriteBatch.Begin();
                }
            }

            int gex = Globals.Camera.Rectangle.X / Globals.mGridExtended.GetGridTileDimension();
            int gey = Globals.Camera.Rectangle.Y / Globals.mGridExtended.GetGridTileDimension();
            int gexd = (int)Math.Ceiling(Globals.Camera.Rectangle.Width / (float)Globals.mGridExtended.GetGridTileDimension());
            int geyd = (int)Math.Ceiling(Globals.Camera.Rectangle.Height / (float)Globals.mGridExtended.GetGridTileDimension());
            for (var x = gex; x < gex + gexd; x++)
            {
                for (var y = gey; y < gey + geyd; y++)
                {
                    if (Globals.mGridExtended.IsObstacle(x, y))
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, mCamera.TranslationMatrix);
                        var tileDim = Globals.mGridExtended.GetGridTileDimension();
                        spriteBatch.Draw(Art.Healthbar, new Rectangle(x * tileDim + tileDim / 2 - Thickness * 3, y * tileDim + tileDim / 2 - Thickness * 3, Thickness * 6, Thickness * 6), Color.Blue);
                    }
                    spriteBatch.End();
                    spriteBatch.Begin();
                }
            }

            if (Globals.mGridTechDemo is not null)
            {
                var tileDim = Globals.mGridTechDemo.GetGridTileDimension();
                int gtdx = Globals.Camera.Rectangle.X / Globals.mGridTechDemo.GetGridTileDimension();
                int gtdy = Globals.Camera.Rectangle.Y / Globals.mGridTechDemo.GetGridTileDimension();
                int gtdxd = (int)Math.Ceiling(Globals.Camera.Rectangle.Width / (float)Globals.mGridTechDemo.GetGridTileDimension());
                int gtdyd = (int)Math.Ceiling(Globals.Camera.Rectangle.Height / (float)Globals.mGridTechDemo.GetGridTileDimension());

                for (var x = gtdx; x < gtdx + gtdxd; x++)
                {
                    for (var y = gtdy; y < gtdy + gtdyd; y++)
                    {
                        if (Globals.mGridTechDemo.IsObstacle(x, y))
                        {
                            spriteBatch.End();
                            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, mCamera.TranslationMatrix);
                            spriteBatch.Draw(Art.Healthbar, new Rectangle(x * tileDim + tileDim / 2 - Thickness * 3, y * tileDim + tileDim / 2 - Thickness * 3, Thickness * 6, Thickness * 6), Color.Red);
                        }
                        spriteBatch.End();
                        spriteBatch.Begin();
                    }
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, mCamera.TranslationMatrix);
            foreach (var k in mEntityManager.Entities.Where(e => Globals.Camera.Rectangle.Intersects(e.Rectangle)))
            {
                // Draws Hitbox of Each of the Entities
                spriteBatch.Draw(Art.Healthbar, new Rectangle(k.GetX(), k.GetY(), k.GetWidth(), Thickness), Color.White);
                spriteBatch.Draw(Art.Healthbar, new Rectangle(k.GetX(), k.GetY(), Thickness, k.GetHeight()), Color.White);
                spriteBatch.Draw(Art.Healthbar, new Rectangle(k.GetX(), k.GetY() + k.GetHeight(), k.GetWidth() + Thickness, Thickness), Color.White);
                spriteBatch.Draw(Art.Healthbar, new Rectangle(k.GetX() + k.GetWidth(), k.GetY(), Thickness, k.GetHeight()), Color.White);

                if (k.GetComponent<CTransform>() is not null)
                {
                    // Draws Endpoint of Paths for each Entity with CTransform
                    spriteBatch.Draw(Art.Healthbar, new Rectangle((int)(k.GetComponent<CTransform>().GetTarget().X + k.GetWidth() / 2f - Thickness * 3), (int)(k.GetComponent<CTransform>().GetTarget().Y + k.GetHeight() / 2f - Thickness * 3), Thickness * 6, Thickness * 6), Color.HotPink);
                    // Draws each Point on Path of Entity
                    if (k.GetComponent<CTransform>().GetPath() is not null)
                    {
                        foreach (var i in k.GetComponent<CTransform>().GetPath())
                        {
                            spriteBatch.Draw(Art.Healthbar, new Rectangle(i.X + k.mTexture.Width / 2 - Thickness * 3, i.Y + k.mTexture.Height / 2 - Thickness * 3, Thickness * 6, Thickness * 6), Color.Purple);

                        }
                    }
                }
            }
            spriteBatch.End();
            spriteBatch.Begin();
            // FPS
            if (mGameScreen is not TechdemoScreen)
            {
                mLastFpsUpdate += Globals.GameTime.ElapsedGameTime.TotalSeconds;
                if (mLastFpsUpdate >= FpsUpdateTimer)
                {
                    mLastFpsUpdate--;
                    mFps = mFpsCount * (1 / FpsUpdateTimer);
                    mFpsCount = 0;
                }

                mFpsCount++;
                var displayWidth = mCamera.Rectangle.Width * mCamera.Zoom;
                var fpsX = displayWidth - 130;
                const int fpsY = 220;
                spriteBatch.DrawString(Art.Arial12, "FPS:", new Vector2(fpsX, fpsY), Color.Blue);
                spriteBatch.DrawString(Art.Arial12,
                    "Now:   " + mFps.ToString("N0"),
                    new Vector2(fpsX, fpsY + 20),
                    Color.Yellow);
                spriteBatch.DrawString(Art.Arial12,
                    "Least: " + mFpsLeast.ToString("N0"),
                    new Vector2(fpsX, fpsY + 40),
                    Color.Red);
                spriteBatch.DrawString(Art.Arial12,
                    "Most:  " + mFpsMost.ToString("N0"),
                    new Vector2(fpsX, fpsY + 60),
                    Color.Green);
                spriteBatch.DrawString(Art.Arial12,
                    "Avg:   " + mFpsAvg.ToString("N0"),
                    new Vector2(fpsX, fpsY + 80),
                    Color.Cyan);
            }
        }

        void IScreen.Resize(Point newSize)
        {

        }
        void IScreen.Update(Input input)
        {
            // F3 again removes DebugScreen
            if (input.GetKeys().Contains(Keys.F3))
            {
                ScreenManager.RemoveScreens();
            }

            // F4 toggles SpawnMode
            if (input.GetKeys().Contains(Keys.F4))
            {
                mSpawnMode = !mSpawnMode;
            }

            // F5 toggles FogOfWar
            if (input.GetKeys().Contains(Keys.F5))
            {
                mFogOfWar.mEnabled = !mFogOfWar.mEnabled;
            }
            if (input.GetKeys().Contains(Keys.F6))
            {
                if (Globals.mGameSpeed > 0.05f)
                {
                    Globals.mGameSpeed -= 0.05f;
                }
            }
            if (input.GetKeys().Contains(Keys.F7))
            {
                Globals.mGameSpeed += 0.05f;
            }

            if (input.GetKeys().Contains(Keys.W))
            {
                ScreenManager.AddScreen(new GameWonBackgroundScreen(Globals.Resolution.mScreenSize));
                ScreenManager.AddScreen(new GameWonScreen(Globals.Resolution.mScreenSize.X, Globals.Resolution.mScreenSize.Y, mGameScreen));
            }

            if (input.GetKeys().Contains(Keys.L))
            {
                ScreenManager.AddScreen(new GameOverBackgroundScreen(Globals.Resolution.mScreenSize));
                ScreenManager.AddScreen(new GameOverScreen(Globals.Resolution.mScreenSize.X, Globals.Resolution.mScreenSize.Y, mGameScreen));
            }

            if (input.GetKeys().Contains(Keys.E))
            {
                mAnimations.AddAnimation(new Explosion1(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()), 0.7f));
            }

            if (input.GetKeys().Contains(Keys.F))
            {
                mAnimations.AddAnimation(new PlanetAnimated1(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2())));
            }

            if (input.GetKeys().Contains(Keys.G))
            {
                mAnimations.AddAnimation(new ArrowAnimation(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()), 1.0f));
            }

            if (input.GetKey(Keys.V))
            {
                var ki = (PlayerKi)mKi;
                ki.TooglePause();
            }

            if (mSpawnMode)
            {
                SpawnMode(input);
            }

            // FPS
            if (mFps < mFpsLeast && mFps > 5)
            {
                mFpsLeast = mFps;
            }
            if (mFps > mFpsMost)
            {
                mFpsMost = mFps;
            }
            mFpsQueue.Enqueue(mFps);
            if (mFpsQueue.Count > MaxFpsSamples)
            {
                mFpsQueue.Dequeue();
                mFpsAvg = mFpsQueue.Average(i => i);
            }
            else
            {
                mFpsAvg = mFps;
            }
        }

        private void SpawnMode(Input input)
        {
            // Number 1 spawns Player Bomber
            // currently unused
            //var planetAddToGrid = new List<Entity>();
            if (input.GetKeys().Contains(Keys.D1))
            {
                var bomber = new EAttacker(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()), mSpawnPlayer);
                mEntityManager.Add(bomber);
                Globals.mStatistics["BuiltAttack"] += 1;
                Globals.mRunStatistics["BuiltAttack"] += 1;
                mSpawnPlayer.AddEntity(bomber);
            }
            // Number 2 spawns Player Transport
            else if (input.GetKeys().Contains(Keys.D2))
            {
                var transporter = new ETransport(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()), mSpawnPlayer);
                mEntityManager.Add(transporter);
                Globals.mStatistics["BuiltTransport"] += 1;
                Globals.mRunStatistics["BuiltTransport"] += 1;
                mSpawnPlayer.AddEntity(transporter);
            }
            // Number 3 spawns Planet1
            else if (input.GetKeys().Contains(Keys.D3))
            {
                var planet1 = new EPlanet(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()), 0, Globals.mGridStandard);
                if (Globals.mGridTechDemo is not null)
                {
                    Globals.mGridTechDemo.InsertPlanetIntoGrid(planet1);
                }
                //planetAddToGrid.Add(planet1);
                mEntityManager.Add(planet1);
                mKi.AddEntity(planet1);
            }
            // Number 4 spawns Planet2
            else if (input.GetKeys().Contains(Keys.D4))
            {
                var planet2 = new EPlanet(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()), 1, Globals.mGridStandard);
                //planetAddToGrid.Add(planet2);
                if (Globals.mGridTechDemo is not null)
                {
                    Globals.mGridTechDemo.InsertPlanetIntoGrid(planet2);
                }
                mEntityManager.Add(planet2);
                mKi.AddEntity(planet2);
            }
            // Number 5 spawns Planet3
            else if (input.GetKeys().Contains(Keys.D5))
            {
                var planet3 = new EPlanet(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()), 2, Globals.mGridStandard);
                //planetAddToGrid.Add(planet3);
                if (Globals.mGridTechDemo is not null)
                {
                    Globals.mGridTechDemo.InsertPlanetIntoGrid(planet3);
                }
                mEntityManager.Add(planet3);
                mKi.AddEntity(planet3);
            }
            // Number 6 spawns Resource with 100 Oxygen and 100 Metal
            else if (input.GetKeys().Contains(Keys.D6))
            {
                var resource = new EResource(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()), 100, 100);
                mEntityManager.Add(new EResource(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()), 100, 100));
                mKi.AddEntity(resource);
            }
            // Number 7 spawns ESpy
            else if (input.GetKeys().Contains(Keys.D7))
            {
                var spy = new ESpy(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()), mSpawnPlayer);
                mEntityManager.Add(spy);
                Globals.mStatistics["BuiltSpy"] += 1;
                Globals.mRunStatistics["BuiltSpy"] += 1;
                mSpawnPlayer.AddEntity(spy);
            }
            // Number 8 spawns EMedic
            else if (input.GetKeys().Contains(Keys.D8))
            {
                var medic = new EMedic(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()), mSpawnPlayer);
                mEntityManager.Add(medic);
                Globals.mStatistics["BuiltMedic"] += 1;
                Globals.mRunStatistics["BuiltMedic"] += 1;
                mSpawnPlayer.AddEntity(medic);
            }
            // Number 9 spawns EMoerser
            else if (input.GetKeys().Contains(Keys.D9))
            {
                var moerser = new EMoerser(mCamera.ScreenToWorld(input.GetMousePosition().ToVector2()), mSpawnPlayer);
                mEntityManager.Add(moerser);
                Globals.mStatistics["BuiltMoerser"] += 1;
                Globals.mRunStatistics["BuiltMoerser"] += 1;
                mSpawnPlayer.AddEntity(moerser);
            }
            else if (input.GetKeys().Contains(Keys.D0))
            {
                mSpawnPlayer = mSpawnPlayer is PlayerPlayer ? mKi : mPlayer;
            }
        }
    }
}
