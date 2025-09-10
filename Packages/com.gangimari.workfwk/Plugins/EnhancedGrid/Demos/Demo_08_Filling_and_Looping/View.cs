namespace echo17.EnhancedUI.EnhancedGrid.Demo_08
{
    using UnityEngine;
    using UnityEngine.UI;

    public class View : BasicGridCell
    {
        public Text label;

        private RectTransform _rectTransform;

        private void Awake()
        {
            // cache the rect transform to be used in the UpdateCell method
            _rectTransform = this.GetComponent<RectTransform>();
        }

        public void UpdateCell(Model data)
        {
            // set the cell size to the actual cell size, not the calculated group layout size
            _rectTransform.sizeDelta = data.cellSize;

            label.text = DataIndex.ToString();
        }
    }
}
