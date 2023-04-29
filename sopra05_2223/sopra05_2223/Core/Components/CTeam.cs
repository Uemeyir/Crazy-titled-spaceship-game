using Newtonsoft.Json;
using sopra05_2223.Core.Entity.Base;
using sopra05_2223.Core.Entity.Base.SpaceBase;
using sopra05_2223.Core.Entity.Projectile;
using sopra05_2223.Core.ShipBase;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Components
{
    internal enum Team
    {
        Player,
        Ki,
        Neutral
    }

    internal sealed class CTeam : Component
    {
        [JsonRequired]
        internal Team mTeam;
        [JsonIgnore]
        internal Player mProtagonist;

        internal CTeam(Player player)
        {
            mTeam = player switch
            {
                PlayerPlayer => Team.Player,
                PlayerKi => Team.Ki,
                _ => Team.Neutral
            };

            mProtagonist = player;

        }

        [JsonConstructor]
        internal CTeam(Team team)
        {
            mTeam = team;
        }

        public void ChangeTeam(Player player)
        {
            player.AddEntity(mEntity);

            if (mEntity is ESpaceBase)
            {
                mEntity.RemoveComponent(mEntity.GetComponent<CShipBase>());
                mEntity.AddComponent(new CShipBase());
                mEntity.GetComponent<BuildSlotManager>().UnBlockBuildSlotManager();
            }

            mProtagonist?.RemoveEntity(mEntity);

            switch (player)
            {
                case PlayerPlayer:
                    if (mEntity is not EProjectile)
                    {
                        mEntity.AddComponent(new CSelect(true));
                    }
                    mTeam = Team.Player;
                    break;
                case PlayerKi:
                    mEntity.RemoveComponent(mEntity.GetComponent<CSelect>());
                    mTeam = Team.Ki;
                    break;
            }
            mProtagonist = player;
            mEntity.ChangeTexture();
        }

        public void SetNeutral()
        {
            mTeam = Team.Neutral;
            mProtagonist?.RemoveEntity(mEntity);
            mProtagonist = null;
            if (mEntity is ESpaceBase)
            {
                mEntity.GetComponent<BuildSlotManager>().BlockBuildSlotManager();
                mEntity.RemoveComponent(mEntity.GetComponent<CShipBase>());
            }

            if (mEntity is EBase && mEntity.GetComponent<CSelect>() is null)
            {
                    mEntity.AddComponent(new CSelect(false));
            }

            mEntity.ChangeTexture();
        }

        public Team GetTeam()
        {
            return mTeam;
        }

        public bool IsNeutral()
        {
            return mTeam == Team.Neutral;
        }

        public Player GetProtagonist()
        {
            return mProtagonist;
        }

        internal override void Update()
        {
        }
    }
}
