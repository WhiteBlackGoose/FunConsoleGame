using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleGameEngine;
using HonkSharp.Fluency;
using HonkSharp.Functional;

new MyGame().Construct(40, 40, 27, 27, FramerateMode.Unlimited);

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
    private const int SpawnDecreaseInterval = 10_000;
    
    private const int EnemyCountLimitMin = 5;
    private const int EnemyCountLimitMax = 30;
    private const int EnemyCountLimitIncreaseInterval = 35_000;
    
    private const int Cooldown = 100;
    
    private int enemyCountLimit;
    private bool isGameOver;

    private int bulletsShot;
    private int bulletsHit;
    private int enemiesKilled;

    public override void Create()
    {
        bulletsShot = 0;
        bulletsHit = 0;
        enemiesKilled = 0;
        enemyCountLimit = EnemyCountLimitMin;
        phyTimer.Clear();
        uiTimer.Clear();
        bullets.Clear();
        enemies.Clear();
        enemyBullets.Clear();

        person.SetCoords(20, 20);
        
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
        
        isGameOver = true;
    }
    
    private static bool Intersect(ILocatable a, ILocatable b)
        => Math.Abs(a.X - b.X) <= 1 && Math.Abs(a.Y - b.Y) <= 1;
    
    public override void Update()
    {
        if (isGameOver)
        {
            if (Engine.GetKeyDown(ConsoleKey.Spacebar))
            {
                Create();
                isGameOver = false;
            }
            return; 
        }
        
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
                                        .Let(out bulletsShot, bulletsShot + 1)
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
                                                            .Let(out bulletsHit, bulletsHit + 1)
                                                            .ReplaceWith(false),
                                
                                true => true,

                                // bullet outside of the world
                                false => bulletRemove(bullet, bullet.X, bullet.Y + 3, AnimationFigures.SmallExplosion).ReplaceWith(false)
                            }
                    )
                );
        }
        
        if (Engine.GetKey(ConsoleKey.W) && person.Y > 13)
            person.Move(0, -0.01);
        if (Engine.GetKey(ConsoleKey.S) && person.Y < Engine.WindowSize.Y - 2)
            person.Move(0, 0.01);
        if (Engine.GetKey(ConsoleKey.A) && person.X > 2)
            person.Move(-0.01, 0);
        if (Engine.GetKey(ConsoleKey.D) && person.X < Engine.WindowSize.X - 3)
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
            WriteCentralText((enemiesKilled, bulletsHit) switch
                {
                    (0, _) => "PRESS SPACE TO START",
                    (_, 0) => $"GAME OVER: {enemiesKilled}",
                    _ => $"GAME OVER: {enemiesKilled} ({HitPercentInfo()})"
                }, 6);
        else
        {
            
            Engine.WriteText(new(0, 0), enemiesKilled.ToString(), 10);
            Engine.WriteText(new(0, 1), enemyCountLimit.ToString(), 10);
            if (bulletsShot is not 0)
                Engine.WriteText(new(0, 2), HitPercentInfo(), 10);

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
        
        Console.Title = "Space battle";
        
        string HitPercentInfo()
            => $"{((double) bulletsHit / bulletsShot * 100):00.0}%";
    }
}

