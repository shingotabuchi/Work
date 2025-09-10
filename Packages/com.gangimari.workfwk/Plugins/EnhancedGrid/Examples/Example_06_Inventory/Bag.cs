namespace echo17.EnhancedUI.EnhancedGrid.Example_06
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
	/// Inventory has multiple bags, so each bag will store a list of items owned
	/// </summary>
    [Serializable]
    public class Bag
    {
        public string type;
        public List<BagItem> items;
    }
}
