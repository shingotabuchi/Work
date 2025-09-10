namespace echo17.EnhancedUI.EnhancedGrid
{
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UI;
    using EnhancedUI.Helpers;

    [CustomEditor(typeof(EnhancedGridSnap))]
    public class EnhancedGridSnapInspector : Editor
    {
        private EnhancedGridSnap _snap;

        protected void _SetTarget()
        {
            _snap = (EnhancedGridSnap)target;
        }

        public override void OnInspectorGUI()
        {
            _snap = target as EnhancedGridSnap;

            if (_snap == null)
                _SetTarget();

            if (_snap == null)
                return;

            _snap.Snapping = _CreateProperty<bool>("Snapping", _snap.Snapping);
            _snap.SnapVelocityThreshold = _CreateProperty<Vector2>("Snap Velocity Threshold", _snap.SnapVelocityThreshold);

            float snapWatchViewportOffsetX = _snap.SnapWatchViewportOffset.x;
            float snapWatchViewportOffsetY = _snap.SnapWatchViewportOffset.y;
            snapWatchViewportOffsetX = _CreateNormalizedSlider("Snap Watch Viewport Offset X", _snap.SnapWatchViewportOffset.x);
            snapWatchViewportOffsetY = _CreateNormalizedSlider("Snap Watch Viewport Offset Y", _snap.SnapWatchViewportOffset.y);
            _snap.SnapWatchViewportOffset = new Vector2(snapWatchViewportOffsetX, snapWatchViewportOffsetY);

            float snapJumpToViewportOffsetX = _snap.SnapJumpToViewportOffset.x;
            float snapJumpToViewportOffsetY = _snap.SnapJumpToViewportOffset.y;
            snapJumpToViewportOffsetX = _CreateNormalizedSlider("Snap Jump To Viewport Offset X", snapJumpToViewportOffsetX);
            snapJumpToViewportOffsetY = _CreateNormalizedSlider("Snap Jump To Viewport Offset Y", snapJumpToViewportOffsetY);
            _snap.SnapJumpToViewportOffset = new Vector2(snapJumpToViewportOffsetX, snapJumpToViewportOffsetY);

            float snapCellOffsetX = _snap.SnapJumpToCellOffset.x;
            float snapCellOffsetY = _snap.SnapJumpToCellOffset.y;
            snapCellOffsetX = _CreateNormalizedSlider("Snap Jump To Cell Offset X", snapCellOffsetX);
            snapCellOffsetY = _CreateNormalizedSlider("Snap Jump To Cell Offset Y", snapCellOffsetY);
            _snap.SnapJumpToCellOffset = new Vector2(snapCellOffsetX, snapCellOffsetY);

            _snap.SnapTweenTypeX = _CreateProperty<TweenType>("Snap Tween Type X", _snap.SnapTweenTypeX);
            _snap.SnapTweenTimeX = _CreateProperty<float>("Snap Tween Time X", _snap.SnapTweenTimeX);
            _snap.SnapTweenTypeY = _CreateProperty<TweenType>("Snap Tween Type Y", _snap.SnapTweenTypeY);
            _snap.SnapTweenTimeY = _CreateProperty<float>("Snap Tween Time Y", _snap.SnapTweenTimeY);
            _snap.SnapWhileDragging = _CreateProperty<bool>("Snap While Dragging", _snap.SnapWhileDragging);
            _snap.ForceSnapOnEndDrag = _CreateProperty<bool>("Force Snap On End Drag", _snap.ForceSnapOnEndDrag);
        }

        private T _CreateProperty<T>(string label, object value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(200.0f));
            EditorGUI.BeginChangeCheck();

            object newValue = null;

            if (typeof(T) == typeof(TweenType))
            {
                newValue = (TweenType)EditorGUILayout.EnumPopup((TweenType)value);
            }
            else if (typeof(T) == typeof(Vector2))
            {
                newValue = EditorGUILayout.Vector2Field("", (Vector2)value);
            }
            else if (typeof(T) == typeof(int))
            {
                newValue = EditorGUILayout.IntField((int)value);
            }
            else if (typeof(T) == typeof(float))
            {
                newValue = EditorGUILayout.FloatField((float)value);
            }
            else if (typeof(T) == typeof(bool))
            {
                newValue = EditorGUILayout.Toggle((bool)value);
            }
            else if (typeof(T) == typeof(Sprite))
            {
                newValue = (Sprite)EditorGUILayout.ObjectField((Sprite)value, typeof(Sprite), true);
            }
            else if (typeof(T) == typeof(Color))
            {
                newValue = EditorGUILayout.ColorField((Color)value);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_snap, $"{label} changed");
            }
            GUILayout.EndHorizontal();

            return (T)newValue;
        }

        private float _CreateNormalizedSlider(string label, float value)
        {
            return _CreateFloatSlider(label, value, 0f, 1f);
        }

        private float _CreateFloatSlider(string label, float value, float min, float max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(230.0f));
            EditorGUI.BeginChangeCheck();

            float newValue = EditorGUILayout.Slider(value, min, max);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_snap, $"{label} changed");
            }
            GUILayout.EndHorizontal();

            return newValue;
        }
    }
}
