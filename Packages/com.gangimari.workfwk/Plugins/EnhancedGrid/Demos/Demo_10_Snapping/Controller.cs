namespace echo17.EnhancedUI.EnhancedGrid.Demo_10
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
	/// Demo showing how you can attach a snap addon component to cause the grid to snap
	/// to a position based on various parameters.
	///
	/// The snap velocity thresholds tell the grid when to start snapping based on how fast
	/// the grid is scrolling. When it dips below the threshold, it will begin the snap.
	///
	/// The snap watch viewport offset is where in the visible area to watch for a cell to snap.
	/// This will typically be the same as the snap jump to viewport offset, but not necessarily.
	///
	/// The snap jump to viewport offset is where to move the cell to within the visible area.
	/// This will typically be the same as the snap watch viewport offset, but not necessarily.
	///
	/// The jump to cell offset is where to center the cell when it jumps to the new location.
	/// 
	/// Each axis can have a different tween type and duration.
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject cellPrefab;
        public int numberOfCells;
        public Vector2 cellSize;

        public InputField snapVelocityThresholdXInputField;
        public InputField snapVelocityThresholdYInputField;
        public Slider snapWatchViewportOffsetXSlider;
        public Slider snapWatchViewportOffsetYSlider;
        public Slider snapJumpToViewportOffsetXSlider;
        public Slider snapJumpToViewportOffsetYSlider;
        public Slider snapJumpToCellOffsetXSlider;
        public Slider snapJumpToCellOffsetYSlider;
        public Dropdown snapTweenTypeXDropdown;
        public InputField snapTweenTimeXInputField;
        public Dropdown snapTweenTypeYDropdown;
        public InputField snapTweenTimeYInputField;

        private List<Model> _data;
        private EnhancedGridSnap _snap;

        void Awake()
        {
            _snap = grid.GetComponent<EnhancedGridSnap>();

            Application.targetFrameRate = 60;

            // fill the dropdowns with values from EnhancedGrid

            var tweenTypes = Enum.GetValues(typeof(Helpers.TweenType));
            List<Dropdown.OptionData> tweenTypeOptions = new List<Dropdown.OptionData>();

            for (var i = 0; i < tweenTypes.Length; i++)
            {
                tweenTypeOptions.Add(new Dropdown.OptionData()
                {
                    text = Enum.GetName(typeof(Helpers.TweenType), i)
                });
            }

            snapVelocityThresholdXInputField.text = _snap.SnapVelocityThreshold.x.ToString();
            snapVelocityThresholdYInputField.text = _snap.SnapVelocityThreshold.y.ToString();

            snapWatchViewportOffsetXSlider.value = _snap.SnapWatchViewportOffset.x;
            snapWatchViewportOffsetYSlider.value = _snap.SnapWatchViewportOffset.x;

            snapJumpToViewportOffsetXSlider.value = _snap.SnapJumpToViewportOffset.x;
            snapJumpToViewportOffsetYSlider.value = _snap.SnapJumpToViewportOffset.x;

            snapJumpToCellOffsetXSlider.value = _snap.SnapJumpToCellOffset.x;
            snapJumpToCellOffsetYSlider.value = _snap.SnapJumpToCellOffset.x;

            snapTweenTypeXDropdown.AddOptions(tweenTypeOptions);
            snapTweenTypeXDropdown.value = (int)_snap.SnapTweenTypeX;

            snapTweenTimeXInputField.text = _snap.SnapTweenTimeX.ToString();

            snapTweenTypeYDropdown.AddOptions(tweenTypeOptions);
            snapTweenTypeYDropdown.value = (int)_snap.SnapTweenTypeY;

            snapTweenTimeYInputField.text = _snap.SnapTweenTimeY.ToString();
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

            // set up some delegates to call when different actions occur
            _snap.gridSnapStarted = _GridSnapStarted;
            _snap.gridSnapCompleted = _GridSnapCompleted;
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
            (cell as View).UpdateCell();
        }

        /// <summary>
		/// Called when the grid starts snapping
		/// </summary>
		/// <param name="grid">The grid that started snapping</param>
		/// <param name="snap">The snap component of the grid</param>
		/// <param name="snapDataIndex">The data index of the cell that is being snapped</param>
        private void _GridSnapStarted(EnhancedGrid grid, EnhancedGridSnap snap, int snapDataIndex)
        {
            if (grid == this.grid)
            {
                Debug.Log($"Grid started snap to data index {snapDataIndex}");
            }
        }

        /// <summary>
		/// Called when the grid stops snapping
		/// </summary>
		/// <param name="grid">The grid that stopped snapping</param>
		/// <param name="snap">The snap component of the grid</param>
		/// <param name="snapDataIndex">The data index of the cell that is being snapped</param>
        private void _GridSnapCompleted(EnhancedGrid grid, EnhancedGridSnap snap, int snapDataIndex)
        {
            if (grid == this.grid)
            {
                Debug.Log($"Grid completed snapped to data index {snapDataIndex}");
            }
        }

        public void SnapVelocityThresholdXInputField_OnValueChanged(InputField inputField)
        {
            float value;
            float.TryParse(inputField.text.Trim(), out value);

            _snap.SnapVelocityThreshold = new Vector2(value, _snap.SnapVelocityThreshold.y);
        }

        public void SnapVelocityThresholdYInputField_OnValueChanged(InputField inputField)
        {
            float value;
            float.TryParse(inputField.text.Trim(), out value);

            _snap.SnapVelocityThreshold = new Vector2(_snap.SnapVelocityThreshold.x, value);
        }

        public void SnapWatchViewportOffsetXSlider_OnValueChanged(Slider slider)
        {
            _snap.SnapWatchViewportOffset = new Vector2(slider.value, _snap.SnapWatchViewportOffset.y);
        }

        public void SnapWatchViewportOffsetYSlider_OnValueChanged(Slider slider)
        {
            _snap.SnapWatchViewportOffset = new Vector2(_snap.SnapWatchViewportOffset.x, slider.value);
        }

        public void SnapJumpToViewportOffsetXSlider_OnValueChanged(Slider slider)
        {
            _snap.SnapJumpToViewportOffset = new Vector2(slider.value, _snap.SnapJumpToViewportOffset.y);
        }

        public void SnapJumpToViewportOffsetYSlider_OnValueChanged(Slider slider)
        {
            _snap.SnapJumpToViewportOffset = new Vector2(_snap.SnapJumpToViewportOffset.x, slider.value);
        }

        public void SnapJumpToCellOffsetXSlider_OnValueChanged(Slider slider)
        {
            _snap.SnapJumpToCellOffset = new Vector2(slider.value, _snap.SnapJumpToCellOffset.y);
        }

        public void SnapJumpToCellOffsetYSlider_OnValueChanged(Slider slider)
        {
            _snap.SnapJumpToCellOffset = new Vector2(_snap.SnapJumpToCellOffset.x, slider.value);
        }

        public void SnapTweenTypeXDropdown_OnValueChanged(Dropdown dropDown)
        {
            _snap.SnapTweenTypeX = (Helpers.TweenType)dropDown.value;
        }

        public void SnapTweenTimeXInputField_OnValueChanged(InputField inputField)
        {
            float value;
            float.TryParse(inputField.text.Trim(), out value);

            _snap.SnapTweenTimeX = value;
        }

        public void SnapTweenTypeYDropdown_OnValueChanged(Dropdown dropDown)
        {
            _snap.SnapTweenTypeY = (Helpers.TweenType)dropDown.value;
        }

        public void SnapTweenTimeYInputField_OnValueChanged(InputField inputField)
        {
            float value;
            float.TryParse(inputField.text.Trim(), out value);

            _snap.SnapTweenTimeY = value;
        }
    }
}