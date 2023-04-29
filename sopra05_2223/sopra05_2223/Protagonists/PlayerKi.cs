using sopra05_2223.Core.Entity;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using sopra05_2223.Core;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity.Base;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.Core.ShipBase;
using sopra05_2223.Core.Entity.Base.SpaceBase;
using System.Linq;

namespace sopra05_2223.Protagonists
{
    internal sealed class PlayerKi : Player
    {
        [JsonRequired]
        private int mThreatLevel;
        // how much it will grow after mThreatTimer in minutes have gone by
        private const int ThreatSpeed = 10;
        // how long it will take to increase mThreatLevel in minutes
        private const double ThreatTimer = 1;
        private double mThreatUpdateTimer;

        private bool mPrepared;

        [JsonRequired]
        private int mAttackerQueue;
        [JsonRequired]
        private int mMoerserQueue;
        [JsonRequired]
        private int mMedicQueue;

        // used to limit the amount of ressources the ai can have at one time
        private int mMaxMetal;
        private int mMaxOxygen;
        private bool mPaused;

        private HashSet<Entity> mAttackShips = new();

        public PlayerKi(int oxygen, int iron)
        {
            OxygenReserve = oxygen;
            IronReserve = iron;

            mThreatUpdateTimer = 0;

            mThreatLevel = 0;
        }

        [JsonConstructor]
        internal PlayerKi(int threatLevel, int moerserQueue,int medicQueue, int atackerQueue)
        {
            mThreatLevel = threatLevel;
            mMoerserQueue = moerserQueue;
            mMedicQueue = medicQueue;
            mAttackerQueue = atackerQueue;
        }

        public override void Update()
        {
            if (mPaused)
            {
                return;
            }
            base.Update();
            UpdateTimer();
            PullResources();
            TakeBases();
            QueueNotNegative();

            Action();
        }

        private void UpdateTimer()
        {
            mThreatUpdateTimer += Globals.GameTime.ElapsedGameTime.TotalMinutes;

            // time has elapsed, time to make ai more ready to fight
            if (ThreatTimer <= mThreatUpdateTimer)
            {
                mThreatUpdateTimer -= ThreatTimer;
                mThreatLevel += ThreatSpeed;
            }
        }

        private void Action()
        {
            // check if there are bases in need of defending
            var defenseTarget = CheckDefense();
            if (defenseTarget != null)
            {
                var closestShipBase = GetClosestSpaceBaseForAi(defenseTarget);
                ProduceReinforcement(mThreatLevel, closestShipBase);
                Defend(defenseTarget);
            }

            if (mThreatLevel >= 10 && !mPrepared)
            {
                // Prepare for attack if threat is high enough
                Prepare();
            }
            else if (mPrepared)
            {
                // Attack
                var target = ChooseTarget();
                if (target != null)
                {
                    Attack(target);
                }
            }
        }

        private Entity CheckDefense()
        {
            if (mSpaceBases.Count > 0 && !mSpaceBases[0].mEntityManager.mIsMinimapUpdateFrame)
            {
                return null;
            } 
            if (mPlanetBases.Count > 0 && !mPlanetBases[0].mEntityManager.mIsMinimapUpdateFrame)
            {
                return null;
            }
            var threshold = 1200;
            Entity defend = null;

            // check if there are enemy ships in proximity to Planetbases
            foreach (var pBase in mPlanetBases)
            {
                var closest = mOpponent.GetClosestEntity(pBase);
                // there is no entity owned by the other player
                if (closest == null)
                {
                    break;
                }
                var dist = new Vector2(closest.GetX() - pBase.GetX(), closest.GetY() - pBase.GetY());
                if (dist.Length() < threshold)
                {
                    var closeDefenders = pBase.mCloseEntities.Where(c => c.GetComponent<CTeam>().mTeam == Team.Ki);
                    var closeAttackers = pBase.mCloseEntities.Where(c => c.GetComponent<CTeam>().mTeam != Team.Ki);
                    if (closeDefenders.Count() < closeAttackers.Count())
                    {
                        defend = pBase;
                        break;
                    }
                    
                }
            }

            // check if there are enemy ships in proximity to Spacebases. Spacebases get higher prio. there is only one defense target atm.
            foreach (var pBase in mSpaceBases)
            {
                var closest = mOpponent.GetClosestEntity(pBase);
                // there is no entity owned by the other player
                if (closest == null)
                {
                    break;
                }
                var dist = new Vector2(closest.GetX() - pBase.GetX(), closest.GetY() - pBase.GetY());
                if (dist.Length() < threshold)
                {
                    var closeDefenders = pBase.mCloseEntities.Where(c => c.GetComponent<CTeam>().mTeam == Team.Ki);
                    var closeAttackers = pBase.mCloseEntities.Where(c => c.GetComponent<CTeam>().mTeam != Team.Ki);
                    if (closeDefenders.Count() < closeAttackers.Count())
                    {
                        defend = pBase;
                        break;
                    }
                }
            }

            return defend;
        }

        private Entity ChooseTarget()
        {
            Entity targetingEntity;
            var rand = (ushort) Globals.RandomNumber();
            if (mSpaceBases.Count > 0)
            {
                targetingEntity = mSpaceBases[rand % mSpaceBases.Count];
            } else if (mPlanetBases.Count > 0)
            {
                targetingEntity = mPlanetBases[rand % mPlanetBases.Count];
            }
            else
            {
                return null;
            }
            var spaceTarget = mOpponent.GetClosestSpaceBase(targetingEntity);
            var planetTarget = mOpponent.GetClosestPlanetBase(targetingEntity);
            var shipTarget = mOpponent.GetClosestShip(targetingEntity);

            var distanceSpace = (float) mEm.mWorldSize.Y * mEm.mWorldSize.X;
            var distancePlanet = distanceSpace;


            if (planetTarget != null)
            {
                distancePlanet = Vector2.Distance(new Vector2(targetingEntity.GetX(), targetingEntity.GetY()),
                    new Vector2(planetTarget.GetX(), planetTarget.GetY()));
            } else if (spaceTarget != null)
            {
                distanceSpace = Vector2.Distance(new Vector2(targetingEntity.GetX(), targetingEntity.GetY()),
                    new Vector2(spaceTarget.GetX(), spaceTarget.GetY()));
            }
            
            if (distanceSpace + 2000 < distancePlanet)
            {
                return spaceTarget;
            }
            if (distancePlanet < (float)mEm.mWorldSize.Y * mEm.mWorldSize.X)
            {
                return planetTarget;
            }
            return shipTarget;
        }

        private void Defend(Entity target)
        {
            if (target.GetComponent<CTeam>().GetProtagonist() is PlayerKi
                    && target is ESpaceBase && target.GetComponent<BuildSlotManager>() != null
                                   && target.GetComponent<BuildSlotManager>()
                                       .AddTask(Globals.sBuildTime[typeof(EAttacker)], typeof(EAttacker)) &&
                                         target.GetComponent<BuildSlotManager>().AddTask(Globals.sBuildTime[typeof(EMoerser)],
                                             typeof(EMoerser)))
            {
                target.GetComponent<CStorage>()?.RemoveFromStorage(false, Globals.sCosts[typeof(EAttacker)].X);
                target.GetComponent<CStorage>()?.RemoveFromStorage(false, Globals.sCosts[typeof(EMoerser)].X);
            }
        }

        private void ProduceReinforcement(int threatLevel, Entity e)
        {
            if (e == null)
            {
                return;
            }
            if (threatLevel is >= 10 and < 50)
            {
                ProduceAttackerInSpecificBase(1, e);
                ProduceMoerserInSpecificBase(1, e);
            }
            else if (threatLevel is >= 50 and < 100)
            {
                ProduceAttackerInSpecificBase(2, e);
                ProduceMoerserInSpecificBase(2, e);
            }
            else if (threatLevel is > 100)
            {
                ProduceAttackerInSpecificBase(3, e);
                ProduceMoerserInSpecificBase(3, e);
            }
            
        }

        private Point GetClosestTarget(Entity attacker, List<Point> targets)
        {
            var coordinatesAttacker = new Vector2(attacker.GetX() + attacker.GetWidth() / 2,
                attacker.GetY() + attacker.GetHeight() / 2);
            var closest = targets[0];
            var dist = Vector2.Distance(coordinatesAttacker, closest.ToVector2());
            
            foreach (var pnt in targets)
            {
                if (Vector2.Distance(coordinatesAttacker, pnt.ToVector2()) <  dist)
                {
                    closest = pnt;
                    dist = Vector2.Distance(coordinatesAttacker, pnt.ToVector2());
                }
            }

            return closest;
        }

        private void Attack(Entity targetEntity)
        {
            if (mShips.Count > 0 && targetEntity != null && StrongEnough(CalculateAttackPower(mShips) / 2, mThreatLevel))
            {
                CreateAttackSquad();
                MoveAttackSquad(targetEntity);
            }
            else
            {
                mPrepared = false;
            }
        }

        private void MoveAttackSquad(Entity targetEntity)
        {
            var target = new Point(targetEntity.GetX(), targetEntity.GetY());
            // target upper left corner of target entity
            var target1 = target;

            // target upper right corner of target entity
            var target2 = target + new Point(targetEntity.GetWidth(), 0);

            // target lower left corner of target entity
            var target3 = target + new Point(0, targetEntity.GetHeight());

            // target lower right corner of target entity
            var target4 = target + new Point(targetEntity.GetWidth(), targetEntity.GetHeight());

            foreach (var attacker in mAttackShips)
            {
                var targets = new List<Point>();

                if (Globals.mGridStandard.TestIfMouseClickInObstacle(target1.X + attacker.GetWidth() / 2 + 100,
                        target1.Y - attacker.GetHeight() / 2 - 150) == false)
                {
                    targets.Add(target1);
                }
                if (Globals.mGridStandard.TestIfMouseClickInObstacle(target2.X + attacker.GetWidth() / 2 + 150,
                        target2.Y + attacker.GetHeight() / 2 + 100) == false)
                {
                    targets.Add(target2);
                }
                if (Globals.mGridStandard.TestIfMouseClickInObstacle(target3.X - attacker.GetWidth() / 2 - 150,
                        target3.Y - attacker.GetHeight() / 2 + 100) == false)
                {
                    targets.Add(target3);
                }
                if (Globals.mGridStandard.TestIfMouseClickInObstacle(target4.X - attacker.GetWidth() / 2 - 100,
                        target4.Y + attacker.GetHeight() + 150) == false)
                {
                    targets.Add(target4);
                }

                if (targets.Count == 0)
                {
                    continue;
                }

                var closestTarget = GetClosestTarget(attacker, targets);
                var targetToMoveTo = new Point(0, 0);

                if (closestTarget == target1)
                {
                    targetToMoveTo = closestTarget + new Point(attacker.GetWidth() / 2 + 100,
                        -attacker.GetHeight() / 2 - 150);
                    target1 += new Point(attacker.GetWidth() + 20, 0);
                }
                if (closestTarget == target2)
                {
                    targetToMoveTo = closestTarget + new Point(attacker.GetWidth() / 2 + 150,
                        attacker.GetHeight() / 2 + 100);
                    target2 += new Point(0, attacker.GetHeight() + 20);
                }
                if (closestTarget == target3)
                {
                    targetToMoveTo = closestTarget + new Point(-attacker.GetWidth() / 2 - 150,
                        -attacker.GetHeight() / 2 + 100);
                    target3 += new Point(0, -attacker.GetHeight() + 20);
                }

                if (closestTarget == target4)
                {
                    targetToMoveTo = closestTarget + new Point(-attacker.GetWidth() / 2 - 150,
                        -attacker.GetHeight() / 2 + 100);
                    target4 += new Point(-attacker.GetWidth() - 20, 0);
                }

                if (mEm is not null)
                {
                    Globals.mMoveEntities.SetTargetForKi(mEm, attacker, targetToMoveTo);
                }
            }
        }

        private void CreateAttackSquad()
        {
            // reversing to get access to last added ships
            mShips.Reverse();
            foreach (var attacker in mShips)
            {
                if (StrongEnough(CalculateAttackPower(mAttackShips.ToArray().ToList()), mThreatLevel))
                {
                    break;
                }

                if (attacker is EAttacker)
                {
                    mAttackShips.Add(attacker);

                }
                else if (attacker is EMedic)
                {
                    mAttackShips.Add(attacker);
                }
                else if (attacker is EMoerser)
                {
                    mAttackShips.Add(attacker);
                }
            }
            mShips.Reverse();
        }

        private void Prepare()
        {
            // if ai is strong enough already, do nothing, else produce attackers
            if (StrongEnough(CalculateAttackPower(mAttackShips.ToArray().ToList()), mThreatLevel)  || StrongEnough(CalculateAttackPower(mShips) / 2, mThreatLevel))
            {
                // gonna be set to unprepared after attack
                mPrepared = true;
                return;
            }


            if (!(mMedicQueue == 0 && mAttackerQueue == 0 && mMoerserQueue == 0))
            {
                return;
            }

            var amountAttacker = mThreatLevel / 10;
            var amountMoerser = mThreatLevel / 20;
            var amountMedic = mThreatLevel / 10;

            var neededMetal = amountMedic * Globals.sCosts[typeof(EMedic)].X + amountAttacker + Globals.sCosts[typeof(EAttacker)].X +
                                  amountMoerser * Globals.sCosts[typeof(EMoerser)].X;
            var neededOxygen = amountMedic * Globals.sCosts[typeof(EMedic)].Y + amountAttacker + Globals.sCosts[typeof(EAttacker)].Y +
                              amountMoerser * Globals.sCosts[typeof(EMoerser)].Y;

            if (neededMetal < IronReserve && neededOxygen < OxygenReserve)
            {
                ProduceAttacker(amountAttacker - mAttackerQueue);
                ProduceMoerser(amountMoerser - mMoerserQueue);
                ProduceMedic(amountMedic - mMedicQueue);
            }
        }

        private void ProduceAttacker(int limit)
        {
            var count = 0;

            while (IronReserve > Globals.sCosts[typeof(EAttacker)].X &&
                   OxygenReserve > Globals.sCosts[typeof(EAttacker)].Y && count < limit)
            {
                var rand = (ushort)Globals.RandomNumber();
                var pBase = mSpaceBases[rand % mSpaceBases.Count];

                if (pBase.GetComponent<BuildSlotManager>() != null && pBase.GetComponent<BuildSlotManager>()
                        .AddTask(Globals.sBuildTime[typeof(EAttacker)], typeof(EAttacker)))
                {
                    IronReserve -= Globals.sCosts[typeof(EAttacker)].X;
                    OxygenReserve -= Globals.sCosts[typeof(EAttacker)].Y;
                    count++;
                }
                else
                {
                    break;
                }
            }
            mAttackerQueue += count;
        }

        private void ProduceAttackerInSpecificBase(int limit, Entity e)
        {
            while (limit >= 0 && IronReserve > Globals.sCosts[typeof(EAttacker)].X && OxygenReserve > Globals.sCosts[typeof(EAttacker)].Y)
            {
                if (e.GetComponent<BuildSlotManager>() != null && e.GetComponent<BuildSlotManager>()
                        .AddTask(Globals.sBuildTime[typeof(EAttacker)], typeof(EAttacker)))
                {
                    IronReserve -= Globals.sCosts[typeof(EAttacker)].X;
                    OxygenReserve -= Globals.sCosts[typeof(EAttacker)].Y;
                }
                limit--;
            }
        }

        private void ProduceMedic(int limit)
        {
            var count = 0;

            while (IronReserve > Globals.sCosts[typeof(EMedic)].X &&
                   OxygenReserve > Globals.sCosts[typeof(EMedic)].Y && count < limit)
            {
                var rand = (ushort)Globals.RandomNumber();
                var pBase = mSpaceBases[rand % mSpaceBases.Count];

                if (pBase.GetComponent<BuildSlotManager>() != null && pBase.GetComponent<BuildSlotManager>()
                        .AddTask(Globals.sBuildTime[typeof(EMedic)], typeof(EMedic)))
                {
                    IronReserve -= Globals.sCosts[typeof(EMedic)].X;
                    OxygenReserve -= Globals.sCosts[typeof(EMedic)].Y;
                    count++;
                }
                else
                {
                    break;
                }
            }
            mMedicQueue += count;
        }

        private void ProduceMoerser(int limit)
        {
            var count = 0;

            while (IronReserve > Globals.sCosts[typeof(EMoerser)].X &&
                   OxygenReserve > Globals.sCosts[typeof(EMoerser)].Y && count < limit)
            {
                var rand = (ushort)Globals.RandomNumber();
                var pBase = mSpaceBases[rand % mSpaceBases.Count];

                if (pBase.GetComponent<BuildSlotManager>() != null && pBase.GetComponent<BuildSlotManager>()
                        .AddTask(Globals.sBuildTime[typeof(EMoerser)], typeof(EMoerser)))
                {
                    IronReserve -= Globals.sCosts[typeof(EMoerser)].X;
                    OxygenReserve -= Globals.sCosts[typeof(EMoerser)].Y;
                    count++;
                }
                else
                {
                    break;
                }
            }
            mMoerserQueue += count;
        }

        private void ProduceMoerserInSpecificBase(int limit, Entity e)
        {
            while (limit >= 0 && IronReserve > Globals.sCosts[typeof(EMoerser)].X && OxygenReserve > Globals.sCosts[typeof(EMoerser)].Y)
            {
                if (e.GetComponent<BuildSlotManager>() != null && e.GetComponent<BuildSlotManager>()
                        .AddTask(Globals.sBuildTime[typeof(EMoerser)], typeof(EMoerser)))
                {
                    IronReserve -= Globals.sCosts[typeof(EMoerser)].X;
                    OxygenReserve -= Globals.sCosts[typeof(EMoerser)].Y;
                }
                limit--;
            }
        }

        private bool StrongEnough(int attackPower, int threatLevel)
        {
            // change this to a better function to change attacking behavior
            return threatLevel / 2 <= attackPower;
        }

        private int CalculateAttackPower(List<Entity> ships)
        {
            var power = 0.0;
            var multiplier = 1.0;
            foreach (var ship in ships)
            {
                if (ship is EAttacker)
                {
                    power += 5;
                }
                else if (ship is EMedic)
                {
                    multiplier += 0.25;
                } else if (ship is EMoerser)
                {
                    power += 10;
                }
            }
            return (int) (power * multiplier);
        }

        public override void RemoveEntity(Entity e)
        {
            switch (e)
            {
                case EShip:
                    mThreatLevel += 1;
                    mAttackShips.Remove(e);
                    break;
                case EBase:
                    mThreatLevel += 20;
                    if (e.GetComponent<CStorage>() != null)
                    {
                        // reduce ai resource capacity
                        mMaxMetal -= e.GetComponent<CStorage>().GetMaxMetal();
                        mMaxOxygen -= e.GetComponent<CStorage>().GetMaxOxygen();

                        // calculate and distribute resources to base.
                        // Metal
                        var amount = (int) (IronReserve * 1f / (mPlanetBases.Count + mSpaceBases.Count));
                        e.GetComponent<CStorage>().AddToStorage(false, amount);
                        IronReserve -= amount;
                        // Oxygen
                        amount = (int) (OxygenReserve * 1f / (mPlanetBases.Count + mSpaceBases.Count));
                        e.GetComponent<CStorage>().AddToStorage(true, amount);
                        OxygenReserve -= amount;
                    }
                    break;
            }
            base.RemoveEntity(e);
        }

        public override void AddEntity(Entity e)
        {
            base.AddEntity(e);
            if (e is EBase && e.GetComponent<CStorage>() != null) 
            {
                var metal = e.GetComponent<CStorage>().GetMetal();
                var oxygen = e.GetComponent<CStorage>().GetOxygen();
                e.GetComponent<CStorage>().RemoveFromStorage(false, metal);
                e.GetComponent<CStorage>().RemoveFromStorage(true, oxygen);

                AddIron(metal);
                AddOxygen(oxygen);

                mMaxMetal += e.GetComponent<CStorage>().GetMaxMetal();
                mMaxOxygen += e.GetComponent<CStorage>().GetMaxOxygen();
            } else if (e is EAttacker)
            {
                mAttackerQueue -= 1;
            } else if (e is EMedic)
            {
                mMedicQueue -= 1;
            } else if (e is EMoerser)
            {
                mMoerserQueue -= 1;
            }
        }

        // Pulls Generated Resources from PlanetBases to make ai able to "beam" resources
        private void PullResources()
        {
            foreach (var e in mPlanetBases)
            {
                var metal = e.GetComponent<CStorage>().GetMetal();
                var oxygen = e.GetComponent<CStorage>().GetOxygen();
                e.GetComponent<CStorage>().RemoveFromStorage(false, metal);
                e.GetComponent<CStorage>().RemoveFromStorage(true, oxygen);

                AddIron(metal);
                AddOxygen(oxygen);
            }
        }

        // add metal with limit to amount
        public override void AddIron(int toIncrease)
        {
            if (toIncrease < 0)
            {
                return;
            }
            if (toIncrease + IronReserve > mMaxMetal)
            {
                IronReserve = mMaxMetal;
            }
            else
            {
                IronReserve += toIncrease;
            }
        }

        // add oxygen with limit to amount
        public override void AddOxygen(int toIncrease)
        {
            if (toIncrease < 0)
            {
                return;
            }
            if (toIncrease + OxygenReserve > mMaxOxygen)
            {
                OxygenReserve = mMaxOxygen;
            }
            else
            {
                OxygenReserve += toIncrease;
            }
        }

        // takes all bases which are takeable by ai
        private void TakeBases()
        {
            EntityManager emanager = null;
            if (mShips.Count != 0)
            {
                emanager = mShips[0].mEntityManager;
            }

            if (emanager != null)
            {
                foreach (var e in emanager.Entities)
                {

                    var cTakeBase = e.GetComponent<CTakeBase>();
                    if (cTakeBase != null && e.GetComponent<CTeam>().mTeam is Team.Neutral
                                          && (cTakeBase.IsShipInActionRadius(this) && !cTakeBase.IsShipInActionRadius(this.GetOpponent())))
                    {
                            // Basis einnehmen.
                            e.GetComponent<CTeam>().ChangeTeam(this);
                            e.GetComponent<CHealth>()?.ChangeHealth(50);
                            e.RemoveComponent(e.GetComponent<CView>());
                    }
                }
            }

        }

        public void ResetQueues()
        {
            mAttackerQueue = 0;
            mMedicQueue = 0;
            mMoerserQueue = 0;
        }

        private void QueueNotNegative()
        {
            if (mMoerserQueue < 0)
            {
                mMoerserQueue = 0;
            }
            if (mMedicQueue < 0)
            {
                mMedicQueue = 0;
            }
            if (mAttackerQueue < 0)
            {
                mAttackerQueue = 0;
            }
        }

        internal void TooglePause()
        {
            mPaused = !mPaused;
        }
    }
}
