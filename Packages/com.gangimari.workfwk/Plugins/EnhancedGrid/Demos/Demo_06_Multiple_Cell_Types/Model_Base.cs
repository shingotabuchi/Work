namespace echo17.EnhancedUI.EnhancedGrid.Demo_06
{
    using UnityEngine;

    public enum CellType
    {
        A,
        B,
        C
    }

    /// <summary>
	/// Base class for the different model types.
	/// All models in this demo inherit from this base type.
	/// </summary>
    public class Model_Base
    {
        public CellType cellType;
        public Vector2 cellSize;
    }
}
