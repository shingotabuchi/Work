using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Fwk.Editor
{
    public static class AddressablesBuildTool
    {
        [MenuItem("Tools/Build/Build Addressables")]
        public static void BuildAddressables()
        {
            Debug.Log("[AddressablesBuildTool] Starting Addressables Build...");

            // Step 1: Backup original settings
            AddressableFolderAutoAssign.BackupOriginalSettings();

            // Step 2: Prepare temporary build settings
            AddressableFolderAutoAssign.PrepareBuildSettings();

            // Step 3: Auto assign addressables
            AddressableFolderAutoAssign.AssignAddressables();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Step 4: Build Addressables
            AddressableAssetSettings.BuildPlayerContent();

            // Step 5: Restore original settings
            AddressableFolderAutoAssign.RestoreOriginalSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[AddressablesBuildTool] Addressables Build Completed.");
        }

        [MenuItem("Tools/Build/Clean Addressables Cache")]
        public static void CleanAddressablesCache()
        {
            Debug.Log("[AddressablesBuildTool] Cleaning Addressables Cache...");

            UnityEngine.AddressableAssets.Addressables.ClearResourceLocators();
            Caching.ClearCache(); // Clears Unity's asset bundle cache
            AddressableAssetSettings.CleanPlayerContent(); // Clears built Addressables

            AssetDatabase.Refresh();

            Debug.Log("[AddressablesBuildTool] Addressables Cache Cleaned.");
        }
    }
}
