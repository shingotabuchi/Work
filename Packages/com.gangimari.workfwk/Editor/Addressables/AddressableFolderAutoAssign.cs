using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Fwk.Editor
{
    [InitializeOnLoad]
    public static class AddressableFolderAutoAssign
    {
        private const string BuildPath = "Assets/AddressableAssetsData/Build";
        private const string BuildPathMeta = BuildPath + ".meta";
        private const string ResourceFolder = "Assets/AddressableResources";
        private const string DefaultGroupName = "Default Local Group";
        private const string SettingsAssetPath = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
        private const string SortSettingsAssetPath = "Assets/AddressableAssetsData/AddressableAssetGroupSortSettings.asset";
        private const string BuildGroupsPath = BuildPath + "/AssetGroups/";
        private const string BuildSettingsPath = BuildPath + "/AddressableAssetSettings.asset";
        private const string BuildSettingsMetaPath = BuildSettingsPath + ".meta";
        private const string BackupSettingsPath = BuildPath + "/AddressableAssetSettings_Backup.asset";
        private const string BackupSettingsMetaPath = BackupSettingsPath + ".meta";
        private const string BackupSortSettingsPath = BuildPath + "/AddressableAssetGroupSortSettings_Backup.asset";
        private const string BackupSortSettingsMetaPath = BackupSortSettingsPath + ".meta";

        static AddressableFolderAutoAssign()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingEditMode)
                {
                    BackupOriginalSettings();
                    PrepareBuildSettings();
                    AssignAddressables();
                }
                else if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    RestoreOriginalSettings();
                }
            };
        }

        public static void BackupOriginalSettings()
        {
            if (File.Exists(SettingsAssetPath))
            {
                var buildDir = Path.GetDirectoryName(BackupSettingsPath);
                if (!Directory.Exists(buildDir))
                {
                    Directory.CreateDirectory(buildDir);
                }

                File.Copy(SettingsAssetPath, BackupSettingsPath, overwrite: true);
                Debug.Log("[AddressableFolderAutoAssign] Backed up AddressableAssetSettings.asset.");
            }
            else
            {
                Debug.LogWarning("[AddressableFolderAutoAssign] No AddressableAssetSettings.asset found to backup.");
            }

            if (File.Exists(SortSettingsAssetPath))
            {
                File.Copy(SortSettingsAssetPath, BackupSortSettingsPath, overwrite: true);
                Debug.Log("[AddressableFolderAutoAssign] Backed up AddressableAssetGroupSortSettings.asset.");
            }
            else
            {
                Debug.LogWarning("[AddressableFolderAutoAssign] No AddressableAssetGroupSortSettings.asset found to backup.");
            }
        }

        public static void PrepareBuildSettings()
        {
            if (File.Exists(SettingsAssetPath))
            {
                var buildDir = Path.GetDirectoryName(BuildSettingsPath);
                if (!Directory.Exists(buildDir))
                {
                    Directory.CreateDirectory(buildDir);
                }

                File.Copy(SettingsAssetPath, BuildSettingsPath, overwrite: true);
                AssetDatabase.Refresh();

                var playModeSettings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(BuildSettingsPath);
                if (playModeSettings != null)
                {
                    AddressableAssetSettingsDefaultObject.Settings = playModeSettings;
                    Debug.Log("[AddressableFolderAutoAssign] Using Play Mode AddressableAssetSettings.");
                }
                else
                {
                    Debug.LogError("[AddressableFolderAutoAssign] Failed to load Play Mode AddressableAssetSettings.");
                }
            }
            else
            {
                Debug.LogWarning("[AddressableFolderAutoAssign] No AddressableAssetSettings.asset found to prepare for Play Mode.");
            }
        }

        public static void RestoreOriginalSettings()
        {
            ClearAddressableFlags();
            if (File.Exists(BackupSettingsPath))
            {
                if (File.Exists(SettingsAssetPath))
                {
                    File.Delete(SettingsAssetPath);
                }
                File.Delete(BackupSettingsMetaPath);
                File.Move(BackupSettingsPath, SettingsAssetPath);
                Debug.Log("[AddressableFolderAutoAssign] Restored AddressableAssetSettings.asset from backup.");
            }
            else
            {
                Debug.LogWarning("[AddressableFolderAutoAssign] No backup found to restore AddressableAssetSettings.asset.");
            }

            if (File.Exists(BackupSortSettingsPath))
            {
                if (File.Exists(SortSettingsAssetPath))
                {
                    File.Delete(SortSettingsAssetPath);
                }
                File.Delete(BackupSortSettingsMetaPath);
                File.Move(BackupSortSettingsPath, SortSettingsAssetPath);
                Debug.Log("[AddressableFolderAutoAssign] Restored AddressableAssetGroupSortSettings.asset from backup.");
            }
            else
            {
                Debug.LogWarning("[AddressableFolderAutoAssign] No backup found to restore AddressableAssetGroupSortSettings.asset.");
            }

            File.Delete(BuildSettingsPath);
            File.Delete(BuildSettingsMetaPath);

            AssetDatabase.Refresh();

            var originalSettings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(SettingsAssetPath);
            if (originalSettings != null)
            {
                AddressableAssetSettingsDefaultObject.Settings = originalSettings;
                Debug.Log("[AddressableFolderAutoAssign] Restored original AddressableAssetSettings.");
            }
            else
            {
                Debug.LogWarning("[AddressableFolderAutoAssign] No original AddressableAssetSettings found to restore.");
            }
        }

        private static void ClearAddressableFlags()
        {
            var guids = AssetDatabase.FindAssets("", new[] { ResourceFolder });

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(path)) continue;

                var entry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid);
                if (entry != null)
                {
                    AddressableAssetSettingsDefaultObject.Settings.RemoveAssetEntry(guid);
                }
            }
            if (guids != null && guids.Length > 0)
            {
                AddressableAssetSettingsDefaultObject.Settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, null, true);
                AssetDatabase.SaveAssets();
            }
        }

        public static void AssignAddressables()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("[AddressableFolderAutoAssign] Could not load AddressableAssetSettings.");
                return;
            }

            var templateGroup = settings.DefaultGroup;
            var defaultSchemas = templateGroup.Schemas.ToList();

            var guids = AssetDatabase.FindAssets("", new[] { ResourceFolder })
                .Where(g =>
                {
                    var p = AssetDatabase.GUIDToAssetPath(g);
                    return !AssetDatabase.IsValidFolder(p);
                })
                .ToArray();

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var rel = path.Substring(ResourceFolder.Length).TrimStart('/');
                var slash = rel.IndexOf('/');
                var groupName = slash >= 0
                    ? rel.Substring(0, slash)
                    : DefaultGroupName;

                var group = settings.FindGroup(groupName)
                    ?? settings.CreateGroup(groupName, false, false, false, defaultSchemas);

                // Always overwrite BundleMode to PackSeparately
                var bundleSchema = group.Schemas.OfType<BundledAssetGroupSchema>().FirstOrDefault();
                if (bundleSchema != null)
                {
                    bundleSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
                }

                if (!settings.GetLabels().Contains(groupName))
                {
                    settings.AddLabel(groupName, postEvent: true);
                }

                var entry = settings.CreateOrMoveEntry(guid, group, false, false);
                entry.address = path;
                entry.SetLabel(groupName, true);
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
            AssetDatabase.SaveAssets();
            Debug.Log($"[AddressableFolderAutoAssign] Assigned {guids.Length} assets (temporary Play Mode only).");
        }
    }
}
