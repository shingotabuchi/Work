namespace echo17.EnhancedUI.EnhancedGrid.Demo_09
{
    using UnityEngine;
    using UnityEngine.UI;

    public class View : BasicGridCell
    {
        public Text label;

        private RectTransform _rectTransform;

        private void Awake()
        {
            // cache the cell's rect transform to use in the UpdateCell method
            _rectTransform = GetComponent<RectTransform>();
        }

        public void UpdateCell(Model data)
        {
            // set the actual size of the cell based on the data's stored cell size
            _rectTransform.sizeDelta = data.cellSize;

            label.text = data.text;
        }
    }
}
