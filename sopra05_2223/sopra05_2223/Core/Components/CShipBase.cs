using System.Linq;
using sopra05_2223.Screens;

namespace sopra05_2223.Core.Components
{
    internal sealed class CShipBase : Component
    {
        internal override void Update()
        {
            var x = mEntity.GetComponent<CSelect>();
            if (x != null && x.mSelected && !Globals.ScreenManager.GetStack().OfType<ShipBaseScreen>().Any())
            {
                Globals.ScreenManager.AddScreen(new ShipBaseScreen(Globals.Resolution.mScreenSize, mEntity));
                x.mSelected = false;
            }
        }
    }
}
