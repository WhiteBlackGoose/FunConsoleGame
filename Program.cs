using System;
using System.Collections.Generic;
using ConsoleGameEngine;



new MyGame().Construct(40, 40, 20, 20, FramerateMode.Unlimited);

public static class EngineExtensions
{
    public static bool InBounds(this ConsoleEngine engine, int x, int y)
        => x >= 0 && y >= 0 && x < engine.WindowSize.X && y < engine.WindowSize.Y;
}
public sealed class MyGame : ConsoleGame
{
    public sealed class Figure
    {
        private Point pivot;
        private (Point Coords, char Char)[] points;
        private int color;

        public Figure(string content, int color)
        {
            this.color = color;
            var arr = content.Replace("\r", "").Split("\n");
            var statesList = new List<(Point Coords, char Char)>();
            var maxWidth = 0;
            for (var i = 0; i < arr.Length; i++)
            {
                maxWidth = Math.Max(maxWidth, arr[i].Length);
                for (var j = 0; j < arr[i].Length; j++)
                    if (arr[i][j] != ' ')
                        statesList.Add((new(j, i), arr[i][j]));
            }
            points = statesList.ToArray();
            pivot = new(maxWidth / 2, arr.Length / 2);
        }

        public void RenderTo(ConsoleEngine engine, int x, int y)
        {
            foreach (var point in points)
            {
                var coords = new Point(point.Coords.X - pivot.X + x, point.Coords.Y - pivot.Y + y);
                if (!engine.InBounds(coords.X, coords.Y))
                    continue;
                engine.SetPixel(new(coords.X, coords.Y), color, point.Char);
            }
        }
    }
    
    public static class Figures
    {
        public static Figure Person = new(
@"
  A
|/|\|
[_Z_]
", 3);

        public static Figure Bullet = new(
@"
A
", 6);

        public static Figure Enemy = new(
@"
\/_\/
 [U]
  Y
", 4);

        public static Figure EnemyBullet = new(
@"
Y
", 7);
    }

    public abstract class MovableObject
    {
        public int X => (int)x;
        protected double x;

        public int Y => (int)y;
        protected double y;

        public void Move(double x, double y)
            => (this.x, this.y) = (this.x + x, this.y + y);
    }

    public sealed class Person : MovableObject
    {
        
    }

    public sealed class Bullet : MovableObject
    {
        private double speedX;
        private double speedY;
        public bool IsEnemy { get; }

        public Bullet(double x, double y, double speedX, double speedY, bool isEnemy)
            => (this.x, this.y, this.speedX, this.speedY, this.IsEnemy) = (x, y, speedX, speedY, isEnemy);

        public void MoveByDelta(double time)
            => Move(speedX * time, speedY * time);
    }

    private HashSet<Bullet> bullets = new();
    private Person person = new();
    
    public override void Create()
    {
        person.Move(20, 20);
    }

    private bool a = false;
    
    
    private List<Bullet> toRemove = new();
    public override void Update()
    {
        if (Engine.GetKeyDown(ConsoleKey.UpArrow))
            bullets.Add(new(person.X, person.Y, 0, -4, isEnemy: false));

        if (Engine.GetKey(ConsoleKey.W))
            person.Move(0, -0.01);
        if (Engine.GetKey(ConsoleKey.S))
            person.Move(0, 0.01);
        if (Engine.GetKey(ConsoleKey.A))
            person.Move(-0.01, 0);
        if (Engine.GetKey(ConsoleKey.D))
            person.Move(0.01, 0);
        
        toRemove.Clear();
        foreach (var bullet in bullets)
        {
            if (!Engine.InBounds(bullet.X, bullet.Y))
                toRemove.Add(bullet);
            else
                bullet.MoveByDelta(0.005);
        }

        for (int i = 0; i < toRemove.Count; i++)
            bullets.Remove(toRemove[i]);
    }

    public override void Render()
    {
        Engine.ClearBuffer();
        
        Engine.WriteText(new(0, 0), bullets.Count.ToString(), 1);

        foreach (var b in bullets)
            Figures.Bullet.RenderTo(Engine, b.X, b.Y);

        Figures.Person.RenderTo(Engine, person.X, person.Y);


        Engine.DisplayBuffer();
    }
}

