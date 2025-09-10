namespace echo17.EnhancedUI.EnhancedGrid.Example_08
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class PaletteColorButton : MonoBehaviour
    {
        public Image buttonImage;
        public Image selectedImage;

        public int ColorIndex { get; private set; }

        private Action<int> _selected;

        public void Initialize(int colorIndex, Color color, Action<int> selected)
        {
            ColorIndex = colorIndex;
            _selected = selected;

            buttonImage.color = color;

            ToggleSelected(false);
        }

        public void ToggleSelected(bool selected)
        {
            selectedImage.color = new Color(selectedImage.color.r, selectedImage.color.g, selectedImage.color.b, selected ? 1f : 0);
        }

        public void Button_OnClick()
        {
            if (_selected != null)
            {
                _selected(ColorIndex);
            }
        }
    }
}
