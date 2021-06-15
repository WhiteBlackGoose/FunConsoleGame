using HonkSharp.Fluency;
using HonkSharp.Functional;

public abstract class MovableObject
{
    public int X => (int)x;
    protected double x;

    public int Y => (int)y;
    protected double y;

    public Unit Move(double x, double y)
        => ((this.x, this.y) = (this.x + x, this.y + y)).Discard();
}