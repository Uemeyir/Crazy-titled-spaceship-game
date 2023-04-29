using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using sopra05_2223.Protagonists;

namespace sopra05_2223.Core.Components
{
    internal sealed class CTakeBase : Component
    {
        // Store a Reference to the Player's Protagonist for reliable use in ShowTakeBaseButton.
        [JsonIgnore]
        internal PlayerPlayer mPlayer;

        [JsonConstructor]
        internal CTakeBase(){}

        internal CTakeBase(Player player)
        {
            mPlayer = (PlayerPlayer)(player is PlayerPlayer ? player : player.GetOpponent());
        }

        internal override void Update()
        {
        }

        internal void ShowTakeBaseButton(SpriteBatch spriteBatch, Matrix translationMatrix)
        {
            if (mEntity.GetComponent<CTeam>().mTeam == Team.Neutral && (IsShipInActionRadius(mPlayer) && !IsShipInActionRadius(mPlayer.GetOpponent())) && mPlayer.GetOpponent().mPlanetBases.Count + mPlayer.GetOpponent().mSpaceBases.Count != 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, null, translationMatrix);
                spriteBatch.Draw(Art.TakeBaseButton, new Rectangle(mEntity.GetX(), mEntity.GetY() - 10, 600, 100), Color.White);
                spriteBatch.End();
                spriteBatch.Begin();
            }
        }

        internal bool IsShipInActionRadius(Player player)
        {
            var ship = player.GetClosestShip(mEntity);
            return ship != null 
                   && (int)Vector2.Distance(new Vector2(ship.GetX() + ship.GetWidth() / 2,
                        ship.GetY() + ship.GetHeight() / 2),
                    new Vector2(mEntity.GetX() + mEntity.GetWidth() / 2, mEntity.GetY() + mEntity.GetHeight() / 2)) <
                Globals.sActionRadius["Attacker"];
        }
    }
}
