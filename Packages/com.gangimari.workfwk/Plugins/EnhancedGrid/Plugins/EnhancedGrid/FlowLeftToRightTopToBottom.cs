namespace echo17.EnhancedUI.EnhancedGrid
{
    using UnityEngine;
    using Helpers;

    public struct FlowLeftToRightTopToBottom : IFlow
    {
        private EnhancedGrid _grid;

        public FlowLeftToRightTopToBottom(EnhancedGrid grid)
        {
            _grid = grid;
        }

        public EnhancedGrid.EnhancedGridFlowDirection GetFlowDirection()
        {
            return EnhancedGrid.EnhancedGridFlowDirection.LeftToRightTopToBottom;
        }

        public bool IsEndOfGroup(float groupFlowDirectionSize, Vector2 cellMinSize)
        {
            return (groupFlowDirectionSize + _grid.CellLayoutSpacing + cellMinSize.x + _grid.ContentPaddingLeft + _grid.ContentPaddingRight) >= _grid.WrapSize;
        }

        public float ResetFlowDirectionSize(Vector2 cellMinSize)
        {
            return cellMinSize.x;
        }

        public float IncrementFlowDirectionSize(Vector2 cellMinSize)
        {
            return (_grid.CellLayoutSpacing + cellMinSize.x);
        }

        public float GetGroupMaxCellSize(float currentGroupMaxCellSize, Vector2 cellMinSize)
        {
            return Mathf.Max(currentGroupMaxCellSize, cellMinSize.y);
        }

        public Vector2 GetMaxBounds(float maxGroupFlowDirectionSize, float totalGroupSize)
        {
            return new Vector2(_grid.ContentPaddingLeft + _grid.ContentPaddingRight + maxGroupFlowDirectionSize, _grid.ContentPaddingTop + _grid.ContentPaddingBottom + totalGroupSize);
        }

        public int GetGroupIndex(int referenceIndex, int groupCount)
        {
            return referenceIndex;
        }

        public Vector2 GetInitialGroupPosition(float groupOffset)
        {
            return new Vector2(_grid.ContentPaddingLeft, _grid.ContentPaddingTop + groupOffset);
        }

        public Vector2 GetInitialGroupSize(float groupFlowDirectionSize, float groupMaxCellSize)
        {
            return new Vector2(groupFlowDirectionSize, groupMaxCellSize);
        }

        public Vector2 GetGroupOffsetStart(Vector2 maxBounds, Vector2 groupSize)
        {
            return Vector2.zero;
        }

        public Vector2 GetGroupOffsetCenter(Vector2 maxBounds, Vector2 groupSize)
        {
            return new Vector2((maxBounds.x - groupSize.x - _grid.ContentPaddingLeft - _grid.ContentPaddingRight) / 2.0f, 0f);
        }

        public Vector2 GetGroupOffsetEnd(Vector2 maxBounds, Vector2 groupSize)
        {
            return new Vector2(maxBounds.x - groupSize.x - _grid.ContentPaddingLeft - _grid.ContentPaddingRight, 0f);
        }

        public Vector2 GetGroupExpandSize(Vector2 maxBounds, Vector2 groupSize)
        {
            return new Vector2(maxBounds.x - _grid.ContentPaddingLeft - _grid.ContentPaddingRight, groupSize.y);
        }

        public float GetExpandAvailable(GroupLayout groupLayout)
        {
            return groupLayout.logicRect.width - groupLayout.flowDirectionSize;
        }

        public float IncrementExpandAvailable(Vector2 cellMinSize)
        {
            return cellMinSize.x;
        }

        public Vector2 GetActualCellSizeExpand(Vector2 cellMinSize, float expandBy, float groupMaxCellSize)
        {
            return new Vector2(cellMinSize.x + expandBy, groupMaxCellSize);
        }

        public Vector2 GetActualCellSizeFixed(Vector2 cellMinSize, float groupMaxCellSize)
        {
            return new Vector2(cellMinSize.x, groupMaxCellSize);
        }

        public Vector2 GetInitialCellOffset(Rect groupRect, Vector2 firstGroupCellActualSize)
        {
            return groupRect.position;
        }

        public int GetCellDataIndex(int referenceIndex, int groupStartCellDataIndex, int groupEndCellDataIndex)
        {
            return referenceIndex;
        }

        public int GetNextCellDataIndex(int referenceIndex, int groupStartCellDataIndex, int groupEndCellDataIndex)
        {
            return (referenceIndex < groupEndCellDataIndex ? referenceIndex + 1 : -1);
        }

        public Vector2 GetCellOffset(Vector2 currentCellActualSize, Vector2 nextCellActualSize)
        {
            return new Vector2(currentCellActualSize.x + _grid.CellLayoutSpacing, 0f);
        }
    }
}