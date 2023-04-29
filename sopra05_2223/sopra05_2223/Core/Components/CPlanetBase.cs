using sopra05_2223.Screens;
using System.Linq;

namespace sopra05_2223.Core.Components
{
    internal sealed class CPlanetBase : Component
    {
        internal override void Update()
        {
            var x = mEntity.GetComponent<CSelect>();
            if (x != null && x.mSelected && !Globals.ScreenManager.GetStack().OfType<PlanetBaseScreen>().Any())
            {
                Globals.ScreenManager.AddScreen(new PlanetBaseScreen(mEntity, Globals.Resolution.mScreenSize));
                x.mSelected = false;
            }
        }
    }
}
