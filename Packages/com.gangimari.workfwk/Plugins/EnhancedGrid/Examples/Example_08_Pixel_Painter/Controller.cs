namespace echo17.EnhancedUI.EnhancedGrid.Example_08
{
    using UnityEngine;
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
	/// Example showing how you can use a grid with selection to create pixel art.
	///
	/// The Controller handling the grid and the logic for pixel painting are separated
	/// into two separate files for clarity, both with partial classes of Controller.
	/// </summary>
    public partial class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject cellPrefab;
        public Vector2 pixelSize;

        private void _SetupGrid()
        {
            grid.InitializeGrid(this);
            grid.RecalculateGrid();
        }

        public int GetCellCount(EnhancedGrid grid)
        {
            return canvasSize.x * canvasSize.y;
        }

        public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
        {
            return cellPrefab;
        }

        public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
        {
            return new CellProperties(pixelSize, 0f);
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            var location = _GetLocationFromDataIndex(dataIndex);
            (cell as PixelView).UpdateCell(_pixels[location.x, location.y]);
        }
    }
}