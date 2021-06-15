using HonkSharp.Functional;

public sealed class Bullet : MovableObject
{
    private double speedX;
    private double speedY;
    public bool IsEnemy { get; }

    public Bullet(double x, double y, double speedX, double speedY, bool isEnemy)
        => (this.x, this.y, this.speedX, this.speedY, this.IsEnemy) = (x, y, speedX, speedY, isEnemy);

    public Unit MoveByDelta(double time)
        => Move(speedX * time, speedY * time);
}