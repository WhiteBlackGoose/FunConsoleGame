using ConsoleGameEngine;

public static class EngineExtensions
{
    public static bool InBounds(this ConsoleEngine engine, int x, int y)
        => x >= 0 && y >= 0 && x < engine.WindowSize.X && y < engine.WindowSize.Y;
}
