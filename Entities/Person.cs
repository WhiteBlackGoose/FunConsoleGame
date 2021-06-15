using HonkSharp.Functional;

public sealed class Person : ILocatable
{
    private readonly MovableObject movable = new();
    
    public Unit Move(double x, double y)
        => movable.Move(x, y);
        
    public int X => movable.X;
    public int Y => movable.Y;
}