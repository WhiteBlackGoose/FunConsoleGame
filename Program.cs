using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleGameEngine;
using HonkSharp.Fluency;
using HonkSharp.Functional;

new MyGame().Construct(40, 40, 20, 20, FramerateMode.Unlimited);

public sealed class MyGame : ConsoleGame
{
    private readonly SynchronousTimer phyTimer = new();
    private readonly UISyncTimer uiTimer = new();
    private readonly Random random = new();
    
    private readonly HashSet<Bullet> bullets = new();
    private readonly Person person = new();
    
    private readonly HashSet<Bullet> enemyBullets = new();
    private readonly HashSet<Enemy> enemies = new();
    
    
    private const int DefaultSpawnInterval = 3000;
    private const int MinimumSpawnInterval = 1000;
    private const int SpawnDecrease = 100;
    private const int SpawnDecreaseInterval = 25_000;
    
    private const int EnemyCountLimitMin = 5;
    private const int EnemyCountLimitMax = 30;
    private const int EnemyCountLimitIncreaseInterval = 50_000;
    
    private const int Cooldown = 100;
    
    private int enemyCountLimit = EnemyCountLimitMin;
    private bool isGameOver;
    
    private int enemiesKilled = 0;

    public override void Create()
    {
        isGameOver = false;
        
        person.Move(20, 20);
        
        var ev = phyTimer.AddEvent(
            DefaultSpawnInterval,
            null,
            _ => enemies.Count >= enemyCountLimit || enemies.Add(new Enemy(random.Next(4, Engine.WindowSize.X - 4), 5))
        );
        
        phyTimer.AddEvent(
            SpawnDecreaseInterval,
            null,
            _ => (ev.Interval -= SpawnDecrease).Pipe(interval => interval > MinimumSpawnInterval)
            );
        
        phyTimer.AddEvent(
            Cooldown,
            null,
            _ => enemies.Select(e => e.ShootIfCan(phyTimer, Engine, enemyBullets, uiTimer)).ToArray().ReplaceWith(true)
            );
        
        phyTimer.AddEvent(
            EnemyCountLimitIncreaseInterval,
            null,
            _ => (enemyCountLimit++).Pipe(enemyCount => enemyCount < EnemyCountLimitMax)
            );
    }
    
    private static bool Intersect(ILocatable a, ILocatable b)
        => (a.X - b.X).Pipe(Math.Abs) <= 1
           && (a.Y - b.Y).Pipe(Math.Abs) <= 1;
    
    public override void Update()
    {
        if (isGameOver)
            return;
        
        if (Engine.GetKeyDown(ConsoleKey.UpArrow))
        {
            var bullet = new Bullet(person.X, person.Y, 0, -4, isEnemy: false);
            bullets.Add(bullet);
            phyTimer.AddEvent(1, null, i => 
                bullet.MoveByDelta(0.005)
                    .Pipe(_ =>
                        Engine.InBounds(bullet.X, bullet.Y)
                            .Let(out Func<Bullet, int, int, AnimationFigure, Unit> bulletRemove, 
                                (bullet, x, y, an) => bullet.Pipe(bullets.Remove).ReplaceWith(bullet)
                                        .Pipe(
                                            bullet => 
                                                an.RegisterRenderingTo(uiTimer, Engine, x, y)
                                        )
                                        .Discard()
                                )
                            switch
                        {
                            true when enemies
                                    .FirstOrDefault(enemy => (enemy, bullet).Pipe(Intersect))
                                    is { } killedEnemy => bulletRemove(bullet, killedEnemy.X, killedEnemy.Y, AnimationFigures.Explosion)
                                                        .ReplaceWith(killedEnemy)
                                                        .Pipe(enemies.Remove)
                                                        .Let(out enemiesKilled, enemiesKilled + 1)
                                                        .ReplaceWith(false),
                            
                            true => true,

                            // bullet outside of the world
                            false => bulletRemove(bullet, bullet.X, bullet.Y + 3, AnimationFigures.SmallExplosion).ReplaceWith(false)
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
        if (enemyBullets.Any(b => Intersect(person, b)))
            isGameOver = true;
    }

    private void WriteCentralText(string text, int color)
        => Engine.WriteText(new(Engine.WindowSize.X / 2 - text.Length / 2, Engine.WindowSize.Y / 2), text, color);
    
    public override void Render()
    {
        Engine.ClearBuffer();
        
        if (isGameOver)
            WriteCentralText($"GAME OVER: {enemiesKilled}", 6);
        else
        {
            
            Engine.WriteText(new(0, 0), enemiesKilled.ToString(), 1);

            foreach (var b in bullets)
                Figures.Bullet.RenderTo(Engine, b.X, b.Y);

            Figures.Person.RenderTo(Engine, person.X, person.Y);
            
            foreach (var e in enemies)
                Figures.Enemy.RenderTo(Engine, e.X, e.Y);

            foreach (var b in enemyBullets)
                Figures.EnemyBullet.RenderTo(Engine, b.X, b.Y);
            
        }
        
        uiTimer.Emit();

        Engine.DisplayBuffer();
    }
}

