namespace echo17.EnhancedUI.EnhancedGrid.Example_03
{
    using UnityEngine;
    using UnityEngine.UI;

    public class ChatCell : BasicGridCell
    {
        public Text chatLabel;
        public RectTransform backgroundRectTransform;
        public string cellIdentifier;

        public void UpdateCell(Chat data, float cellTemplateWidthWithPadding)
        {
            chatLabel.text = data.text;

            // set the rect transform's size based on the calculated size of the cell stored in the data
            backgroundRectTransform.sizeDelta = new Vector2(cellTemplateWidthWithPadding, data.cellHeight);
        }

        public override string GetCellTypeIdentifier()
        {
            return cellIdentifier;
        }
    }
}
