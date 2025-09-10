namespace echo17.EnhancedUI.EnhancedGrid.Demo_03
{
    using UnityEngine;

    /// <summary>
	/// This model stores the cell's size and size mode,
	/// which is used by the controller to tell the grid
	/// the cell's properties
	/// </summary>
    public class Model
    {
        public Vector2 cellSize;
        public float expansionWeight;
    }
}
