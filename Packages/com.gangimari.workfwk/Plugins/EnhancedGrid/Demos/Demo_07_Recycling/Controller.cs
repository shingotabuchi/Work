namespace echo17.EnhancedUI.EnhancedGrid.Demo_07
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
	/// Demo giving a better visualization of how the recycling works behind the scenes.
	/// 
	/// When recycling is on, the grid will search for visible cells based on an occlusion grid.
	/// It starts in the highest level of the grid, and if the cell is in that grid, it recursively
	/// checks its child grids to see if they fall inside the viewable area.
	///
	/// If recycling is on, then the viewable area is the scroll view rect plus the extended visible
	/// area. You can make your extended visible area larger so that the grid doesn't have to constantly
	/// check for new visible cells. 
	///
	/// When recycling is off, occlusion and extended visible areas are not relevant since all cells are created.
	///
	/// Note: Your minimum scroll for active cell update should be less than the extended visible area or you
	/// will see "popping" of cells becoming active after entering the view area.
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject cellPrefab;
        public int numberOfCells;
        public Vector2 cellMinSize;
        public Vector2 cellMaxSize;
        public int randomSeed;

        public Toggle recycleToggle;
        public GameObject recycleSettings;
        public Slider occlusionDepthSlider;
        public InputField occlusionDepthInputField;
        public InputField extendVisibleAreaTopInputField;
        public InputField extendVisibleAreaBottomInputField;
        public InputField extendVisibleAreaLeftInputField;
        public InputField extendVisibleAreaRightInputField;
        public InputField minimumScrollXInputField;
        public InputField minimumScrollYInputField;
        public GameObject warning;

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

                _data.Add(modelElement);
            }

            grid.RecalculateGrid();

            recycleToggle.isOn = grid.RecycleCells;
            recycleSettings.SetActive(grid.RecycleCells);

            occlusionDepthSlider.value = grid.OcclusionDepth;
            occlusionDepthInputField.text = grid.OcclusionDepth.ToString();

            extendVisibleAreaTopInputField.text = grid.ExtendVisibleAreaTop.ToString();
            extendVisibleAreaBottomInputField.text = grid.ExtendVisibleAreaBottom.ToString();
            extendVisibleAreaLeftInputField.text = grid.ExtendVisibleAreaLeft.ToString();
            extendVisibleAreaRightInputField.text = grid.ExtendVisibleAreaRight.ToString();

            minimumScrollXInputField.text = grid.MinimumScrollForActiveCellUpdate.x.ToString();
            minimumScrollYInputField.text = grid.MinimumScrollForActiveCellUpdate.y.ToString();
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
		/// If recycling has changed, we need to rebuild the grid
		/// </summary>
		/// <param name="recycleToggle"></param>
        public void Recycle_OnValueChanged(Toggle recycleToggle)
        {
            grid.RecycleCells = recycleToggle.isOn;

            recycleSettings.SetActive(grid.RecycleCells);

            grid.RecalculateGrid_MaintainPosition();
        }

        public void OcclusionDepthSlider_OnValueChanged()
        {
            occlusionDepthInputField.text = occlusionDepthSlider.value.ToString();
        }

        public void RecalculateGridButton_OnClick()
        {
            int occlusionDepth;
            float extendVisibleAreaTop;
            float extendVisibleAreaBottom;
            float extendVisibleAreaLeft;
            float extendVisibleAreaRight;
            float minimumScrollX;
            float minimumScrollY;

            // grab and clean settings

            int.TryParse(occlusionDepthInputField.text.Trim(), out occlusionDepth);
            float.TryParse(extendVisibleAreaTopInputField.text.Trim(), out extendVisibleAreaTop);
            float.TryParse(extendVisibleAreaBottomInputField.text.Trim(), out extendVisibleAreaBottom);
            float.TryParse(extendVisibleAreaLeftInputField.text.Trim(), out extendVisibleAreaLeft);
            float.TryParse(extendVisibleAreaRightInputField.text.Trim(), out extendVisibleAreaRight);
            float.TryParse(minimumScrollXInputField.text.Trim(), out minimumScrollX);
            float.TryParse(minimumScrollYInputField.text.Trim(), out minimumScrollY);

            occlusionDepth = Mathf.Clamp(occlusionDepth, 1, 5);
            extendVisibleAreaTop = Mathf.Clamp(extendVisibleAreaTop, 0, extendVisibleAreaTop);
            extendVisibleAreaBottom = Mathf.Clamp(extendVisibleAreaBottom, 0, extendVisibleAreaBottom);
            extendVisibleAreaLeft = Mathf.Clamp(extendVisibleAreaLeft, 0, extendVisibleAreaLeft);
            extendVisibleAreaRight = Mathf.Clamp(extendVisibleAreaRight, 0, extendVisibleAreaRight);
            minimumScrollX = Mathf.Clamp(minimumScrollX, 0, minimumScrollX);
            minimumScrollY = Mathf.Clamp(minimumScrollY, 0, minimumScrollY);

            occlusionDepthInputField.text = occlusionDepth.ToString();
            extendVisibleAreaTopInputField.text = extendVisibleAreaTop.ToString();
            extendVisibleAreaBottomInputField.text = extendVisibleAreaBottom.ToString();
            extendVisibleAreaLeftInputField.text = extendVisibleAreaLeft.ToString();
            extendVisibleAreaRightInputField.text = extendVisibleAreaRight.ToString();
            minimumScrollXInputField.text = minimumScrollX.ToString();
            minimumScrollYInputField.text = minimumScrollY.ToString();

            if (
                (minimumScrollX > extendVisibleAreaLeft || minimumScrollX > extendVisibleAreaRight)
                ||
                (minimumScrollY > extendVisibleAreaTop || minimumScrollY > extendVisibleAreaBottom)
                )
            {
                warning.SetActive(true);
            }
            else
            {
                warning.SetActive(false);
            }

            // update the grid

            grid.OcclusionDepth = occlusionDepth;
            grid.ExtendVisibleAreaTop = extendVisibleAreaTop;
            grid.ExtendVisibleAreaBottom = extendVisibleAreaBottom;
            grid.ExtendVisibleAreaLeft = extendVisibleAreaLeft;
            grid.ExtendVisibleAreaRight = extendVisibleAreaRight;
            grid.MinimumScrollForActiveCellUpdate = new Vector2(minimumScrollX, minimumScrollY);

            grid.RecalculateGrid_MaintainPosition();
        }
    }
}