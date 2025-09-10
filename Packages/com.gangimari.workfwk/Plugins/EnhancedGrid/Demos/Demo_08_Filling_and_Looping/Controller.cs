namespace echo17.EnhancedUI.EnhancedGrid.Demo_08
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
	/// Demo showing how cells can be repeated to fill the visible space, or
	/// for looping infinitely.
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject cellPrefab;
        public int numberOfCells;
        public Vector2 cellMinSize;
        public Vector2 cellMaxSize;
        public int randomSeed;

        public Dropdown repeatModeXDropdown;
        public Dropdown repeatModeYDropdown;

        private List<Model> _data;

        void Awake()
        {
            UnityEngine.Random.InitState(randomSeed);

            Application.targetFrameRate = 60;

            // fill the dropdowns with data from EnhancedGrid

            var repeatModes = Enum.GetValues(typeof(EnhancedGrid.EnhancedGridRepeatMode));
            List<Dropdown.OptionData> repeatModeOptions = new List<Dropdown.OptionData>();

            for (var i = 0; i < repeatModes.Length; i++)
            {
                repeatModeOptions.Add(new Dropdown.OptionData()
                {
                    text = Enum.GetName(typeof(EnhancedGrid.EnhancedGridRepeatMode), i)
                });
            }

            repeatModeXDropdown.AddOptions(repeatModeOptions);
            repeatModeXDropdown.value = (int)EnhancedGrid.EnhancedGridRepeatMode.Loop;

            repeatModeYDropdown.AddOptions(repeatModeOptions);
            repeatModeYDropdown.value = (int)EnhancedGrid.EnhancedGridRepeatMode.Loop;

            grid.RepeatModeX = (EnhancedGrid.EnhancedGridRepeatMode)repeatModeXDropdown.value;
            grid.RepeatModeY = (EnhancedGrid.EnhancedGridRepeatMode)repeatModeYDropdown.value;
        }

        void Start()
        {
            grid.InitializeGrid(this);

            _data = new List<Model>();

            for (var i = 0; i < numberOfCells; i++)
            {
                var modelElement = new Model();

                modelElement.cellSize = new Vector2(
                                                    UnityEngine.Random.Range(cellMinSize.x, cellMaxSize.x),
                                                    UnityEngine.Random.Range(cellMinSize.y, cellMaxSize.y)
                                                    );

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
            return new CellProperties(_data[dataIndex].cellSize, 0f);
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            (cell as View).UpdateCell(_data[dataIndex]);
        }

        /// <summary>
		/// Sets the repeat mode of the grid based on the dropdown selections
		/// </summary>
        public void RecalculateGridButton_OnClick()
        {
            grid.RepeatModeX = (EnhancedGrid.EnhancedGridRepeatMode)repeatModeXDropdown.value;
            grid.RepeatModeY = (EnhancedGrid.EnhancedGridRepeatMode)repeatModeYDropdown.value;

            grid.RecalculateGrid();
        }
    }
}