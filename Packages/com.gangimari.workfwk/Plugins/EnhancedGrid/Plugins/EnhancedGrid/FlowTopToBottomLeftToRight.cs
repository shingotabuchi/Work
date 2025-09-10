namespace echo17.EnhancedUI.EnhancedGrid
{
    using UnityEngine;
    using Helpers;

    public struct FlowTopToBottomLeftToRight : IFlow
    {
        private EnhancedGrid _grid;

        public FlowTopToBottomLeftToRight(EnhancedGrid grid)
        {
            _grid = grid;
        }

        public EnhancedGrid.EnhancedGridFlowDirection GetFlowDirection()
        {
            return EnhancedGrid.EnhancedGridFlowDirection.TopToBottomLeftToRight;
        }

        public bool IsEndOfGroup(float groupFlowDirectionSize, Vector2 cellMinSize)
        {
            return (groupFlowDirectionSize + _grid.CellLayoutSpacing + cellMinSize.y + _grid.ContentPaddingTop + _grid.ContentPaddingBottom) >= _grid.WrapSize;
        }

        public float ResetFlowDirectionSize(Vector2 cellMinSize)
        {
            return cellMinSize.y;
        }

        public float IncrementFlowDirectionSize(Vector2 cellMinSize)
        {
            return (_grid.CellLayoutSpacing + cellMinSize.y);
        }

        public float GetGroupMaxCellSize(float currentGroupMaxCellSize, Vector2 cellMinSize)
        {
            return Mathf.Max(currentGroupMaxCellSize, cellMinSize.x);
        }

        public Vector2 GetMaxBounds(float maxGroupFlowDirectionSize, float totalGroupSize)
        {
            return new Vector2(_grid.ContentPaddingLeft + _grid.ContentPaddingRight + totalGroupSize, _grid.ContentPaddingTop + _grid.ContentPaddingBottom + maxGroupFlowDirectionSize);
        }

        public int GetGroupIndex(int referenceIndex, int groupCount)
        {
            return referenceIndex;
        }

        public Vector2 GetInitialGroupPosition(float groupOffset)
        {
            return new Vector2(_grid.ContentPaddingLeft + groupOffset, _grid.ContentPaddingTop);
        }

        public Vector2 GetInitialGroupSize(float groupFlowDirectionSize, float groupMaxCellSize)
        {
            return new Vector2(groupMaxCellSize, groupFlowDirectionSize);
        }

        public Vector2 GetGroupOffsetStart(Vector2 maxBounds, Vector2 groupSize)
        {
            return Vector2.zero;
        }

        public Vector2 GetGroupOffsetCenter(Vector2 maxBounds, Vector2 groupSize)
        {
            return new Vector2(0f, (maxBounds.y - groupSize.y - _grid.ContentPaddingTop - _grid.ContentPaddingBottom) / 2.0f);
        }

        public Vector2 GetGroupOffsetEnd(Vector2 maxBounds, Vector2 groupSize)
        {
            return new Vector2(0f, maxBounds.y - groupSize.y - _grid.ContentPaddingTop - _grid.ContentPaddingBottom);
        }

        public Vector2 GetGroupExpandSize(Vector2 maxBounds, Vector2 groupSize)
        {
            return new Vector2(groupSize.x, maxBounds.y - _grid.ContentPaddingTop - _grid.ContentPaddingBottom);
        }

        public float GetExpandAvailable(GroupLayout groupLayout)
        {
            return groupLayout.logicRect.height - groupLayout.flowDirectionSize;
        }

        public float IncrementExpandAvailable(Vector2 cellMinSize)
        {
            return cellMinSize.y;
        }

        public Vector2 GetActualCellSizeExpand(Vector2 cellMinSize, float expandBy, float groupMaxCellSize)
        {
            return new Vector2(groupMaxCellSize, cellMinSize.y + expandBy);
        }

        public Vector2 GetActualCellSizeFixed(Vector2 cellMinSize, float groupMaxCellSize)
        {
            return new Vector2(groupMaxCellSize, cellMinSize.y);
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
            return new Vector2(0f, currentCellActualSize.y + _grid.CellLayoutSpacing);
        }
    }
}