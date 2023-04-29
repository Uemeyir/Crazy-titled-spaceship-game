using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Protagonists;
using sopra05_2223.SoundManagement;

namespace sopra05_2223.Core.Components
{
    internal sealed class CMoveResource : Component
    {
        private bool mSearch;
        private readonly List<Entity.Entity> mInRange = new();
        private int mShown;
        private int mMoveOxygen;
        private int mMoveMetal;
        public string mUserInput = "";
        public bool mSelectedType;
        private static readonly float sYOffset = 0.15f;
        internal override void Update()
        {
            if (mSearch && mEntity.GetComponent<CTeam>().GetProtagonist() is PlayerPlayer)
            {
                GetInRange(2000);
            }
        }

        internal void ShowNext()
        {
            mInRange[mShown].GetComponent<CStorage>().SetSelected(false);
            if (mInRange.Count == 0)
            {
                return;
            }

            if (mShown == mInRange.Count - 1)
            {
                mShown = 0;
            }
            else
            {
                mShown += 1;
                mShown %= mInRange.Count;
            }
            mInRange[mShown].GetComponent<CStorage>().SetSelected(true);
        }

        internal void Clear()
        {
            foreach (var k in mInRange)
            {
                k.GetComponent<CStorage>()?.SetSelected(false);
            }
        }

        internal void SetMoveMetal(int move)
        {
            mMoveMetal = move;
        }

        internal void SetMoveOxygen(int move)
        {
            mMoveOxygen = move;
        }

        internal int GetMoveMetal()
        {
            return mMoveMetal;
        }
        internal int GetMoveOxygen()
        {
            return mMoveOxygen;
        }

        internal void Load()
        {
            var eSn = mInRange[mShown].GetComponent<CStorage>();
            var e = mEntity.GetComponent<CStorage>();

            if (mSelectedType)
            {
                if (eSn.GetOxygen() + mMoveOxygen <=
                    eSn.GetMaxOxygen() && e.GetOxygen() >= mMoveOxygen)
                {
                    eSn.AddToStorage(true, mMoveOxygen);
                    e.RemoveFromStorage(true, mMoveOxygen);
                    Globals.SoundManager.PlayPitchedSound(SoundEnum.Collect2, -5);
                }
                else
                {
                    var oxygenLeft = eSn.GetMaxOxygen() - eSn.GetOxygen();
                    if (e.GetOxygen() >= oxygenLeft)
                    {
                        eSn.AddToStorage(true, oxygenLeft);
                        e.RemoveFromStorage(true, oxygenLeft);
                        Globals.SoundManager.PlayPitchedSound(SoundEnum.Collect2, -5);
                    }
                }
            }
            else
            {
                if (eSn.GetMetal() + mMoveMetal <=
                    eSn.GetMaxMetal() &&
                    e.GetMetal() >= mMoveMetal)
                {
                    eSn.AddToStorage(false, mMoveMetal);
                    e.RemoveFromStorage(false, mMoveMetal);
                    Globals.SoundManager.PlayPitchedSound(SoundEnum.Collect2, -5);
                }
                else
                {
                    var metalLeft = eSn.GetMaxMetal() - eSn.GetMetal();
                    if (e.GetMetal() >= metalLeft)
                    {
                        eSn.AddToStorage(false, metalLeft);
                        e.RemoveFromStorage(false, metalLeft);
                        Globals.SoundManager.PlayPitchedSound(SoundEnum.Collect2, -5);
                    }
                }
            }


        }
        internal void Unload()
        {
            var eSn = mInRange[mShown].GetComponent<CStorage>();
            var e = mEntity.GetComponent<CStorage>();

            if (mSelectedType)
            {
                if (e.GetOxygen() + mMoveOxygen <= e.GetMaxOxygen() && eSn.GetOxygen() >= mMoveOxygen)
                {
                    e.AddToStorage(true, mMoveOxygen);
                    eSn.RemoveFromStorage(true, mMoveOxygen);
                    Globals.SoundManager.PlayPitchedSound(SoundEnum.Collect, -5);
                }
                else
                {
                    var oxygenLeft = mInRange[mShown].GetComponent<CStorage>().GetOxygen();
                    if (e.GetMaxOxygen() - e.GetOxygen() >= oxygenLeft)
                    {
                        e.AddToStorage(true, oxygenLeft);
                        eSn.RemoveFromStorage(true, oxygenLeft);
                        Globals.SoundManager.PlayPitchedSound(SoundEnum.Collect, -5);
                    }
                }
            }
            else
            {
                if (e.GetMetal() + mMoveMetal <= e.GetMaxMetal() && eSn.GetMetal() >= mMoveMetal)
                {
                    e.AddToStorage(false, mMoveMetal);
                    eSn.RemoveFromStorage(false, mMoveMetal);
                    Globals.SoundManager.PlayPitchedSound(SoundEnum.Collect, -5);
                }
                else
                {
                    var metalLeft = eSn.GetMetal();
                    if (e.GetMaxMetal() - e.GetMetal() >= metalLeft)
                    {
                        e.AddToStorage(false, metalLeft);
                        eSn.RemoveFromStorage(false, metalLeft);
                        Globals.SoundManager.PlayPitchedSound(SoundEnum.Collect, -5);
                    }
                }
            }
        }

        internal void ShowPrev()
        {
            mInRange[mShown].GetComponent<CStorage>().SetSelected(false);
            if (mInRange.Count == 0)
            {
                return;
            }

            if (mShown == 0)
            {
                mShown = mInRange.Count - 1;
            }
            else
            {
                mShown -= 1;
                mShown %= mInRange.Count;
            }
            mInRange[mShown].GetComponent<CStorage>().SetSelected(true);
        }

        internal void GetInRange(int range)
        {
            mInRange.Clear();
            var inRange = mEntity.mEntityManager.EntitiesInRadius(range, mEntity);
            foreach (var k in inRange)
            {
                if (k.GetComponent<CStorage>() != null)
                {
                    mInRange.Add(k);
                }
            }
            mShown = 0;
            mSearch = false;
            if (mInRange.Count > 0)
            {
                mInRange[0].GetComponent<CStorage>()?.SetSelected(true);
            }
        }

        internal List<Entity.Entity> GetEntitesInRange()
        {
            return mInRange;
        }

        public void Draw(SpriteBatch spriteBatch, Point screenSize)
        {
            if (mInRange.Count != 0)
            {
                spriteBatch.Draw(Art.ShipBaseBg, new Rectangle((int)(screenSize.X * 0.6f), (int)(screenSize.Y * (0.7f- sYOffset)), (int)(screenSize.X * 0.4f), (int)(screenSize.Y * 0.3f)), Color.Gray * 2f);
                spriteBatch.Draw(mInRange[mShown].mTexture, new Rectangle((int)(screenSize.X * 0.65f), (int)(screenSize.Y * (0.75f- sYOffset)), (int)(screenSize.X * 0.1f), (int)(screenSize.X * 0.1f)), Color.White);
                spriteBatch.Draw(Art.PrevStorage, new Rectangle((int)(screenSize.X * 0.62f), (int)(screenSize.Y * (0.75f- sYOffset)), (int)(screenSize.X * 0.02f), (int)(screenSize.X * 0.1f)), Color.White * 0.6f);
                spriteBatch.Draw(Art.PrevStorage, new Rectangle((int)(screenSize.X * 0.76f) + (int)(screenSize.X * 0.01f), (int)(screenSize.Y * (0.75f- sYOffset)) + (int)(screenSize.X * 0.05f), (int)(screenSize.X * 0.02f), (int)(screenSize.X * 0.1f)), null, Color.White * 0.6f, MathHelper.ToRadians(180), new Vector2(Art.PrevStorage.Width / 2f, Art.PrevStorage.Height / 2f), SpriteEffects.None, 0f);

                DrawResources(spriteBatch, screenSize);

                if (mSelectedType)
                {
                    spriteBatch.Draw(Art.Sauerstoffflasche, new Rectangle((int)(screenSize.X * 0.92f), (int)(screenSize.Y * (0.74f - sYOffset)), (int)(screenSize.Y * 0.05f), (int)(screenSize.Y * 0.05f)), Color.White);
                    spriteBatch.Draw(Art.Metall, new Rectangle((int)(screenSize.X * 0.92f), (int)(screenSize.Y * (0.79f- sYOffset)), (int)(screenSize.Y * 0.05f), (int)(screenSize.Y * 0.05f)), Color.Gray * 0.7f);
                }
                else
                {
                    spriteBatch.Draw(Art.Sauerstoffflasche, new Rectangle((int)(screenSize.X * 0.92f), (int)(screenSize.Y * (0.74f - sYOffset)), (int)(screenSize.Y * 0.05f), (int)(screenSize.Y * 0.05f)), Color.Gray * 0.7f);
                    spriteBatch.Draw(Art.Metall, new Rectangle((int)(screenSize.X * 0.92f), (int)(screenSize.Y * (0.79f- sYOffset)), (int)(screenSize.Y * 0.05f), (int)(screenSize.Y * 0.05f)), Color.White);
                }

                spriteBatch.Draw(Art.LoadButton, new Rectangle((int)(screenSize.X * 0.82f), (int)(screenSize.Y * (0.9f- sYOffset)), (int)(screenSize.X * 0.04f), (int)(screenSize.X * 0.04f)), Color.White);
                spriteBatch.Draw(Art.UnloadButton, new Rectangle((int)(screenSize.X * 0.88f), (int)(screenSize.Y * (0.9f - sYOffset)), (int)(screenSize.X * 0.04f), (int)(screenSize.X * 0.04f)), Color.White);

                spriteBatch.Draw(Art.MinusButton, new Rectangle((int)(screenSize.X * 0.62f), (int)(screenSize.Y * (0.95f- sYOffset)), (int)(screenSize.X * 0.025f), (int)(screenSize.X * 0.025f)), Color.White);
                spriteBatch.Draw(Art.PlusButton, new Rectangle((int)(screenSize.X * 0.76f), (int)(screenSize.Y * (0.95f- sYOffset)), (int)(screenSize.X * 0.025f), (int)(screenSize.X * 0.025f)), Color.White);

                if (mUserInput != "")
                {
                    spriteBatch.DrawString(Art.Arial22, mUserInput, new Vector2(screenSize.X * 0.67f, screenSize.Y * (0.95f- sYOffset)), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(Art.Arial22, "0", new Vector2(screenSize.X * 0.67f, screenSize.Y * (0.95f- sYOffset)), Color.White);
                }
            }
        }

        private void DrawResources(SpriteBatch spriteBatch, Point screenSize)
        {
            var x = mInRange[mShown].GetComponent<CStorage>();
            var oxygenPercent = x.GetOxygen() / (float)x.GetMaxOxygen();
            var metalPercent = x.GetMetal() / (float)x.GetMaxMetal();

            spriteBatch.Draw(Art.Healthbar, new Rectangle((int)(screenSize.X * 0.8f), (int)(screenSize.Y * (0.75f- sYOffset)), (int)(screenSize.X * 0.10f), (int)(screenSize.Y * 0.03f)), new Rectangle(0, 0, Art.Healthbar.Width, Art.Healthbar.Height), Color.White * 0.5f);
            spriteBatch.Draw(Art.Healthbar, new Rectangle((int)(screenSize.X * 0.8f), (int)(screenSize.Y * (0.80f- sYOffset)), (int)(screenSize.X * 0.10f), (int)(screenSize.Y * 0.03f)), new Rectangle(0, 0, Art.Healthbar.Width, Art.Healthbar.Height), Color.White * 0.5f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, Art.OxygenEffect);
            Art.OxygenEffect.Parameters["OxygenPercent"].SetValue(oxygenPercent);
            spriteBatch.Draw(Art.Healthbar, new Rectangle((int)(screenSize.X * 0.8f), (int)(screenSize.Y * (0.75f- sYOffset)), (int)(screenSize.X * 0.10f), (int)(screenSize.Y * 0.03f)), new Rectangle(0, 0, Art.Healthbar.Width, Art.Healthbar.Height), Color.White);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, Art.MetalEffect);
            Art.MetalEffect.Parameters["MetalPercent"].SetValue(metalPercent);
            spriteBatch.Draw(Art.Healthbar, new Rectangle((int)(screenSize.X * 0.8f), (int)(screenSize.Y * (0.80f- sYOffset)), (int)(screenSize.X * 0.10f), (int)(screenSize.Y * 0.03f)), new Rectangle(0, 0, Art.Healthbar.Width, Art.Healthbar.Height), Color.White);
            spriteBatch.End();
            spriteBatch.Begin();

            spriteBatch.DrawString(Art.Arial12, x.GetOxygen() + " / " + x.GetMaxOxygen(), new Vector2(screenSize.X * 0.81f, screenSize.Y * (0.75f- sYOffset)), Color.Black);
            spriteBatch.DrawString(Art.Arial12, x.GetMetal() + " / " + x.GetMaxMetal(), new Vector2(screenSize.X * 0.81f, screenSize.Y * (0.80f- sYOffset)), Color.Black);

        }
    }
}
