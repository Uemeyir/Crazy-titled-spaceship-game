using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using sopra05_2223.Core.Entity.Ships;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace sopra05_2223.Core.Components;

internal sealed class CTransform : Component
{
    [JsonRequired]
    private Vector2 mVelocity; // direction of movement
    [JsonRequired]
    private Vector2 mForce; // change in direction of movement
    [JsonRequired]
    private readonly float mMaxForcePerMillis;    // limit of inside forces
    [JsonRequired]
    private float mMaxVelocityPerSecond; // limit of movement per calculation 
    [JsonRequired]
    private float mSquadVelocityPerMillis;   // desired velocity by squad

    [JsonRequired]
    private Vector2 mOutsideForce;  // change from outside sources
    [JsonRequired]
    private int mBounceCount;   // amount of bounces from outside forces
    private const float OutsideForceMultiplier = 2;

    private float mMaxVel;

    [JsonRequired]
    private Point mTarget;
    [JsonRequired]
    private Point mTransitoryTarget;
    [JsonRequired] 
    private float mDistanceToClick;
    [JsonRequired]
    private List<Point> mPath = new();

    [JsonRequired]
    private SteeringEnum mBehavior;

    private Vector2 mDistanceToTarget;

    private double[] mAngles;
    private int mAngleCounter;
    private double mLastUpdateTime;

    [JsonConstructor]
    internal CTransform(float maxVelocity, Point target)
    {
        mTarget = target;
        mMaxVelocityPerSecond = maxVelocity;
        mMaxVel = mMaxVelocityPerSecond;
        // Play with this value to get snappier response
        mMaxForcePerMillis = maxVelocity / 8;
        mSquadVelocityPerMillis = mMaxVelocityPerSecond;
        mOutsideForce = new Vector2(0, 0);
        mForce = new Vector2(0, 0);
        mVelocity = new Vector2(0, 0);
        mTransitoryTarget = new Point(int.MaxValue, int.MaxValue);

        mBehavior = SteeringEnum.Arrive;

        // removing jitter kind of
        mAngles = new double[16];
        for (var i = 0; i < mAngles.Length; i++)
        {
            mAngles[i] = 0;
        }
    }

    internal override void Update()
    {
        var updateMillis = Globals.GameTime.ElapsedGameTime.TotalMilliseconds;
        mLastUpdateTime = updateMillis;

        var steering = new Vector2(0, 0);
        // Steering behavior
        if (mBehavior == SteeringEnum.Arrive)
        {
            steering += Arrival(updateMillis);
        }
        else if (mBehavior == SteeringEnum.Seek)
        {
            steering += Seek(updateMillis);
        }
        else if (mBehavior == SteeringEnum.Follow)
        {
            steering += Follow(updateMillis);
        }

        if (mEntity is EShip)
        {
            steering += Avoid(updateMillis);
        }


        // calculate Steering force
        mForce = steering - mVelocity;

        if (mForce.Length() > mMaxForcePerMillis && mForce.Length() != 0)
        {
            mForce.Normalize();
            mForce *= (float)(mMaxForcePerMillis * updateMillis);
        }
        mForce += mOutsideForce * (mBounceCount * OutsideForceMultiplier);

        // calculate new velocity
        mVelocity += mForce;
        if (GetSpeedInDistancePerMillis() > mSquadVelocityPerMillis && mVelocity.Length() != 0)
        {
            mVelocity.Normalize();
            mVelocity *= (float)(mSquadVelocityPerMillis * updateMillis);
        }


        // change position
        if (0 < mEntity.GetX() + mVelocity.X && mEntity.GetX() + mEntity.GetWidth() + mVelocity.X < mEntity.mEntityManager.mWorldSize.X)
        {
            mEntity.SetX((int)(mEntity.GetX() + mVelocity.X));

        }

        if (0 < mEntity.GetY() + mVelocity.Y && mEntity.GetY() + mEntity.GetHeight() + mVelocity.Y < mEntity.mEntityManager.mWorldSize.Y)
        {
            mEntity.SetY((int)(mEntity.GetY() + mVelocity.Y));
        }


        // reset forces at the end of loop;
        mOutsideForce = Vector2.Zero;
        mBounceCount = 0;

        UpdateAngle();
    }

    private Vector2 Avoid(double updateMillis)
    {
        var margin = 400;
        var velo = new Vector2(0, 0);

        foreach (var other in mEntity.mCloseEntities)
        {
            if (other is EShip && other != mEntity)
            {
                var distance = Vector2.Distance(new Vector2(mEntity.GetX(), mEntity.GetY()),
                    new Vector2(other.GetX(), other.GetY()));
                if (distance < margin)
                {
                    const float multiplicator = 5f;
                    var dir = new Vector2(mEntity.GetX() - other.GetX(), mEntity.GetY() - other.GetY());
                    dir.Normalize();
                    velo += dir * multiplicator / distance;
                }
            }
        }

        if (velo.Length() > 0)
        {
            velo.Normalize();
            velo *= mSquadVelocityPerMillis * (float)updateMillis / 2;
        }
        return velo;
    }

    // this behavior is used to follow a set path. 
    private Vector2 Follow(double updateMillis)
    {
        var position = new Vector2(mEntity.GetX(), mEntity.GetY());
        mDistanceToTarget = mTarget.ToVector2() - position;
        if (mDistanceToTarget.Length() < 50 && mPath.Count > 0)
        {
            SetPath(mPath);
            return Seek(updateMillis);
        }
        if (mPath.Count > 0)
        {
            return Seek(updateMillis);
        }
        return Arrival(updateMillis);

    }

    // this behavior is used to try to reach a set point in the smallest time possible.
    // the entity will not break, but overshoot the target, as breaking is not important in reaching it fastest
    private Vector2 Seek(double updateMillis)
    {
        var position = new Vector2(mEntity.GetX(), mEntity.GetY());
        // calculate mDesiredVelocity
        var velocity = mTarget.ToVector2() - position;
        // if len = 0 -> infinty -> numbers are broken
        if (velocity.Length() != 0)
        {
            velocity.Normalize();
        }

        velocity *= (float)(mSquadVelocityPerMillis * updateMillis);
        return velocity;
    }

    // same as seek but the Entity will slow down and stop at the point
    private Vector2 Arrival(double updateSeconds)
    {
        var position = new Vector2(mEntity.GetX(), mEntity.GetY());
        const int slowingDistance = 100;
        // calculate mDesiredVelocity
        var targetOffset = mTarget.ToVector2() - position;
        var distance = targetOffset.Length();
        var rampedSpeed = (float)(mSquadVelocityPerMillis * updateSeconds) * (distance / slowingDistance);
        var clippedSpeed = Math.Min(rampedSpeed, (float)(mSquadVelocityPerMillis * updateSeconds));

        if (distance > 0)
        {
            return (clippedSpeed / distance) * targetOffset;
        }
        return new Vector2(0, 0);
    }


    // used to manipulate the Entity from outside.
    public void AddOutsideForce(Vector2 acceleration)
    {
        mBounceCount++;
        mOutsideForce += acceleration;
    }

    // sets the target which the entity is supposed to reach
    public void SetTarget(Point target)
    {
        mSquadVelocityPerMillis = mMaxVelocityPerSecond;
        
        if (mMaxVel < mMaxVelocityPerSecond)
        {
            mSquadVelocityPerMillis = mMaxVel;
        }
        else
        {
            mSquadVelocityPerMillis = mMaxVelocityPerSecond;
        }
        mTarget = target;
    }


    internal float GetMaxVelocity()
    {
        return mMaxVelocityPerSecond;
    }

    internal Point GetTarget()
    {
        return mTarget;
    }

    internal List<Point> GetPath()
    {
        return mPath;
    }

    internal void SetSquadVelocity(float minVelocity)
    {
        this.mSquadVelocityPerMillis = minVelocity;
        mMaxVel = minVelocity;
    }

    // sets path, also sets first point of path as current target
    internal void SetPath(List<Point> path)
    {
        this.mPath = path;
        if (mMaxVel < mMaxVelocityPerSecond)
        {
            mSquadVelocityPerMillis = mMaxVel;
        }
        else
        {
            mSquadVelocityPerMillis = mMaxVelocityPerSecond;
        }
        // Set first point of path as target
        if (mPath.Count > 0)
        {
            mBehavior = SteeringEnum.Follow;
            this.SetTarget(mPath[0]);
            this.mPath.RemoveAt(0);
        }

    }

    public float GetAngle()
    {
        if (mMaxVelocityPerSecond == 0)
        {
            return (float)Math.PI;
        }
        var avg = 0.0;
        for (var i = 0; i < mAngles.Length; i++)
        {
            avg += mAngles[i];
        }
        avg /= mAngles.Length;
        return (float) avg;
    }

    private void UpdateAngle()
    {
        var vecVertical = new Vector2(0, 1);
        vecVertical.Normalize();
        var vecShip = new Vector2(0, 1);
        if (mVelocity.Length() > 0)
        {
            vecShip = new Vector2(mVelocity.X, mVelocity.Y);
        }

        vecShip.Normalize();
        var angle = (float)Math.Acos((vecVertical.X * vecShip.X + vecVertical.Y * vecShip.Y));
        angle += (float)Math.PI;

        // add pi bc MATH acos goes from 0 to 180 deg. 
        if (mVelocity.X < 0)
        {
            angle = -angle;
        }
        mAngles[mAngleCounter] = angle;
        mAngleCounter++;
        if (mAngleCounter >= mAngles.Length)
        {
            mAngleCounter = 0;
        }
    }


    internal double GetSpeedInDistancePerMillis()
    {
        return mVelocity.Length() / mLastUpdateTime;
    }

    internal Vector2 GetDistanceToTarget()
    {
        return mDistanceToTarget;
    }

    internal void SetTransitoryTarget(Point target)
    {
        mTransitoryTarget = target;
    }

    internal Point GetTransitoryTarget()
    {
        return mTransitoryTarget;
    }

    internal void SetDistanceToClick(float dist)
    {
        mDistanceToClick = dist;
    }

    internal float GetDistanceToClick()
    {
        return mDistanceToClick;
    }

}



internal enum SteeringEnum
{
    Arrive,
    Seek,
    Follow
}