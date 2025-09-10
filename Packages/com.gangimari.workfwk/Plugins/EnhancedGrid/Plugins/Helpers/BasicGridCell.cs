namespace echo17.EnhancedUI.EnhancedGrid
{
    using UnityEngine;

    /// <summary>
    /// A helper class to simplify setup of your views. You do not have to
    /// inherit from this class, but you do need to inherit the IEnhancedGridCell interface,
    /// which this class does for you.
    /// </summary>
    public class BasicGridCell : MonoBehaviour, IEnhancedGridCell
    {
        /// <summary>
        /// Implements the DataIndex property of the interface.
        /// </summary>
        public int DataIndex { get; set; }

        /// <summary>
        /// Implements the RepeatIndex property of the interface.
        /// </summary>
        public int RepeatIndex { get; set; }

        /// <summary>
        /// Implements the ContainerTransform property of the interface.
        /// </summary>
        public Transform ContainerTransform { get; set; }

        /// <summary>
        /// Implements the GetCellTypeIdentifier property of the interface.
        /// Note that if you are using multiple cell types, you will need to override
        /// this in your view.
        /// </summary>
        public virtual string GetCellTypeIdentifier()
        {
            return "";
        }

        /// <summary>
        /// Used to sort the cells for draw order in the hierarchy.
        /// Higher values will be drawn last (on top).
        /// </summary>
        /// <returns></returns>
        public virtual float GetDrawPriority()
        {
            return 0f;
        }
    }
}
