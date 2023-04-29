using Microsoft.Xna.Framework;
using sopra05_2223.Core.Components;
using sopra05_2223.Core.Entity.Resource;
using sopra05_2223.Protagonists;
using sopra05_2223.SoundManagement;

namespace sopra05_2223.Core.Entity.Ships;


// abstraction of any ship like entity. 
// has standard behavior for collision
internal abstract class EShip : Entity
{

    protected EShip(Vector2 pos, Player owner) : base(pos, owner)
    {
    }

    public override void Collides(Entity other)
    {
        if (other == null || other == this)
        {
            return;
        }

        if (IsColliding(other))
        {
            switch (other)
            {
                case EShip:
                    CollidesWithShip(other);
                    break;
                case EResource:
                    CollidesWithResource(other);
                    break;
            }
        }
    }

    private void CollidesWithShip(Entity other)
    {
        // calculate direction
        var center = new Vector2(GetX() + GetWidth() / 2f, GetY() + GetHeight() / 2f);
        var otherCenter = new Vector2(other.GetX() + other.GetWidth() / 2f, other.GetY() + other.GetHeight() / 2f);
        var pushDir = center - otherCenter;

        pushDir.Normalize();

        other.GetComponent<CTransform>().AddOutsideForce(-pushDir);
    }

    private void CollidesWithResource(Entity other)
    {
        // Ship requires a storage to pickup the resource.
        var storage = GetComponent<CStorage>();
        if (storage != null)
        {
            var compResource = other.GetComponent<CResource>();
            var metalStorageLeft = storage.GetMaxMetal() - storage.GetMetal();
            var oxygenStorageLeft = storage.GetMaxOxygen() - storage.GetOxygen();

            var addedOxygen = (int)compResource.Deplete(metalStorageLeft, false);
            var addedMetal = (int)compResource.Deplete(oxygenStorageLeft, true);

            storage.AddToStorage(false, addedOxygen);
            storage.AddToStorage(true, addedMetal);

            if ((addedOxygen > 0 || addedMetal > 0) && InCameraView())
            {
                var rand = Globals.RandomNumber() % 2 == 0;
                var pan = (GetPos().X - Globals.Camera.Rectangle.X) / (Globals.Camera.Rectangle.Width * 1f) * 2 - 1;
                if (rand)
                {
                    Globals.SoundManager.PlaySoundAt(SoundEnum.Collect, pan);
                }
                else
                {
                    Globals.SoundManager.PlaySoundAt(SoundEnum.Collect2, pan);
                }
            }

            if (compResource.GetMetal() == 0 && compResource.GetOxygen() == 0)
            {
                mEntityManager.Remove(other);
            }

            Globals.mStatistics["TotalResources"] += addedOxygen + addedMetal;
            Globals.mStatistics["ScrapResources"] += addedOxygen + addedMetal;
        } else if (this is ECollector && InCameraView())
        {

            var rand = Globals.RandomNumber() % 2 == 0;
            var pan = (GetPos().X - Globals.Camera.Rectangle.X) / (Globals.Camera.Rectangle.Width * 1f) * 2 - 1;
            if (rand)
            {
                Globals.SoundManager.PlaySoundAt(SoundEnum.Collect, pan);
            }
            else
            {
                Globals.SoundManager.PlaySoundAt(SoundEnum.Collect2, pan);
            }
        }
    }
}
