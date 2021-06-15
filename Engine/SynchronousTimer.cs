using System;
using System.Collections.Generic;
using HonkSharp.Fluency;
using HonkSharp.Functional;

public sealed class SynchronousTimer
{
    private sealed class TimerEvent
    {
        public int TimeLeft { get; set; }
        public int CurrentIter { get; set; }
        
        public int Interval { get; init; }
        public int? IterNumber { get; init; }
        public Func<int, bool> Callback { get; init; }
    }
    
    private readonly HashSet<TimerEvent> events = new();

    private readonly Queue<TimerEvent> queue = new();
    private bool isEmitted = false;
    
    public Unit AddEvent(int interval, int? numOfIterations, Func<int, bool> callback)
        => new TimerEvent { TimeLeft = interval, CurrentIter = 0, Interval = interval, IterNumber = numOfIterations,
            Callback = callback }
            .Pipe(ev => isEmitted 
                ? ev.Pipe(queue.Enqueue).Discard() 
                : events.Add(ev).Discard());
    
    private Unit AddEvent(TimerEvent ev)
        => events.Add(ev).Discard();

    private readonly List<TimerEvent> eventsToDelete = new();
    public void Emit()
    {
        eventsToDelete.Clear();
        
        isEmitted = true;
        foreach (var e in events)
        {
            if (e.TimeLeft is <= 1)
            {
                if (!e.Callback(e.CurrentIter))
                    eventsToDelete.Add(e);
                else
                {
                    e.CurrentIter++;
                    if (e.IterNumber is not null && e.CurrentIter == e.IterNumber)
                        eventsToDelete.Add(e);
                    else
                        e.TimeLeft = e.Interval;
                }
            }
            else
                e.TimeLeft--;
        }
        isEmitted = false;
        while (queue.Count > 0)
            AddEvent(queue.Dequeue());

        for (int i = 0; i < eventsToDelete.Count; i++)
            events.Remove(eventsToDelete[i]);
    }
}