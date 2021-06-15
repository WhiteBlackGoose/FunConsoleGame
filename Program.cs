using System;
using System.Collections.Generic;
using ConsoleGameEngine;
using HonkSharp.Fluency;

new MyGame().Construct(40, 40, 20, 20, FramerateMode.Unlimited);

public sealed class MyGame : ConsoleGame
{
    private readonly HashSet<Bullet> bullets = new();
    private readonly Person person = new();
    private readonly SynchronousTimer phyTimer = new();
    private readonly UISyncTimer uiTimer = new();
    // private readonly HashSet<Enemy> enemies;
    
    public override void Create()
    {
        person.Move(20, 20);
    }
    
    private readonly List<Bullet> toRemove = new();
    public override void Update()
    {
        if (Engine.GetKeyDown(ConsoleKey.UpArrow))
        {
            var bullet = new Bullet(person.X, person.Y, 0, -4, isEnemy: false);
            bullets.Add(bullet);
            phyTimer.AddEvent(1, null, i => 
                bullet.MoveByDelta(0.005)
                    .Pipe(_ => Engine.InBounds(bullet.X, bullet.Y) switch
                        {
                            true => true,
                            false => bullets.Remove(bullet).ReplaceWith(bullet).Pipe(
                                bullet => 
                                    AnimationFigures.Explosion.RegisterRenderingTo(uiTimer, Engine, bullet.X, bullet.Y + 3)
                                ).ReplaceWith(false)
                        }
                    )
                );
        }

        if (Engine.GetKey(ConsoleKey.W))
            person.Move(0, -0.01);
        if (Engine.GetKey(ConsoleKey.S))
            person.Move(0, 0.01);
        if (Engine.GetKey(ConsoleKey.A))
            person.Move(-0.01, 0);
        if (Engine.GetKey(ConsoleKey.D))
            person.Move(0.01, 0);

        phyTimer.Emit();
        
    }

    public override void Render()
    {
        Engine.ClearBuffer();
        
        Engine.WriteText(new(0, 0), bullets.Count.ToString(), 1);

        foreach (var b in bullets)
            Figures.Bullet.RenderTo(Engine, b.X, b.Y);

        Figures.Person.RenderTo(Engine, person.X, person.Y);

        uiTimer.Emit();

        Engine.DisplayBuffer();
    }
}

