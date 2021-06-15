using System;
using System.Collections.Generic;
using ConsoleGameEngine;
using HonkSharp.Functional;

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

    public Unit RenderTo(ConsoleEngine engine, int x, int y)
    {
        foreach (var point in points)
        {
            var coords = new Point(point.Coords.X - pivot.X + x, point.Coords.Y - pivot.Y + y);
            if (!engine.InBounds(coords.X, coords.Y))
                continue;
            engine.SetPixel(new(coords.X, coords.Y), color, point.Char);
        }
        return Unit.Flow;
    }
}