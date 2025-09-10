namespace echo17.EnhancedUI.EnhancedGrid.Demo_04
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
	/// Cell view showing how you can align your cells inside their calculated layouts
	/// </summary>
    public class View : BasicGridCell
    {
        public Text dataIndexLabel;
        public Text descriptionLabel;
        public Image backgroundImage;
        public Color aligningCellColor;
        public Color nonAligningCellColor;

        // pointer to an image inside the cell to show how the cell alignment changes
        public RectTransform actualCell;

        public void UpdateCell(Model data, bool isAligning)
        {
            // set the actual cell image to the size, anchor, and pivot
            actualCell.sizeDelta = data.cellSize;
            actualCell.anchorMin = data.anchor;
            actualCell.anchorMax = data.anchor;
            actualCell.pivot = data.pivot;

            // change the color to distinguish between cells that are aligning and those that are not
            backgroundImage.color = (isAligning ? aligningCellColor : nonAligningCellColor);

            dataIndexLabel.text = DataIndex.ToString();
            descriptionLabel.text = (isAligning ? "aligning cell" : "non-aligning cell");
        }
    }
}
