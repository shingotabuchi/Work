namespace echo17.EnhancedUI.EnhancedGrid.Demo_01
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
	/// The view for the cells in demo 1.
	/// This view is very basic, only displaying a label with the data index of the cell.
	/// It inherits from BaseGridCell to handle most of the IEnhancedGridCell interface for you.
	/// </summary>
    public class View : BasicGridCell
    {
        /// <summary>
		/// The text label to show the data index
		/// </summary>
        public Text label;

        /// <summary>
		/// Called by the controller to update the cell's view
		/// </summary>
        public void UpdateCell()
        {
            // set the label to the data index of the cell
            label.text = DataIndex.ToString();
        }
    }
}
