using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sopra05_2223.Core.Entity.Misc;
using sopra05_2223.Core.Entity.Ships;

namespace sopra05_2223.Core.Components;

// this class represents the lifespan of a given Entity. if the life is over, it will delete the entity from the entity manager
public sealed class CLifespan : Component
{
    [JsonRequired]
    private readonly int mLifespan;
    [JsonRequired]
    private bool mIsActive;

    private int mElapsed;

    private readonly ESpy mSpy;

    [JsonConstructor]
    internal CLifespan(int duration, ESpy spy = null)
    {
        mLifespan = duration;
        mIsActive = true;
        mSpy = spy;
    }

    internal override void Update()
    {
        mElapsed += Globals.GameTime.ElapsedGameTime.Milliseconds;
        if (mElapsed > mLifespan || !mIsActive)
        {
            if (mEntity is EBuoy)
            {
                mSpy.GetComponent<CBuoyPlacer>().mBuoyCount += 1;
            }
            mEntity.mEntityManager.Remove(mEntity);
            mEntity.GetComponent<CTeam>()?.mProtagonist.RemoveEntity(mEntity);
        }
    }

    internal float GetRemaining()
    {
        return mLifespan - mElapsed;
    }

    internal void Draw(SpriteBatch spriteBatch, Matrix transformMatrix)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, effect:Art.OxygenEffect, transformMatrix:transformMatrix);
        var left = GetRemaining() / mLifespan;
        Art.OxygenEffect.Parameters["OxygenPercent"].SetValue(left);
        spriteBatch.Draw(Art.Healthbar, new Rectangle((int)(mEntity.GetX() - mEntity.GetWidth() * 0.5f), mEntity.GetY() + mEntity.GetHeight(), mEntity.GetWidth() * 2, (int)(mEntity.GetHeight() * 0.05f)), new Rectangle(0,0,Art.Healthbar.Width, Art.Healthbar.Height), Color.White);
        spriteBatch.End();
        spriteBatch.Begin();

    }

    internal void End()
    {
        mIsActive = false;
    }
}