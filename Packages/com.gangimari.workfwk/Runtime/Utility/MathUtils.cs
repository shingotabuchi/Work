using System.Runtime.CompilerServices;

public static class MathUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Square(this float x) => x * x;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Square(this double x) => x * x;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Square(this int x) => x * x;
}
