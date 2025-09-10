namespace echo17.EnhancedUI.EnhancedGrid.Example_07
{
    using UnityEngine;
    using UnityEngine.UI;

    public class ViewChannel : BasicGridCell
    {
        public Text channelNameLabel;
        public Image backgroundImage;

        public Color selectedBackgroundColor;
        public Color selectedForegroundColor;

        public Color unSelectedBackgroundColor;
        public Color unSelectedForegroundColor;

        public void UpdateCell(ModelChannel data)
        {
            channelNameLabel.text = data.name;
            backgroundImage.color = data.isSelected ? selectedBackgroundColor : unSelectedBackgroundColor;
            channelNameLabel.color = data.isSelected ? selectedForegroundColor : unSelectedForegroundColor;
        }
    }
}
