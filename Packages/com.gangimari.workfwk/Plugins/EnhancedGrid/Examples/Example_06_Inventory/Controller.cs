namespace echo17.EnhancedUI.EnhancedGrid.Example_06
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using EnhancedUI.Helpers;

    /// <summary>
	/// Example showing one way to show inventory.
	/// The same grid is used to show different categories of items.
	/// Selection triggers item information updates.
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject cellPrefab;
        public Vector2 cellSize;
        public int maxSlotsPerBag;
        public Image selectedItemImage;
        public Text selectedItemNameText;
        public Text selectedItemDescriptionText;
        public Text inventoryTypeText;
        public string imagePath = "EnhancedGridExamples/Images";

        /// <summary>
		/// A json file storing all possible items
		/// </summary>
        public TextAsset itemDatabaseData;

        /// <summary>
        /// A json file storing what items are owned
        /// </summary>
        public TextAsset inventoryData;

        /// <summary>
		/// A database containing all possible items
		/// </summary>
        private ItemDatabase _itemDatabase;

        /// <summary>
		/// A database showing what items are owned
		/// </summary>
        private Inventory _inventory;

        private int _currentBagIndex = 0;

        void Awake()
        {
            Application.targetFrameRate = 60;
        }

        void Start()
        {
            // load in our item database and inventory

            _LoadItemDatabase();
            _LoadInventory();

            // set the first item in the inventory as selected
            var selectedBagItem = _inventory.bags[0].items[0];
            _ItemSelected(selectedBagItem, _LookupItem(selectedBagItem.itemId));

            inventoryTypeText.text = _inventory.bags[_currentBagIndex].type.CapitalizeFirstLetterOfWords();

            grid.InitializeGrid(this);
            grid.RecalculateGrid();
        }

        public int GetCellCount(EnhancedGrid grid)
        {
            return _inventory.bags[_currentBagIndex].items.Count;
        }

        public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
        {
            return cellPrefab;
        }

        public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
        {
            return new CellProperties(cellSize, 0f);
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            var bagItem = _inventory.bags[_currentBagIndex].items[dataIndex];

            (cell as BagItemView).UpdateCell(bagItem, _LookupItem(bagItem.itemId), _ItemSelected);
        }

        /// <summary>
		/// Change bags in the inventory database
		/// </summary>
		/// <param name="bagIndex">The bad index to change to</param>
        public void BagButton_OnClick(int bagIndex)
        {
            _currentBagIndex = bagIndex;

            inventoryTypeText.text = _inventory.bags[_currentBagIndex].type.CapitalizeFirstLetterOfWords();

            grid.RecalculateGrid();
        }

        /// <summary>
		/// Loads in all possible items into the item database from a json file
		/// </summary>
        private void _LoadItemDatabase()
        {
            _itemDatabase = JsonUtility.FromJson<ItemDatabase>(itemDatabaseData.text);

            foreach (var item in _itemDatabase.items)
            {
                item.sprite = Resources.Load<Sprite>($"{imagePath}/{item.image}");
            }
        }

        /// <summary>
		/// Loads in owned items into an inventory database from a json file
		/// </summary>
        private void _LoadInventory()
        {
            _inventory = JsonUtility.FromJson<Inventory>(inventoryData.text);

            foreach (var bag in _inventory.bags)
            {
                for (var i = bag.items.Count; i < maxSlotsPerBag; i++)
                {
                    bag.items.Add(new BagItem()
                    {
                        itemId = -1,
                        quantity = 0
                    });
                }
            }
        }

        /// <summary>
		/// Looks up an item from the item database based on its itemId
		/// </summary>
		/// <param name="itemId">The Id to look up</param>
		/// <returns></returns>
        private Item _LookupItem(int itemId)
        {
            return _itemDatabase.items.Where(i => i.itemId == itemId).FirstOrDefault();
        }

        /// <summary>
		/// Updates the item detail information
		/// </summary>
		/// <param name="bagItem">The inventory bag item (with quantity)</param>
		/// <param name="item">The item database item (base description)</param>
        private void _ItemSelected(BagItem bagItem, Item item)
        {
            selectedItemImage.sprite = item.sprite;
            selectedItemNameText.text = $"{item.name} (Qty: {bagItem.quantity})";
            selectedItemDescriptionText.text = item.description;
        }
    }
}