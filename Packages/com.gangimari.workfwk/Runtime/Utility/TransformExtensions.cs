using UnityEngine;

public static class TransformExtensions
{
    public static void SetScale(this Transform transform, float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }
}