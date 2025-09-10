namespace echo17.EnhancedUI.EnhancedGrid.Example_05
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using EnhancedUI.Helpers;

    /// <summary>
	/// Demo showing one way to use a grid for board games. The controller
	/// and the chess logic are separated into separate files for clarity,
	/// both using the partial class of Controller.
	/// </summary>
    public partial class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject cellPrefab;
        public Vector2 boardSquareSize;

        void Awake()
        {
            Application.targetFrameRate = 60;
        }

        void Start()
        {
            _SetupBoard();

            grid.InitializeGrid(this);
            grid.RecalculateGrid();
        }

        public int GetCellCount(EnhancedGrid grid)
        {
            return _boardSquares.Length;
        }

        public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
        {
            return cellPrefab;
        }

        public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
        {
            return new CellProperties(boardSquareSize, 0f);
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            var location = _GetXYFromDataIndex(dataIndex);

            (cell as View).UpdateCell(_currentPlayerID, _boardSquares[location.x, location.y], imagePath, _SquareSelected);
        }
    }
}