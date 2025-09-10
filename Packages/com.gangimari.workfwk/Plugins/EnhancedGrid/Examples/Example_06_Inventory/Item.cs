namespace echo17.EnhancedUI.EnhancedGrid.Example_06
{
    using System;
    using UnityEngine;

    /// <summary>
	/// One item in the item database.
	/// Note: this differs from a bag item in that it is just raw information,
	/// where a bag item is an owned object referencing this item class with quantity.
	/// </summary>
    [Serializable]
    public class Item
    {
        public int itemId;
        public string name;
        public string description;
        public float value;
        public string image;

        public Sprite sprite;
    }
}
