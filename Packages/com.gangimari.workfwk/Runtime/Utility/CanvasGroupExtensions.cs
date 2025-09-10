using UnityEngine;

public static class CanvasGroupExtensions
{
    public static void SetActive(this CanvasGroup self, bool active)
    {
        self.alpha = active ? 1f : 0f;
        self.blocksRaycasts = active;
        self.interactable = active;
    }
}