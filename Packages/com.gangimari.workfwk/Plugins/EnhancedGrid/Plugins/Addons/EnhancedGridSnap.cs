namespace echo17.EnhancedUI.EnhancedGrid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using echo17.EnhancedUI.Helpers;

    /// <summary>
    /// Addon that allows you to cause the grid to snap when scrolling.
    /// Add this component to the same object as the EnhancedGrid component.
    /// </summary>
    public class EnhancedGridSnap : MonoBehaviour
    {
        /// <summary>
        /// Delegate called when the grid has snapped
        /// </summary>
        /// <param name="grid">The grid that snapped</param>
        /// <param name="snap">This component</param>
        /// <param name="dataIndex">The data index of the cell that was snapped to</param>
        public delegate void GridSnapped(EnhancedGrid grid, EnhancedGridSnap snap, int dataIndex);

        /// <summary>
        /// The delegate called when the grid starts snapping
        /// </summary>
        public GridSnapped gridSnapStarted;

        /// <summary>
        /// The delegate called when the grid stops snapping
        /// </summary>
        public GridSnapped gridSnapCompleted;

        /// <summary>
        /// Whether snapping is enabled
        /// </summary>
        [SerializeField]
        private bool _snapping = true;
        public virtual bool Snapping
        {
            get
            {
                return _snapping;
            }
            set
            {
                if (_snapping != value)
                {
                    _snapping = value;
                }
            }
        }

        /// <summary>
        /// The velocity that this component will monitor to know when to snap.
        /// If the velocity drops below the threshold, a snap will occur.
        /// </summary>
        [SerializeField]
        private Vector2 _snapVelocityThreshold = new Vector2(300f, 300f);
        public virtual Vector2 SnapVelocityThreshold
        {
            get
            {
                return _snapVelocityThreshold;
            }
            set
            {
                if (_snapVelocityThreshold != value)
                {
                    _snapVelocityThreshold = value;
                }
            }
        }

        /// <summary>
        /// Where in the viewport to watch for cells to snap to (0..1).
        /// Typically this will be the same as the SnapJumpToViewportOffset.
        /// </summary>
        [SerializeField]
        private Vector2 _snapWatchViewportOffset = new Vector2(0.5f, 0.5f);
        public virtual Vector2 SnapWatchViewportOffset
        {
            get
            {
                return _snapWatchViewportOffset;
            }
            set
            {
                if (_snapWatchViewportOffset != value)
                {
                    _snapWatchViewportOffset = value;
                }
            }
        }

        /// <summary>
        /// Where in the viewport to center the cell after the snap (0..1).
        /// Typically this will be the same as the SnapWatchViewportOffset.
        /// </summary>
        [SerializeField]
        private Vector2 _snapJumpToViewportOffset = new Vector2(0.5f, 0.5f);
        public virtual Vector2 SnapJumpToViewportOffset
        {
            get
            {
                return _snapJumpToViewportOffset;
            }
            set
            {
                if (_snapJumpToViewportOffset != value)
                {
                    _snapJumpToViewportOffset = value;
                }
            }
        }

        /// <summary>
        /// Where inside the cell to center for the snap (0..1).
        /// </summary>
        [SerializeField]
        private Vector2 _snapJumpToCellOffset = new Vector2(0.5f, 0.5f);
        public virtual Vector2 SnapJumpToCellOffset
        {
            get
            {
                return _snapJumpToCellOffset;
            }
            set
            {
                if (_snapJumpToCellOffset != value)
                {
                    _snapJumpToCellOffset = value;
                }
            }
        }

        /// <summary>
        /// What method to use to tween along the x axis. See EnhancedTween class.
        /// </summary>
        [SerializeField]
        private TweenType _snapTweenTypeX = TweenType.easeOutSine;
        public virtual TweenType SnapTweenTypeX
        {
            get
            {
                return _snapTweenTypeX;
            }
            set
            {
                if (_snapTweenTypeX != value)
                {
                    _snapTweenTypeX = value;
                }
            }
        }

        /// <summary>
        /// The duration of the snap tween along the x axis.
        /// </summary>
        [SerializeField]
        private float _snapTweenTimeX = 1.0f;
        public virtual float SnapTweenTimeX
        {
            get
            {
                return _snapTweenTimeX;
            }
            set
            {
                if (_snapTweenTimeX != value)
                {
                    _snapTweenTimeX = value;
                }
            }
        }

        /// <summary>
        /// What method to use to tween along the y axis. See EnhancedTween class.
        /// </summary>
        [SerializeField]
        private TweenType _snapTweenTypeY = TweenType.easeOutSine;
        public virtual TweenType SnapTweenTypeY
        {
            get
            {
                return _snapTweenTypeY;
            }
            set
            {
                if (_snapTweenTypeY != value)
                {
                    _snapTweenTypeY = value;
                }
            }
        }

        /// <summary>
        /// The duration of the snap tween along the y axis.
        /// </summary>
        [SerializeField]
        private float _snapTweenTimeY = 1.0f;
        public virtual float SnapTweenTimeY
        {
            get
            {
                return _snapTweenTimeY;
            }
            set
            {
                if (_snapTweenTimeY != value)
                {
                    _snapTweenTimeY = value;
                }
            }
        }

        /// <summary>
        /// Whether to snap while dragging. Typically you would want
        /// this turned off to avoid jitter during dragging.
        /// </summary>
        [SerializeField]
        private bool _snapWhileDragging;
        public virtual bool SnapWhileDragging
        {
            get
            {
                return _snapWhileDragging;
            }
            set
            {
                if (_snapWhileDragging != value)
                {
                    _snapWhileDragging = value;
                }
            }
        }

        /// <summary>
        /// Whether to force the grid to snap when the pointer (mouse or finger)
        /// is lifted. This is usefull if there is no inertia at the end of the drag.
        /// </summary>
        [SerializeField]
        private bool _forceSnapOnEndDrag;
        public virtual bool ForceSnapOnEndDrag
        {
            get
            {
                return _forceSnapOnEndDrag;
            }
            set
            {
                if (_forceSnapOnEndDrag != value)
                {
                    _forceSnapOnEndDrag = value;
                }
            }
        }

        // Internal links and members to keep track of state

        protected EnhancedGrid _grid;
        protected bool _snapJumping = false;
        protected bool _snapInertia = false;
        protected bool _snapBeforeDrag = false;
        protected int _snapDataIndex;

        private void Awake()
        {
            // cache the grid component
            _grid = GetComponent<EnhancedGrid>();

            // make sure there is a grid
            Debug.Assert(_grid != null, "EnhancedGrid is a required component for DragDrop. Please add EnhancedGrid component to the same game object as DragDrop.");

            // attach the delegates of the grid

            _grid.snapGridScrolled = SnapGridScrolled;
            _grid.snapInterruptTween = SnapInterruptTween;
            _grid.snapGridOnBeginDrag = SnapGridOnBeginDrag;
            _grid.snapGridOnDrag = SnapGridOnDrag;
            _grid.snapGridOnEndDrag = SnapGridOnEndDrag;
        }

        /// <summary>
        /// Called by the grid to let this component know the grid was scrolled
        /// </summary>
        /// <param name="grid">The grid that was scrolled</param>
        /// <param name="scrollPosition">The position in pixels of the grid.</param>
        /// <param name="normalizedScrollPosition">The normalized position (0..1) of the grid</param>
        protected virtual void SnapGridScrolled(EnhancedGrid grid, Vector2 scrollPosition, Vector2 normalizedScrollPosition)
        {
            // if the snapping is turned on, handle it
            if (_snapping && !_snapJumping)
            {
                if (_grid.ScrollRect.velocity != Vector2.zero)
                {
                    // if the speed has dropped below the threshhold velocity
                    if (
                            (Mathf.Abs(_grid.ScrollRect.velocity.x) <= _snapVelocityThreshold.x)
                            &&
                            (Mathf.Abs(_grid.ScrollRect.velocity.y) <= _snapVelocityThreshold.y)
                       )
                    {
                        DoSnap();
                    }
                }
            }
        }

        /// <summary>
        /// Called by the grid to let this component know that dragging has started
        /// </summary>
        /// <param name="grid">The grid that was dragged.</param>
        /// <param name="data">The drag data.</param>
        protected virtual void SnapGridOnBeginDrag(EnhancedGrid grid, PointerEventData data)
        {
            // capture the snapping and set it to false if desired
            _snapBeforeDrag = _snapping;
            if (!_snapWhileDragging)
            {
                Snapping = false;
            }
        }

        /// <summary>
        /// Called by the grid to let this component know that dragging is occurring
        /// </summary>
        /// <param name="grid">The grid that was dragged.</param>
        /// <param name="data">The drag data.</param>
        /// <param name="dragPreviousPosition">The previous position of the drag.</param>
        protected virtual void SnapGridOnDrag(EnhancedGrid grid, PointerEventData data, Vector2 dragPreviousPosition)
        {

        }

        /// <summary>
        /// Called by the grid to let this component know that dragging has stopped
        /// </summary>
        /// <param name="grid">The grid that stopped dragging</param>
        /// <param name="data">The drag data</param>
        /// <param name="dragPreviousPosition">The previous position of the drag</param>
        protected virtual void SnapGridOnEndDrag(EnhancedGrid grid, PointerEventData data, Vector2 dragPreviousPosition)
        {
            // reset the snapping and looping to what it was before the drag
            Snapping = _snapBeforeDrag;

            if (_forceSnapOnEndDrag && _snapping && dragPreviousPosition == data.position)
            {
                DoSnap();
            }
        }

        /// <summary>
        /// Called by the grid in case the snap was interrupted
        /// </summary>
        protected virtual void SnapInterruptTween()
        {
            // reset the snapJumping and scroll rect inertia
            _snapJumping = false;
            _grid.ScrollRect.inertia = _snapInertia;
        }

        /// <summary>
        /// Snaps the grid
        /// </summary>
        public virtual void DoSnap()
        {
            // exit if no data
            if (_grid.OriginalDataCount == 0) return;

            _snapJumping = true;

            // zero out the velocity and inertia to avoid jitter and jumps
            _grid.ScrollRect.velocity = Vector2.zero;
            _snapInertia = _grid.ScrollRect.inertia;
            _grid.ScrollRect.inertia = false;

            // get the snap position based on where in the viewport this component is looking at for the cell
            var snapPosition = _grid.ScrollPosition + new Vector2(_grid.ViewportSize.x * Mathf.Clamp01(_snapWatchViewportOffset.x), _grid.ViewportSize.y * Mathf.Clamp01(_snapWatchViewportOffset.y));

            // determine what cell is at that location
            _snapDataIndex = _grid.GetDataIndexAtPosition(snapPosition, false);

            // let anyone registered to the snap started delegate know that snapping has begun
            if (gridSnapStarted != null) gridSnapStarted(_grid, this, _snapDataIndex);

            // jump cell to the new location based on the snap jump to viewport offset and snap jump to cell offset
            _grid.JumpToDataIndex(
                            _snapDataIndex,
                            _snapTweenTypeX,
                            _snapTweenTimeX,
                            EnhancedGrid.EnhancedGridLoopJumpDirection.Closest,
                            _snapTweenTypeY,
                            _snapTweenTimeY,
                            EnhancedGrid.EnhancedGridLoopJumpDirection.Closest,
                            _snapJumpToViewportOffset,
                            _snapJumpToCellOffset,
                            _SnapJumpCompleted
                            );
        }

        /// <summary>
        /// Called when the snap tween completes
        /// </summary>
        /// <param name="grid">The grid that was snapped</param>
        protected virtual void _SnapJumpCompleted(EnhancedGrid grid)
        {
            // reset the snap jump to false and restore the inertia state
            _snapJumping = false;
            _grid.ScrollRect.inertia = _snapInertia;

            // let anyone registered to the snap completed delegate know that snapping has ended
            if (gridSnapCompleted != null) gridSnapCompleted(_grid, this, _snapDataIndex);
        }
    }
}