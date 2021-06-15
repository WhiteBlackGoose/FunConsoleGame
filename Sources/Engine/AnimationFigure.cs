using System.Linq;
using ConsoleGameEngine;
using HonkSharp.Fluency;

public sealed class AnimationFigure
{
    private readonly Figure[] figures;
    private readonly int interval;
    private readonly bool finite;

    public AnimationFigure(string[] contents, int color, int interval, bool finite)
        => (figures, this.interval, this.finite) = (contents.Select(c => new Figure(c, color)).ToArray(), interval, finite);

    public void RegisterRenderingTo(UISyncTimer timer, ConsoleEngine engine, int x, int y)
        => timer.AddEvent(interval, figures.Length.NullIf(_ => !finite),
            i => figures[i % figures.Length].RenderTo(engine, x, y).ReplaceWith(true));
}