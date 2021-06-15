public static class AnimationFigures
{
    public static readonly AnimationFigure Explosion = new(new[]
        {
@"
*
",
@"
 %
% %
 %
",
@"
 % %
% % %
 % %
",
@"
 % % %
%     %
%  *  %
%     %
 % % %
",
@"
 . . .
.     .
.     .
.     .
 . . .
"
        }, color: 2, interval: 75, finite: true
    );
    
    public static AnimationFigure SmallExplosion = new(new[]
    {
@"
.
",
@"
*
",
@"
 %
% %
 %
",
@"
 .
. .
 .
"}, color: 2, interval: 75, finite: true);
}