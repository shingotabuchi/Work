namespace echo17.EnhancedUI.EnhancedGrid
{
    using UnityEngine;

    /// <summary>
    /// Controllers need to implement this interface.
    /// </summary>
    public interface IEnhancedGridDelegate
    {
        /// <summary>
        /// Returns the number of cells that will be in the scroller.
        /// </summary>
        /// <param name="grid">Grid requesting the value</param>
        /// <returns>Number of cells in grid</returns>
        int GetCellCount(EnhancedGrid grid);

        /// <summary>
        /// Gets the size and size mode of a cell.
        /// </summary>
        /// <param name="grid">Grid requesting the value</param>
        /// <param name="dataIndex">The dataIndex of the cell</param>
        /// <returns>The size and size mode of a cell</returns>
        CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex);

        /// <summary>
        /// Gets the prefab game object to instantiate for a cell view.
        /// </summary>
        /// <param name="grid">Grid requesing the prefab</param>
        /// <param name="dataIndex">The dataIndex of the cell</param>
        /// <returns>The prefab of the cell view</returns>
        GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex);

        /// <summary>
        /// Updates a cell when it becomes visible.
        /// </summary>
        /// <param name="grid">Grid calling the update</param>
        /// <param name="cell">The cell that needs updated</param>
        /// <param name="dataIndex">The dataIndex of the cell</param>
        /// <param name="repeatIndex">The repeatIndex of the cell</param>
        /// <param name="cellLayout">The cell's layout in the grid</param>
        /// <param name="groupLayout">The cell's parent group layout in the grid</param>
        void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout);
    }
}
