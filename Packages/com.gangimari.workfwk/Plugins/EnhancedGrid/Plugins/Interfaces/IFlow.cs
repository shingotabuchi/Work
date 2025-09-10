namespace echo17.EnhancedUI.EnhancedGrid
{
    using UnityEngine;
    using Helpers;

    /// <summary>
    /// Interface used internally for the flow of the grid cells.
    /// </summary>
    public interface IFlow
    {
        EnhancedGrid.EnhancedGridFlowDirection GetFlowDirection();
        bool IsEndOfGroup(float groupFlowDirectionSize, Vector2 cellMinSize);
        float ResetFlowDirectionSize(Vector2 cellMinSize);
        float IncrementFlowDirectionSize(Vector2 cellMinSize);
        float GetGroupMaxCellSize(float currentGroupMaxCellSize, Vector2 cellMinSize);
        Vector2 GetMaxBounds(float maxGroupFlowDirectionSize, float totalGroupSize);
        Vector2 GetInitialGroupPosition(float groupOffset);
        int GetGroupIndex(int referenceIndex, int groupCount);
        Vector2 GetInitialGroupSize(float groupFlowDirectionSize, float groupMaxCellSize);
        Vector2 GetGroupOffsetStart(Vector2 maxBounds, Vector2 groupSize);
        Vector2 GetGroupOffsetCenter(Vector2 maxBounds, Vector2 groupSize);
        Vector2 GetGroupOffsetEnd(Vector2 maxBounds, Vector2 groupSize);
        Vector2 GetGroupExpandSize(Vector2 maxBounds, Vector2 groupSize);
        float GetExpandAvailable(GroupLayout groupLayout);
        float IncrementExpandAvailable(Vector2 cellMinSize);
        Vector2 GetActualCellSizeExpand(Vector2 cellMinSize, float expandBy, float groupMaxCellSize);
        Vector2 GetActualCellSizeFixed(Vector2 cellMinSize, float groupMaxCellSize);
        Vector2 GetInitialCellOffset(Rect groupRect, Vector2 firstGroupCellActualSize);
        int GetCellDataIndex(int referenceIndex, int groupStartCellDataIndex, int groupEndCellDataIndex);
        int GetNextCellDataIndex(int referenceIndex, int groupStartCellDataIndex, int groupEndCellDataIndex);
        Vector2 GetCellOffset(Vector2 currentCellActualSize, Vector2 nextCellActualSize);
    }
}
