using System.Collections.Generic;

public static class ListUtils
{
    public static void Shuffle<T>(this IList<T> list)
    {
        var rng = new System.Random();
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int k = rng.Next(i + 1);
            (list[i], list[k]) = (list[k], list[i]);
        }
    }

    public static T GetRandomElement<T>(this IReadOnlyList<T> list)
    {
        if (list == null || list.Count == 0)
        {
            throw new System.ArgumentException("List is null or empty.");
        }

        var rng = new System.Random();
        int index = rng.Next(list.Count);
        return list[index];
    }
}
