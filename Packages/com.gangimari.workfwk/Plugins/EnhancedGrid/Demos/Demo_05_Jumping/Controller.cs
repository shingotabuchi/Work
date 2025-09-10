namespace echo17.EnhancedUI.EnhancedGrid.Demo_05
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
	/// Demo showing how to jump to different cells within the grid.
	/// Scroller offset is where inside the visible grid rect to center the cell onto.
	/// Cell Offset is where inside the cell to center on.
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject cellPrefab;
        public int numberOfCells;
        public Vector2 cellMinSize;
        public Vector2 cellMaxSize;
        public int randomSeed;

        public InputField jumpToDataIndexInputField;
        public Slider scrollerOffsetXSlider;
        public Slider scrollerOffsetYSlider;
        public Slider cellOffsetXSlider;
        public Slider cellOffsetYSlider;
        public Dropdown tweenTypeXDropdown;
        public InputField tweenTimeXInputField;
        public Dropdown tweenTypeYDropdown;
        public InputField tweenTimeYInputField;

        private List<Model> _data;

        void Awake()
        {
            UnityEngine.Random.InitState(randomSeed);

            Application.targetFrameRate = 60;

            // fill in the dropdowns with values from the EnhancedGrid

            var tweenTypes = Enum.GetValues(typeof(Helpers.TweenType));
            List<Dropdown.OptionData> tweenTypeOptions = new List<Dropdown.OptionData>();

            for (var i = 0; i < tweenTypes.Length; i++)
            {
                tweenTypeOptions.Add(new Dropdown.OptionData()
                {
                    text = Enum.GetName(typeof(Helpers.TweenType), i)
                });
            }

            tweenTypeXDropdown.AddOptions(tweenTypeOptions);
            tweenTypeXDropdown.value = (int)Helpers.TweenType.easeOutSine;

            tweenTypeYDropdown.AddOptions(tweenTypeOptions);
            tweenTypeYDropdown.value = (int)Helpers.TweenType.easeOutSine;
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
            (cell as View).UpdateCell();
        }

        public void JumpToButton_OnClick()
        {
            // grab and clean settings

            int jumpToDataIndex;
            var tweenTypeX = (Helpers.TweenType)tweenTypeXDropdown.value;
            float tweenTimeX;
            var tweenTypeY = (Helpers.TweenType)tweenTypeYDropdown.value;
            float tweenTimeY;
            var scrollerOffset = new Vector2(scrollerOffsetXSlider.value, scrollerOffsetYSlider.value);
            var cellOffset = new Vector2(cellOffsetXSlider.value, cellOffsetYSlider.value);

            int.TryParse(jumpToDataIndexInputField.text.Trim(), out jumpToDataIndex);
            float.TryParse(tweenTimeXInputField.text.Trim(), out tweenTimeX);
            float.TryParse(tweenTimeYInputField.text.Trim(), out tweenTimeY);

            jumpToDataIndex = Mathf.Clamp(jumpToDataIndex, 0, _data.Count - 1);
            tweenTimeX = Mathf.Clamp(tweenTimeX, 0, tweenTimeX);
            tweenTimeY = Mathf.Clamp(tweenTimeY, 0, tweenTimeY);

            jumpToDataIndexInputField.text = jumpToDataIndex.ToString();
            tweenTimeXInputField.text = tweenTimeX.ToString();
            tweenTimeYInputField.text = tweenTimeY.ToString();

            // jump

            grid.JumpToDataIndex(jumpToDataIndex,
                                 tweenTypeX: tweenTypeX,
                                 tweenTimeX: tweenTimeX,
                                 tweenTypeY: tweenTypeY,
                                 tweenTimeY: tweenTimeY,
                                 scrollerOffset: scrollerOffset,
                                 cellOffset: cellOffset,
                                 jumpCompleted: JumpCompleted
                                 );
        }

        /// <summary>
		/// Called after the jump is completed. You set this in the JumpToDataIndex method (optionally).
		/// </summary>
		/// <param name="jumpGrid"></param>
        private void JumpCompleted(EnhancedGrid jumpGrid)
        {
            if (jumpGrid == grid)
            {
                Debug.Log("Jump Completed!");
            }
        }
    }
}