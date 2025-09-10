namespace echo17.EnhancedUI.EnhancedGrid.Demo_03
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
	/// Demo showing how you can expand cells in a grid.
	/// If an individual cell size mode is set to fixed, then it will not
	/// expand to take up the remaining space of a grid group.
	/// If the cell is set to expand, then it will share the remaining space of
	/// a grid group with other cells in the same group that also have
	/// their size mode set to expand.
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject cellPrefab;
        public int numberOfCells;
        public Vector2 cellMinSize;
        public Vector2 cellMaxSize;
        public int randomSeed;
        public Toggle expansionToggle;

        private List<Model> _data;

        void Awake()
        {
            UnityEngine.Random.InitState(randomSeed);

            Application.targetFrameRate = 60;
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

                // set the cell size mode. In this demo, every even numbered cell is fixed,
                // the odd ones will expand with a random expansion weight between 25% and 200%.
                // these values will be normalized so that all the cells of the group cannot take
                // more or less of the available expansion pixels in that group.
                //
                // for example: if a group has three cells and each cell takes 25% of the expansion available, for a total of 75% expansion
                // in that group, then these weights will be adjusted so that each cell takes 33% (25% * (1 / 75%)).
                //
                // another example: if a group has two cells, one with 50% and one with 200% expansion weights for a total of 250% expansion,
                // then these values will be normalized to 20% (50% * (1 / 250%)) and 80% (200% * (1 / 250%)), respectively.
                modelElement.expansionWeight = i % 2 == 0 ? 0f : UnityEngine.Random.Range(0.25f, 2.0f);

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
            // pass back the cell size and size mode based on the model (data) for the cell
            return new CellProperties(_data[dataIndex].cellSize, _data[dataIndex].expansionWeight);
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            // pass the normalized expansion weight of the cell properties to display in the view.
            // the normlized expansion weight is the weight adjusted so that
            // all the expanding cells of a group will utilized the full amount
            // of available expansion available in the group, without going over
            // or under the amount.
            (cell as View).UpdateCell(expansionToggle.isOn, _data[dataIndex], cellLayout.cellProperties.expansionWeight, groupLayout.expansionAvailable);
        }

        /// <summary>
		/// The expand group toggle value changed, so show the appropriate grid alignment
		/// </summary>
		/// <param name="expandGroupToggle"></param>
        public void ExpandGroup_OnValueChanged(Toggle expandGroupToggle)
        {
            if (expandGroupToggle.isOn)
            {
                grid.FlowGroupAlignment = EnhancedGrid.EnhancedGridFlowGroupAlignment.Expand;
            }
            else
            {
                grid.FlowGroupAlignment = EnhancedGrid.EnhancedGridFlowGroupAlignment.Start;
            }

            // recalculate the grid, but keep the same position since no data changed
            grid.RecalculateGrid_MaintainPosition();
        }
    }
}