using Newtonsoft.Json;

namespace sopra05_2223.Core.Components;

internal sealed class CSelect : Component
{
    [JsonRequired]
    internal bool mSelected = false;
    [JsonRequired]
    internal bool AllowGroupSelection
    {
        get;
    }

    [JsonConstructor]
    internal CSelect(bool allowGroupSelection)
    {
        AllowGroupSelection = allowGroupSelection;
    }

    internal override void Update()
    {
    }
}