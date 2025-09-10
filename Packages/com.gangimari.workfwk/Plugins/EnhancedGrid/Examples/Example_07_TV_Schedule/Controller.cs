namespace echo17.EnhancedUI.EnhancedGrid.Example_07
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using EnhancedUI.Helpers;

    /// <summary>
	/// Example showing how to make an irregular grid, like a TV or streaming programming grid.
	/// Arrows keys are used to navigate, similar to using a remote control. Mouse is ignored.
	///
	/// Note that there are three grids: time slots, channels, and programs. When one grid is scrolled,
	/// they are all scrolled.
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid gridTimeSlots;
        public EnhancedGrid gridPrograms;
        public EnhancedGrid gridChannels;

        public GameObject viewTimeSlotPrefab;
        public GameObject viewProgramPrefab;
        public GameObject viewChannelPrefab;

        public Vector2 timeSlotCellSize;
        public Vector2 channelCellSize;

        /// <summary>
		/// Json file storing the tv schedule programming
		/// </summary>
        public TextAsset tvDatabaseData;

        public TweenType scrollTweenType;
        public float scrollTweenTime;
        public Vector2 scrollFocusPriority;

        private ModelProgramMatrix _programMatrix;

        void Awake()
        {
            Application.targetFrameRate = 60;
        }

        void Start()
        {
            // load the program matrix with data from the tv schedule database
            _programMatrix = new ModelProgramMatrix(tvDatabaseData);

            // update the grid's wrap size to the maximum width of the matrix
            gridPrograms.WrapSize = (_programMatrix.TotalTimeSlots * timeSlotCellSize.x) + gridPrograms.ContentPaddingLeft + gridPrograms.ContentPaddingRight + 1f;

            // set up all the grids to use a delegate when scrolled.
            // this allows us to sync up the positions of the grids

            gridTimeSlots.gridScrolled = _GridScrolled;
            gridPrograms.gridScrolled = _GridScrolled;
            gridChannels.gridScrolled = _GridScrolled;

            gridTimeSlots.InitializeGrid(this);
            gridPrograms.InitializeGrid(this);
            gridChannels.InitializeGrid(this);

            gridTimeSlots.RecalculateGrid();
            gridPrograms.RecalculateGrid();
            gridChannels.RecalculateGrid();
        }

        void Update()
        {
            // check for arrow key presses and change
            // the program selection accordingly. check
            // if a scroll is needed.

            if (!gridPrograms.IsTweening)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    _programMatrix.MoveSelectionLeft();
                    _RefreshAllGrids();
                    _CheckIfScrollNeeded();
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    _programMatrix.MoveSelectionRight();
                    _RefreshAllGrids();
                    _CheckIfScrollNeeded();
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _programMatrix.MoveSelectionUp();
                    _RefreshAllGrids();
                    _CheckIfScrollNeeded();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _programMatrix.MoveSelectionDown();
                    _RefreshAllGrids();
                    _CheckIfScrollNeeded();
                }
            }
        }

        public int GetCellCount(EnhancedGrid grid)
        {
            if (grid == gridTimeSlots)
            {
                return _programMatrix.TimeSlots.Count();
            }
            else if (grid == gridPrograms)
            {
                return _programMatrix.TotalProgramCount;
            }
            else if (grid == gridChannels)
            {
                return _programMatrix.Channels.Count();
            }

            return 0;
        }

        public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
        {
            if (grid == gridTimeSlots)
            {
                return viewTimeSlotPrefab;
            }
            else if (grid == gridPrograms)
            {
                return viewProgramPrefab;
            }
            else if (grid == gridChannels)
            {
                return viewChannelPrefab;
            }

            return null;
        }

        public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
        {
            // time slots and channels all have the same size.
            // program sizes vary depending on their duration.

            if (grid == gridTimeSlots)
            {
                return new CellProperties(timeSlotCellSize, 0f);
            }
            else if (grid == gridPrograms)
            {
                var program = _programMatrix.GetProgram(dataIndex);
                return new CellProperties(new Vector2(program.timeSlotCount * timeSlotCellSize.x, channelCellSize.y), 0f);
            }
            else if (grid == gridChannels)
            {
                return new CellProperties(channelCellSize, 0f);
            }

            return new CellProperties(Vector2.one, 0f);
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            if (grid == gridTimeSlots)
            {
                (cell as ViewTimeSlot).UpdateCell(_programMatrix.TimeSlots[dataIndex]);
            }
            else if (grid == gridPrograms)
            {
                // look up the program from the matrix based on the data index
                var program = _programMatrix.GetProgram(dataIndex);

                (cell as ViewProgram).UpdateCell(program);
            }
            else if (grid == gridChannels)
            {
                (cell as ViewChannel).UpdateCell(_programMatrix.Channels[dataIndex]);
            }
        }

        private void _GridScrolled(EnhancedGrid grid, Vector2 scrollPosition, Vector2 normalizedScrollPosition)
        {
            if (grid == gridTimeSlots)
            {
                // set the program grid's x position based on the time slot grid's x position
                gridPrograms.ScrollNormalizedPosition = new Vector2(normalizedScrollPosition.x, gridPrograms.ScrollNormalizedPosition.y);
            }
            else if (grid == gridChannels)
            {
                // set the program grid's y position based on the channel grid's y position
                gridPrograms.ScrollNormalizedPosition = new Vector2(gridPrograms.ScrollNormalizedPosition.x, normalizedScrollPosition.y);
            }
            else if (grid == gridPrograms)
            {
                // set the time slot and channel grid positions based on the program grid's position

                gridTimeSlots.ScrollNormalizedPosition = new Vector2(normalizedScrollPosition.x, gridTimeSlots.ScrollNormalizedPosition.y);
                gridChannels.ScrollNormalizedPosition = new Vector2(gridChannels.ScrollNormalizedPosition.x, normalizedScrollPosition.y);
            }
        }

        private void _RefreshAllGrids()
        {
            gridTimeSlots.RefreshActiveCells();
            gridPrograms.RefreshActiveCells();
            gridChannels.RefreshActiveCells();
        }

        private void _CheckIfScrollNeeded()
        {
            // jump to the selected program index, making the cell
            // fully visible inside the viewable rect

            gridPrograms.JumpToMakeDataIndexFullyViewable(
                _programMatrix.SelectedProgramIndex,
                focusPriority: scrollFocusPriority,
                tweenTypeX: scrollTweenType,
                tweenTimeX: scrollTweenTime,
                tweenTypeY: scrollTweenType,
                tweenTimeY: scrollTweenTime
                );
        }
    }
}