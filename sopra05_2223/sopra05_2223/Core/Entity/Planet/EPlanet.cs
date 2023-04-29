using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity.Ships;
using sopra05_2223.Pathfinding;

namespace sopra05_2223.Core.Entity.Planet
{
    internal sealed class EPlanet : Entity
    {
        [JsonRequired]
        private readonly int mTextureNumber;

        internal EPlanet(Vector2 pos, int textureNumber, Grid grid) : base(pos, null)
        {
            // make sure we use a valid index
            mTexture = textureNumber >= 0 && textureNumber < Art.sPlanets.Length ? Art.sPlanets[textureNumber] : Art.sPlanets[0];
            mTextureNumber = textureNumber;
            mWidth = mTexture.Width;
            mHeight = mTexture.Height;
            grid.InsertPlanetIntoGrid(this);
        }

        [JsonConstructor]
        internal EPlanet(Vector2 pos, int textureNumber) : base(pos, null)
        {
            mTextureNumber = textureNumber;
            mTexture = mTextureNumber >= 0 && textureNumber < Art.sPlanets.Length ? Art.sPlanets[textureNumber] : Art.sPlanets[0];
            mWidth = mTexture.Width;
            mHeight = mTexture.Height;
        }

        public override void Collides(Entity other)
        {
            if (other == null || other == this)
            {
                return;
            }

            if (IsColliding(other) && other is EShip ship)
            {
                CollidesWithShip(ship);
            }
        }

        // No need for texture change, as this Entity will not belong to any Team.
        internal override void ChangeTexture()
        {
        }

        private void CollidesWithShip(EShip other)
        {
            // calculate direction
            var center = new Vector2(GetX() + GetWidth() / 2f, GetY() + GetHeight() / 2f);
            var otherCenter = new Vector2(other.GetX() + other.GetWidth() / 2f, other.GetY() + other.GetHeight() / 2f);
            var pushDir = center - otherCenter;

            pushDir.Normalize();
            other.GetComponent<CTransform>().AddOutsideForce(-pushDir);
        }
    }
}
