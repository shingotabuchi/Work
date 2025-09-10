namespace echo17.EnhancedUI.EnhancedGrid.Demo_04
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
	/// Demo showing how you can align cells to their layouts
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject cellPrefab;
        public int numberOfCells;
        public Vector2 cellMinSize;
        public Vector2 cellMaxSize;
        public int randomSeed;

        public Slider sliderAnchorX;
        public Slider sliderAnchorY;
        public Slider sliderPivotX;
        public Slider sliderPivotY;

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

                // default the anchor and pivot to the upper left corner
                // these are in unity coordinates where the y values go from
                // bottom to top. Note that this is different from EnhancedGrid,
                // which goes a more natural top to bottom.
                modelElement.anchor = new Vector2(0f, 1f);
                modelElement.pivot = new Vector2(0f, 1f);

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
            // update the cell view, aligning every even cell with its anchor and pivot
            (cell as View).UpdateCell(_data[dataIndex], _IsAligning(dataIndex));
        }

        /// <summary>
		/// In this example, the even cells will be the ones that align to their anchor and pivot
		/// to distinguish from non-aligning cells
		/// </summary>
		/// <param name="dataIndex"></param>
		/// <returns></returns>
        private bool _IsAligning(int dataIndex)
        {
            return dataIndex % 2 == 0;
        }

        /// <summary>
		/// Update the cells' x anchors
		/// </summary>
        public void SliderAnchorX_OnValueChanged()
        {
            for (var i = 0; i < _data.Count; i++)
            {
                if (_IsAligning(i))
                {
                    _data[i].anchor = new Vector2(sliderAnchorX.value, _data[i].anchor.y);
                }
            }

            // the cell sizes and number of cells didn't change, only the model data,
            // so we don't have to rebuild the grid, just refresh the cells that are visible
            grid.RefreshActiveCells();
        }

        /// <summary>
		/// Update the cell's y anchors
		/// </summary>
        public void SliderAnchorY_OnValueChanged()
        {
            for (var i = 0; i < _data.Count; i++)
            {
                if (_IsAligning(i))
                {
                    _data[i].anchor = new Vector2(_data[i].anchor.x, sliderAnchorY.value);
                }
            }

            // the cell sizes and number of cells didn't change, only the model data,
            // so we don't have to rebuild the grid, just refresh the cells that are visible
            grid.RefreshActiveCells();
        }

        /// <summary>
		/// Update the cell's x pivots
		/// </summary>
        public void SliderPivotX_OnValueChanged()
        {
            for (var i = 0; i < _data.Count; i++)
            {
                if (_IsAligning(i))
                {
                    _data[i].pivot = new Vector2(sliderPivotX.value, _data[i].pivot.y);
                }
            }

            // the cell sizes and number of cells didn't change, only the model data,
            // so we don't have to rebuild the grid, just refresh the cells that are visible
            grid.RefreshActiveCells();
        }

        /// <summary>
		/// Update the cell's y pivots
		/// </summary>
        public void SliderPivotY_OnValueChanged()
        {
            for (var i = 0; i < _data.Count; i++)
            {
                if (_IsAligning(i))
                {
                    _data[i].pivot = new Vector2(_data[i].pivot.x, sliderPivotY.value);
                }
            }

            // the cell sizes and number of cells didn't change, only the model data,
            // so we don't have to rebuild the grid, just refresh the cells that are visible
            grid.RefreshActiveCells();
        }
    }
}