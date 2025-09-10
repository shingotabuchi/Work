namespace echo17.EnhancedUI.EnhancedGrid.Demo_11
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
	/// Demo showing how you can use the model to drive cell selection.
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject cellPrefab;
        public int numberOfCells;
        public Vector2 cellSize;

        /// <summary>
        /// More than one cell can be selected
        /// </summary>
        public Toggle multiSelectToggle;

        /// <summary>
		/// Whether selecting a cell turns it off and on, or just on
		/// </summary>
        public Toggle toggleSelectionToggle;

        private List<Model> _data;

        void Awake()
        {
            Application.targetFrameRate = 60;
        }

        void Start()
        {
            grid.InitializeGrid(this);

            _data = new List<Model>();

            for (var i = 0; i < numberOfCells; i++)
            {
                var modelElement = new Model();

                _data.Add(modelElement);
            }

            grid.RecalculateGrid();
        }

        public int GetCellCount(EnhancedGrid grid)
        {
            return _data.Count;
        }

        public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
        {
            return cellPrefab;
        }

        public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
        {
            return new CellProperties(cellSize, 0f);
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            // pass a delegate method _CellSelected to the view.
            // the view will call this when an action, like a button
            // press occurs on the cell
            (cell as View).UpdateCell(_data[dataIndex], _CellSelected);
        }

        /// <summary>
		/// Called from the view when the cell is selected
		/// </summary>
		/// <param name="dataIndex">The data index of the cell</param>
        private void _CellSelected(int dataIndex)
        {
            if (toggleSelectionToggle.isOn)
            {
                // if you can turn on and off a cell, then just set the cell's selection state to the opposite of what it is
                _data[dataIndex].selected = !_data[dataIndex].selected;
            }
            else
            {
                // toggling is not set, so just always set the selection to true when the cell is selected
                _data[dataIndex].selected = true;
            }

            // if only one cell can be selected at a time, then set the other cell's data to not selected
            if (!multiSelectToggle.isOn)
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    if (i != dataIndex)
                    {
                        _data[i].selected = false;
                    }
                }
            }

            // no need to recalculate the grid, just refresh the active cells
            grid.RefreshActiveCells();
        }

        public void MultiSelectToggle_OnValueChanged()
        {
            if (!multiSelectToggle.isOn)
            {
                // multi-select was turned off, so turn off all cells if more than one cell is already selected

                int selectedCount = 0;

                for (var i = 0; i < _data.Count; i++)
                {
                    if (_data[i].selected)
                    {
                        selectedCount++;
                    }
                }

                if (selectedCount > 1)
                {
                    for (var i = 0; i < _data.Count; i++)
                    {
                        _data[i].selected = false;
                    }
                }

                grid.RefreshActiveCells();
            }
        }
    }
}