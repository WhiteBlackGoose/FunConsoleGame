using HonkSharp.Fluency;
using HonkSharp.Functional;

public sealed class MovableObject
{
    public int X { get => (int)x; set => x = value; }
    private double x;

    public int Y { get => (int)y; set => y = value; }
    private double y;

    public Unit Move(double x, double y)
        => ((this.x, this.y) = (this.x + x, this.y + y)).Discard();
}