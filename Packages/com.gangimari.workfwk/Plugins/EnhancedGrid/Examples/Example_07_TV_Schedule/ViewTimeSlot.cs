namespace echo17.EnhancedUI.EnhancedGrid.Example_07
{
    using UnityEngine;
    using UnityEngine.UI;

    public class ViewTimeSlot : BasicGridCell
    {
        public Text timeSlotLabel;
        public Image backgroundImage;

        public Color selectedBackgroundColor;
        public Color selectedForegroundColor;

        public Color unSelectedBackgroundColor;
        public Color unSelectedForegroundColor;

        public void UpdateCell(ModelTimeSlot data)
        {
            timeSlotLabel.text = data.startTime.ToString("h:mm");
            backgroundImage.color = data.isSelected ? selectedBackgroundColor : unSelectedBackgroundColor;
            timeSlotLabel.color = data.isSelected ? selectedForegroundColor : unSelectedForegroundColor;
        }
    }
}
