using System;
using HonkSharp.Fluency;

public sealed class UISyncTimer
{
    private readonly SynchronousTimer timer = new();

    public void AddEvent(int interval, int? numOfIterations, Func<int, bool> callback)
        => timer.AddEvent(
            interval: 1,
            numOfIterations: numOfIterations?.Pipe(a => a * interval),
            callback: i => callback(i / interval)
            );

    public void Emit()
        => timer.Emit();
    
    public void Clear()
        => timer.Clear();
}