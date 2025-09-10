namespace echo17.EnhancedUI.EnhancedGrid
{
    using UnityEngine;

    /// <summary>
    /// All views for cells need to implement this interface.
    /// You can use the BasiGridCell helper class to simplify some
    /// of this setup.
    /// </summary>
    public interface IEnhancedGridCell
    {
        // These are used internally by the grid

        int DataIndex { get; set; }
        int RepeatIndex { get; set; }
        Transform ContainerTransform { get; set; }

        // These are to be set by the user

        /// <summary>
        /// If you have multiple cell types, then you will need to override
        /// this method to return a unique string for each cell type.
        /// </summary>
        /// <returns></returns>
        string GetCellTypeIdentifier();

        /// <summary>
        /// Controls the order that cells are drawn in. Higher values are
        /// drawn last (on top).
        /// </summary>
        /// <returns></returns>
        float GetDrawPriority();
    }
}
