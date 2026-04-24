public static class GameMode
{
    public enum Mode
    {
        Escape,
        Explore
    }

    public static Mode Current { get; private set; } = Mode.Explore;

    public static void SetMode(Mode mode)
    {
        Current = mode;
    }
}
