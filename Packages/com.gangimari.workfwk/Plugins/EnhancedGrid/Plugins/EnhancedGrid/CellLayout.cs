namespace echo17.EnhancedUI.EnhancedGrid
{
    using UnityEngine;

    /// <summary>
    /// Defines the extents that a cell will occupy.
    /// </summary>
    public class CellLayout
    {
        /// <summary>
        /// The minimum size and size mode of the cell.
        /// </summary>
        public CellProperties cellProperties;

        /// <summary>
        /// The actual size of the cell after being added into a group.
        /// Depending on the other cell sizes in the group, this could equal the minimum size defined in cellProperties,
        /// or it could be the maximum size of the group.
        /// </summary>
        public Vector2 actualSize;

        /// <summary>
        /// The rect that this cell occupies.
        /// EnhancedGrid reverses the y coordinates for simplicity.
        /// </summary>
        public Rect logicRect;

        /// <summary>
        /// The rect that Unity uses.
        /// </summary>
        public Rect uiRect;

        /// <summary>
        /// The group that this cell layout belongs to.
        /// </summary>
        public int groupLayoutIndex;

        /// <summary>
        /// The serialized data index of the cell layout.
        /// </summary>
        public int dataIndex;

        /// <summary>
        /// If the repeat mode for the grid is not set to none,
        /// then this cell layout might be a copy, in which case
        /// the repeat index won't necessarily equal the dataIndex.
        /// If the grid repeat mode is none, then the repeatIndex
        /// will equal the dataIndex.
        /// </summary>
        public int repeatIndex;
    }
}
