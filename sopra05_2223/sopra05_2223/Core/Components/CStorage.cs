using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using sopra05_2223.Core.Entity.Base.SpaceBase;
using sopra05_2223.SoundManagement;

namespace sopra05_2223.Core.Components
{
    internal sealed class CStorage : Component
    {
        // Specifies the different Sizes of Storages:
        // {Ship Small, Ship Medium, Ship Large, Base Small, Base Medium, Base Large}
        private static readonly int[] sStorageSizes = { 100, 500, 1000, 1000, 5000, 10000 };
        [JsonRequired]
        // Stores Maximum Amount of Oxygen
        private readonly int mMaxOxygen; // type = 1
        [JsonRequired]
        // Stores Maximum Amount of Metal
        private readonly int mMaxMetal; // type = 0
        [JsonRequired]
        // Stores current Amount of Oxygen
        internal int mStoredOxygen;
        [JsonRequired]
        // Stores current Amount of Metal
        internal int mStoredMetal;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        internal Entity.Entity mTarget;

        private bool mSelectedStorage;

        // Constructor
        // Type: Type of Storage to use: type 0 is Ship Storage, type 1 is Base Storage
        // Size: Size of Storage: size 0 is Small, size 1 is Medium, size 2 is Large

        internal CStorage(int type, int size)
        {

            mMaxMetal = sStorageSizes[type * 3 + size];
            mMaxOxygen = sStorageSizes[type * 3 + size];
        }

        [JsonConstructor]
        internal CStorage(int storedOxygen, int maxOxygen, int storedMetal, int maxMetal)
        {
            mStoredMetal = storedMetal;
            mMaxMetal = maxMetal;
            mStoredOxygen = storedOxygen;
            mMaxOxygen = maxOxygen;
        }

        // Constructor
        // Allows unbounded Storage Sizes using maxOxygen and maxMetal
        // Setting full to true generates a filled storage
        internal CStorage(int maxOxygen, int maxMetal, bool full)
        {
            mMaxOxygen = maxOxygen;
            mMaxMetal = maxMetal;
            if (full)
            {
                mStoredMetal = maxMetal;
                mStoredOxygen = maxOxygen;
            }
        }

        internal override void Update()
        {
            if (mTarget != null && mTarget.GetComponent<CTeam>().GetTeam() == Team.Player && Vector2.Distance(mEntity.GetPos(), mTarget.GetPos()) <= Globals.sActionRadius["Base"])
            {
                var targetStorage = mTarget.GetComponent<CStorage>();
                if (mTarget is ESpaceBase)
                {
                    // Unload Storage
                    // Bases have unlimited storage so we will not check for overflow here.
                    targetStorage.AddMetal(mStoredMetal);
                    targetStorage.AddOxygen(mStoredOxygen);
                    if(mStoredMetal > 0 || mStoredOxygen > 0 && InCameraView()){
                        Globals.SoundManager.PlayPitchedSound(SoundEnum.Collect2, -5);
                    }
                    mStoredMetal = 0;
                    mStoredOxygen = 0;
                }
                else // EPlanetBase
                {
                    // Load Storage
                    var metalDelta = mMaxMetal - mStoredMetal;
                    var transferredMetal = targetStorage.mStoredMetal > metalDelta ? metalDelta : targetStorage.mStoredMetal;
                    var oxygenDelta = mMaxOxygen - mStoredOxygen;
                    var transferredOxygen = targetStorage.mStoredOxygen > oxygenDelta ? oxygenDelta : targetStorage.mStoredOxygen;

                    AddMetal(transferredMetal);
                    AddOxygen(transferredOxygen);

                    targetStorage.mStoredMetal -= transferredMetal;
                    targetStorage.mStoredOxygen -= transferredOxygen;
                    if (transferredMetal > 0 && transferredOxygen > 0 && InCameraView())
                    {
                        Globals.SoundManager.PlayPitchedSound(SoundEnum.Collect, -5);
                    }
                }

                // reset target
                mTarget = null;
            }
        }

        internal void SetSelected(bool selected)
        {
            mSelectedStorage = selected;
        }

        internal bool GetSelected()
        {
            return mSelectedStorage;
        }

        // Adds to Storage, returns the actual amount added to Storage
        // type: Type of Resource to be added: type 0 is Metal, type 1 is Oxygen
        // amount: Amount of Resources to be added

        // => return value will be used soon
        // => internal property too, cant make private
        internal void AddToStorage(bool type, int amount)
        {
            if (amount < 0)
            {
                return;
            }
            if (type)
            {
                AddOxygen(amount);
                mEntity.GetComponent<CTeam>().GetProtagonist()?.AddOxygen(amount);
            }
            else
            {
                AddMetal(amount);
                mEntity.GetComponent<CTeam>().GetProtagonist()?.AddIron(amount);
            }
        }

        // Adds Oxygen to Storage, returns the actual amount added to Storage
        // amount: Amount of Resources to be added
        private void AddOxygen(int amount)
        {
            if (mStoredOxygen + amount <= mMaxOxygen)
            {
                mStoredOxygen += amount;
            }
        }

        // Adds Metal to Storage, returns the actual amount added to Storage
        // amount: Amount of Resources to be added
        private void AddMetal(int amount)
        {
            if (mStoredMetal + amount <= mMaxMetal)
            {
                mStoredMetal += amount;
            }
        }

        // Removes from Storage, returns the actual amount removed from Storage
        // type: Type of Resource to be removed: type 0 is Metal, type 1 is Oxygen
        // amount: Amount of Resources to be removed

        // return value will be used soon
        internal void RemoveFromStorage(bool type, int amount)
        {
            if (amount < 0)
            {
                return;
            }
            if (type)
            {
                RemoveOxygen(amount);
            }
            else
            {
                RemoveMetal(amount);
            }
        }

        // Removes Oxygen from Storage, returns the actual amount removed from Storage
        // amount: Amount of Resources to be removed
        private void RemoveOxygen(int amount)
        {
            if (mStoredOxygen - amount >= 0)
            {
                mStoredOxygen -= amount;
            }
        }

        // Removes Metal from Storage, returns the actual amount removed from Storage
        // amount: Amount of Resources to be removed
        private void RemoveMetal(int amount)
        {
            if (mStoredMetal - amount >= 0)
            {
                mStoredMetal -= amount;
            }
        }

        internal int GetMetal()
        {
            return mStoredMetal;
        }

        internal int GetOxygen()
        {
            return mStoredOxygen;
        }
        internal int GetMaxMetal()
        {
            return mMaxMetal;
        }

        internal int GetMaxOxygen()
        {
            return mMaxOxygen;
        }

        internal void RemoveTarget()
        {
            mTarget = null;
        }
    }
}


