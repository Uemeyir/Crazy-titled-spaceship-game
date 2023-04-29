using Microsoft.Xna.Framework;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity.Base.PlanetBase;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.InputSystem;
using sopra05_2223.Pathfinding;
using sopra05_2223.Protagonists;
using sopra05_2223.SoundManagement;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Core;
using sopra05_2223.Core.Entity.Base.SpaceBase;

namespace sopra05_2223.Screens
{
    internal class TechdemoScreen : GameScreen
    {
        private const float FpsUpdateTimer = 1;
        private double mLastFpsUpdate;
        private double mFps;
        private double mFpsLeast = 60;
        private double mFpsMost;
        private double mFpsAvg;
        private int mFpsCount;
        private readonly Queue<double> mFpsQueue = new();
        private const int MaxFpsSamples = 100;

        public TechdemoScreen(Point screenSize, Point worldSize, SoundManager soundManager, PlayerPlayer player, PlayerKi ki) : base(screenSize, worldSize, soundManager, player, ki, false)
        {
            Initialize();
        }

        private void Initialize()
        {
            // Manually increasing camera speed because the TechDemo-map is huge.
            // mCamera.Speed = 100;

            var em = GetEntityManager();
            em.RemoveForTechDemo();
            var worldSize = GetWorldSize();
            var techDemoGrid = new Grid(worldSize.X, worldSize.Y, 1000);
            var extendedGrid = new Grid(worldSize.X, worldSize.Y, 400);
            var standardSizedGrid = new Grid(worldSize.X, worldSize.Y, 100);

            var player = GetPlayer();
            var ki = GetKi();
            player.SetOpponent(ki);
            ki.SetOpponent(player);

            var aiBase = new EPlanetBase1(new Vector2(32500, 32500), ki, techDemoGrid);
            mEntityManager.Add(aiBase);
            ki.AddEntity(aiBase);
            standardSizedGrid.InsertBaseIntoGrid(aiBase);
            extendedGrid.InsertBaseIntoGrid(aiBase);


            var aiBase2 = new EPlanetBase1(new Vector2(60500, 62500), ki, techDemoGrid);
            mEntityManager.Add(aiBase2);
            ki.AddEntity(aiBase2);
            standardSizedGrid.InsertBaseIntoGrid(aiBase2);
            extendedGrid.InsertBaseIntoGrid(aiBase2);

            var aiBase3 = new EPlanetBase1(new Vector2(64500, 72500), ki, techDemoGrid);
            mEntityManager.Add(aiBase3);
            ki.AddEntity(aiBase3);
            standardSizedGrid.InsertBaseIntoGrid(aiBase3);
            extendedGrid.InsertBaseIntoGrid(aiBase3);


            var aiBase4 = new EPlanetBase1(new Vector2(46500, 34500), ki, techDemoGrid);
            mEntityManager.Add(aiBase4);
            ki.AddEntity(aiBase4);
            standardSizedGrid.InsertBaseIntoGrid(aiBase4);
            extendedGrid.InsertBaseIntoGrid(aiBase4);

            var aiBase5 = new EPlanetBase1(new Vector2(50500, 22500), ki, techDemoGrid);
            mEntityManager.Add(aiBase5);
            ki.AddEntity(aiBase5);
            standardSizedGrid.InsertBaseIntoGrid(aiBase5);
            extendedGrid.InsertBaseIntoGrid(aiBase5);


            var aiBase6 = new EPlanetBase1(new Vector2(32500, 72500), ki, techDemoGrid);
            mEntityManager.Add(aiBase6);
            ki.AddEntity(aiBase6);
            standardSizedGrid.InsertBaseIntoGrid(aiBase6);
            extendedGrid.InsertBaseIntoGrid(aiBase6);


            for (var i = 0; i < 10; i++)
            {
                var attackerAi = new EAttacker(new Vector2(14500, 15000 + i * 600), ki);
                var moerserAi = new EMoerser(new Vector2(13500, 15000 + i * 600), ki);
                var attackerAi2 = new EAttacker(new Vector2(12500, 15000 + i * 600), ki);
                var attackerAi3 = new EAttacker(new Vector2(11500, 15000 + i * 600), ki);

                var attacker = new EAttacker(new Vector2(24500, 15000 + i * 600), player);
                var moerser = new EMoerser(new Vector2(23500, 15000 + i * 600), player);
                var attacker2 = new EAttacker(new Vector2(22500, 15000 + i * 600), player);
                var moerser2 = new EMoerser(new Vector2(25500, 15000 + i * 600), player);

                var attacker3 = new EAttacker(new Vector2(10000 + i * 600, 23500), player);
                var moerser3 = new EMoerser(new Vector2(10000 + i * 600, 24100), player);

                mEntityManager.Add(attacker);
                mEntityManager.Add(attacker2);
                mEntityManager.Add(attackerAi);
                mEntityManager.Add(attackerAi2);
                mEntityManager.Add(attackerAi3);
                mEntityManager.Add(attacker3);
                mEntityManager.Add(moerser);
                mEntityManager.Add(moerser2);
                mEntityManager.Add(moerserAi);
                mEntityManager.Add(moerser3);

                player.AddEntity(attacker);
                player.AddEntity(attacker2);
                player.AddEntity(attacker3);
                ki.AddEntity(attackerAi);
                ki.AddEntity(attackerAi2);
                ki.AddEntity(attackerAi3);
                player.AddEntity(moerser);
                player.AddEntity(moerser2);
                player.AddEntity(moerser3);
                ki.AddEntity(moerserAi);
            }

            // 1000 ships are produced
            for (var i = 0; i < 50; i++)
            {
                // ------------- 200 attacker ----------------------------------------------------------------------------
                var firstColumnBomberPlayer = new EAttacker(new Vector2(4500, 5000 + i * 600), player);
                var secondColumnBomberPlayer = new EAttacker(new Vector2(4500, 45000 + i * 600), player);
                var thirdColumnBomberPlayer = new EAttacker(new Vector2(6000, 5000 + i * 600), player);
                var fourthColumnBomberPlayer = new EAttacker(new Vector2(6000, 45000 + i * 600), player);
                var fifthColumnBomberPlayer = new EAttacker(new Vector2(7500, 5000 + i * 600), player);
                var sixthColumnBomberPlayer = new EAttacker(new Vector2(7500, 45000 + i * 600), player);

                mEntityManager.Add(firstColumnBomberPlayer);
                mEntityManager.Add(secondColumnBomberPlayer);
                mEntityManager.Add(thirdColumnBomberPlayer);
                mEntityManager.Add(fourthColumnBomberPlayer);
                mEntityManager.Add(fifthColumnBomberPlayer);
                mEntityManager.Add(sixthColumnBomberPlayer);

                player.AddEntity(secondColumnBomberPlayer);
                player.AddEntity(firstColumnBomberPlayer);
                player.AddEntity(thirdColumnBomberPlayer);
                player.AddEntity(fourthColumnBomberPlayer);
                player.AddEntity(fifthColumnBomberPlayer);
                player.AddEntity(sixthColumnBomberPlayer);


                var firstColumnBomberKi = new EAttacker(new Vector2(74000, 5000 + i * 600), ki);
                var secondColumnBomberKi= new EAttacker(new Vector2(74000, 45000 + i * 600), ki);
                var thirdColumnBomberKi= new EAttacker(new Vector2(72500, 5000 + i * 600), ki);
                var fourthColumnBomberKi= new EAttacker(new Vector2(72500, 45000 + i * 600), ki);
                var fifthColumnBomberKi = new EAttacker(new Vector2(70000, 45000 + i * 600), ki);

                mEntityManager.Add(firstColumnBomberKi);
                mEntityManager.Add(secondColumnBomberKi);
                mEntityManager.Add(thirdColumnBomberKi);
                mEntityManager.Add(fourthColumnBomberKi);
                mEntityManager.Add(fifthColumnBomberKi);

                ki.AddEntity(secondColumnBomberKi);
                ki.AddEntity(firstColumnBomberKi);
                ki.AddEntity(thirdColumnBomberKi);
                ki.AddEntity(fourthColumnBomberKi);
                ki.AddEntity(fifthColumnBomberKi);

                // -------------- 200 moerser --------------------------------------------------------------------------------
                var firstColumnMoerserPlayer = new EMoerser(new Vector2(1000, 5000 + i * 700), player);
                var secondColumnMoerserPlayer = new EMoerser(new Vector2(1000, 45000 + i * 700), player);
                var thirdColumnMoerserPlayer = new EMoerser(new Vector2(3000, 5000 + i * 700), player);
                var fourthColumnMoerserPlayer = new EMoerser(new Vector2(3000, 45000 + i * 700), player);


                mEntityManager.Add(firstColumnMoerserPlayer);
                mEntityManager.Add(secondColumnMoerserPlayer);
                mEntityManager.Add(thirdColumnMoerserPlayer);
                mEntityManager.Add(fourthColumnMoerserPlayer);

                player.AddEntity(firstColumnMoerserPlayer);
                player.AddEntity(secondColumnMoerserPlayer);
                player.AddEntity(thirdColumnMoerserPlayer);
                player.AddEntity(fourthColumnMoerserPlayer);


                var firstColumnMoerserKi = new EMoerser(new Vector2(79000, 5000 + i * 700), ki);
                var secondColumnMoerserKi= new EMoerser(new Vector2(79000, 45000 + i * 700), ki);
                var thirdColumnMoerserKi= new EMoerser(new Vector2(77000, 5000 + i * 700), ki);
                var fourthColumnMoerserKi= new EMoerser(new Vector2(77000, 45000 + i * 700), ki);

                mEntityManager.Add(firstColumnMoerserKi);
                mEntityManager.Add(secondColumnMoerserKi);
                mEntityManager.Add(thirdColumnMoerserKi);
                mEntityManager.Add(fourthColumnMoerserKi);

                ki.AddEntity(firstColumnMoerserKi);
                ki.AddEntity(secondColumnMoerserKi);
                ki.AddEntity(thirdColumnMoerserKi);
                ki.AddEntity(fourthColumnMoerserKi);
            }

            for (var i = 0; i < 30; i++)
            {
                var fColumnAttacker = new EAttacker(new Vector2(70000, 35000 + i * 600), ki);
                mEntityManager.Add(fColumnAttacker);
                ki.AddEntity(fColumnAttacker);
            }
            for (var i = 0; i < 10; i++)
            {
                var pb1T = new EPlanetBase1(new Vector2(70000, 1500 + i * 2000), ki, techDemoGrid);
                var sp1T = new ESpaceBase1(new Vector2(70000, 25000 + i * 1500), ki, techDemoGrid);
                
                mEntityManager.Add(pb1T);
                mEntityManager.Add(sp1T);
                standardSizedGrid.InsertPlanetIntoGrid(pb1T);
                standardSizedGrid.InsertPlanetIntoGrid(sp1T);
            }

            Globals.mGridStandard = standardSizedGrid;
            Globals.mGridExtended = extendedGrid;
            Globals.mGridTechDemo = techDemoGrid;
            //SetStandardGrid(standardSizedGrid);
            //SetTechDemoGrid(techDemoGrid);
            mFogOfWar.mEnabled = false;
        }

        protected override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            // FPS
            mLastFpsUpdate += Globals.GameTime.ElapsedGameTime.TotalSeconds;
            if (mLastFpsUpdate >= FpsUpdateTimer)
            {
                mLastFpsUpdate--;
                mFps = mFpsCount * (1 / FpsUpdateTimer);
                mFpsCount = 0;
            }
            mFpsCount++;
            var displayWidth = mCamera.Rectangle.Width * mCamera.Zoom;
            var fpsX = displayWidth - 200;
            const int fpsY = 220;
            spriteBatch.DrawString(Art.Arial12, "FPS:", new Vector2(fpsX, fpsY), Color.Blue);
            spriteBatch.DrawString(Art.Arial12, "Now:                " + mFps.ToString("N0"), new Vector2(fpsX, fpsY + 20), Color.Yellow);
            spriteBatch.DrawString(Art.Arial12, "Least:              " + mFpsLeast.ToString("N0"), new Vector2(fpsX, fpsY + 40), Color.Red);
            spriteBatch.DrawString(Art.Arial12, "Most:               " + mFpsMost.ToString("N0"), new Vector2(fpsX, fpsY + 60), Color.Green);
            spriteBatch.DrawString(Art.Arial12, "Avg:                " + mFpsAvg.ToString("N0"), new Vector2(fpsX, fpsY + 80), Color.Cyan);
            
            var entites = mEntityManager.Entities;
            var friendly = 0;
            var enemy = 0;
            var neutral = 0;
            var moving = 0;
            foreach (var e in entites)
            {
                switch (e.GetComponent<CTeam>().mTeam)
                {
                    case Team.Ki:
                        enemy++;
                        break;
                    case Team.Neutral:
                        neutral++;
                        break;
                    case Team.Player:
                        friendly++;
                        break;
                }

                if (e.GetComponent<CTransform>() != null && e.GetComponent<CTransform>().GetSpeedInDistancePerMillis() != 0)
                {
                    moving++;
                }
            }

            spriteBatch.DrawString(Art.Arial12, "Entities:  ", new Vector2(fpsX, fpsY + 120), Color.Yellow);
            spriteBatch.DrawString(Art.Arial12, "Total Entities:     " + mEntityManager.Entities.Count.ToString("N0"), new Vector2(fpsX, fpsY + 140), Color.DeepPink);
            spriteBatch.DrawString(Art.Arial12, "Friendly Entities:  " + friendly.ToString("N0"), new Vector2(fpsX, fpsY + 160), Color.CornflowerBlue);
            spriteBatch.DrawString(Art.Arial12, "Enemy Entities:     " + enemy.ToString("N0"), new Vector2(fpsX, fpsY + 180), Color.Orange);
            spriteBatch.DrawString(Art.Arial12, "Neutral Entities:   " + neutral.ToString("N0"), new Vector2(fpsX, fpsY + 200), Color.White);
            spriteBatch.DrawString(Art.Arial12, "Moving Entities:    " + moving.ToString("N0"), new Vector2(fpsX, fpsY + 220), Color.White);
        }

        protected override void Update(Input input)
        {
            base.Update(input);
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
    }
}
