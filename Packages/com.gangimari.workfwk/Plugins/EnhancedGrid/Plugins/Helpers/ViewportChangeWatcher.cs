namespace echo17.EnhancedUI.Helpers
{
    using UnityEngine;
    using EnhancedUI.EnhancedGrid;

    /// <summary>
    /// This helper class watches a grid's viewport and notifies any listeners if the
    /// viewport size changes. Can be useful for recalculating grids to fit the viewport.
    /// </summary>
    public class ViewportChangeWatcher : MonoBehaviour
    {
        /// <summary>
        /// Delegate to notify listeners of viewport changtes
        /// </summary>
        /// <param name="grid">The grid that changed</param>
        /// <param name="newSize">The new size of the viewport</param>
        public delegate void ViewportChanged(EnhancedGrid grid, Vector2 newSize);

        /// <summary>
        /// The grid that is being watched
        /// </summary>
        public EnhancedGrid grid;

        /// <summary>
        /// Public delegate to notify listeners of the viewport change
        /// </summary>
        public ViewportChanged viewportChanged;

        /// <summary>
        /// How much time between looking for changes
        /// </summary>
        private const float _TimeBetweenScreenChangeCalculations = 0.5f;

        /// <summary>
        /// The last time that a change was checked
        /// </summary>
        private float _lastScreenChangeCalculationTime = 0;

        /// <summary>
        /// The size of the viewport when it was last checked
        /// </summary>
        private Vector2 _lastViewportSize;

        private void Awake()
        {
            // initialize the last check time
            _lastScreenChangeCalculationTime = Time.time;
        }

        private void LateUpdate()
        {
            if (grid.ViewportSize.x != _lastViewportSize.x || grid.ViewportSize.y != _lastViewportSize.y)
            {
                // the viewport changed size

                if (Time.time - _lastScreenChangeCalculationTime >= _TimeBetweenScreenChangeCalculations)
                {
                    // enough time has passed, so we log the current time and viewport size

                    _lastScreenChangeCalculationTime = Time.time;
                    _lastViewportSize = new Vector2(grid.ViewportSize.x, grid.ViewportSize.y);

                    if (viewportChanged != null)
                    {
                        // at least one listener, so notify them

                        viewportChanged(grid, grid.ViewportSize);
                    }
                }
            }
        }
    }
}
