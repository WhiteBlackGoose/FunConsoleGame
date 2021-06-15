public static class Figures
{
    public static readonly Figure Person = new(
@"
  A
|/|\|
[_Z_]
", 3);

    public static readonly Figure Bullet = new(
@"
A
", 6);

    public static readonly Figure Enemy = new(
@"
\/_\/
 [U]
  Y
", 4);

    public static readonly Figure EnemyBullet = new(
@"
Y
", 7);
}