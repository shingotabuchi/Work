namespace echo17.EnhancedUI.EnhancedGrid.Demo_05
{
    using UnityEngine;
    using UnityEngine.UI;

    public class View : BasicGridCell
    {
        public Text dataIndexLabel;

        public void UpdateCell()
        {
            dataIndexLabel.text = DataIndex.ToString();
        }
    }
}
