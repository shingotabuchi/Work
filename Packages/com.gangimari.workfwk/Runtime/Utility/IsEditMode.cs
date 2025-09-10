public static class IsEditMode
{
    public static bool Value
    {
        get
        {
#if UNITY_EDITOR
                return !UnityEditor.EditorApplication.isPlaying;
#else
            return false;
#endif
        }
    }
}