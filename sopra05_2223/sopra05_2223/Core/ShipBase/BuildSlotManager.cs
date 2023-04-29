using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.ShipBase
{
    internal sealed class BuildSlotManager : Component
    {
        [JsonRequired]
        private List<BuildSlot> mBuildSlots;

        private const int NumberOfSlots = 5;
        private const int NumberOfPositions = 12;
        private int mRotate;
        private int mDistance = 800;
        [JsonRequired]
        private List<float> mPositions;

        [JsonRequired]
        private int mCurrentSpawn;


        public BuildSlotManager()
        {
            InitializeBuildManager();
        }

        [JsonConstructor]
        public BuildSlotManager(int currentSpawn)
        {
            mCurrentSpawn = currentSpawn;
        }

        // Schedules new Build Task.
        // Returns true if there was an empty slot found (and starts Task)
        // Returns false if all slots are occupied
        internal bool AddTask(float length, Type type)
        {
            foreach (var k in mBuildSlots)
            {
                if (!k.IsUsed())
                {
                    var currentTime = Globals.GameTime.TotalGameTime.Milliseconds;
                    k.SetStart(currentTime);
                    k.SetDuration(length);
                    k.SetType(type);

                    mCurrentSpawn += 1;
                    mCurrentSpawn %= NumberOfPositions;
                    if (mCurrentSpawn == 0)
                    {
                        mRotate += 360 / NumberOfPositions / 2;
                        GeneratePositions(mRotate);
                        if (mDistance < 1600)
                        {
                            mDistance += 400;
                        }
                        else
                        {
                            mDistance = 800;
                        }
                    }

                    k.SetDestination(new Vector2((mEntity.GetX() + mEntity.GetWidth() * 0.5f) - mPositions[2 * mCurrentSpawn] * mDistance, (mEntity.GetY() + mEntity.GetHeight() * 0.5f) - mPositions[2 * mCurrentSpawn + 1] * mDistance));
                    k.SetUsed();

                    return true;
                }
            }
            return false;
        }

        private void GeneratePositions(int rotate = 0)
        {
            mPositions.Clear();
            for (var i = 0; i < NumberOfPositions; i++)
            {
                mPositions.Add(MathF.Cos(i * (360 / (float)NumberOfPositions * MathF.PI / 180) + rotate * MathF.PI / 180));
                mPositions.Add(MathF.Sin(i * (360 / (float)NumberOfPositions * MathF.PI / 180) + rotate * MathF.PI / 180));
            }
        }

        private void InitializeBuildManager()
        {
            mPositions = new List<float>();
            mBuildSlots = new List<BuildSlot>();
            
            for (var i = 0; i < NumberOfSlots; i++)
            {
                mBuildSlots.Add(new BuildSlot());
            }

            GeneratePositions();
        }

        internal void Draw(SpriteBatch spriteBatch, Point screenSize)
        {
            for (var i = 0; i < mBuildSlots.Count; i++)
            {
                var j = mBuildSlots[i].GetType();

                if (j == typeof(ETransport))
                {
                    spriteBatch.Draw(Art.Transport,
                        new Rectangle((int)(screenSize.X * 0.07), (int)(screenSize.Y * 0.21 + screenSize.Y * 0.15 * i), (int)(screenSize.Y * 0.10), (int)(screenSize.Y * 0.10)),
                        new Rectangle(0, 0, Art.Transport.Width, Art.Transport.Height),
                        Color.White);
                }
                else if (j == typeof(EAttacker))
                {
                    spriteBatch.Draw(Art.Bomber,
                        new Rectangle((int)(screenSize.X * 0.07), (int)(screenSize.Y * 0.21 + screenSize.Y * 0.15 * i), (int)(screenSize.Y * 0.10), (int)(screenSize.Y * 0.10)),
                        new Rectangle(0, 0, Art.Bomber.Width, Art.Bomber.Height),
                        Color.White);
                }
                else if (j == typeof(ESpy))
                {
                    spriteBatch.Draw(Art.Spy,
                        new Rectangle((int)(screenSize.X * 0.07), (int)(screenSize.Y * 0.21 + screenSize.Y * 0.15 * i), (int)(screenSize.Y * 0.10), (int)(screenSize.Y * 0.10)),
                        new Rectangle(0, 0, Art.Spy.Width, Art.Spy.Height),
                        Color.White);
                }
                else if (j == typeof(EMedic))
                {
                    spriteBatch.Draw(Art.Medic,
                        new Rectangle((int)(screenSize.X * 0.07), (int)(screenSize.Y * 0.21 + screenSize.Y * 0.15 * i), (int)(screenSize.Y * 0.10), (int)(screenSize.Y * 0.10)),
                        new Rectangle(0, 0, Art.Medic.Width, Art.Medic.Height),
                        Color.White);
                }
                else if (j == typeof(EMoerser))
                {
                    spriteBatch.Draw(Art.Moerser,
                        new Rectangle((int)(screenSize.X * 0.07), (int)(screenSize.Y * 0.21 + screenSize.Y * 0.15 * i), (int)(screenSize.Y * 0.10), (int)(screenSize.Y * 0.10)),
                        new Rectangle(0, 0, Art.Moerser.Width, Art.Moerser.Height),
                        Color.White);
                }
            }
        }

        internal void BlockBuildSlotManager()
        {
            mPositions = new List<float>();
            mBuildSlots = new List<BuildSlot>();
        }

        internal void UnBlockBuildSlotManager()
        {
            InitializeBuildManager();
        }

        internal int GetSlots()
        {
            return mBuildSlots.Count;
        }

        internal List<BuildSlot> GetBuildSlots()
        {
            return mBuildSlots;
        }


        internal override void Update()
        {
            var player = mEntity.GetComponent<CTeam>()?.GetProtagonist();
            foreach (var k in mBuildSlots)
            {
                k.Update();
                if (k.IsFinished() && player is not null)
                {
                    var j = k.GetType();
                    if (j == typeof(ETransport))
                    {
                        var newShip = new ETransport(k.GetDestination(), player);
                        mEntity.mEntityManager.Add(newShip);
                        if (player is PlayerPlayer)
                        {
                            Globals.mStatistics["BuiltTransport"] += 1;
                            Globals.mRunStatistics["BuiltTransport"] += 1;
                        }
                        player.AddEntity(newShip);
                        newShip.AddCollectors(6);
                    }
                    else if (j == typeof(EAttacker))
                    {
                        var newShip = new EAttacker(k.GetDestination(), player);
                        mEntity.mEntityManager.Add(newShip);
                        if (player is PlayerPlayer)
                        {
                            Globals.mStatistics["BuiltAttack"] += 1;
                            Globals.mRunStatistics["BuiltAttack"] += 1;
                            Globals.mBuilt["Attacker"] = true;
                        }

                        player.AddEntity(newShip);
                    }
                    else if (j == typeof(ESpy))
                    {
                        var newShip = new ESpy(k.GetDestination(), player);
                        mEntity.mEntityManager.Add(newShip);
                        if (player is PlayerPlayer)
                        {
                            Globals.mStatistics["BuiltSpy"] += 1;
                            Globals.mRunStatistics["BuiltSpy"] += 1;
                            Globals.mBuilt["Spy"] = true;
                        }
                        player.AddEntity(newShip);
                    }
                    else if (j == typeof(EMedic))
                    {
                        var newShip = new EMedic(k.GetDestination(), player);
                        mEntity.mEntityManager.Add(newShip);
                        if (player is PlayerPlayer)
                        {
                            Globals.mStatistics["BuiltMedic"] += 1;
                            Globals.mRunStatistics["BuiltMedic"] += 1;
                            Globals.mBuilt["Medic"] = true;
                        }
                        player.AddEntity(newShip);
                    }
                    else if (j == typeof(EMoerser))
                    {
                        var newShip = new EMoerser(k.GetDestination(), player);
                        mEntity.mEntityManager.Add(newShip);
                        if (player is PlayerPlayer)
                        {
                            Globals.mStatistics["BuiltMoerser"] += 1;
                            Globals.mRunStatistics["BuiltMoerser"] += 1;
                            Globals.mBuilt["Moerser"] = true;
                        }
                        player.AddEntity(newShip);
                    }
                    k.Reset();
                }
            }
        }
    }
}
