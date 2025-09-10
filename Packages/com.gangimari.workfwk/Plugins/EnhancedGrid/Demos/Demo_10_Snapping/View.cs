namespace echo17.EnhancedUI.EnhancedGrid.Demo_10
{
    using UnityEngine;
    using UnityEngine.UI;

    public class View : BasicGridCell
    {
        public Text label;

        public void UpdateCell()
        {
            label.text = DataIndex.ToString();
        }
    }
}
