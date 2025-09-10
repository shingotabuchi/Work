namespace echo17.EnhancedUI.EnhancedGrid.Demo_07
{
    using UnityEngine;
    using UnityEngine.UI;

    public class View : BasicGridCell
    {
        public Text label;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = this.GetComponent<RectTransform>();
        }

        public void UpdateCell(Model data)
        {
            _rectTransform.sizeDelta = data.cellSize;

            label.text = DataIndex.ToString();
        }
    }
}
