namespace echo17.EnhancedUI.EnhancedGrid
{
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UI;
    using EnhancedUI.Helpers;

    [CustomEditor(typeof(EnhancedGrid))]
    public class EnhancedGridInspector : Editor
    {
        private EnhancedGrid _grid;

        protected void _SetTarget()
        {
            _grid = (EnhancedGrid)target;
        }

        public override void OnInspectorGUI()
        {
            _grid = target as EnhancedGrid;

            if (_grid == null)
                _SetTarget();

            if (_grid == null)
                return;

            _grid.FlowDirection = _CreateProperty<EnhancedGrid.EnhancedGridFlowDirection>("Flow Direction", _grid.FlowDirection);
            _grid.FlowGroupAlignment = _CreateProperty<EnhancedGrid.EnhancedGridFlowGroupAlignment>("Flow Group Alignement", _grid.FlowGroupAlignment);
            _grid.WrapMode = _CreateProperty<EnhancedGrid.EnhancedGridWrapMode>("Wrap Mode", _grid.WrapMode);
            if (_grid.WrapMode == EnhancedGrid.EnhancedGridWrapMode.Size) { _grid.WrapSize = _CreateProperty<float>("Wrap Size", _grid.WrapSize); }
            if (_grid.WrapMode == EnhancedGrid.EnhancedGridWrapMode.CellCount) { _grid.WrapCellCount = _CreateProperty<int>("Wrap Cell Count", _grid.WrapCellCount); }
            _grid.CellLayoutSpacing = _CreateProperty<float>("Cell Layout Spacing", _grid.CellLayoutSpacing);
            _grid.GroupLayoutSpacing = _CreateProperty<float>("Group Layout Spacing", _grid.GroupLayoutSpacing);
            _grid.ContentPaddingTop = _CreateProperty<float>("Content Padding Top", _grid.ContentPaddingTop);
            _grid.ContentPaddingBottom = _CreateProperty<float>("Content Padding Bottom", _grid.ContentPaddingBottom);
            _grid.ContentPaddingLeft = _CreateProperty<float>("Content Padding Left", _grid.ContentPaddingLeft);
            _grid.ContentPaddingRight = _CreateProperty<float>("Content Padding Right", _grid.ContentPaddingRight);
            _grid.RecycleCells = _CreateProperty<bool>("Recycle Cells", _grid.RecycleCells);
            if (_grid.RecycleCells)
            {
                _grid.DestroyCellsWhenRecalculatingGrid = _CreateProperty<bool>("Destroy Cells When Recalculating Grid", _grid.DestroyCellsWhenRecalculatingGrid);
                _grid.OcclusionDepth = _CreateIntSlider("Occlusion Depth", _grid.OcclusionDepth, 1, 5);
                _grid.ExtendVisibleAreaTop = _CreateProperty<float>("Extend Visible Area Top", _grid.ExtendVisibleAreaTop);
                _grid.ExtendVisibleAreaBottom = _CreateProperty<float>("Extend Visible Area Bottom", _grid.ExtendVisibleAreaBottom);
                _grid.ExtendVisibleAreaLeft = _CreateProperty<float>("Extend Visible Area Left", _grid.ExtendVisibleAreaLeft);
                _grid.ExtendVisibleAreaRight = _CreateProperty<float>("Extend Visible Area Right", _grid.ExtendVisibleAreaRight);
                _grid.MinimumScrollForActiveCellUpdate = _CreateProperty<Vector2>("Minimum Scroll for Active Cell Update", _grid.MinimumScrollForActiveCellUpdate);
            }
            _grid.RepeatModeX = _CreateProperty<EnhancedGrid.EnhancedGridRepeatMode>("Repeat Mode X", _grid.RepeatModeX);
            _grid.RepeatModeY = _CreateProperty<EnhancedGrid.EnhancedGridRepeatMode>("Repeat Mode Y", _grid.RepeatModeY);
            _grid.MaxVelocity = _CreateProperty<Vector2>("Max Velocity", _grid.MaxVelocity);
            _grid.InterruptTweeningOnDrag = _CreateProperty<bool>("Interrupt Tween On Drag", _grid.InterruptTweeningOnDrag);
            _grid.TweenPaused = _CreateProperty<bool>("Tween Paused", _grid.TweenPaused);
            _grid.LoopWhileDragging = _CreateProperty<bool>("Loop While Dragging", _grid.LoopWhileDragging);

            _grid.ShowOcclusionSectors = _CreateProperty<bool>("Show Occlusion Sectors", _grid.ShowOcclusionSectors);
            _grid.ShowGroupLayouts = _CreateProperty<bool>("Show Group Layouts", _grid.ShowGroupLayouts);
            _grid.ShowCellLayouts = _CreateProperty<bool>("Show Cell Layouts", _grid.ShowCellLayouts);
            _grid.ShowCells = _CreateProperty<bool>("Show Cells", _grid.ShowCells);

            if (GUILayout.Button("Recalculate Grid"))
            {
                _grid.RecalculateGrid();
            }
        }

        private T _CreateProperty<T>(string label, object value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(230.0f));
            EditorGUI.BeginChangeCheck();

            object newValue = null;

            if (typeof(T) == typeof(EnhancedGrid.EnhancedGridFlowDirection))
            {
                newValue = (EnhancedGrid.EnhancedGridFlowDirection)EditorGUILayout.EnumPopup((EnhancedGrid.EnhancedGridFlowDirection)value);
            }
            else if (typeof(T) == typeof(EnhancedGrid.EnhancedGridFlowGroupAlignment))
            {
                newValue = (EnhancedGrid.EnhancedGridFlowGroupAlignment)EditorGUILayout.EnumPopup((EnhancedGrid.EnhancedGridFlowGroupAlignment)value);
            }
            else if (typeof(T) == typeof(EnhancedGrid.EnhancedGridWrapMode))
            {
                newValue = (EnhancedGrid.EnhancedGridWrapMode)EditorGUILayout.EnumPopup((EnhancedGrid.EnhancedGridWrapMode)value);
            }
            else if (typeof(T) == typeof(EnhancedGrid.EnhancedGridRepeatMode))
            {
                newValue = (EnhancedGrid.EnhancedGridRepeatMode)EditorGUILayout.EnumPopup((EnhancedGrid.EnhancedGridRepeatMode)value);
            }
            else if (typeof(T) == typeof(TweenType))
            {
                newValue = (TweenType)EditorGUILayout.EnumPopup((TweenType)value);
            }
            if (typeof(T) == typeof(ScrollRect))
            {
                newValue = (ScrollRect)EditorGUILayout.ObjectField((ScrollRect)value, typeof(ScrollRect), true);
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
                Undo.RecordObject(_grid, $"{label} changed");
            }
            GUILayout.EndHorizontal();

            return (T)newValue;
        }

        private int _CreateIntSlider(string label, int value, int min, int max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(230.0f));
            EditorGUI.BeginChangeCheck();

            int newValue = EditorGUILayout.IntSlider(value, min, max);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_grid, $"{label} changed");
            }
            GUILayout.EndHorizontal();

            return newValue;
        }
    }
}
