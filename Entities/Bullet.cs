using HonkSharp.Functional;

public sealed class Bullet
{
    private readonly MovableObject movable = new();
    
    private readonly double speedX;
    private readonly double speedY;
    public bool IsEnemy { get; }

    public Bullet(int x, int y, double speedX, double speedY, bool isEnemy)
        => (movable.X, movable.Y, this.speedX, this.speedY, IsEnemy) = (x, y, speedX, speedY, isEnemy);

    public Unit MoveByDelta(double time)
        => movable.Move(speedX * time, speedY * time);
    
    public int X => movable.X;
    public int Y => movable.Y;
}