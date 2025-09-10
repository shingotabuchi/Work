namespace echo17.EnhancedUI.EnhancedGrid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using echo17.EnhancedUI.Helpers;
    using UnityEngine.EventSystems;
    using System.Linq;

    /// <summary>
	/// EnhancedGrid: data-driven, one or two dimensional layout system for Unity,
	/// with virtualization and recycling for better performance.
	///
	/// The underlying data of the grid is serialized (one dimensional), so the controller
	/// delegate will need to supply the data in this manner. This allows flexibility in
	/// the flow without worrying about two dimensions. You can always store your data
	/// in the controller as two dimensional (or any structure, really) and serialize it
	/// for the grid.
	/// </summary>
    public class EnhancedGrid : MonoBehaviour,
                                IBeginDragHandler,
                                IEndDragHandler,
                                IDragHandler

    {
        #region Public

        #region Public Delegates

        /// <summary>
		/// Called when the tweening is toggled from off to on, or vice-versa
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="isTweening">Whether the tweening is on or not</param>
        public delegate void GridTweeningChanged(EnhancedGrid grid, bool isTweening);
        public GridTweeningChanged gridTweeningChanged;

        /// <summary>
		/// Called when the grid's scrolling is toggled from off to on, or vice-versa
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="isScrolling">Whether the scrolling is on or not</param>
        public delegate void GridScrollingChanged(EnhancedGrid grid, bool isScrolling);
        public GridScrollingChanged gridScrollingChanged;

        /// <summary>
		/// Called whenever the grid's scroll position is changed
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="scrollPosition">Scroll position in pixels</param>
		/// <param name="normalizedScrollPosition">Scroll position between zero and one</param>
        public delegate void GridScrolled(EnhancedGrid grid, Vector2 scrollPosition, Vector2 normalizedScrollPosition);
        public GridScrolled gridScrolled;

        /// <summary>
		/// Called when a grid's cell visibility has changed. This happens when the cell
		/// comes into the visible area or leaves the visible area.
		/// When cells are newly created, this is the order of delegate calls:
		/// 1) gridCellCreated
		/// 2) gridCellActivated
		/// When cells are reused, this is the order of delegate calls:
		/// 1) gridCellVisibilityChanged
		/// 2) gridCellReused
		/// 3) gridCellActivated
		/// When cells are recycled, this is the order of the delegate calls:
		/// 1) gridCellWillRecycle
		/// 2) gridCellVisibilityChanged
		/// 3) gridCellDidRecycle
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="cell">The IEnhancedGridCell data</param>
		/// <param name="isVisible">Whether the cell is visible or not</param>
        public delegate void GridCellVisibilityChanged(EnhancedGrid grid, IEnhancedGridCell cell, bool isVisible);
        public GridCellVisibilityChanged gridCellVisibilityChanged;

        /// <summary>
		/// Called when the cell is about to be recycled into the cell pool for later use.
		/// This happens instead of cells being destroyed to reduce garbage collection.
		/// When cells are newly created, this is the order of delegate calls:
		/// 1) gridCellCreated
		/// 2) gridCellActivated
		/// When cells are reused, this is the order of delegate calls:
		/// 1) gridCellVisibilityChanged
		/// 2) gridCellReused
		/// 3) gridCellActivated
		/// When cells are recycled, this is the order of the delegate calls:
		/// 1) gridCellWillRecycle
		/// 2) gridCellVisibilityChanged
		/// 3) gridCellDidRecycle
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="cell">The cell that will be recycled</param>
        public delegate void GridCellWillRecycle(EnhancedGrid grid, IEnhancedGridCell cell);
        public GridCellWillRecycle gridCellWillRecycle;

        /// <summary>
		/// Called after the cell is recycled into the cell pool for later use.
		/// This happens instead of cells being destroyed to reduce garbage collection.
		/// When cells are newly created, this is the order of delegate calls:
		/// 1) gridCellCreated
		/// 2) gridCellActivated
		/// When cells are reused, this is the order of delegate calls:
		/// 1) gridCellVisibilityChanged
		/// 2) gridCellReused
		/// 3) gridCellActivated
		/// When cells are recycled, this is the order of the delegate calls:
		/// 1) gridCellWillRecycle
		/// 2) gridCellVisibilityChanged
		/// 3) gridCellDidRecycle
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="cell">The cell that was recycled</param>
        public delegate void GridCellDidRecycle(EnhancedGrid grid, IEnhancedGridCell cell);
        public GridCellDidRecycle gridCellDidRecycle;

        /// <summary>
		/// Called when a new cell is created. This happens when there are no
		/// cells in the object pool to reuse.
		/// When cells are newly created, this is the order of delegate calls:
		/// 1) gridCellCreated
		/// 2) gridCellActivated
		/// When cells are reused, this is the order of delegate calls:
		/// 1) gridCellVisibilityChanged
		/// 2) gridCellReused
		/// 3) gridCellActivated
		/// When cells are recycled, this is the order of the delegate calls:
		/// 1) gridCellWillRecycle
		/// 2) gridCellVisibilityChanged
		/// 3) gridCellDidRecycle
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="cell">The cell that was recycled</param>
        public delegate void GridCellCreated(EnhancedGrid grid, IEnhancedGridCell cell);
        public GridCellCreated gridCellCreated;

        /// <summary>
		/// Called when a cell is reused from the cell object pool. This happens
		/// when there is an available cell that was previously recycled. Cells are
		/// reused whenever possible to avoid memory allocation.
		/// When cells are newly created, this is the order of delegate calls:
		/// 1) gridCellCreated
		/// 2) gridCellActivated
		/// When cells are reused, this is the order of delegate calls:
		/// 1) gridCellVisibilityChanged
		/// 2) gridCellReused
		/// 3) gridCellActivated
		/// When cells are recycled, this is the order of the delegate calls:
		/// 1) gridCellWillRecycle
		/// 2) gridCellVisibilityChanged
		/// 3) gridCellDidRecycle
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="cell">The cell that was recycled</param>
        public delegate void GridCellReused(EnhancedGrid grid, IEnhancedGridCell cell);
        public GridCellReused gridCellReused;

        /// <summary>
		/// Called when a cell is activated after being created or reused.
		/// When cells are newly created, this is the order of delegate calls:
		/// 1) gridCellCreated
		/// 2) gridCellActivated
		/// When cells are reused, this is the order of delegate calls:
		/// 1) gridCellVisibilityChanged
		/// 2) gridCellReused
		/// 3) gridCellActivated
		/// When cells are recycled, this is the order of the delegate calls:
		/// 1) gridCellWillRecycle
		/// 2) gridCellVisibilityChanged
		/// 3) gridCellDidRecycle
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="cell">The cell that was recycled</param>
        public delegate void GridCellActivated(EnhancedGrid grid, IEnhancedGridCell cell);
        public GridCellActivated gridCellActivated;

        /// <summary>
		/// Called when a cell is about to be destroyed. This only happens if one of the following is true:
		/// 1) DestroyCellsWhenRecalculatingGrid property is set to true.
		/// 2) RecycleCells property is changed (to either true or false).
		/// 3) RecycleCells property is set to false and RecalculateGrid method is called.
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="cell">The cell that was recycled</param>
        public delegate void GridCellWillDestroy(EnhancedGrid grid, IEnhancedGridCell cell);
        public GridCellWillDestroy gridCellWillDestroy;

        /// <summary>
		/// Called when the grid starts to be dragged by a pointer (mouse or finger)
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="data">PointerEventData for the drag</param>
        public delegate void GridOnBeginDrag(EnhancedGrid grid, PointerEventData data);
        public GridOnBeginDrag gridOnBeginDrag;

        /// <summary>
		/// Called when the grid is dragged by a pointer (mouse or finger)
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="data">PointerEventData for the drag</param>
		/// <param name="dragPreviousPosition">The position of the pointer during the previous OnDrag call</param>
        public delegate void GridOnDrag(EnhancedGrid grid, PointerEventData data, Vector2 dragPreviousPosition);
        public GridOnDrag gridOnDrag;

        /// <summary>
		/// Called when the grid has stopped dragging
		/// </summary>
		/// <param name="grid">The grid reporting the change</param>
		/// <param name="data">PointerEventData for the drag</param>
		/// <param name="dragPreviousPosition">The position of the pointer during the previous OnDrag call</param>
        public delegate void GridOnEndDrag(EnhancedGrid grid, PointerEventData data, Vector2 dragPreviousPosition);
        public GridOnEndDrag gridOnEndDrag;

        #region Addon Delegates

        // These should not be attached to by your scripts or they will interfere with addons.

        public delegate void SnapGridScroller(EnhancedGrid grid, Vector2 position, Vector2 normalizedScrollPosition);
        public SnapGridScroller snapGridScrolled;

        public delegate void SnapInterruptTween();
        public SnapInterruptTween snapInterruptTween;

        public GridOnBeginDrag snapGridOnBeginDrag;
        public GridOnDrag snapGridOnDrag;
        public GridOnEndDrag snapGridOnEndDrag;

        #endregion 

        #endregion // Public Delegates

        #region Public Enums

        /// <summary>
		/// The eight flow directions possible for cell layouts
		/// </summary>
        public enum EnhancedGridFlowDirection
        {
            LeftToRightTopToBottom,
            RightToLeftTopToBottom,
            LeftToRightBottomToTop,
            RightToLeftBottomToTop,
            TopToBottomLeftToRight,
            BottomToTopLeftToRight,
            TopToBottomRightToLeft,
            BottomToTopRightToLeft
        }

        /// <summary>
		/// Wrap modes for the cell layouts.
		/// None: no wrap mode. This will make a one-dimensional grid, or list.
		/// Size: wrap by a maximum size.
		/// CellCount: wrap when a certain number of cells per group is reached.
		/// </summary>
        public enum EnhancedGridWrapMode
        {
            None,
            Size,
            CellCount,
            SizeDynamic,
        }

        /// <summary>
		/// How the cells align inside their group (row or column)
		/// Start: align cells toward the beginning of the group.
		/// Center: align cells in the middle of the group.
		/// End: align cells towarde the end of the group.
		/// Expand: stretch cells to fill the remaining group space
		///         (if the cells have their size mode set to expand).
		/// </summary>
        public enum EnhancedGridFlowGroupAlignment
        {
            Start,
            Center,
            End,
            Expand
        }

        /// <summary>
		/// Determines how the grid will fill with cells.
		/// FillWithOverlap: fill the empty viewable space with cells,
		///                 even if the extra cells will overlap the viewable area.
		/// FillNoOverlap: fill the empty viewable space with cells, but
		///                 don't overlap the viewable area. Some extra space may
		///                 exist.
		/// Loop: fill the empty space and create an infinite loop on the specified axis.
		/// </summary>
        public enum EnhancedGridRepeatMode
        {
            None,
            FillWithOverlap,
            FillNoOverlap,
            Loop,
        }

        /// <summary>
		/// When jumping while looping, determine which direction to go.
		/// Closest: finds the closest instance of the data index specified.
		/// Backward: finds the closest instance of the data index specified before the current position.
		/// Forward: finds the closest instance of the data index specified after the current position.
		/// </summary>
        public enum EnhancedGridLoopJumpDirection
        {
            Closest,
            Backward,
            Forward
        }

        #endregion // Public Enums

        #region Public Inspector Fields

        /// <summary>
		/// The direction the grid will create cells. See EnhancedGridFlowDirection.
		/// </summary>
        [SerializeField]
        private EnhancedGridFlowDirection _flowDirection;
        public virtual EnhancedGridFlowDirection FlowDirection
        {
            get
            {
                return _flowDirection;
            }
            set
            {
                if (_flowDirection != value)
                {
                    _flowDirection = value;
                    _CreateFlow();
                }
            }
        }

        /// <summary>
		/// The alignment of the cells within a group (row or column). See EnhancedGridFlowGroupAlignment.
		/// </summary>
        [SerializeField]
        private EnhancedGridFlowGroupAlignment _flowGroupAlignment;
        public virtual EnhancedGridFlowGroupAlignment FlowGroupAlignment
        {
            get
            {
                return _flowGroupAlignment;
            }
            set
            {
                if (_flowGroupAlignment != value)
                {
                    _flowGroupAlignment = value;
                }
            }
        }

        /// <summary>
		/// The way cell flow will wrap inside the grid. See EnhancedGridWrapMode.
		/// </summary>
        [SerializeField]
        private EnhancedGridWrapMode _wrapMode;
        public virtual EnhancedGridWrapMode WrapMode
        {
            get
            {
                return _wrapMode;
            }
            set
            {
                if (_wrapMode != value)
                {
                    _wrapMode = value;
                }
            }
        }

        /// <summary>
		/// The maximum size to use for wrapping cell flow. This is only relevant
		/// if the WrapMode is set to Size.
		/// </summary>
        [SerializeField]
        private float _wrapSize;
        public virtual float WrapSize
        {
            get
            {
                return _wrapSize;
            }
            set
            {
                if (_wrapSize != value)
                {
                    _wrapSize = value;
                }
            }
        }

        /// <summary>
		/// The maximum number of cells to use for wrapping cell flow. This is only
		/// relevant if WrapMode is set to CellCount.
		/// </summary>
        [SerializeField]
        private int _wrapCellCount;
        public virtual int WrapCellCount
        {
            get
            {
                return _wrapCellCount;
            }
            set
            {
                if (_wrapCellCount != value)
                {
                    _wrapCellCount = value;
                }
            }
        }

        /// <summary>
		/// The amount of space in pixels between cells within a group (row or column).
		/// </summary>
        [SerializeField]
        private float _cellLayoutSpacing;
        public virtual float CellLayoutSpacing
        {
            get
            {
                return _cellLayoutSpacing;
            }
            set
            {
                if (_cellLayoutSpacing != value)
                {
                    _cellLayoutSpacing = value;
                }
            }
        }

        /// <summary>
		/// The amount of space in pixels between groups (rows or columns)
		/// </summary>
        [SerializeField]
        private float _groupLayoutSpacing;
        public virtual float GroupLayoutSpacing
        {
            get
            {
                return _groupLayoutSpacing;
            }
            set
            {
                if (_groupLayoutSpacing != value)
                {
                    _groupLayoutSpacing = value;
                }
            }
        }

        //      /// <summary>
        ///// The minimum size to set for the content game object inside the
        ///// ScrollView's Viewport. This is useful if you want to force content
        ///// to the end, like in a chat window.
        ///// </summary>
        //      [SerializeField]
        //      private Vector2 _contentMinimumSize;
        //      public virtual Vector2 ContentMinimumSize
        //      {
        //          get
        //          {
        //              return _contentMinimumSize;
        //          }
        //          set
        //          {
        //              if (_contentMinimumSize != value)
        //              {
        //                  _contentMinimumSize = value;
        //              }
        //          }
        //      }

        /// <summary>
        /// The amount of space to allocate for inside the viewport at the top.
        /// </summary>
        [SerializeField]
        private float _contentPaddingTop;
        public virtual float ContentPaddingTop
        {
            get
            {
                return _contentPaddingTop;
            }
            set
            {
                if (_contentPaddingTop != value)
                {
                    _contentPaddingTop = value;
                }
            }
        }

        /// <summary>
		/// The amount of space to allocate for inside the viewport at the bottom.
		/// </summary>
        [SerializeField]
        private float _contentPaddingBottom;
        public virtual float ContentPaddingBottom
        {
            get
            {
                return _contentPaddingBottom;
            }
            set
            {
                if (_contentPaddingBottom != value)
                {
                    _contentPaddingBottom = value;
                }
            }
        }

        /// <summary>
		/// The amount of space to allocate for inside the viewport at the left.
		/// </summary>
        [SerializeField]
        private float _contentPaddingLeft;
        public virtual float ContentPaddingLeft
        {
            get
            {
                return _contentPaddingLeft;
            }
            set
            {
                if (_contentPaddingLeft != value)
                {
                    _contentPaddingLeft = value;
                }
            }
        }

        /// <summary>
		/// The amount of space to allocate for inside the viewport at the right.
		/// </summary>
        [SerializeField]
        private float _contentPaddingRight;
        public virtual float ContentPaddingRight
        {
            get
            {
                return _contentPaddingRight;
            }
            set
            {
                if (_contentPaddingRight != value)
                {
                    _contentPaddingRight = value;
                }
            }
        }

        /// <summary>
		/// Whether the grid is recycling cells or not.
		/// Set this to true for large sets of data to see a performance boost.
		/// Set this to false for small sets, or grids that have their entire set
		/// of cells visible, as you will get a performance boost not having to
		/// check for cell visibility.
		/// </summary>
        [SerializeField]
        private bool _recycleCells = true;
        public virtual bool RecycleCells
        {
            get
            {
                return _recycleCells;
            }
            set
            {
                if (_recycleCells != value)
                {
                    _recycleCells = value;

                    // destroy all previous cells since we are changing how the grid recycles
                    _ClearObjects(true);
                }
            }
        }

        /// <summary>
		/// Whether to destroy cells when calling RecalculateGrid. By default, this is true
		/// so that it will clear out the cell object pool and start fresh. Note that this
		/// will cause a garbage collection call, so if you are recalculating the grid
		/// often, then turning this off might give a performance boost.
		/// </summary>
        [SerializeField]
        private bool _destroyCellsWhenRecalculatingGrid = true;
        public virtual bool DestroyCellsWhenRecalculatingGrid
        {
            get
            {
                return _destroyCellsWhenRecalculatingGrid;
            }
            set
            {
                if (_destroyCellsWhenRecalculatingGrid != value)
                {
                    _destroyCellsWhenRecalculatingGrid = value;
                }
            }
        }

        /// <summary>
		/// How deep to create an occlusion sector graph. Having low values is good for small
		/// sets of data so that the grid won't have to traverse a deep graph to determine
		/// a cell's visibility. Larger data sets will benefit from deeper occlusion depth as
		/// fewer cells will need to be checked when determining visibility. Finding a balance
		/// for your particular case will take some trial and error for best performance.
		/// OcclusionDepth = 1: Grid is divided into 4 sectors and cells are partitioned into those sectors.
		/// OcclusionDepth = 2: Grid is divided into 16 sectors (4 sub sectors for each of the base 4 sectors).
		/// OcclusionDepth = 3: Grid is divided into 64 sectors (4 sub sectors for each of the 4 sub sectors of the base 4 sub sectors)
		/// ... so on.
		/// High occlusion depths are not recommended as this will require the grid to traverse
		/// the graph more, slowing down visibility checks for cells.
		/// </summary>
        [SerializeField]
        private int _occlusionDepth = 2;
        public virtual int OcclusionDepth
        {
            get
            {
                return _occlusionDepth;
            }
            set
            {
                value = Mathf.Clamp(value, 1, value);

                if (_occlusionDepth != value)
                {
                    _occlusionDepth = value;
                }
            }
        }

        /// <summary>
		/// Cause cells to be visible, even beyond the actual visibility of the ScrollView's Viewport.
		/// Extends the area of the Viewport top.
		/// Having this value above zero will allow you to check for cell visibility less.
		/// MinimumScrollForActiveCellUpdate y value should be less than ExtendVisibleAreaTop or you will
		/// see cells pop into visibility after entering the Viewport rect.
		/// </summary>
        [SerializeField]
        private float _extendVisibleAreaTop = 500f;
        public virtual float ExtendVisibleAreaTop
        {
            get
            {
                return _extendVisibleAreaTop;
            }
            set
            {
                value = Mathf.Clamp(value, 0, value);

                if (_extendVisibleAreaTop != value)
                {
                    _extendVisibleAreaTop = value;
                }
            }
        }

        /// <summary>
		/// Cause cells to be visible, even beyond the actual visibility of the ScrollView's Viewport.
		/// Extends the area of the Viewport bottom.
		/// Having this value above zero will allow you to check for cell visibility less.
		/// MinimumScrollForActiveCellUpdate y value should be less than ExtendVisibleAreaBottom or you will
		/// see cells pop into visibility after entering the Viewport rect.
		/// </summary>
        [SerializeField]
        private float _extendVisibleAreaBottom = 500f;
        public virtual float ExtendVisibleAreaBottom
        {
            get
            {
                return _extendVisibleAreaBottom;
            }
            set
            {
                value = Mathf.Clamp(value, 0, value);

                if (_extendVisibleAreaBottom != value)
                {
                    _extendVisibleAreaBottom = value;
                }
            }
        }

        /// <summary>
		/// Cause cells to be visible, even beyond the actual visibility of the ScrollView's Viewport.
		/// Extends the area of Viewport left.
		/// Having this value above zero will allow you to check for cell visibility less.
		/// MinimumScrollForActiveCellUpdate x value should be less than ExtendVisibleAreaLeft or you will
		/// see cells pop into visibility after entering the Viewport rect.
		/// </summary>
        [SerializeField]
        private float _extendVisibleAreaLeft = 500f;
        public virtual float ExtendVisibleAreaLeft
        {
            get
            {
                return _extendVisibleAreaLeft;
            }
            set
            {
                value = Mathf.Clamp(value, 0, value);

                if (_extendVisibleAreaLeft != value)
                {
                    _extendVisibleAreaLeft = value;
                }
            }
        }

        /// <summary>
		/// Cause cells to be visible, even beyond the actual visibility of the ScrollView's Viewport.
		/// Extends the area of the Viewport right.
		/// Having this value above zero will allow you to check for cell visibility less.
		/// MinimumScrollForActiveCellUpdate x value should be less than ExtendVisibleAreaRight or you will
		/// see cells pop into visibility after entering the Viewport rect.
		/// </summary>
        [SerializeField]
        private float _extendVisibleAreaRight = 500f;
        public virtual float ExtendVisibleAreaRight
        {
            get
            {
                return _extendVisibleAreaRight;
            }
            set
            {
                value = Mathf.Clamp(value, 0, value);

                if (_extendVisibleAreaRight != value)
                {
                    _extendVisibleAreaRight = value;
                }
            }
        }

        /// <summary>
		/// How far the grid can scroll before it needs to do another cell visibility check.
		/// MinimumScrollForActiveCellUpdate x values should be less than ExtendVisibileAreaLeft and ExtendVisibleAreaRight
		/// or you will see cells pop into visibility after entering the Viewport rect.
		/// MinimumScrollForActiveCellUpdate y values should be less than ExtendVisibileAreaTop and ExtendVisibleAreaBottom
		/// or you will see cells pop into visibility after entering the Viewport rect.
		/// </summary>
        [SerializeField]
        private Vector2 _minimumScrollForActiveCellUpdate = new Vector2(450f, 450f);
        public virtual Vector2 MinimumScrollForActiveCellUpdate
        {
            get
            {
                return _minimumScrollForActiveCellUpdate;
            }
            set
            {
                value = new Vector2(Mathf.Clamp(value.x, 0, value.x), Mathf.Clamp(value.y, 0, value.y));

                if (_minimumScrollForActiveCellUpdate != value)
                {
                    _minimumScrollForActiveCellUpdate = value;
                }
            }
        }

        /// <summary>
		/// How the grid should fill up its empty space on the x axis. See EnhancedGridRepeatMode.
		/// </summary>
        [SerializeField]
        private EnhancedGridRepeatMode _repeatModeX;
        public virtual EnhancedGridRepeatMode RepeatModeX
        {
            get
            {
                return _repeatModeX;
            }
            set
            {
                if (_repeatModeX != value)
                {
                    _repeatModeX = value;
                }
            }
        }

        /// <summary>
		/// How the grid should fill up its empty space on the y axis. See EnhancedGridRepeatMode.
		/// </summary>
        [SerializeField]
        private EnhancedGridRepeatMode _repeatModeY;
        public virtual EnhancedGridRepeatMode RepeatModeY
        {
            get
            {
                return _repeatModeY;
            }
            set
            {
                if (_repeatModeY != value)
                {
                    _repeatModeY = value;
                }
            }
        }

        /// <summary>
		/// The maximum scroll speed the grid can travel at. This especially helps in scrolling
		/// quickly while looping to avoid massive spikes in velocity when the grid jumps backward
		/// or forward to a scroll jump position.
		/// </summary>
        [SerializeField]
        private Vector2 _maxVelocity = new Vector2(3000f, 3000f);
        public virtual Vector2 MaxVelocity
        {
            get
            {
                return _maxVelocity;
            }
            set
            {
                if (_maxVelocity != value)
                {
                    _maxVelocity = new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y));
                }
            }
        }

        /// <summary>
		/// If this is set to true and the user starts to drag the grid while it
		/// is tweening (from a jump or snap), then it will stop that tween.
		/// </summary>
        [SerializeField]
        private bool _interruptTweeningOnDrag;
        public virtual bool InterruptTweeningOnDrag
        {
            get
            {
                return _interruptTweeningOnDrag;
            }
            set
            {
                if (_interruptTweeningOnDrag != value)
                {
                    _interruptTweeningOnDrag = value;
                }
            }
        }

        /// <summary>
		/// Sets whether the tween should be running or not.
		/// See ToggleTweenPaused method if you want to resume tweening from the current
		/// scroll position instead of where the tween left off when paused.
		/// </summary>
        [SerializeField]
        private bool _tweenPaused;
        public virtual bool TweenPaused
        {
            get
            {
                return _tweenPaused;
            }
            set
            {
                if (_tweenPaused != value)
                {
                    _tweenPaused = value;
                }
            }
        }

        /// <summary>
		/// Whether to do a loop jump if the user is dragging the scroller. Generally,
		/// this would be a bad idea as you can get unexpected behaviour while trying
		/// to jump scroll positions and drag at the same time.
		/// </summary>
        [SerializeField]
        private bool _loopWhileDragging;
        public virtual bool LoopWhileDragging
        {
            get
            {
                return _loopWhileDragging;
            }
            set
            {
                if (_loopWhileDragging != value)
                {
                    _loopWhileDragging = value;
                }
            }
        }

        #region Debugging Properties

        /// <summary>
        /// Shows a graphical representation of the occlusions sectors' layouts.
        /// Used in debugging. This should only be turned on to do testing or understand how layouts are working.
        /// </summary>
        [SerializeField]
        private bool _showOcclusionSectors;
        public virtual bool ShowOcclusionSectors
        {
            get
            {
                return _showOcclusionSectors;
            }
            set
            {
                if (_showOcclusionSectors != value)
                {
                    _showOcclusionSectors = value;
                }
            }
        }

        /// <summary>
		/// Shows a graphical representation of the groups (rows or columns) of the grid.
		/// Used in debugging. This should only be turned on to do testing or understand how layouts are working.
		/// </summary>
        [SerializeField]
        private bool _showGroupLayouts;
        public virtual bool ShowGroupLayouts
        {
            get
            {
                return _showGroupLayouts;
            }
            set
            {
                if (_showGroupLayouts != value)
                {
                    _showGroupLayouts = value;
                }
            }
        }

        /// <summary>
		/// Shows a graphical representation of the cell layouts of the grid.
		/// Used in debugging. This should only be turned on to do testing or understand how layouts are working.
		/// </summary>
        [SerializeField]
        private bool _showCellLayouts;
        public virtual bool ShowCellLayouts
        {
            get
            {
                return _showCellLayouts;
            }
            set
            {
                if (_showCellLayouts != value)
                {
                    _showCellLayouts = value;
                }
            }
        }

        /// <summary>
		/// If on, this will show the cells of the grid. The only real practical use for this to
		/// be off is if you are debugging cell layouts and don't want to see the cells that may be
		/// in the way.
		/// </summary>
        [SerializeField]
        private bool _showCells = true;
        public virtual bool ShowCells
        {
            get
            {
                return _showCells;
            }
            set
            {
                if (_showCells != value)
                {
                    _showCells = value;
                }
            }
        }

        #endregion

        #endregion // Public Inspector Fields

        #region Public Fields

        /// <summary>
		/// The underlying ScrollRect the grid is using.
		/// </summary>
        private ScrollRect _scrollRect;
        public virtual ScrollRect ScrollRect
        {
            get
            {
                if (_scrollRect == null)
                {
                    _scrollRect = GetComponent<ScrollRect>();
                }
                return _scrollRect;
            }
        }

        /// <summary>
		/// The underlying RectTransform of the ScrollRect.
		/// </summary>
        public virtual RectTransform ScrollRectTransform
        {
            get
            {
                return _scrollRect.GetComponent<RectTransform>();
            }
        }

        /// <summary>
		/// The underlying RectTransform of the content object.
		/// </summary>
        public virtual RectTransform ContentRectTransform
        {
            get
            {
                return _scrollRect.content;
            }
        }

        /// <summary>
		/// The underlying Viewport that the grid is using.
		/// </summary>
        public virtual RectTransform Viewport
        {
            get
            {
                return _scrollRect.viewport;
            }
        }

        /// <summary>
		/// The size of the Viewport the grid is using.
		/// </summary>
        public virtual Vector2 ViewportSize
        {
            get
            {
                return _scrollRect.viewport.rect.size;
            }
        }

        /// <summary>
		/// The RectTransform of the group layouts section of the grid.
		/// Used internally as a parent of the group layout objects when ShowGroupLayouts is turned on.
		/// </summary>
        private RectTransform _contentGroupLayoutsRectTransform;
        public virtual RectTransform ContentGroupLayoutsRectTransform
        {
            get
            {
                return _contentGroupLayoutsRectTransform;
            }
        }

        /// <summary>
		/// The RectTransform of the cell layouts section of the grid.
		/// Used internally as a parent of the cell layout objects when ShowCellLayouts is turned on.
		/// </summary>
        private RectTransform _contentCellLayoutsRectTransform;
        public virtual RectTransform ContentCellLayoutsRectTransform
        {
            get
            {
                return _contentCellLayoutsRectTransform;
            }
        }

        /// <summary>
		/// The RectTransform of the occlusion sectors section of the grid.
		/// Used internally as a parent of the occlusion sector objects when ShowOcclusionSectors is turned on.
		/// </summary>
        private RectTransform _contentOcclusionSectorsRectTransform;
        public virtual RectTransform ContentOcclusionSectorsRectTransform
        {
            get
            {
                return _contentOcclusionSectorsRectTransform;
            }
        }

        /// <summary>
		/// The RectTransform of the parent container for the cells of the grid.
		/// </summary>
        private RectTransform _contentCellsRectTransform;
        public virtual RectTransform ContentCellsRectTransform
        {
            get
            {
                return _contentCellsRectTransform;
            }
        }

        /// <summary>
		/// The GameObject of the ScrollRect
		/// </summary>
        public virtual GameObject ScrollView
        {
            get
            {
                return _scrollRect != null ? _scrollRect.gameObject : null;
            }
        }

        /// <summary>
		/// The size of the content transform minus the viewport size.
		/// This is the maximum value that can be scrolled to in ScrollPosition.
		/// </summary>
        public virtual Vector2 ScrollSize
        {
            get
            {
                return new Vector2(Mathf.Max(ContentRectTransform.rect.width - ViewportSize.x, 0), Mathf.Max(ContentRectTransform.rect.height - ViewportSize.y, 0));
            }
        }

        /// <summary>
		/// The scroll position in pixels, calculated by the normalized scroll position times the scroll size.
		/// Note that EnhancedGrid has y coordinates going more naturally from top to bottom, unlike Unity's
		/// bottom to top.
		/// </summary>
        public virtual Vector2 ScrollPosition
        {
            get
            {
                return new Vector2(_scrollNormalizedPosition.x * ScrollSize.x, _scrollNormalizedPosition.y * ScrollSize.y);
            }
            set
            {
                ScrollNormalizedPosition = new Vector2(ScrollSize.x == 0 ? 0 : value.x / ScrollSize.x, ScrollSize.y == 0 ? 0 : value.y / ScrollSize.y);
            }
        }

        /// <summary>
		/// The scroll position from zero to one.
		/// Note that EnhancedGrid has y coordinates going more naturally from top to bottom, unlike Unity's
		/// bottom to top.
		/// </summary>
        private Vector2 _scrollNormalizedPosition;
        public virtual Vector2 ScrollNormalizedPosition
        {
            get
            {
                return _scrollNormalizedPosition;
            }
            set
            {
                value = new Vector2(Mathf.Clamp01(value.x), Mathf.Clamp01(value.y));

                _scrollNormalizedPosition = value;

                _scrollRect.normalizedPosition = new Vector2(_scrollNormalizedPosition.x, 1f - _scrollNormalizedPosition.y);
            }
        }

        /// <summary>
		/// Whether the grid is currently scrolling.
		/// </summary>
        public virtual bool IsScrolling
        {
            get; private set;
        }

        /// <summary>
		/// Whether the grid is currently being dragged.
		/// </summary>
        public virtual bool IsDragging
        {
            get
            {
                return _dragFingerCount > 0;
            }
        }

        /// <summary>
		/// How many cell layouts were created before filling in with repeats and loops.
		/// </summary>
		private int _originalDataCount;
        public virtual int OriginalDataCount
        {
            get
            {
                return _originalDataCount;
            }
        }

        /// <summary>
		/// The rect that defines what is visible for activating / deactivating cells
		/// </summary>
        public Rect VisibleRect { get; private set; }

        /// <summary>
		/// Whether the scroller is currently tweening
		/// </summary>
        public bool IsTweening { get; private set; }

        #endregion // Public Fields

        #region Public Methods

        /// <summary>
		/// This method must be called before any calls to RecalculateGrid.
		/// </summary>
		/// <param name="EnhancedGridDelegate">The controller that will be handling the grid calls</param>
        public virtual void InitializeGrid(IEnhancedGridDelegate EnhancedGridDelegate)
        {
            // cache the controller
            _EnhancedGridDelegate = EnhancedGridDelegate;

            // cache the scroll rect
            _scrollRect = GetComponent<ScrollRect>();

            // load some objects and palettes

            _LoadPalettes();
            _LoadResources();

            // create some containers for objects in the grid

            _contentOcclusionSectorsRectTransform = _AddContentSubset("OcclusionSectors");
            _contentGroupLayoutsRectTransform = _AddContentSubset("GroupLayouts");
            _contentCellLayoutsRectTransform = _AddContentSubset("CellLayouts");
            _contentCellsRectTransform = _AddContentSubset("Cells");

            // create a debug container

            _contentDebugRectTransform = _AddContentSubset("Debug");
            _contentDebugRectTransform.anchorMin = Vector2.zero;
            _contentDebugRectTransform.anchorMax = Vector2.one;

            _isInitialized = true;
        }

        /// <summary>
		/// Helper method that calls RecalculateGrid with the current normalized position.
		/// </summary>
        public virtual void RecalculateGrid_MaintainPosition()
        {
            RecalculateGrid(_scrollNormalizedPosition.x, _scrollNormalizedPosition.y);
        }

        /// <summary>
		/// Helper method that calls ReclaculateGrid, but tries to keep the same data index
		/// in view in case the grid has changed a lot from its previous state (new data added,
		/// cell size changes, etc.).
		/// </summary>
		/// <param name="watchScrollerOffset">Where inside the Viewport (0..1) to look for a data index to maintain position with</param>
		/// <param name="newScrollerOffset">Where the Viewport should be centered on (0..1) after recalculating the grid</param>
		/// <param name="newCellOffset">Where inside the cell (0..1) to center the grid on</param>
        public virtual void RecalculateGrid_MaintainDataIndex(Vector2? watchScrollerOffset = null, Vector2? newScrollerOffset = null, Vector2? newCellOffset = null)
        {
            // if no watch scroller offset was specified, just look at the center of the grid
            if (watchScrollerOffset == null)
            {
                watchScrollerOffset = new Vector2(0.5f, 0.5f);
            }

            // determine which cell to focus on based on the watch scroller offset

            var watchPosition = ScrollPosition + (watchScrollerOffset.Value * ViewportSize);
            var watchDataIndex = GetDataIndexAtPosition(watchPosition, false);
            if (watchDataIndex > _cellLayouts.Count - 1)
            {
                watchDataIndex = _cellLayouts.Count - 1;
            }
            var logicRect = _cellLayouts[watchDataIndex].logicRect;

            // if no new scroller offset is specified, just use the watch scroller offset.
            // keeping this value the same as the watch scroller offset is the best way to
            // ensure no apparant jump after the recalculation.

            if (newScrollerOffset == null)
            {
                newScrollerOffset = watchScrollerOffset.Value;
            }

            // if no cell offset is specified, just try to use the cell offset before
            // the recalculation based on where the watch scroller offset is set to

            if (newCellOffset == null)
            {
                newCellOffset = new Vector2(
                                            (watchPosition.x - logicRect.xMin) / logicRect.width,
                                            (watchPosition.y - logicRect.yMin) / logicRect.height
                                         );
            }

            // call the recalculate method, passing the calculated values above

            RecalculateGrid_SetDataIndex(watchDataIndex, newScrollerOffset.Value, newCellOffset.Value);
        }

        /// <summary>
		/// Helper method to recalculate a grid, but maintain a data index with offsets
		/// </summary>
		/// <param name="dataIndex">The data index to focus on</param>
		/// <param name="scrollerOffset">Where in the Viewport (0..1) the grid should be centered on</param>
		/// <param name="newCellOffset">Where in the cell layout (0..1) the grid should be centered on</param>
        public virtual void RecalculateGrid_SetDataIndex(int dataIndex, Vector2 scrollerOffset, Vector2 newCellOffset)
        {
            // first recalculate the grid

            RecalculateGrid();

            // ignore data indices outside the grid range

            if (dataIndex > (_cellLayouts.Count - 1))
            {
                // the data index is no longer a part of the data set

                return;
            }

            // get the cell layout and set the grid scroll position based on the scroller offset and cell offsets

            var logicRect = _cellLayouts[dataIndex].logicRect;
            ScrollPosition = logicRect.UpperLeft() - (scrollerOffset * ViewportSize) + (newCellOffset * logicRect.size);

            _ShowVisibleCells();
        }

        /// <summary>
		/// Method to generate the grid based on the delegate controller's callback methods.
		/// </summary>
		/// <param name="scrollNormalizedPositionX">The normalized x axis position (0..1) after the grid is calculated</param>
		/// <param name="scrollNormalizedPositionY">The normalized y axis position (0..1) after the grid is calculated</param>
        public virtual void RecalculateGrid(float scrollNormalizedPositionX = 0f, float scrollNormalizedPositionY = 0)
        {
            // don't bother if we are only in the editor
            if (!Application.IsPlaying(this)) return;

            if (WrapMode == EnhancedGridWrapMode.SizeDynamic)
            {
                WrapSize = ScrollRectTransform.rect.width;
            }

            // make sure we have a delegate controller
            Debug.Assert(_EnhancedGridDelegate != null, "No controller delegate has been assigned to this grid.");

            // set the original data count before repeats and loops
            _originalDataCount = _EnhancedGridDelegate.GetCellCount(this);

            // make sure we have a positive count
            Debug.Assert(_originalDataCount >= 0, $"Cell count {_originalDataCount} must be positive");

            // cache some movement values to reset later in LateUpdate

            _previousMovementType = _scrollRect.movementType;
            _resetMovementType = true;
            _scrollRect.movementType = ScrollRect.MovementType.Clamped;

            // remove all the objects (recycling if necessary)

            _ClearObjects();

            // force the canvas to update so that we have all the correct dimensions for our viewport and content rects

            Canvas.ForceUpdateCanvases();

            // set up the flow objects to determine the cell layouts

            _CreateFlow();

            _cellLayouts.Resize<CellLayout>(_originalDataCount);
            _groupLayouts.Resize<GroupLayout>(_originalDataCount);
            _groupLayouts.Clear();

            GroupLayout groupLayout = null;
            float totalGroupSize = 0f;
            Vector2 maxBounds = Vector2.zero;
            float maxFlowDirectionSize = 0f;

            // gather cell properties from the controller delegate

            for (var c = 0; c < _cellLayouts.Count; c++)
            {
                _cellLayouts[c].cellProperties = _EnhancedGridDelegate.GetCellProperties(this, c);
                _cellLayouts[c].dataIndex = c;
                _cellLayouts[c].repeatIndex = c;
            }

            // create group (row or column) layouts

            for (var c = 0; c < _cellLayouts.Count; c++)
            {
                bool endOfGroup = false;

                if (c == 0)
                {
                    endOfGroup = true;
                }
                else if (WrapMode == EnhancedGridWrapMode.None)
                {
                    // do nothing, only a single group
                }
                else if (WrapMode == EnhancedGridWrapMode.Size || WrapMode == EnhancedGridWrapMode.SizeDynamic)
                {
                    endOfGroup = _flow.IsEndOfGroup(groupLayout.flowDirectionSize, _cellLayouts[c].cellProperties.minSize);
                }
                else if (WrapMode == EnhancedGridWrapMode.CellCount)
                {
                    endOfGroup = groupLayout.cellCount >= WrapCellCount;
                }

                if (endOfGroup)
                {
                    // we've wrapped, so start a new group

                    if (groupLayout != null)
                    {
                        totalGroupSize += groupLayout.maxCellSize;
                        maxFlowDirectionSize = Mathf.Max(maxFlowDirectionSize, groupLayout.flowDirectionSize);
                    }

                    _groupLayouts.Count++;
                    groupLayout = _groupLayouts.Last();
                    groupLayout.cellCount = 1;
                    groupLayout.startCellLayoutRepeatIndex = c;
                    groupLayout.endCellLayoutRepeatIndex = c;
                    groupLayout.maxCellSize = 0;

                    groupLayout.flowDirectionSize = _flow.ResetFlowDirectionSize(_cellLayouts[c].cellProperties.minSize);
                }
                else
                {
                    // no wrap yet, just continue to build group

                    groupLayout.cellCount++;
                    groupLayout.endCellLayoutRepeatIndex = c;

                    groupLayout.flowDirectionSize += _flow.IncrementFlowDirectionSize(_cellLayouts[c].cellProperties.minSize);
                }

                _cellLayouts[c].groupLayoutIndex = _groupLayouts.Count - 1;

                groupLayout.maxCellSize = _flow.GetGroupMaxCellSize(groupLayout.maxCellSize, _cellLayouts[c].cellProperties.minSize);
            }

            // get the size of the group (row or column)

            if (_groupLayouts.Count > 0)
            {
                maxFlowDirectionSize = Mathf.Max(maxFlowDirectionSize, _groupLayouts.Last().flowDirectionSize);
            }

            // get the size of the axis perpendicular to the group (row or column) flow direction

            totalGroupSize += ((_groupLayouts.Count - 1) * GroupLayoutSpacing) + (_groupLayouts.Count > 0 ? _groupLayouts.Last().maxCellSize : 0);

            // calculate the content bounds

            maxBounds = _flow.GetMaxBounds(maxFlowDirectionSize, totalGroupSize);
            maxBounds = new Vector2(Mathf.Max(maxBounds.x, _repeatModeX == EnhancedGridRepeatMode.None ? ViewportSize.x : maxBounds.x), Mathf.Max(maxBounds.y, _repeatModeY == EnhancedGridRepeatMode.None ? ViewportSize.y : maxBounds.y));

            // create group (row or column) layout rects

            int groupIndex = 0;
            var groupOffset = 0f;
            var position = Vector2.zero;
            var size = Vector2.zero;

            for (var g = 0; g < _groupLayouts.Count; g++)
            {
                _groupLayouts[g].groupColorIndex = g;

                groupIndex = _flow.GetGroupIndex(g, _groupLayouts.Count);
                position = _flow.GetInitialGroupPosition(groupOffset);
                size = _flow.GetInitialGroupSize(_groupLayouts[groupIndex].flowDirectionSize, _groupLayouts[groupIndex].maxCellSize);

                switch (FlowGroupAlignment)
                {
                    case EnhancedGrid.EnhancedGridFlowGroupAlignment.Start:

                        position += _flow.GetGroupOffsetStart(maxBounds, size);

                        break;

                    case EnhancedGrid.EnhancedGridFlowGroupAlignment.Center:

                        position += _flow.GetGroupOffsetCenter(maxBounds, size);

                        break;

                    case EnhancedGrid.EnhancedGridFlowGroupAlignment.End:

                        position += _flow.GetGroupOffsetEnd(maxBounds, size);

                        break;

                    case EnhancedGrid.EnhancedGridFlowGroupAlignment.Expand:

                        size = _flow.GetGroupExpandSize(maxBounds, size);

                        break;
                }

                _groupLayouts[groupIndex].logicRect = new Rect(position.x, position.y, size.x, size.y);
                groupOffset += (_groupLayouts[groupIndex].maxCellSize + GroupLayoutSpacing);
            }

            // push group layouts down to the bottom if necessary

            if (_flowDirection == EnhancedGridFlowDirection.LeftToRightBottomToTop || _flowDirection == EnhancedGridFlowDirection.RightToLeftBottomToTop)
            {
                // get the position of the first element and compare it to the maximum bounds to get the amount of empty space

                groupOffset += (ContentPaddingTop + ContentPaddingBottom - GroupLayoutSpacing);
                var emptySpace = maxBounds.y - groupOffset;

                if (emptySpace > 0)
                {
                    // there was some empty space, so push all group layouts down by that amount

                    for (var g = 0; g < _groupLayouts.Count; g++)
                    {
                        var logicRect = _groupLayouts[g].logicRect;
                        _groupLayouts[g].logicRect = new Rect(logicRect.xMin, logicRect.yMin + emptySpace, logicRect.width, logicRect.height);
                    }
                }
            }

            // create cell layout rects

            for (var g = 0; g < _groupLayouts.Count; g++)
            {
                if (FlowGroupAlignment == EnhancedGridFlowGroupAlignment.Expand)
                {
                    // if the group alignment is set to expand, then calculate the available space the cells can expand into

                    _groupLayouts[g].expansionAvailable = _flow.GetExpandAvailable(_groupLayouts[g]);
                    var expandCellCount = 0;
                    var totalExpansionWeight = 0f;

                    for (var c = _groupLayouts[g].startCellLayoutRepeatIndex; c <= _groupLayouts[g].endCellLayoutRepeatIndex; c++)
                    {
                        if (_cellLayouts[c].cellProperties.expansionWeight > 0)
                        {
                            expandCellCount++;
                            totalExpansionWeight += _cellLayouts[c].cellProperties.expansionWeight;
                        }
                    }

                    // normalize the expansion weight
                    float expansionAdjustment = 1f / totalExpansionWeight;

                    for (var c = _groupLayouts[g].startCellLayoutRepeatIndex; c <= _groupLayouts[g].endCellLayoutRepeatIndex; c++)
                    {
                        if (_cellLayouts[c].cellProperties.expansionWeight > 0)
                        {
                            // adjust the expansion weight to be normalized
                            _cellLayouts[c].cellProperties = new CellProperties(_cellLayouts[c].cellProperties.minSize, _cellLayouts[c].cellProperties.expansionWeight * expansionAdjustment);

                            _cellLayouts[c].actualSize = _flow.GetActualCellSizeExpand(_cellLayouts[c].cellProperties.minSize, (_cellLayouts[c].cellProperties.expansionWeight * _groupLayouts[g].expansionAvailable), _groupLayouts[g].maxCellSize);
                        }
                        else
                        {
                            _cellLayouts[c].actualSize = _flow.GetActualCellSizeFixed(_cellLayouts[c].cellProperties.minSize, _groupLayouts[g].maxCellSize);
                        }
                    }
                }
                else
                {
                    // no expansion, so just set the cell layouts as fixed

                    for (var c = _groupLayouts[g].startCellLayoutRepeatIndex; c <= _groupLayouts[g].endCellLayoutRepeatIndex; c++)
                    {
                        _cellLayouts[c].actualSize = _flow.GetActualCellSizeFixed(_cellLayouts[c].cellProperties.minSize, _groupLayouts[g].maxCellSize);
                    }
                }

                Vector2 cellOffset = _flow.GetInitialCellOffset(_groupLayouts[g].logicRect, _cellLayouts[_groupLayouts[g].startCellLayoutRepeatIndex].actualSize);

                // create the cell's logic rects based on their calculated sizes

                for (var c = _groupLayouts[g].startCellLayoutRepeatIndex; c <= _groupLayouts[g].endCellLayoutRepeatIndex; c++)
                {
                    int dataIndex = _flow.GetCellDataIndex(c, _groupLayouts[g].startCellLayoutRepeatIndex, _groupLayouts[g].endCellLayoutRepeatIndex);
                    int nextCellDataIndex = _flow.GetNextCellDataIndex(c, _groupLayouts[g].startCellLayoutRepeatIndex, _groupLayouts[g].endCellLayoutRepeatIndex);
                    _cellLayouts[dataIndex].logicRect = new Rect(cellOffset.x, cellOffset.y, _cellLayouts[dataIndex].actualSize.x, _cellLayouts[dataIndex].actualSize.y);
                    cellOffset += _flow.GetCellOffset(_cellLayouts[dataIndex].actualSize, nextCellDataIndex != -1 ? _cellLayouts[nextCellDataIndex].actualSize : Vector2.zero);
                }
            }

            // repeat (if necessary)

            // fill in x

            if (_repeatModeX != EnhancedGridRepeatMode.None)
            {
                if (maxBounds.x < ViewportSize.x)
                {
                    var totalPadding = _contentPaddingLeft + _contentPaddingRight;
                    var maxBoundsNoPaddingX = maxBounds.x - totalPadding;

                    var fillCount = Mathf.CeilToInt((ViewportSize.x - totalPadding - maxBoundsNoPaddingX) / (maxBoundsNoPaddingX + _groupLayoutSpacing));

                    var doesOverlap = (maxBounds.x + (fillCount * (maxBoundsNoPaddingX + _groupLayoutSpacing))) > ViewportSize.x;

                    if (doesOverlap && _repeatModeX == EnhancedGridRepeatMode.FillNoOverlap)
                    {
                        fillCount--;
                    }

                    _CreateCopies(fillCount, CopyDirection.X, ref maxBounds);
                }
            }

            // fill in y

            if (_repeatModeY != EnhancedGridRepeatMode.None)
            {
                if (maxBounds.y < ViewportSize.y)
                {
                    var totalPadding = _contentPaddingTop + _contentPaddingBottom;
                    var maxBoundsNoPaddingY = maxBounds.y - totalPadding;

                    var fillCount = Mathf.CeilToInt((ViewportSize.y - totalPadding - maxBoundsNoPaddingY) / (maxBoundsNoPaddingY + _groupLayoutSpacing));

                    var doesOverlap = (maxBounds.y + (fillCount * (maxBoundsNoPaddingY + _groupLayoutSpacing))) > ViewportSize.y;

                    if (doesOverlap && _repeatModeY == EnhancedGridRepeatMode.FillNoOverlap)
                    {
                        fillCount--;
                    }

                    _CreateCopies(fillCount, CopyDirection.Y, ref maxBounds);
                }
            }

            // cache the size of a single instance of filled cell content

            var maxBoundsNoPaddingSingleInstance = new Vector2(maxBounds.x - _contentPaddingLeft - _contentPaddingRight, maxBounds.y - _contentPaddingTop - _contentPaddingBottom);

            // add copies for x loop

            if (_repeatModeX == EnhancedGridRepeatMode.Loop)
            {
                _CreateCopies(2, CopyDirection.X, ref maxBounds);

                _loopJumpX[0] = _contentPaddingLeft + (maxBoundsNoPaddingSingleInstance.x * 0.5f);
                _loopJumpX[1] = _contentPaddingLeft + (maxBoundsNoPaddingSingleInstance.x * 1.5f) + (_groupLayoutSpacing);
                _loopJumpX[2] = _contentPaddingLeft + (maxBoundsNoPaddingSingleInstance.x * 2.5f) + (_groupLayoutSpacing * 2.0f);

                //_CreateDebugVerticalLineGameObject(_loopJumpX[0], Color.magenta);
                //_CreateDebugVerticalLineGameObject(_loopJumpX[1], Color.red);
                //_CreateDebugVerticalLineGameObject(_loopJumpX[2], Color.green);
            }

            // add copies for y loop

            if (_repeatModeY == EnhancedGridRepeatMode.Loop)
            {
                _CreateCopies(2, CopyDirection.Y, ref maxBounds);

                _loopJumpY[0] = _contentPaddingTop + (maxBoundsNoPaddingSingleInstance.y * 0.5f);
                _loopJumpY[1] = _contentPaddingTop + (maxBoundsNoPaddingSingleInstance.y * 1.5f) + (_groupLayoutSpacing);
                _loopJumpY[2] = _contentPaddingTop + (maxBoundsNoPaddingSingleInstance.y * 2.5f) + (_groupLayoutSpacing * 2.0f);

                //_CreateDebugHorizontalLineGameObject(_loopJumpY[0], Color.magenta);
                //_CreateDebugHorizontalLineGameObject(_loopJumpY[1], Color.red);
                //_CreateDebugHorizontalLineGameObject(_loopJumpY[2], Color.green);
            }

            // reverse the y coordinates for ui.
            // Unity ui coordinates start at the bottom of the screen and go upwards, but for our grid
            // it makes more sense to start at the top and go down.

            for (var g = 0; g < _groupLayouts.Count; g++)
            {
                _groupLayouts[g].uiRect = _groupLayouts[g].logicRect.ReverseY();
            }
            for (var c = 0; c < _cellLayouts.Count; c++)
            {
                _cellLayouts[c].uiRect = _cellLayouts[c].logicRect.ReverseY();
            }

            // calculate the occlusion sectors

            if (_baseOcclusionSector == null)
            {
                _baseOcclusionSector = new OcclusionSector();
            }
            _baseOcclusionSector.Reset("", (RecycleCells ? OcclusionDepth : 0), new Rect(0, 0, maxBounds.x, maxBounds.y));

            for (var c = 0; c < _cellLayouts.Count; c++)
            {
                _baseOcclusionSector.AddCell(_cellLayouts[c].dataIndex, _cellLayouts[c].repeatIndex, _cellLayouts[c].logicRect);
            }

            // set up the content transform

            // cache the content rect transform settings to be set at the end of this method

            var previousContentAnchorMin = ContentRectTransform.anchorMin;
            var previousContentAnchorMax = ContentRectTransform.anchorMax;
            var previousContentPivot = ContentRectTransform.pivot;
            var previousContentRotation = ContentRectTransform.rotation;
            var previousContentScale = ContentRectTransform.localScale;

            // set the content rect transform to its normalized settings

            ContentRectTransform.anchorMin = new Vector2(0.0f, 1.0f);
            ContentRectTransform.anchorMax = new Vector2(0.0f, 1.0f);
            ContentRectTransform.pivot = new Vector2(0.0f, 1.0f);
            ContentRectTransform.offsetMin = Vector2.zero;
            ContentRectTransform.offsetMax = Vector2.zero;
            ContentRectTransform.anchoredPosition = Vector2.zero;
            ContentRectTransform.sizeDelta = maxBounds;
            ContentRectTransform.rotation = Quaternion.identity;
            ContentRectTransform.localScale = Vector2.one;

            _contentOcclusionSectorsRectTransform.anchoredPosition = Vector2.zero;
            _contentGroupLayoutsRectTransform.anchoredPosition = Vector2.zero;
            _contentCellLayoutsRectTransform.anchoredPosition = Vector2.zero;
            _contentCellsRectTransform.anchoredPosition = Vector2.zero;

            // create the occlusion sector debug objects

            if (_showOcclusionSectors)
            {
                EnhancedList<OcclusionSector> baseSectors = _baseOcclusionSector.GetBaseSectors();

                for (var s = 0; s < baseSectors.Count; s++)
                {
                    _CreateOcclusionSectorGameObject(baseSectors[s], s);
                }
            }

            // create the group layout debug objects

            if (_showGroupLayouts)
            {
                for (var g = 0; g < _groupLayouts.Count; g++)
                {
                    _CreateGroupLayoutGameObject(g, _groupLayouts[g].groupColorIndex);
                }
            }

            // create the cell layout debug objects

            if (_showCellLayouts)
            {
                for (var c = 0; c < _cellLayouts.Count; c++)
                {
                    _CreateCellLayoutGameObject(_cellLayouts[c].dataIndex, _cellLayouts[c].repeatIndex);
                }
            }

            // set the scroll position (without calling the change event of the ScrollRect)

            _scrollRect.onValueChanged.RemoveListener(_ScrollRect_OnValueChanged);

            // make sure we are inside the jump bounds, otherwise reset to the middle section

            if (_repeatModeX == EnhancedGridRepeatMode.Loop)
            {
                if ((scrollNormalizedPositionX * ScrollSize.x) <= _loopJumpX[0])
                {
                    scrollNormalizedPositionX = (_contentPaddingLeft + maxBoundsNoPaddingSingleInstance.x + _groupLayoutSpacing) / ScrollSize.x;
                }
                else if (((scrollNormalizedPositionX * ScrollSize.x) + ViewportSize.x) >= _loopJumpX[2])
                {
                    scrollNormalizedPositionX = (ContentRectTransform.rect.width - _contentPaddingRight - maxBoundsNoPaddingSingleInstance.x - _groupLayoutSpacing - ViewportSize.x) / ScrollSize.x;
                }
            }

            if (_repeatModeY == EnhancedGridRepeatMode.Loop)
            {
                if ((scrollNormalizedPositionY * ScrollSize.y) <= _loopJumpY[0])
                {
                    scrollNormalizedPositionY = (_contentPaddingTop + maxBoundsNoPaddingSingleInstance.y + _groupLayoutSpacing) / ScrollSize.y;
                }
                else if (((scrollNormalizedPositionY * ScrollSize.y) + ViewportSize.y) >= _loopJumpY[2])
                {
                    scrollNormalizedPositionY = (ContentRectTransform.rect.height - _contentPaddingBottom - maxBoundsNoPaddingSingleInstance.y - _groupLayoutSpacing - ViewportSize.y) / ScrollSize.y;
                }
            }

            // set the normalized scroll position and reinstate the ScrollRect change listener

            ScrollNormalizedPosition = new Vector2(scrollNormalizedPositionX, scrollNormalizedPositionY);

            _scrollRect.onValueChanged.AddListener(_ScrollRect_OnValueChanged);


            // create the cells

            if (_showCells)
            {
                if (_recycleCells)
                {
                    _ShowVisibleCells();
                }
                else
                {
                    IEnhancedGridCell cell;
                    for (var c = 0; c < _cellLayouts.Count; c++)
                    {
                        cell = _CreateCellGameObject(_cellLayouts[c].dataIndex, _cellLayouts[c].repeatIndex);
                        _activeCells.Add(cell);
                    }

                    _SortActiveCells();
                }
            }

            // reset the content transform

            //ContentRectTransform.anchorMin = previousContentAnchorMin;
            //ContentRectTransform.anchorMax = previousContentAnchorMax;
            //ContentRectTransform.pivot = previousContentPivot;
            ContentRectTransform.rotation = previousContentRotation;
            ContentRectTransform.localScale = previousContentScale;


            // disconnect the scrollbars.
            // this gets around a bug in some versions of Unity where
            // the scrollbar causes large content rects to not have inertia.
            // we will re-establish the existing scrollbar connections in the LateUpdate method.

            _horizontalScrollbar = _scrollRect.horizontalScrollbar;
            _verticalScrollber = _scrollRect.verticalScrollbar;
            _scrollRect.horizontalScrollbar = null;
            _scrollRect.verticalScrollbar = null;
            _resetScrollbars = true;
        }

        /// <summary>
		/// Jumps to a cell based on its data index.
		/// </summary>
		/// <param name="dataIndex">The data index of the cell.</param>
		/// <param name="tweenTypeX">How to tween on the x axis. See EnhancedTween class.</param>
		/// <param name="tweenTimeX">Duration to tween on the x axis.</param>
		/// <param name="loopJumpDirectionX">If looping, which direction to go on the x axis.</param>
		/// <param name="tweenTypeY">How to tween on the y axis. See EnhancedTween class.</param>
		/// <param name="tweenTimeY">Duration to tween on the y axis.</param>
		/// <param name="loopJumpDirectionY">If looping, which direction to go on the y axis.</param>
		/// <param name="scrollerOffset">Where to center the Viewport (0..1)</param>
		/// <param name="cellOffset">Where to center the cell layout (0..1)</param>
		/// <param name="jumpCompleted">What method to call when the jump has completed</param>
        public virtual void JumpToDataIndex(
            int dataIndex,
            TweenType tweenTypeX = TweenType.immediate,
            float tweenTimeX = 0f,
            EnhancedGridLoopJumpDirection loopJumpDirectionX = EnhancedGridLoopJumpDirection.Closest,
            TweenType tweenTypeY = TweenType.immediate,
            float tweenTimeY = 0f,
            EnhancedGridLoopJumpDirection loopJumpDirectionY = EnhancedGridLoopJumpDirection.Closest,
            Vector2? scrollerOffset = null,
            Vector2? cellOffset = null,
            Action<EnhancedGrid> jumpCompleted = null
            )
        {
            // make sure we have valid parameters

            Debug.Assert(dataIndex >= 0 && dataIndex < _originalDataCount, $"JumpToDataIndex: dataIndex {dataIndex} must be between 0 and {_originalDataCount - 1}");
            Debug.Assert(tweenTimeX >= 0, "Tween time x must be greater than or equal to zero.");
            Debug.Assert(tweenTimeY >= 0, "Tween time y must be greater than or equal to zero.");

            CellLayout cellLayout = null;

            // if we are filling or looping, then check for the appropriate index to jump to based on the jump directions
            // specified in the parameters

            if (_repeatModeX != EnhancedGridRepeatMode.None || _repeatModeY != EnhancedGridRepeatMode.None)
            {
                var targetLocation = ScrollPosition + new Vector2(
                                                                    (scrollerOffset != null ? (scrollerOffset.Value.x * ViewportSize.x) : 0f),
                                                                    (scrollerOffset != null ? (scrollerOffset.Value.y * ViewportSize.y) : 0f)
                                                                    );

                var closestDistanceSquared = Mathf.Infinity;
                for (var i = 0; i < _cellLayouts.Count; i++)
                {
                    if (_cellLayouts[i].dataIndex == dataIndex)
                    {
                        var distanceSquared = (_cellLayouts[i].logicRect.center - targetLocation).sqrMagnitude;
                        if (distanceSquared < closestDistanceSquared)
                        {
                            var correctDirectionX = false;
                            var correctDirectionY = false;

                            if (
                                (loopJumpDirectionX == EnhancedGridLoopJumpDirection.Backward && _cellLayouts[i].logicRect.center.x <= targetLocation.x)
                                ||
                                (loopJumpDirectionX == EnhancedGridLoopJumpDirection.Forward && _cellLayouts[i].logicRect.center.x >= targetLocation.x)
                                ||
                                loopJumpDirectionX == EnhancedGridLoopJumpDirection.Closest
                                )
                            {
                                correctDirectionX = true;
                            }

                            if (
                                (loopJumpDirectionY == EnhancedGridLoopJumpDirection.Backward && _cellLayouts[i].logicRect.center.y <= targetLocation.y)
                                ||
                                (loopJumpDirectionY == EnhancedGridLoopJumpDirection.Forward && _cellLayouts[i].logicRect.center.y >= targetLocation.y)
                                ||
                                loopJumpDirectionY == EnhancedGridLoopJumpDirection.Closest
                                )
                            {
                                correctDirectionY = true;
                            }

                            if (correctDirectionX && correctDirectionY)
                            {
                                closestDistanceSquared = distanceSquared;
                                cellLayout = _cellLayouts[i];
                            }
                        }
                    }
                }
            }
            else
            {
                cellLayout = _cellLayouts[dataIndex];
            }

            Debug.Assert(cellLayout != null, $"Could not find a cell layout for dataIndex {dataIndex}");

            // get the jump locations based on the cell layout logic rect and the scroller offset / cell offset

            var jumpToLocationX = cellLayout.logicRect.x - (scrollerOffset != null ? scrollerOffset.Value.x * ViewportSize.x : 0f) + (cellOffset != null ? (cellOffset.Value.x * cellLayout.logicRect.width) : 0f);
            var jumpToLocationY = cellLayout.logicRect.y - (scrollerOffset != null ? scrollerOffset.Value.y * ViewportSize.y : 0f) + (cellOffset != null ? (cellOffset.Value.y * cellLayout.logicRect.height) : 0f);
            var jumpToLocation = new Vector2(jumpToLocationX, jumpToLocationY);

            // start tweening to the jump location

            StartCoroutine(TweenScrollPosition(tweenTypeX, tweenTimeX, tweenTypeY, tweenTimeY, ScrollPosition, jumpToLocation, jumpCompleted, false));
        }

        /// <summary>
		/// Jump not to a data index, but to an offset from the current scrollposition
		/// </summary>
		/// <param name="offset">How much to jump</param>
		/// <param name="tweenTypeX">How to tween on the x axis. See EnhancedTween class.</param>
		/// <param name="tweenTimeX">Duration to tween on the x axis.</param>
		/// <param name="tweenTypeY">How to tween on the y axis. See EnhacnedTween class.</param>
		/// <param name="tweenTimeY">Duration to tween on the y axis.</param>
		/// <param name="jumpCompleted">What method to call when the jump has completed</param>
        public virtual void JumpOffset(
            Vector2 offset,
            TweenType tweenTypeX = TweenType.immediate,
            float tweenTimeX = 0f,
            TweenType tweenTypeY = TweenType.immediate,
            float tweenTimeY = 0f,
            Action<EnhancedGrid> jumpCompleted = null
            )
        {
            // if no offset, then just call jump completed and exit

            if (offset == Vector2.zero)
            {
                if (jumpCompleted != null)
                {
                    jumpCompleted(this);
                }

                return;
            }

            // make sure parameters are valid

            Debug.Assert(tweenTimeX >= 0, "Tween time x must be greater than or equal to zero.");
            Debug.Assert(tweenTimeY >= 0, "Tween time y must be greater than or equal to zero.");

            var jumpToLocation = ScrollPosition + offset;

            // start tweening

            StartCoroutine(TweenScrollPosition(tweenTypeX, tweenTimeX, tweenTypeY, tweenTimeY, ScrollPosition, jumpToLocation, jumpCompleted, false));
        }

        /// <summary>
		/// Jumps to a data index, but tries to fit the entire cell into the Viewport
		/// </summary>
		/// <param name="dataIndex">The data index of cell</param>
		/// <param name="useExtendedVisibleArea">Whether to use the extended visible area or just the Viewport rect</param>
		/// <param name="focusPriority">Where in the cell to focus on (0..1 on each axis). This is useful if the cell is larger than the Viewport and you want to bring in a specific part of the cell.</param>
		/// <param name="tweenTypeX">How to tween on the x axis. See EnhancedTween class.</param>
		/// <param name="tweenTimeX">Duration to tween on the x axis.</param>
		/// <param name="tweenTypeY">How to tween on the y axis. See EnhancedTween class.</param>
		/// <param name="tweenTimeY">Duration to tween on the y axis.</param>
		/// <param name="jumpCompleted">What method to call when the jump has completed</param>
        public virtual void JumpToMakeDataIndexFullyViewable(
            int dataIndex,
            bool useExtendedVisibleArea = false,
            Vector2? focusPriority = null,
            TweenType tweenTypeX = TweenType.immediate,
            float tweenTimeX = 0f,
            TweenType tweenTypeY = TweenType.immediate,
            float tweenTimeY = 0f,
            Action<EnhancedGrid> jumpCompleted = null
            )
        {
            // calculate the offset to make this cell visible
            var offset = GetOffsetToMakeDataIndexFullyViewable(dataIndex, useExtendedVisibleArea, focusPriority);

            // if offset is zero, just call jump completed and return

            if (offset == Vector2.zero)
            {
                if (jumpCompleted != null)
                {
                    jumpCompleted(this);

                    return;
                }
            }
            else
            {
                // jump to the offset

                JumpOffset(offset, tweenTypeX, tweenTimeX, tweenTypeY, tweenTimeY, jumpCompleted);
            }
        }

        /// <summary>
        /// Toggles the tween paused state.
        /// Use this if you want to resume tweening from the current scroll position instead of
        /// where the tween left off when paused.
        /// </summary>
        /// <param name="newTweenTimeX">Optional new tween time for x scroll position. -1 will use the remaining tween time left.</param>
        /// <param name="newTweenTimeY">Optional new tween time for y scroll position. -1 will use the remaining tween time left.</param>
        public virtual void ToggleTweenPaused(float newTweenTimeX = -1, float newTweenTimeY = -1)
        {
            if (!_tweenPaused)
            {
                _tweenPaused = true;
                _tweenPauseToggledOff = false;
            }
            else
            {
                _tweenPaused = false;
                _tweenPauseToggledOff = true;
                _tweenPauseNewTweenTimeX = newTweenTimeX;
                _tweenPauseNewTweenTimeY = newTweenTimeY;
            }
        }

        /// <summary>
		/// Returns the data index of a particular scroll position.
		/// </summary>
		/// <param name="position">The position to look at</param>
		/// <param name="mustBeInCellLayout">If the position is not inside a cell layout (like in the space between cells or groups) and this value is true, then -1 is returned</param>
		/// <returns>The data index of the cell at the position</returns>
        public virtual int GetDataIndexAtPosition(Vector2 position, bool mustBeInCellLayout)
        {
            // calculate the visible rect around this position

            var subsetRect = new Rect(position.x - ViewportSize.x - _groupLayoutSpacing - _cellLayoutSpacing,
                                        position.y - ViewportSize.y - _groupLayoutSpacing - _cellLayoutSpacing,
                                        (ViewportSize.x * 2.0f) + (_groupLayoutSpacing + _cellLayoutSpacing),
                                        (ViewportSize.y * 2.0f) + (_groupLayoutSpacing + _cellLayoutSpacing)
                                        );

            // get all the cells in the subset rect based on the occlusion sectors (for speed)

            var subsetCells = _baseOcclusionSector.GetOcclusionCells(subsetRect, false);
            var dataIndex = -1;
            var closestDistanceSquared = Mathf.Infinity;

            // check all the subset cell layouts and find the one that is closest

            if (subsetCells.Count > 0)
            {
                for (var i = 0; i < subsetCells.Count; i++)
                {
                    var distanceSquared = subsetCells[i].logicRect.DistanceToSquared(position);

                    if (distanceSquared == 0)
                    {
                        dataIndex = subsetCells[i].dataIndex;
                        break;
                    }
                    else if (!mustBeInCellLayout)
                    {
                        if (distanceSquared < closestDistanceSquared)
                        {
                            dataIndex = subsetCells[i].dataIndex;
                            closestDistanceSquared = distanceSquared;
                        }
                    }
                }
            }
            else
            {
                // no subset cells were found based on the occlusion sectors,
                // so just check all cell layouts

                for (var i = 0; i < _cellLayouts.Count; i++)
                {
                    var distanceSquared = _cellLayouts[i].logicRect.DistanceToSquared(position);

                    if (distanceSquared == 0)
                    {
                        dataIndex = subsetCells[i].dataIndex;
                        break;
                    }
                    else if (!mustBeInCellLayout)
                    {
                        if (distanceSquared < closestDistanceSquared)
                        {
                            dataIndex = _cellLayouts[i].dataIndex;
                            closestDistanceSquared = distanceSquared;
                        }
                    }
                }
            }

            return dataIndex;
        }

        /// <summary>
		/// Determines if a particular cell is visible
		/// </summary>
		/// <param name="dataIndex">The data index of the cell.</param>
		/// <param name="fullyViewable">Is the cell completely in the visible area?</param>
		/// <param name="useExtendedVisibleArea">Should we look in the extended area, or just the Viewport rect</param>
		/// <returns>Whether the cell is viewable</returns>
        public bool GetIsDataIndexViewable(int dataIndex, bool fullyViewable = false, bool useExtendedVisibleArea = false)
        {
            // make sure parameters are valid

            Debug.Assert(dataIndex >= 0 && dataIndex < _originalDataCount, $"GetIsDataIndexViewable: dataIndex {dataIndex} must be between 0 and {_originalDataCount - 1}");

            var cellLayout = _cellLayouts[dataIndex];

            Rect visibleRect;

            // calculate the visible rect based on whether or not we are using the extended visible area

            if (useExtendedVisibleArea)
            {
                visibleRect = new Rect(ScrollPosition.x - _extendVisibleAreaLeft,
                                        ScrollPosition.y - _extendVisibleAreaTop,
                                        ViewportSize.x + (_extendVisibleAreaLeft + _extendVisibleAreaRight),
                                        ViewportSize.y + (_extendVisibleAreaTop + _extendVisibleAreaBottom)
                                        );
            }
            else
            {
                visibleRect = new Rect(ScrollPosition.x,
                                        ScrollPosition.y,
                                        ViewportSize.x,
                                        ViewportSize.y
                                        );
            }

            if (fullyViewable)
            {
                // return true only if the cell is fully inside the visible rect

                return visibleRect.FullyContainsRect(cellLayout.logicRect);
            }
            else
            {
                // return true if any part of the cell is inside the visible rect

                return visibleRect.Overlaps(cellLayout.logicRect);
            }
        }

        /// <summary>
		/// Get a cell layout for a data index.
		/// </summary>
		/// <param name="dataIndex">The data index to check.</param>
		/// <returns>Cell Layout for the cell</returns>
        public CellLayout GetDataIndexCellLayout(int dataIndex)
        {
            // make sure parameters are valid

            Debug.Assert(dataIndex >= 0 && dataIndex < _originalDataCount, $"GetDataIndexCellLayout: dataIndex {dataIndex} must be between 0 and {_originalDataCount - 1}");

            // return the cell layout
            return _cellLayouts[dataIndex];
        }

        /// <summary>
		/// Calculates the scroll position offset necessary to make a cell fully visible
		/// </summary>
		/// <param name="dataIndex">The data index of the cell.</param>
		/// <param name="useExtendedVisibleArea">Whether to use the extended visible area or just the Viewport rect.</param>
		/// <param name="focusPriority">Where in the cell to focus on (0..1 on each axis). This is useful if the cell is larger than the Viewport and you want to bring in a specific part of the cell.</param>
		/// <returns>How far to scroll to make the cell fully visible</returns>
        public Vector2 GetOffsetToMakeDataIndexFullyViewable(
            int dataIndex,
            bool useExtendedVisibleArea = false,
            Vector2? focusPriority = null
            )
        {
            // make sure parameters are valid

            Debug.Assert(dataIndex >= 0 && dataIndex < _originalDataCount, $"GetOffsetToMakeDataIndexFullyViewable: dataIndex {dataIndex} must be between 0 and {_originalDataCount - 1}");

            var cellLayout = _cellLayouts[dataIndex];

            Rect visibleRect;

            // calculate the visible rect based on whether or not we are using the extended visible area

            if (useExtendedVisibleArea)
            {
                visibleRect = new Rect(ScrollPosition.x - _extendVisibleAreaLeft,
                                        ScrollPosition.y - _extendVisibleAreaTop,
                                        ViewportSize.x + (_extendVisibleAreaLeft + _extendVisibleAreaRight),
                                        ViewportSize.y + (_extendVisibleAreaTop + _extendVisibleAreaBottom)
                                        );
            }
            else
            {
                visibleRect = new Rect(ScrollPosition.x,
                                        ScrollPosition.y,
                                        ViewportSize.x,
                                        ViewportSize.y
                                        );
            }

            if (visibleRect.FullyContainsRect(cellLayout.logicRect))
            {
                // the cell is already full visible, so no offset necessary

                return Vector2.zero;
            }
            else
            {
                // cell is not fully visible, so calculate the offset

                var xOffset = 0f;
                var yOffset = 0f;

                if (focusPriority != null && cellLayout.logicRect.width > visibleRect.width)
                {
                    // the cell's width is larger than the visible rect's width and we want to focus on a specific part of the cell

                    var focusPoint = cellLayout.logicRect.PointInRect(focusPriority.Value);

                    if (!visibleRect.Contains(focusPoint))
                    {
                        // focus point is not in the visible rect, so we calculate the offset to get it there

                        if (focusPoint.x < visibleRect.xMin) xOffset = focusPoint.x - visibleRect.xMin;
                        if (focusPoint.x > visibleRect.xMax) xOffset = focusPoint.x - visibleRect.xMax;
                    }
                }
                else if (cellLayout.logicRect.xMin < visibleRect.xMin)
                {
                    // cell's width is either smaller than the visible rect's width or we don't have a focus priority
                    // cell's left side is to the left of the visible rect's left side

                    xOffset = cellLayout.logicRect.xMin - visibleRect.xMin;
                }
                else if (cellLayout.logicRect.xMax > visibleRect.xMax)
                {
                    // cell's width is either smaller than the visible rect's width or we don't have a focus priority
                    // cell's right side is to the right of the visible rect's right side

                    xOffset = cellLayout.logicRect.xMax - visibleRect.xMax;
                }

                if (focusPriority != null && cellLayout.logicRect.height > visibleRect.height)
                {
                    // the cell's height is larger than the visible rect's height and we want to focus on a specific part of the cell

                    var focusPoint = cellLayout.logicRect.PointInRect(focusPriority.Value);

                    if (!visibleRect.Contains(focusPoint))
                    {
                        // focus point is not in the visible rect, so we calculate the offset to get it there

                        if (focusPoint.y < visibleRect.yMin) yOffset = focusPoint.y - visibleRect.yMin;
                        if (focusPoint.y > visibleRect.yMax) yOffset = focusPoint.y - visibleRect.yMax;
                    }
                }
                else if (cellLayout.logicRect.yMin < visibleRect.yMin)
                {
                    // cell's height is either smaller than the visible rect's height or we don't have a focus priority
                    // cell's top side is above the visible rect's top side

                    yOffset = cellLayout.logicRect.yMin - visibleRect.yMin;
                }
                else if (cellLayout.logicRect.yMax > visibleRect.yMax)
                {
                    // cell's height is either smaller than the visible rect's height or we don't have a focus priority
                    // cell's bottom side is below the visible rect's bottom side

                    yOffset = cellLayout.logicRect.yMax - visibleRect.yMax;
                }

                return new Vector2(xOffset, yOffset);
            }
        }

        /// <summary>
        /// This event is fired when the user begins dragging on the scroller.
        /// We can disable looping or snapping while dragging if desired.
        /// <param name="data">The event data for the drag</param>
        /// </summary>
        public virtual void OnBeginDrag(PointerEventData data)
        {
            _dragFingerCount++;
            if (_dragFingerCount > 1) return;

            if (gridOnBeginDrag != null) gridOnBeginDrag(this, data);
            if (snapGridOnBeginDrag != null) snapGridOnBeginDrag(this, data);

            if (IsTweening && _interruptTweeningOnDrag)
            {
                _interruptTween = true;
            }
        }

        /// <summary>
        /// This event is fired while the user is dragging the ScrollRect.
        /// We use it to capture the drag position that will later be used in the OnEndDrag method.
        /// </summary>
        /// <param name="data">The event data for the drag</param>
        public virtual void OnDrag(PointerEventData data)
        {
            if (gridOnDrag != null) gridOnDrag(this, data, _dragPreviousPos);
            if (snapGridOnDrag != null) snapGridOnDrag(this, data, _dragPreviousPos);

            _dragPreviousPos = data.position;
        }

        /// <summary>
        /// This event is fired when the user ends dragging on the scroller.
        /// We can re-enable looping or snapping while dragging if desired.
        /// <param name="data">The event data for the drag</param>
        /// </summary>
        public virtual void OnEndDrag(PointerEventData data)
        {
            _dragFingerCount--;
            if (_dragFingerCount < 0) _dragFingerCount = 0;

            if (gridOnEndDrag != null) gridOnEndDrag(this, data, _dragPreviousPos);
            if (snapGridOnEndDrag != null) snapGridOnEndDrag(this, data, _dragPreviousPos);
        }

        /// <summary>
		/// Used to update the cells without having to rebuild the grid.
		/// </summary>
        public void RefreshActiveCells()
        {
            for (var i = 0; i < _activeCells.Count; i++)
            {
                _EnhancedGridDelegate.UpdateCell(this, _activeCells[i], _activeCells[i].DataIndex, _activeCells[i].RepeatIndex, _cellLayouts[_activeCells[i].RepeatIndex], _groupLayouts[_cellLayouts[_activeCells[i].RepeatIndex].groupLayoutIndex]);
            }

            _SortActiveCells();
        }

        /// <summary>
		/// Resorts the cell objects based on draw priority. Higher priorities will be drawn last (on top).
		/// </summary>
        public void ResortDrawPriority()
        {
            _SortActiveCells();
        }

        /// <summary>
		/// Returns a copy of the list of active (visible) cells to protect the grid's structures.
		/// </summary>
		/// <returns>Copy of the list of visible cells</returns>
        public EnhancedList<IEnhancedGridCell> GetActiveCells()
        {
            EnhancedList<IEnhancedGridCell> activeCellCopy = new EnhancedList<IEnhancedGridCell>();

            for (var i = 0; i < _activeCells.Count; i++)
            {
                activeCellCopy.Add(_activeCells[i]);
            }

            return activeCellCopy;
        }

        #endregion // Public Methods

        #endregion // Public

        #region Protected

        #region Protected Enums

        /// <summary>
		/// Which axis to make copies in.
		/// </summary>
        protected enum CopyDirection
        {
            X,
            Y
        };

        #endregion // Protected Enums

        #region Protected Fields

        /// <summary>
		/// The controller delegate that the grid will reference when getting information about the cells.
		/// </summary>
        protected IEnhancedGridDelegate _EnhancedGridDelegate;

        /// <summary>
		/// The flow direction of the cell layouts
		/// </summary>
        protected IFlow _flow;

        /// <summary>
		/// The base occlusion sectors to which all sub sectors are ultimately attached.
		/// </summary>
        protected OcclusionSector _baseOcclusionSector;

        /// <summary>
		/// Whether the grid is ready for calculation calls
		/// </summary>
        protected bool _isInitialized;

        // Internal members to store and retrieve movement types between builds

        protected bool _resetMovementType;
        protected ScrollRect.MovementType _previousMovementType;

        // Internal lists to store the cell and group layouts

        protected EnhancedList<CellLayout> _cellLayouts = new EnhancedList<CellLayout>();
        protected EnhancedList<GroupLayout> _groupLayouts = new EnhancedList<GroupLayout>();

        // Internal lists to keep track of objects created in the grid

        protected EnhancedList<GameObject> _groupLayoutObjects = new EnhancedList<GameObject>();
        protected EnhancedList<GameObject> _cellLayoutObjects = new EnhancedList<GameObject>();
        protected EnhancedList<GameObject> _occlusionSectorObjects = new EnhancedList<GameObject>();

        // Color schemes used by the grid

        protected Palette _occlusionSectorPalette;
        protected Palette _groupLayoutPalette;
        protected Palette _cellLayoutPalette;

        // Object prefabs for objects created by the grid

        protected GameObject _groupLayoutGameObjectResource;
        protected GameObject _cellLayoutGameObjectResource;
        protected GameObject _occlusionSectorGameObjectResource;
        protected GameObject _cellGameObjectResource;

        // Debug objects

        protected GameObject _debugRectGameObjectResource;
        protected GameObject _debugVerticalLineGameObjectResource;
        protected GameObject _debugHorizontalLineGameObjectResource;
        protected RectTransform _contentDebugRectTransform;
        protected EnhancedList<GameObject> _debugObjects = new EnhancedList<GameObject>();

        // Lists of active and recycle objects

        protected EnhancedList<IEnhancedGridCell> _activeCells = new EnhancedList<IEnhancedGridCell>();
        protected EnhancedList<IEnhancedGridCell> _recyleCellPool = new EnhancedList<IEnhancedGridCell>();

        // Loop jump positions. These are used to simulate infinite loops by jumping
        // backward or forward in the scroller.

        protected float[] _loopJumpX = new float[3];
        protected float[] _loopJumpY = new float[3];

        // Internal tween members.

        protected float _tweenElapsedTime;
        protected bool _interruptTween = false;
        protected float _tweenPauseNewTweenTimeX;
        protected float _tweenPauseNewTweenTimeY;
        protected bool _tweenPauseToggledOff = false;
        protected EnhancedGridRepeatMode _repeatModeBeforeDragX;
        protected EnhancedGridRepeatMode _repeatModeBeforeDragY;
        protected int _dragFingerCount;
        protected Vector2 _dragPreviousPos;

        // Scrollbars

        protected Scrollbar _horizontalScrollbar;
        protected Scrollbar _verticalScrollber;
        protected bool _resetScrollbars;

        /// <summary>
		/// The last location that was used to calculate the active (visible) cells.
		/// </summary>
        protected Vector2 _activeCalculationLastScrollPosition;

        #endregion // Protected Fields

        #region Protected Unity Methods

        protected virtual void OnEnable()
        {
            // when the scroller is enabled, add a listener to the onValueChanged handler
            ScrollRect.onValueChanged.AddListener(_ScrollRect_OnValueChanged);
        }

        protected virtual void OnDisable()
        {
            // when the scroller is disabled, remove the listener
            ScrollRect.onValueChanged.RemoveListener(_ScrollRect_OnValueChanged);
        }

        protected virtual void Update()
        {
            // determine if the scroller has started or stopped scrolling
            // and call the delegate if so.
            if (_scrollRect.velocity != Vector2.zero && !IsScrolling)
            {
                IsScrolling = true;
                if (gridScrollingChanged != null) gridScrollingChanged(this, true);
            }
            else if (_scrollRect.velocity == Vector2.zero && IsScrolling)
            {
                IsScrolling = false;
                if (gridScrollingChanged != null) gridScrollingChanged(this, false);
            }
        }

        protected virtual void LateUpdate()
        {
            if (_maxVelocity != Vector2.zero)
            {
                // keep the maximum velocity below the threshold

                var velocityX = _scrollRect.velocity.x;
                var velocityY = _scrollRect.velocity.y;

                if (_maxVelocity.x > 0)
                {
                    velocityX = Mathf.Sign(_scrollRect.velocity.x) * MathF.Min(Mathf.Abs(_scrollRect.velocity.x), _maxVelocity.x);
                }
                if (_maxVelocity.y > 0)
                {
                    velocityY = Mathf.Sign(_scrollRect.velocity.y) * MathF.Min(Mathf.Abs(_scrollRect.velocity.y), _maxVelocity.y);
                }

                _scrollRect.velocity = new Vector2(velocityX, velocityY);
            }

            if (_resetMovementType)
            {
                // called after recaculating the grid

                _scrollRect.movementType = _previousMovementType;
                _resetMovementType = false;
            }

            // re-establish the scrollbars.
            // this gets around a bug in some versions of Unity where
            // the scrollbar causes large content rects to not have inertia.
            if (_resetScrollbars)
            {
                _resetScrollbars = false;
                _scrollRect.horizontalScrollbar = _horizontalScrollbar;
                _scrollRect.verticalScrollbar = _verticalScrollber;
            }
        }

        #endregion // Private Unity Methods

        #region Protected Methods

        /// <summary>
		/// Called whenever the ScrollRect fires a OnValueChanged event.
		/// </summary>
		/// <param name="val">The normalized scroll position (in Unity coordinates)</param>
        protected virtual void _ScrollRect_OnValueChanged(Vector2 val)
        {
            // reverse the y coordinate so that y goes from top to bottom,
            // instead of Unity's bottom to top
            _scrollNormalizedPosition = new Vector2(val.x, 1.0f - val.y);

            // cache the scrollposition for calculations in this method;
            var scrollPosition = ScrollPosition;
            var doJump = false;

            if (gridScrolled != null) gridScrolled(this, ScrollPosition, ScrollNormalizedPosition);

            if (
                    (_repeatModeX == EnhancedGridRepeatMode.Loop || _repeatModeY == EnhancedGridRepeatMode.Loop)
                    &&
                    (!IsDragging || _loopWhileDragging)
                )
            {
                if (_repeatModeX == EnhancedGridRepeatMode.Loop)
                {
                    // we are x looping, so check if the scroll x position is past any of the loop jump locations

                    if (scrollPosition.x <= _loopJumpX[0])
                    {
                        doJump = true;
                        _scrollNormalizedPosition.x = (_loopJumpX[1] - (_loopJumpX[0] - scrollPosition.x)) / ScrollSize.x;
                    }

                    if ((scrollPosition.x + ViewportSize.x) >= _loopJumpX[2])
                    {
                        doJump = true;
                        _scrollNormalizedPosition.x = (_loopJumpX[1] - _loopJumpX[2] + scrollPosition.x) / ScrollSize.x;
                    }
                }

                if (_repeatModeY == EnhancedGridRepeatMode.Loop)
                {
                    // we are y looping, so check if the scroll y position is past any of the loop jump locations

                    if (scrollPosition.y <= _loopJumpY[0])
                    {
                        doJump = true;
                        _scrollNormalizedPosition.y = (_loopJumpY[1] - (_loopJumpY[0] - scrollPosition.y)) / ScrollSize.y;
                    }

                    if ((scrollPosition.y + ViewportSize.y) >= _loopJumpY[2])
                    {
                        doJump = true;
                        _scrollNormalizedPosition.y = (_loopJumpY[1] - _loopJumpY[2] + scrollPosition.y) / ScrollSize.y;
                    }
                }

                if (doJump)
                {
                    // do a jump to simulate infinite loops

                    // cache the scroll rect's velocity, inertia, and movement type
                    // setting them to no movement values

                    var velocity = _scrollRect.velocity;
                    var inertia = _scrollRect.inertia;
                    var movementType = _scrollRect.movementType;
                    _scrollRect.velocity = Vector2.zero;
                    _scrollRect.inertia = false;
                    _scrollRect.movementType = ScrollRect.MovementType.Clamped;

                    // turn off the onValueChanged delegate temporarily and set the new position

                    _scrollRect.onValueChanged.RemoveListener(_ScrollRect_OnValueChanged);
                    _scrollRect.normalizedPosition = new Vector2(_scrollNormalizedPosition.x, 1f - _scrollNormalizedPosition.y);
                    _scrollRect.onValueChanged.AddListener(_ScrollRect_OnValueChanged);

                    // re-establish the scroll rect's movement values

                    _scrollRect.velocity = velocity;
                    _scrollRect.inertia = inertia;
                    _scrollRect.movementType = movementType;
                }
            }

            // if there is a snap grid addon, call the delegate to let it know the grid has scrolled

            if (snapGridScrolled != null) snapGridScrolled(this, scrollPosition, ScrollNormalizedPosition);

            if (_recycleCells && _showCells)
            {
                // if we are recycling, then calculate which cells are visible, if necessary

                bool updateVisibleCells = false;

                if (doJump)
                {
                    // if we did a jump, then always calculate the visible cells

                    updateVisibleCells = true;
                }
                else
                {
                    if (_minimumScrollForActiveCellUpdate != Vector2.zero)
                    {
                        if (
                                Mathf.Abs(_activeCalculationLastScrollPosition.x - scrollPosition.x) >= _minimumScrollForActiveCellUpdate.x
                                ||
                                Mathf.Abs(_activeCalculationLastScrollPosition.y - scrollPosition.y) >= _minimumScrollForActiveCellUpdate.y
                            )
                        {
                            // more than the minium scroll has occurred, so we need to do another visible calculation

                            updateVisibleCells = true;
                            _activeCalculationLastScrollPosition = scrollPosition;
                        }
                    }
                    else
                    {
                        // no minimum scroll set, so always do a calculation (this is not performant, so try to use MinimumScrollForActiveCellUpdate)

                        updateVisibleCells = true;
                    }
                }

                // update the visible cells if necessary

                if (updateVisibleCells) _ShowVisibleCells();
            }
        }

        /// <summary>
		/// Calcluate the visible cells.
		/// </summary>
        protected virtual void _ShowVisibleCells()
        {
            // calculate the visible rect based on the Viewport and extended visible area
            VisibleRect = new Rect(ScrollPosition.x - _extendVisibleAreaLeft,
                                    ScrollPosition.y - _extendVisibleAreaTop,
                                    ViewportSize.x + (_extendVisibleAreaLeft + _extendVisibleAreaRight),
                                    ViewportSize.y + (_extendVisibleAreaTop + _extendVisibleAreaBottom)
                                    );

            // grab the visible cell layouts by traversing the occlusion sector graph

            var visibleCells = _baseOcclusionSector.GetOcclusionCells(VisibleRect, false);
            bool found;
            int totalRecycled = 0;
            int totalCreated = 0;

            // recycle active cells that are no longer visible

            for (var a = 0; a < _activeCells.Count; a++)
            {
                found = false;
                for (var v = 0; v < visibleCells.Count; v++)
                {
                    if (_activeCells[a].RepeatIndex == visibleCells[v].repeatIndex)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    //Debug.Log($"recycling cell {_activeCells[a].DataIndex}");

                    _RecycleCell(a);
                    totalRecycled++;
                }
            }

            // create new active cells that were not visible before

            for (var v = 0; v < visibleCells.Count; v++)
            {
                found = false;
                for (var a = 0; a < _activeCells.Count; a++)
                {
                    if (visibleCells[v].repeatIndex == _activeCells[a].RepeatIndex)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    //Debug.Log($"adding new visible cell {visibleCells[v].dataIndex}");

                    _CreateCellGameObject(visibleCells[v].dataIndex, visibleCells[v].repeatIndex);
                    totalCreated++;
                }
            }

            // sort the visible cells by draw priority

            _SortActiveCells();
        }

        /// <summary>
		/// Sorts the visible cells by draw priority.
		/// </summary>
        protected virtual void _SortActiveCells()
        {
            List<IEnhancedGridCell> sortedList = _activeCells.ToList().OrderBy(o => o.GetDrawPriority()).ToList();
            for (var i = 0; i < sortedList.Count; i++)
            {
                sortedList[i].ContainerTransform.SetSiblingIndex(i);
            }
        }

        /// <summary>
		/// Creates a game object for a container of sub content.
		/// </summary>
		/// <param name="name">Name of the container</param>
		/// <returns>RectTransform of the container</returns>
        protected virtual RectTransform _AddContentSubset(string name)
        {
            var go = new GameObject(name);

            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.SetParent(ContentRectTransform);
            rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
            rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
            rectTransform.pivot = new Vector2(0.0f, 1.0f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;

            return rectTransform;
        }

        /// <summary>
		/// Loads in color schemes used by debug objects.
		/// </summary>
        protected virtual void _LoadPalettes()
        {
            _occlusionSectorPalette = new Palette(@"EnhancedGrid\Palettes\OcclusionSector");
            _groupLayoutPalette = new Palette(@"EnhancedGrid\Palettes\GroupLayout");
            _cellLayoutPalette = new Palette(@"EnhancedGrid\Palettes\CellLayout");
        }

        /// <summary>
		/// Load in objects for debugging.
		/// </summary>
        protected virtual void _LoadResources()
        {
            if (_groupLayoutGameObjectResource == null)
            {
                _groupLayoutGameObjectResource = Resources.Load<GameObject>(@"EnhancedGrid\Prefabs\GridGroupLayout");
            }

            if (_cellLayoutGameObjectResource == null)
            {
                _cellLayoutGameObjectResource = Resources.Load<GameObject>(@"EnhancedGrid\Prefabs\GridCellLayout");
            }

            if (_occlusionSectorGameObjectResource == null)
            {
                _occlusionSectorGameObjectResource = Resources.Load<GameObject>(@"EnhancedGrid\Prefabs\OcclusionSector");
            }

            if (_cellGameObjectResource == null)
            {
                _cellGameObjectResource = Resources.Load<GameObject>(@"EnhancedGrid\Prefabs\GridCell");
            }

            if (_debugRectGameObjectResource == null)
            {
                _debugRectGameObjectResource = Resources.Load<GameObject>(@"EnhancedGrid\Prefabs\DebugRect");
            }

            if (_debugVerticalLineGameObjectResource == null)
            {
                _debugVerticalLineGameObjectResource = Resources.Load<GameObject>(@"EnhancedGrid\Prefabs\DebugVerticalLine");
            }

            if (_debugHorizontalLineGameObjectResource == null)
            {
                _debugHorizontalLineGameObjectResource = Resources.Load<GameObject>(@"EnhancedGrid\Prefabs\DebugHorizontalLine");
            }
        }

        /// <summary>
		/// Create the flow based on the flow direction.
		/// </summary>
        protected void _CreateFlow()
        {
            if (_flow == null || (_flow != null && _flow.GetFlowDirection() != FlowDirection))
            {
                switch (FlowDirection)
                {
                    case EnhancedGridFlowDirection.LeftToRightTopToBottom: _flow = new FlowLeftToRightTopToBottom(this); break;
                    case EnhancedGridFlowDirection.RightToLeftTopToBottom: _flow = new FlowRightToLeftTopToBottom(this); break;
                    case EnhancedGridFlowDirection.LeftToRightBottomToTop: _flow = new FlowLeftToRightBottomToTop(this); break;
                    case EnhancedGridFlowDirection.RightToLeftBottomToTop: _flow = new FlowRightToLeftBottomToTop(this); break;
                    case EnhancedGridFlowDirection.TopToBottomLeftToRight: _flow = new FlowTopToBottomLeftToRight(this); break;
                    case EnhancedGridFlowDirection.BottomToTopLeftToRight: _flow = new FlowBottomToTopLeftToRight(this); break;
                    case EnhancedGridFlowDirection.TopToBottomRightToLeft: _flow = new FlowTopToBottomRightToLeft(this); break;
                    case EnhancedGridFlowDirection.BottomToTopRightToLeft: _flow = new FlowBottomToTopRightToLeft(this); break;
                }
            }
        }

        /// <summary>
		/// Creates an occlusion sector debug game object
		/// </summary>
		/// <param name="sector">The occlusion sector to base the object on</param>
		/// <param name="sectorIndex">The index of the sector</param>
        protected virtual void _CreateOcclusionSectorGameObject(OcclusionSector sector, int sectorIndex)
        {
            var go = GameObject.Instantiate(_occlusionSectorGameObjectResource, ContentOcclusionSectorsRectTransform);
            go.name = $"OcclusionSector_{sector.Name}";
            _occlusionSectorObjects.Add(go);

            var rectTransform = go.GetComponent<RectTransform>();

            rectTransform.localPosition = sector.UIPosition;
            rectTransform.sizeDelta = sector.Size;

            var text = go.GetComponentInChildren<Text>();
            text.text = $"{sector.Name}\n{sector.LogicPosition}\n{sector.Size}\n{sector.CellRepeatIndicesString}";

            var image = go.GetComponentInChildren<Image>();
            image.color = _occlusionSectorPalette.GetColor(sectorIndex);
        }

        /// <summary>
		/// Creates a group layout debug game object
		/// </summary>
		/// <param name="groupIndex">Index of the group</param>
		/// <param name="groupColorIndex">Color of the group</param>
        protected virtual void _CreateGroupLayoutGameObject(int groupIndex, int groupColorIndex)
        {
            var go = GameObject.Instantiate(_groupLayoutGameObjectResource, ContentGroupLayoutsRectTransform);
            go.name = $"GroupLayout_{groupIndex}";
            _groupLayoutObjects.Add(go);

            var rectTransform = go.GetComponent<RectTransform>();

            rectTransform.localPosition = new Vector3(_groupLayouts[groupIndex].uiRect.x, _groupLayouts[groupIndex].uiRect.y, 0);
            rectTransform.sizeDelta = new Vector2(_groupLayouts[groupIndex].uiRect.width, _groupLayouts[groupIndex].uiRect.height);

            var image = go.GetComponentInChildren<Image>();
            image.color = _groupLayoutPalette.GetColor(groupColorIndex);
        }

        /// <summary>
		/// Creates a cell layout debug game object
		/// </summary>
		/// <param name="dataIndex">Data index of cell</param>
		/// <param name="repeatIndex">Repeat index of cell (if filling or looping)</param>
        protected virtual void _CreateCellLayoutGameObject(int dataIndex, int repeatIndex)
        {
            var go = GameObject.Instantiate(_cellLayoutGameObjectResource, ContentCellLayoutsRectTransform);
            go.name = $"CellLayout_{dataIndex}_{repeatIndex}";
            _cellLayoutObjects.Add(go);

            var rectTransform = go.GetComponent<RectTransform>();

            rectTransform.localPosition = new Vector3(_cellLayouts[repeatIndex].uiRect.x, _cellLayouts[repeatIndex].uiRect.y, 0.0f);
            rectTransform.sizeDelta = new Vector2(_cellLayouts[repeatIndex].uiRect.width, _cellLayouts[repeatIndex].uiRect.height);

            var image = go.GetComponentInChildren<Image>();
            image.color = _cellLayoutPalette.GetColor(dataIndex);
        }

        /// <summary>
		/// Creates a cell game object for the first time (not if a cell is reused).
		/// </summary>
		/// <param name="dataIndex">Data index of cell</param>
		/// <param name="repeatIndex">Repeat index of cell (if filling or looping)</param>
		/// <returns></returns>
        protected virtual IEnhancedGridCell _CreateCellGameObject(int dataIndex, int repeatIndex)
        {
            // grab the prefab from the controller delegate
            var prefab = _EnhancedGridDelegate.GetCellPrefab(this, dataIndex);

            // make sure the prefab is valid
            Debug.Assert(prefab != null, $"No cell prefab found for cell data index {dataIndex}");

            // get the IEnhancedGridCell interface from the prefab
            var cell = prefab.GetComponent<IEnhancedGridCell>();

            // make sure the cell implements the IEnhancedGridCell interface
            Debug.Assert(cell != null, $"Cell prefab at cell data index {dataIndex} must implement the IEnhancedGridCell interface.");

            // get the unique cell type identifier (used when multiple cell types are in the grid)
            string prefabCellTypeIdentifier = cell.GetCellTypeIdentifier();

            // check the recycle pool first to reduce game object creation

            GameObject cellGO = null;

            for (var i = 0; i < _recyleCellPool.Count; i++)
            {
                if (_recyleCellPool[i].GetCellTypeIdentifier() == prefabCellTypeIdentifier)
                {
                    // this recycled cell is the same type as the new cell, so we reuse it

                    cell = _recyleCellPool[i];
                    cell.DataIndex = dataIndex;
                    cell.RepeatIndex = repeatIndex;
                    cell.ContainerTransform.gameObject.SetActive(true);

                    //Debug.Log($"setting gameobject {cell.ContainerTransform.gameObject.name} to visible");

                    _ResetCellContainer(cell.ContainerTransform.gameObject, dataIndex, repeatIndex);

                    _activeCells.Add(cell);
                    _recyleCellPool.RemoveAt(i, ignoreOrder: true);

                    //Debug.Log($"recycled cell {dataIndex}");

                    if (gridCellVisibilityChanged != null) gridCellVisibilityChanged(this, cell, true);
                    if (gridCellReused != null) gridCellReused(this, cell);
                    if (gridCellActivated != null) gridCellActivated(this, cell);

                    _EnhancedGridDelegate.UpdateCell(this, cell, dataIndex, repeatIndex, _cellLayouts[repeatIndex], _groupLayouts[_cellLayouts[repeatIndex].groupLayoutIndex]);

                    return cell;
                }
            }

            // no cell available in the recycle pool. create a new one

            var cellContainerGO = GameObject.Instantiate(_cellGameObjectResource, ContentCellsRectTransform);
            var rectTransform = _ResetCellContainer(cellContainerGO, dataIndex, repeatIndex);
            cellGO = GameObject.Instantiate(prefab, rectTransform);
            cellGO.SetActive(true);

            cell = cellGO.GetComponent<IEnhancedGridCell>();
            cell.DataIndex = dataIndex;
            cell.RepeatIndex = repeatIndex;
            cell.ContainerTransform = cellContainerGO.transform;

            _activeCells.Add(cell);

            //Debug.Log($"created new cell {dataIndex}");

            if (gridCellCreated != null) gridCellCreated(this, cell);
            if (gridCellActivated != null) gridCellActivated(this, cell);

            // tell the controller delegate to update the cell

            _EnhancedGridDelegate.UpdateCell(this, cell, dataIndex, repeatIndex, _cellLayouts[repeatIndex], _groupLayouts[_cellLayouts[repeatIndex].groupLayoutIndex]);

            return cell;
        }

        /// <summary>
		/// Resets a cell's container object
		/// </summary>
		/// <param name="go">Container game object</param>
		/// <param name="dataIndex">Data index of the cell</param>
		/// <param name="repeatIndex">Repeat index of the cell (if filling or looping)</param>
		/// <returns></returns>
        protected virtual RectTransform _ResetCellContainer(GameObject go, int dataIndex, int repeatIndex)
        {
            go.name = $"Cell_{dataIndex}_{repeatIndex}";

            var rectTransform = go.GetComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
            rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
            rectTransform.pivot = new Vector2(0.0f, 1.0f);
            rectTransform.localPosition = new Vector3(_cellLayouts[repeatIndex].uiRect.x, _cellLayouts[repeatIndex].uiRect.y, 0.0f);
            rectTransform.sizeDelta = new Vector2(_cellLayouts[repeatIndex].uiRect.width, _cellLayouts[repeatIndex].uiRect.height);

            return rectTransform;
        }

        /// <summary>
		/// Creates a debug game object for a given rect
		/// </summary>
		/// <param name="rect">The rect position</param>
		/// <param name="color">Color of the debug object</param>
		/// <returns></returns>
        protected virtual GameObject _CreateDebugRectGameObject(Rect rect, Color color)
        {
            var go = GameObject.Instantiate(_debugRectGameObjectResource, _contentDebugRectTransform);
            go.name = $"DebugRect_{rect.ToString()}";
            _debugObjects.Add(go);

            var rectTransform = go.GetComponent<RectTransform>();

            rectTransform.localPosition = new Vector3(rect.x, rect.y, 0);
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);

            var text = go.GetComponentInChildren<Text>();
            text.text = rect.ToString();

            var image = go.GetComponent<Image>();
            image.color = color;

            return go;
        }

        /// <summary>
		/// Creates a debug vertical line object
		/// </summary>
		/// <param name="x">X coordinate of line</param>
		/// <param name="color">Color of line</param>
		/// <param name="width">Width of line</param>
		/// <returns></returns>
        protected virtual GameObject _CreateDebugVerticalLineGameObject(float x, Color color, float width = 10f)
        {
            var go = GameObject.Instantiate(_debugVerticalLineGameObjectResource, _contentDebugRectTransform);
            go.name = $"DebugVerticalLine_{x}";
            _debugObjects.Add(go);

            var rectTransform = go.GetComponent<RectTransform>();

            rectTransform.localPosition = new Vector3(x, 0, 0);

            var image = go.GetComponent<Image>();
            image.color = color;

            return go;
        }

        /// <summary>
        /// Creates a debug horizontal line object
        /// </summary>
        /// <param name="y">Y coordinate of line</param>
        /// <param name="color">Color of line</param>
        /// <param name="height">Height of line</param>
        /// <returns></returns>
        protected virtual GameObject _CreateDebugHorizontalLineGameObject(float y, Color color, float height = 10f)
        {
            var go = GameObject.Instantiate(_debugHorizontalLineGameObjectResource, _contentDebugRectTransform);
            go.name = $"DebugHorizontalLine_{y}";
            _debugObjects.Add(go);

            var rectTransform = go.GetComponent<RectTransform>();

            rectTransform.localPosition = new Vector3(0, -y, 0);

            var image = go.GetComponent<Image>();
            image.color = color;

            return go;
        }

        /// <summary>
		/// Removes all objects from the grid
		/// </summary>
		/// <param name="forceDestroyCells"></param>
        protected virtual void _ClearObjects(bool forceDestroyCells = false)
        {
            for (var i = 0; i < _groupLayoutObjects.Count; i++)
            {
                Destroy(_groupLayoutObjects[i]);
            }
            _groupLayoutObjects.Clear();

            for (var i = 0; i < _cellLayoutObjects.Count; i++)
            {
                Destroy(_cellLayoutObjects[i]);
            }
            _cellLayoutObjects.Clear();

            for (var i = 0; i < _occlusionSectorObjects.Count; i++)
            {
                Destroy(_occlusionSectorObjects[i]);
            }
            _occlusionSectorObjects.Clear();

            if (forceDestroyCells || !_recycleCells || _destroyCellsWhenRecalculatingGrid)
            {
                // we are either recycling, recycling has changed (to true or false), or DestroyCellsWhenRecalculatingGrid is true

                for (var i = 0; i < _activeCells.Count; i++)
                {
                    if (gridCellWillDestroy != null) gridCellWillDestroy(this, _activeCells[i]);
                    Destroy(_activeCells[i].ContainerTransform.gameObject);
                }
                for (var i = 0; i < _recyleCellPool.Count; i++)
                {
                    if (gridCellWillDestroy != null) gridCellWillDestroy(this, _recyleCellPool[i]);
                    Destroy(_recyleCellPool[i].ContainerTransform.gameObject);
                }

                _activeCells.Clear();
                _recyleCellPool.Clear();
            }
            else
            {
                // just recycle the cells without destroying them

                for (var i = 0; i < _activeCells.Count; i++)
                {
                    var cell = _activeCells[i];

                    if (gridCellWillRecycle != null) gridCellWillRecycle(this, cell);

                    cell.ContainerTransform.gameObject.SetActive(false);

                    _recyleCellPool.Add(cell);

                    if (gridCellVisibilityChanged != null) gridCellVisibilityChanged(this, cell, false);
                    if (gridCellDidRecycle != null) gridCellDidRecycle(this, cell);
                }
                _activeCells.Clear();
            }

            for (var i = 0; i < _debugObjects.Count; i++)
            {
                Destroy(_debugObjects[i]);
            }
            _debugObjects.Clear();
        }

        /// <summary>
		/// Deactivates a cell game object, putting it back into the pool of objects that
		/// can be reused.
		/// </summary>
		/// <param name="activeCellIndex"></param>
        protected virtual void _RecycleCell(int activeCellIndex)
        {
            var cell = _activeCells[activeCellIndex];

            if (gridCellWillRecycle != null) gridCellWillRecycle(this, cell);

            cell.ContainerTransform.gameObject.SetActive(false);
            _recyleCellPool.Add(cell);
            _activeCells.RemoveAt(activeCellIndex, ignoreOrder: true);

            if (gridCellVisibilityChanged != null) gridCellVisibilityChanged(this, cell, false);
            if (gridCellDidRecycle != null) gridCellDidRecycle(this, cell);
        }

        /// <summary>
		/// Creates copies of group and cell layouts. This is called if repeat mode is set
		/// to fill or loop.
		/// </summary>
		/// <param name="copyCount">Number of copies to make</param>
		/// <param name="copyDirection">Which direction to make the copies in</param>
		/// <param name="maxBounds">The maximum boundary of the content object</param>
        protected virtual void _CreateCopies(int copyCount, CopyDirection copyDirection, ref Vector2 maxBounds)
        {
            var groupLayoutCount = _groupLayouts.Count;
            var cellLayoutCount = _cellLayouts.Count;

            if (copyCount > 0)
            {
                // expand the cell and group layout lists

                _cellLayouts.Resize<CellLayout>((copyCount + 1) * cellLayoutCount);
                _groupLayouts.Resize<GroupLayout>((copyCount + 1) * groupLayoutCount);

                // expand the max bounds

                var maxBoundsOffset = copyDirection == CopyDirection.X ? new Vector2(maxBounds.x - _contentPaddingLeft - _contentPaddingRight + _groupLayoutSpacing, 0) : new Vector2(0, maxBounds.y - _contentPaddingTop - _contentPaddingBottom + _groupLayoutSpacing);
                var repeatIndex = 0;

                // loop through each copy
                for (var f = 0; f < copyCount; f++)
                {
                    // loop through each cell layout
                    for (var c = 0; c < cellLayoutCount; c++)
                    {
                        // create a new cell with similar properties (changing repeat index, group index, and rects)

                        repeatIndex = (cellLayoutCount * (f + 1)) + c;

                        _cellLayouts[repeatIndex].cellProperties = new CellProperties()
                        {
                            minSize = _cellLayouts[c].cellProperties.minSize,
                            expansionWeight = _cellLayouts[c].cellProperties.expansionWeight
                        };
                        _cellLayouts[repeatIndex].actualSize = _cellLayouts[c].actualSize;
                        _cellLayouts[repeatIndex].logicRect = new Rect(_cellLayouts[c].logicRect.x + maxBoundsOffset.x, _cellLayouts[c].logicRect.y + maxBoundsOffset.y, _cellLayouts[c].logicRect.width, _cellLayouts[c].logicRect.height);
                        _cellLayouts[repeatIndex].groupLayoutIndex = _cellLayouts[c].groupLayoutIndex + (groupLayoutCount * (f + 1));
                        _cellLayouts[repeatIndex].dataIndex = _cellLayouts[c].dataIndex;
                        _cellLayouts[repeatIndex].repeatIndex = repeatIndex;
                    }

                    // loop through each group layout
                    for (var g = 0; g < groupLayoutCount; g++)
                    {
                        // create a new group layout

                        repeatIndex = (groupLayoutCount * (f + 1)) + g;

                        _groupLayouts[repeatIndex].groupColorIndex = g;
                        _groupLayouts[repeatIndex].startCellLayoutRepeatIndex = _groupLayouts[g].startCellLayoutRepeatIndex + cellLayoutCount;
                        _groupLayouts[repeatIndex].endCellLayoutRepeatIndex = _groupLayouts[g].endCellLayoutRepeatIndex + cellLayoutCount;
                        _groupLayouts[repeatIndex].cellCount = _groupLayouts[g].cellCount;
                        _groupLayouts[repeatIndex].logicRect = new Rect(_groupLayouts[g].logicRect.x + maxBoundsOffset.x, _groupLayouts[g].logicRect.y + maxBoundsOffset.y, _groupLayouts[g].logicRect.width, _groupLayouts[g].logicRect.height);
                        _groupLayouts[repeatIndex].flowDirectionSize = _groupLayouts[g].flowDirectionSize;
                        _groupLayouts[repeatIndex].maxCellSize = _groupLayouts[g].maxCellSize;
                    }

                    // increment the offset for this copy iteration

                    maxBoundsOffset += copyDirection == CopyDirection.X ? new Vector2(maxBounds.x - _contentPaddingLeft - _contentPaddingRight + _groupLayoutSpacing, 0) : new Vector2(0, maxBounds.y - _contentPaddingTop - _contentPaddingBottom + _groupLayoutSpacing);
                }

                // update the max bounds

                maxBounds = copyDirection == CopyDirection.X ? new Vector2(maxBoundsOffset.x + _contentPaddingLeft + _contentPaddingRight - _groupLayoutSpacing, maxBounds.y) : new Vector2(maxBounds.x, maxBoundsOffset.y + _contentPaddingTop + _contentPaddingBottom - _groupLayoutSpacing);
            }
        }

        /// <summary>
		/// Tweens the grid to a scroll position
		/// </summary>
		/// <param name="tweenTypeX">The type of the tween on the x axis. See EnhancedTween class.</param>
		/// <param name="timeX">Duration of the tween on the x axis</param>
		/// <param name="tweenTypeY">The type of the tween on the y axis. See EnhancedTween class.</param>
		/// <param name="timeY">Duration of the tween on the y axis</param>
		/// <param name="start">Start position.</param>
		/// <param name="end">End position.</param>
		/// <param name="tweenCompleted">Action to call when the tween has finished.</param>
		/// <param name="forceCalculateRange">If this is true, when the tween is done it will do a calculation to see what cells are visible</param>
		/// <returns></returns>
        protected virtual IEnumerator TweenScrollPosition(TweenType tweenTypeX, float timeX, TweenType tweenTypeY, float timeY, Vector2 start, Vector2 end, Action<EnhancedGrid> tweenCompleted, bool forceCalculateRange)
        {
            // make sure the tween type is not immediate or the time is zero
            if (!((tweenTypeX == TweenType.immediate || timeX == 0) && (tweenTypeY == TweenType.immediate || timeY == 0)))
            {
                // zero out the velocity
                _scrollRect.velocity = Vector2.zero;

                // fire the delegate for the tween start
                IsTweening = true;
                if (gridTweeningChanged != null) gridTweeningChanged(this, true);

                _tweenElapsedTime = 0f;
                var newPosition = Vector2.zero;

                // while the tween has time left, use an easing function
                while ((_tweenElapsedTime < timeX || _tweenElapsedTime < timeY) && !_interruptTween)
                {
                    if (!_tweenPaused)
                    {
                        if (_tweenPauseToggledOff)
                        {
                            _tweenPauseToggledOff = false;
                            start = ScrollPosition;
                            timeX = (_tweenPauseNewTweenTimeX < 0 ? Mathf.Clamp((timeX - _tweenElapsedTime), 0, timeX) : _tweenPauseNewTweenTimeX);
                            timeY = (_tweenPauseNewTweenTimeY < 0 ? Mathf.Clamp((timeY - _tweenElapsedTime), 0, timeY) : _tweenPauseNewTweenTimeY);
                            _tweenElapsedTime = 0;
                        }

                        var newPositionX = EnhancedTween.TweenFloat(tweenTypeX, start.x, end.x, _tweenElapsedTime / (timeX > 0 ? timeX : _tweenElapsedTime));
                        var newPositionY = EnhancedTween.TweenFloat(tweenTypeY, start.y, end.y, _tweenElapsedTime / (timeY > 0 ? timeY : _tweenElapsedTime));

                        // set the scroll position to the tweened position
                        ScrollPosition = new Vector2(newPositionX, newPositionY);

                        // increase the time elapsed
                        _tweenElapsedTime += Time.unscaledDeltaTime;
                    }

                    yield return null;
                }
            }

            if (_interruptTween)
            {
                // the tween was interrupted so we need to set the flag and call the tweening changed delegate.
                // note that we don't set the end position or call the tweenComplete delegate.

                _interruptTween = false;

                if (snapInterruptTween != null) snapInterruptTween();

                IsTweening = false;
                if (gridTweeningChanged != null) gridTweeningChanged(this, false);
            }
            else
            {
                // the time has expired, so we make sure the final scroll position
                // is the actual end position.
                ScrollPosition = end;

                if (forceCalculateRange || ((tweenTypeX == TweenType.immediate || timeX == 0) && (tweenTypeY == TweenType.immediate || timeY == 0)))
                {
                    _ShowVisibleCells();
                }

                // the tween jump is complete, so we fire the delegate
                if (tweenCompleted != null) tweenCompleted(this);

                // fire the delegate for the tween ending
                IsTweening = false;
                if (gridTweeningChanged != null) gridTweeningChanged(this, false);
            }
        }

        #endregion // Protected Methods

        #endregion // Protected
    }
}