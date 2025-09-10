namespace echo17.EnhancedUI.EnhancedGrid.Demo_03
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
	/// Cell view showing the cell's size mode and data index
	/// </summary>
    public class View : BasicGridCell
    {
        public Text dataIndexLabel;
        public Text descriptionLabel;
        public Image backgroundImage;
        public Color fixedCellColor;
        public Color expandingCellColor;

        public void UpdateCell(bool expansionOn, Model data, float normalizedExpansionWeight, float expansionAvailable)
        {
            dataIndexLabel.text = DataIndex.ToString();

            if (data.expansionWeight == 0f)
            {
                descriptionLabel.text = "Fixed Size Cell";
                backgroundImage.color = fixedCellColor;
            }
            else
            {
                var expansionDescription = expansionOn ? $"Expanded {Mathf.RoundToInt(normalizedExpansionWeight * 100)}%\nof {Mathf.RoundToInt(expansionAvailable)} available pixels" : "Group Expansion Off";

                descriptionLabel.text = expansionDescription;
                backgroundImage.color = expandingCellColor;
            }
        }
    }
}
