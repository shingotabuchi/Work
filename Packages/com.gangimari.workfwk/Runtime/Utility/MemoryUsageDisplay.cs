using UnityEngine;
using System.Globalization;

namespace Fwk.Utility
{
    public class MemoryUsageDisplay : MonoBehaviour
    {
        private GUIStyle textStyle;
        private GUIStyle backgroundStyle;
        private Texture2D backgroundTexture;
        private Rect backgroundRect = new Rect(10, 10, 360, 110);

        private string memoryInfoText = "";
        private float updateInterval = 1.0f; // update every 1 second
        private float timeSinceLastUpdate = 0f;

        private void Start()
        {
            // Create text style
            textStyle = new GUIStyle
            {
                fontSize = 22,
                normal = { textColor = Color.white }
            };

            // Create background style
            backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, new Color(0, 0, 0, 0.5f)); // Black with 50% alpha
            backgroundTexture.Apply();

            backgroundStyle = new GUIStyle
            {
                normal = { background = backgroundTexture }
            };

            UpdateMemoryInfo(); // Initialize text immediately
        }

        private void Update()
        {
            timeSinceLastUpdate += Time.unscaledDeltaTime;

            if (timeSinceLastUpdate >= updateInterval)
            {
                UpdateMemoryInfo();
                timeSinceLastUpdate = 0f;
            }
        }

        private void OnGUI()
        {
            // Draw background box
            GUI.Box(backgroundRect, GUIContent.none, backgroundStyle);

            // Draw memory text
            GUI.Label(new Rect(20, 20, 1000, 200), memoryInfoText, textStyle);
        }

        private void UpdateMemoryInfo()
        {
            long monoUsed = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
            long totalAllocated = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
            long totalReserved = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();

            memoryInfoText =
                $"Mono Used: {FormatBytes(monoUsed)}\n" +
                $"Total Allocated: {FormatBytes(totalAllocated)}\n" +
                $"Total Reserved: {FormatBytes(totalReserved)}";
        }

        private string FormatBytes(long bytes)
        {
            return (bytes / (1024f * 1024f)).ToString("F2", CultureInfo.InvariantCulture) + " MB";
        }
    }
}