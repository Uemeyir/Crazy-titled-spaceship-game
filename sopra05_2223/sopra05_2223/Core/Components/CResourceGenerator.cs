using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace sopra05_2223.Core.Components;

internal sealed class CResourceGenerator : Component
{
    [JsonRequired]
    private readonly int mOxygenPerSec;
    [JsonRequired]
    private readonly int mMetalPerSec;
    [JsonRequired]
    private int mElapsed;

    internal CResourceGenerator(int oxygenPerSec, int metalPerSec)
    {
        mOxygenPerSec = oxygenPerSec;
        mMetalPerSec = metalPerSec;
    }

    [JsonConstructor]
    internal CResourceGenerator(int oxygenPerSec, int metalPerSec, double lastUpdateTimePoint)
    {
        mOxygenPerSec = oxygenPerSec;
        mMetalPerSec = metalPerSec;
    }

    internal Point GetResources()
    {
        return new Point(mMetalPerSec, mOxygenPerSec);
    }

    internal override void Update()
    {
        if (mEntity.GetComponent<CTeam>().mTeam == Team.Neutral)
        {
            return;
        }

        mElapsed += Globals.GameTime.ElapsedGameTime.Milliseconds;
        if (mElapsed >= 1000)
        {
            // Add Resources
            mElapsed -= 1000;
            var storage = mEntity.GetComponent<CStorage>();
            if (storage != null)
            {
                if (storage.GetOxygen() + mOxygenPerSec <= storage.GetMaxOxygen())
                {
                    storage.AddToStorage(true, mOxygenPerSec);
                    if (storage.mEntity.GetComponent<CView>() != null)
                    {
                        Globals.mRunStatistics["NaturalResources"] += mOxygenPerSec;
                        Globals.mStatistics["NaturalResources"] += mOxygenPerSec;
                    }
                }

                if (storage.GetMetal() + mMetalPerSec <= storage.GetMaxMetal())
                {
                    storage.AddToStorage(false, mMetalPerSec);
                    if (storage.mEntity.GetComponent<CView>() != null)
                    {
                        Globals.mRunStatistics["NaturalResources"] += mMetalPerSec;
                        Globals.mStatistics["NaturalResources"] += mMetalPerSec;
                    }
                    
                }
            }
        }
    }
}