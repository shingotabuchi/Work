using System;
using UnityEngine;

namespace Fwk.UI
{
    public class SingleSelectGroup : MonoBehaviour
    {
        [SerializeField] private CoolButton[] _buttons;
        [SerializeField] private GameObject _selectedIndicator;

        private int _selectedIndex = 0;

        public void SetSelectedIndex(int index)
        {
            _selectedIndex = index;
            UpdateSelected();
        }

        public void SetCallbacks(Action<int> onSelected)
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                var index = i;
                _buttons[i].SetOnClickAction(() =>
                {
                    foreach (var otherButton in _buttons)
                    {
                        otherButton.interactable = true;
                    }

                    if (_selectedIndex == index)
                    {
                        return;
                    }
                    SetSelectedIndex(index);
                    onSelected?.Invoke(index);
                });
            }
        }

        private void UpdateSelected()
        {
            var button = _buttons[_selectedIndex];
            button.interactable = false;
            _selectedIndicator.transform.SetParent(button.transform, false);
        }
    }
}