namespace echo17.EnhancedUI.EnhancedGrid.Demo_11
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class View : BasicGridCell
    {
        public Image backgroundImage;
        public Text label;

        public Color selectedColor;
        public Color unselectedColor;

        private Action<int> _cellSelected;

        public void UpdateCell(Model data, Action<int> cellSelected)
        {
            // set the delegate to call when the cell selection occurs (button click in this demo)
            _cellSelected = cellSelected;

            // change the background color depending on the selection state stored in the model
            backgroundImage.color = data.selected ? selectedColor : unselectedColor;

            label.text = DataIndex.ToString();
        }

        /// <summary>
		/// When the cell's button is clicked, call the delegate passed in the UpdateCell method
		/// </summary>
        public void CellButton_OnClick()
        {
            if (_cellSelected != null)
            {
                // tell the controller that this cell was selected
                _cellSelected(DataIndex);
            }
        }
    }
}
