namespace echo17.EnhancedUI.EnhancedGrid.Demo_01
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
	/// A controller class for a simple grid setup. This demo does not use a model, instead
	/// just using the underlying data index for the view's display.
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        /// <summary>
		/// link to the grid
		/// </summary>
        public EnhancedGrid grid;

        /// <summary>
		/// link to the cell prefab game object used for cell views
		/// </summary>
        public GameObject cellPrefab;

        /// <summary>
		/// The number of cells to create
		/// </summary>
        public int numberOfCells;

        /// <summary>
		/// The size of the cells. In this demo, all cells are the same size
		/// </summary>
        public Vector2 cellSize;

        void Awake()
        {
            // set the target frame rate to reduce stutter on some devices
            Application.targetFrameRate = 60;
        }

        void Start()
        {
            // tell the grid to use this controller
            grid.InitializeGrid(this);

            // regenerate the grid
            grid.RecalculateGrid();
        }

        /// <summary>
		/// Tells the grid how many cells to use
		/// </summary>
		/// <param name="grid">The requesting grid</param>
		/// <returns></returns>
        public int GetCellCount(EnhancedGrid grid)
        {
            // we aren't using a model for this demo,
			// so just return the number of cells specified in the inspector
            return numberOfCells;
        }

        /// <summary>
		/// Tells the grid what cell prefab to use for a specific game index
		/// </summary>
		/// <param name="grid">The requesting grid</param>
		/// <param name="dataIndex">The data index of the cell</param>
		/// <returns></returns>
        public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
        {
            // there is only one prefab type for this demo, so just pass it back
            return cellPrefab;
        }

        /// <summary>
		/// Tells the grid the size and size mode of the cell for a specific index
		/// </summary>
		/// <param name="grid">The requesting grid</param>
		/// <param name="dataIndex">The data index of the cell</param>
		/// <returns></returns>
        public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
        {
            // all the cells in this demo share the same size, so just pass back the fixed size
            return new CellProperties(cellSize, 0f);
        }

        /// <summary>
		/// The grid calls this method to let you update your cell view
		/// </summary>
		/// <param name="grid">The calling grid</param>
		/// <param name="cell">The cell properties based on the IEnhancedGridCell</param>
		/// <param name="dataIndex">The data index of the cell</param>
		/// <param name="repeatIndex">The repeat index of the cell in case of looping or filling</param>
		/// <param name="cellLayout">The physical attributes of the cell</param>
		/// <param name="groupLayout">The physical attributes of the row or column the cell is contained within</param>
        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            // cast our cell as the view for this demo and call the view's UpdateCell method
            (cell as View).UpdateCell();
        }
    }
}