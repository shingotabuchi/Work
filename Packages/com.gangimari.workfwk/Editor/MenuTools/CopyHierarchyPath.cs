using System.Text;
using UnityEditor;
using UnityEngine;

namespace Fwk.Editor
{
    public static class CopyHierarchyPath
    {
        [MenuItem("GameObject/Copy Path", false, int.MinValue)]
        private static void CopyPath()
        {
            var active = Selection.activeGameObject;
            if (active == null)
                return;

            var obj = active as GameObject;
            var builder = new StringBuilder(obj.transform.name);
            var current = obj.transform.parent;

            while (current != null)
            {
                builder.Insert(0, current.name + "/");
                current = current.parent;
            }

            GUIUtility.systemCopyBuffer = builder.ToString();
        }
    }
}