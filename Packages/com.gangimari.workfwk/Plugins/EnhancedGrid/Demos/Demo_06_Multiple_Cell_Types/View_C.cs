namespace echo17.EnhancedUI.EnhancedGrid.Demo_06
{
    using UnityEngine;
    using UnityEngine.UI;

    public class View_C : BasicGridCell
    {
        public Text label;
        public Text description;

        /// <summary>
		/// Each cell view uses a different prefab, so we need unique cell identifiers to
		/// pass back in the GetCellTypeIdentifier method.
		/// </summary>
        public string cellIdentifier;

        public void UpdateCell(Model_1 data)
        {
            label.text = DataIndex.ToString();
            description.text = $"Cell Type {data.cellType}";
        }

        /// <summary>
		/// This string identifier tells the grid which cell prefab to use.
		/// BasicGridCell just returns an empty string, so we override that here.
		/// </summary>
		/// <returns></returns>
        public override string GetCellTypeIdentifier()
        {
            return cellIdentifier;
        }
    }
}
