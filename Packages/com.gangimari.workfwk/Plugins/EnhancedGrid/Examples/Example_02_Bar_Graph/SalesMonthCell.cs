namespace echo17.EnhancedUI.EnhancedGrid.Example_02
{
    using UnityEngine;
    using UnityEngine.UI;

    public class SalesMonthCell : MonoBehaviour, IEnhancedGridCell
    {
        public int DataIndex { get; set; }
        public int RepeatIndex { get; set; }
        public Transform ContainerTransform { get; set; }

        public Text dateLabel;
        public Text quantityLabel;
        public Image backgroundImage;
        public float scaleFactor;
        public float quantityOffset;

        private RectTransform _rectTransform;

        void Awake()
        {
            // cache the rect transform to be used in UpdateCell
            _rectTransform = GetComponent<RectTransform>();
        }

        public void UpdateCell(SalesMonth data, CellLayout cellLayout)
        {
            dateLabel.text = data.date.ToString("MMM") + "\n" + data.date.ToString("yyyy");
            quantityLabel.text = data.numberOfSales.ToString();

            // set the size of the rect transform to the cell's size, ignoring the cell layout
            _rectTransform.sizeDelta = new Vector2(cellLayout.actualSize.x, data.numberOfSales * scaleFactor);

            // set the quantity label based on the size of the cell
            quantityLabel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, data.numberOfSales * scaleFactor + quantityOffset);

            backgroundImage.color = data.color;
        }

        public virtual string GetCellTypeIdentifier()
        {
            return "";
        }

        public virtual float GetDrawPriority()
        {
            return 0f;
        }
    }
}
