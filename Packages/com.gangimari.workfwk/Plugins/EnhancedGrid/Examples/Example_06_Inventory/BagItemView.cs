namespace echo17.EnhancedUI.EnhancedGrid.Example_06
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
	/// Cell view displaying the item in the inventory grid
	/// </summary>
    public class BagItemView : BasicGridCell
    {
        public Image image;
        public Text quantity;

        private GameObject _imageGameObject;
        private GameObject _quantityGameObject;

        private BagItem _bagItem;
        private Item _item;
        private Action<BagItem, Item> _selected;

        private void Awake()
        {
            _imageGameObject = image.gameObject;
            _quantityGameObject = quantity.transform.parent.gameObject;
        }

        public void UpdateCell(BagItem bagItem, Item item, Action<BagItem, Item> selected)
        {
            _bagItem = bagItem;
            _item = item;

            // set the delegate for when the item is selected
            _selected = selected;

            if (item != null)
            {
                // item exists, so set its picture and quantity

                _imageGameObject.SetActive(true);
                _quantityGameObject.SetActive(true);

                image.sprite = item.sprite;
                quantity.text = $"x {bagItem.quantity}";
            }
            else
            {
                // item slot is empty, so turn off the image and quantity

                _imageGameObject.SetActive(false);
                _quantityGameObject.SetActive(false);
            }
        }

        /// <summary>
		/// Item selected, so call the delegate on the controller
		/// </summary>
        public void Button_OnClick()
        {
            if (_selected != null)
            {
                _selected(_bagItem, _item);
            }
        }
    }
}
