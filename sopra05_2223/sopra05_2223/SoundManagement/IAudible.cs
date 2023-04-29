namespace sopra05_2223.SoundManagement;

public interface IAudible
{
    internal SoundEnum[] GetQueuedSound();

    // maybe later
    // internal int GetPriority();

    internal void ResetSound();

    internal bool IsRemovable();

    internal float GetPan();
}