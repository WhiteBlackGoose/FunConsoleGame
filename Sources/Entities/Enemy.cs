using System.Collections.Generic;
using ConsoleGameEngine;
using HonkSharp.Fluency;

public sealed class Enemy : ILocatable
{
    private readonly MovableObject movable = new();
    
    public Enemy(int x, int y)
        => (movable.X, movable.Y) = (x, y);
    
    public int X => movable.X;
    public int Y => movable.Y;
    
    private int period = 0;
    private const int Cooldown = 35;
    public Bullet? ShootIfCan(SynchronousTimer phyTimer, ConsoleEngine engine, HashSet<Bullet> bullets, UISyncTimer uiTimer)
        => period++ <= Cooldown 
            ? null
            : (period = 0)
            .ReplaceWith(new Bullet(X, Y, 0, 4, isEnemy: true))
            .Alias(out var bullet)
            .Pipe(bullet =>
                phyTimer.AddEvent(1, null, _ =>
                    bullet.MoveByDelta(0.005)
                        .Pipe(_ => engine.InBounds(bullet.X, bullet.Y) switch
                            {
                                true => true,
                                false => bullets.Remove(bullet).ReplaceWith(bullet).Pipe(
                                    bullet => 
                                        AnimationFigures.Explosion.RegisterRenderingTo(uiTimer, engine, bullet.X, bullet.Y + 3)
                                    ).ReplaceWith(false)
                            }
                        )
                )
            )
            .ReplaceWith(bullet)
            .Pipe(bullets.Add)
            .ReplaceWith(bullet);
}