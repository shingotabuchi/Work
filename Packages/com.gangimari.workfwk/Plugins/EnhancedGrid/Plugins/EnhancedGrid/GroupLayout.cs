namespace echo17.EnhancedUI.EnhancedGrid
{
    using UnityEngine;

    /// <summary>
    /// Defines the extens of a cell group. If the cells first flow left to right or
    /// right to left, then the group will be a row. If the cells first flow top to
    /// bottom or bottom to top, then the group will be a column.
    /// </summary>
    public class GroupLayout
    {
        /// <summary>
        /// Used for debugging
        /// </summary>
        public int groupColorIndex;

        /// <summary>
        /// The first cell's repeatIndex of the group.
        /// See CellLayout for repeatIndex explanation.
        /// </summary>
        public int startCellLayoutRepeatIndex;

        /// <summary>
        /// The last cell's repeatIndex of the group
        /// See CellLayout for repeatIndex explanation.
        /// </summary>
        public int endCellLayoutRepeatIndex;

        /// <summary>
        /// The number of cells in the group.
        /// </summary>
        public int cellCount;

        /// <summary>
        /// The rect used for calculations.
        /// EnhancedGrid reverses the y coordinates for simplicity.
        /// </summary>
        public Rect logicRect;

        /// <summary>
        /// The rect that Unity uses.
        /// </summary>
        public Rect uiRect;

        /// <summary>
        /// The size of the group. If the cells first flow left to right or
        /// right to left, then the size will be the width. If the cells first flow top to
        /// bottom or bottom to top, then the size will be the height. 
        /// </summary>
        public float flowDirectionSize;

        /// <summary>
        /// The maximum cell size of the group.
        /// </summary>
        public float maxCellSize;

        /// <summary>
        /// The amount of pixels available for cells to expand into
        /// </summary>
        public float expansionAvailable;
    }
}
