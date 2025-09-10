using UnityEngine;

public static class RectTransformExtensions
{
    public static void SetAnchorX(this RectTransform self, float x)
    {
        self.anchorMax = new Vector2(x, self.anchorMax.y);
        self.anchorMin = new Vector2(x, self.anchorMin.y);
    }
}