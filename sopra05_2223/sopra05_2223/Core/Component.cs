using Newtonsoft.Json;

namespace sopra05_2223.Core;

public abstract class Component
{
    // Reference to the owning Entity of the Component, set in Entity.AddComponent(). 
    [JsonIgnore]
    internal Entity.Entity mEntity;

    internal abstract void Update();

    protected bool InCameraView()
    {
        return mEntity.mEntityManager.GetCameraRectangle().Intersects(mEntity.Rectangle);
    }
}