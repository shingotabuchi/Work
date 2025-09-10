namespace echo17.EnhancedUI.EnhancedGrid.Example_07
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class ViewProgram : BasicGridCell
    {
        public Text titleLabel;
        public Image backgroundImage;

        public Sprite selectedSprite;
        public Sprite unselectedSprite;
        public Color normalColor;
        public Color noDataColor;

        public void UpdateCell(ModelProgram data)
        {
            titleLabel.text = data.title;
            backgroundImage.sprite = data.isSelected ? selectedSprite : unselectedSprite;
            titleLabel.color = data.title == "No Data" ? noDataColor : normalColor;
        }
    }
}
