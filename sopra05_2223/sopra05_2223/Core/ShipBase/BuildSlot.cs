
using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace sopra05_2223.Core.ShipBase
{
    internal sealed class BuildSlot
    {
        // not used for now
        // private float mStart;
        [JsonRequired]
        private Vector2 mDestination;
        [JsonRequired]
        private float mDuration;
        [JsonRequired]
        private float mElapsed;
        [JsonRequired]
        private bool mFinished;
        [JsonRequired]
        private bool mUsed;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private Type mBuiltShipType;

        internal BuildSlot()
        {
        }

        [JsonConstructor]
        internal BuildSlot(Vector2 destination, float duration, float elapsed, bool finished, bool used, Type builtShipType)
        {
            mDestination = destination;
            mDuration = duration;
            mElapsed = elapsed;
            mFinished = finished;
            mUsed = used;
            mBuiltShipType = builtShipType;
        }

        internal void Update()
        {
            if (mElapsed >= mDuration)
            {
                mFinished = true;
            }

            mElapsed += Globals.GameTime.ElapsedGameTime.Milliseconds * 0.001f;
        }

        internal void SetUsed()
        {
            mUsed = true;
        }
        internal void SetDestination(Vector2 dest)
        {
            mDestination = dest;
        }

        internal Vector2 GetDestination()
        {
            return mDestination;
        }
        internal bool IsUsed()
        {
            return mUsed;
        }
        internal void SetStart(float start)
        {
            // mStart = start;
        }
        internal void SetDuration(float duration)
        {
            mDuration = duration;
        }

        internal float GetTotalDuration()
        {
            return mDuration;
        }

        internal float GetElapsedTime()
        {
            return mElapsed;
        }

        internal bool IsFinished()
        {
            return mFinished;
        }

        internal void Reset()
        {
            //mStart = 0;
            mDuration = 0;
            mElapsed = 0;
            mUsed = false;
            mFinished = false;
            mBuiltShipType = null;
        }

        internal void SetType(Type type)
        {
            mBuiltShipType = type;
        }

        internal new Type GetType()
        {
            return mBuiltShipType;
        }
    }
}
