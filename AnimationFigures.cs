public static class AnimationFigures
{
    public static readonly AnimationFigure Explosion = new(new[]
        {
@"
*
",
@"
O
",
@"
 .
. .
 .
",
@"
 @
@@@
 @
",
@"
 % %
% % %
 % %
"
        }, color: 2, interval: 16, true
    );
}