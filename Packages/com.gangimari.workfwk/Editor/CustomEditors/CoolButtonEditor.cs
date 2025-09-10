using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using Fwk.UI;

namespace Fwk.Editor
{
    [CustomEditor(typeof(CoolButton), editorForChildClasses: true)]
    public class CoolButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw all CoolButton-specific properties automatically
            DrawCoolButtonProperties();

            EditorGUILayout.Space();

            // Draw the default Button inspector
            base.OnInspectorGUI();
        }

        private void DrawCoolButtonProperties()
        {
            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                // Skip the script property (m_Script) and base Button properties
                if (property.name == "m_Script" || IsBaseButtonProperty(property.name))
                    continue;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(property, true);

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private bool IsBaseButtonProperty(string propertyName)
        {
            // List of base Button properties to skip (these will be drawn by base.OnInspectorGUI())
            return propertyName == "m_Interactable" ||
                   propertyName == "m_Transition" ||
                   propertyName == "m_Colors" ||
                   propertyName == "m_SpriteState" ||
                   propertyName == "m_AnimationTriggers" ||
                   propertyName == "m_TargetGraphic" ||
                   propertyName == "m_OnClick" ||
                   propertyName == "m_Navigation";
        }
    }
}