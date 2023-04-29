using Newtonsoft.Json;

namespace sopra05_2223.Core.Components;

internal sealed class CView : Component
{
    [JsonRequired]
    internal readonly int mViewRadius;

    [JsonConstructor]
    internal CView(int viewRadius)
    {
        mViewRadius = viewRadius;
    }

    internal override void Update()
    {
    }
}