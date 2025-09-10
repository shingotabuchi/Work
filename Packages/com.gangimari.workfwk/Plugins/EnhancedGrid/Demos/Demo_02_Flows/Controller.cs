namespace echo17.EnhancedUI.EnhancedGrid.Demo_02
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using EnhancedUI.Helpers;

    /// <summary>
	/// This demo shows how you can use different flow layouts to
	/// display your cells. The grid relies on a serialized data set (one dimensional)
	/// so that it can format the cells into different layouts.
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        /// <summary>
		/// The grid to show the cells
		/// </summary>
        public EnhancedGrid grid;

        /// <summary>
        /// Watcher to notify us of viewport size changes
        /// </summary>
        public ViewportChangeWatcher viewportChangeWatcher;

        /// <summary>
		/// The cell prefab game object to use for the cell view
		/// </summary>
        public GameObject cellPrefab;

        /// <summary>
		/// The number of cells to create
		/// </summary>
        public int numberOfCells;

        /// <summary>
		/// The minimum size of the cell. This demo randomizes
		/// between the minimum and maximum sizes to give some variety.
		/// </summary>
        public Vector2 cellMinSize;

        /// <summary>
		/// The maximum size of the cell. This demo randomizes
		/// between the minimum and maximum sizes to give some variety.
		/// </summary>
        public Vector2 cellMaxSize;

        /// <summary>
		/// The random seed to use for cell size generation. The seed
		/// allows you to get consistent results between runs.
		/// </summary>
        public int randomSeed;

        /// <summary>
		/// The dropdown to determine the flow direction for this demo
		/// </summary>
        public Dropdown flowDirectionDropdown;

        /// <summary>
		/// The dropdown to determine the group alignment for this demo
		/// </summary>
        public Dropdown groupAlignmentDropdown;

        /// <summary>
		/// An image to show the different flow layouts for clarity
		/// </summary>
        public Image descriptionImage;

        /// <summary>
		/// The underlying model (data) for the grid
		/// </summary>
        private List<Model> _data;

        void Awake()
        {
            // set the random seed
            UnityEngine.Random.InitState(randomSeed);

            Application.targetFrameRate = 60;

            // hook into the viewport change watcher's notification
            viewportChangeWatcher.viewportChanged = _ViewportChanged;
        }

        void Start()
        {
            grid.InitializeGrid(this);

            // create some data
            _data = new List<Model>();

            for (var i = 0; i < numberOfCells; i++)
            {
                var modelElement = new Model();

                // randomize the cell size based on the seed
                modelElement.cellSize = new Vector2(
                                            UnityEngine.Random.Range(cellMinSize.x, cellMaxSize.x),
                                            UnityEngine.Random.Range(cellMinSize.y, cellMaxSize.y)
                                            );

                _data.Add(modelElement);
            }

            // show the description of the currently selected layout
            _SetImageDescription();

            grid.RecalculateGrid();
        }

        public int GetCellCount(EnhancedGrid grid)
        {
            // this demo uses a model, so return the number of data elements as the cell count
            return _data.Count;
        }

        public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
        {
            return cellPrefab;
        }

        public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
        {
            // this demo uses different cell sizes, so pass back the size for this cell,
            // which is stored in the model
            return new CellProperties(
                                        _data[dataIndex].cellSize,
                                        0f
                                        );
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            (cell as View).UpdateCell();
        }

        /// <summary>
		/// The flow direction dropdown has changed value.
		/// We set the grid flow direction based on the dropdown and update the image description
		/// </summary>
        public void FlowDirectionDropdown_OnValueChanged()
        {
            grid.FlowDirection = (EnhancedGrid.EnhancedGridFlowDirection)flowDirectionDropdown.value;

            _SetImageDescription();

            grid.RecalculateGrid();
        }

        /// <summary>
		/// The group alignment dropdown has changed value.
		/// We set the grid group alignment based on the dropdown and update the image description
		/// </summary>
        public void GroupAlignmentDropdown_OnValueChanged()
        {
            grid.FlowGroupAlignment = (EnhancedGrid.EnhancedGridFlowGroupAlignment)groupAlignmentDropdown.value;

            _SetImageDescription();

            grid.RecalculateGrid();
        }

        /// <summary>
		/// Class to show the flow and group alignments visually
		/// </summary>
        private void _SetImageDescription()
        {
            string flowDirectionString = "";

            switch (flowDirectionDropdown.value)
            {
                case 0: flowDirectionString = "lrtb"; break;
                case 1: flowDirectionString = "rltb"; break;
                case 2: flowDirectionString = "lrbt"; break;
                case 3: flowDirectionString = "rlbt"; break;
                case 4: flowDirectionString = "tblr"; break;
                case 5: flowDirectionString = "btlr"; break;
                case 6: flowDirectionString = "tbrl"; break;
                case 7: flowDirectionString = "btrl"; break;
            }

            string groupAlignmentString = "";

            switch (groupAlignmentDropdown.value)
            {
                case 0: groupAlignmentString = "start"; break;
                case 1: groupAlignmentString = "center"; break;
                case 2: groupAlignmentString = "end"; break;
            }

            string imageName = $"flow_{flowDirectionString}_{groupAlignmentString}";

            var sprite = Resources.Load<Sprite>($"EnhancedGridDemos/Images/{imageName}");

            descriptionImage.sprite = sprite;
        }

        /// <summary>
        /// If the viewport size changed, then recalculate the grid
        /// </summary>
        /// <param name="grid">The grid that changed</param>
        /// <param name="newSize">The new size of the viewport</param>
        private void _ViewportChanged(EnhancedGrid grid, Vector2 newSize)
        {
            grid.RecalculateGrid();
        }
    }
}