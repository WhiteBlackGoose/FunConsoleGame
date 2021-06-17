using System;
using HonkSharp.Fluency;
using HonkSharp.Functional;

public sealed class UISyncTimer
{
    private readonly SynchronousTimer timer = new();

    public Unit AddEvent(int interval, int? numOfIterations, Func<int, bool> callback)
        => timer.AddEvent(
            interval: 1,
            numOfIterations: numOfIterations?.Pipe(a => a * interval),
            callback: i => callback(i / interval)
            ).Discard();

    public void Emit()
        => timer.Emit();
    
    public void Clear()
        => timer.Clear();
}