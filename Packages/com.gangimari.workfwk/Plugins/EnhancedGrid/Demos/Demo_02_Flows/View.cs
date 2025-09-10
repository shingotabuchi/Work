namespace echo17.EnhancedUI.EnhancedGrid.Demo_02
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
	/// Very simple view that just shows the data index of the cell
	/// </summary>
    public class View : BasicGridCell
    {
        public Text label;

        public void UpdateCell()
        {
            label.text = DataIndex.ToString();
        }
    }
}
