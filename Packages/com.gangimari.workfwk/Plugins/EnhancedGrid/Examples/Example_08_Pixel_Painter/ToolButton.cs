namespace echo17.EnhancedUI.EnhancedGrid.Example_08
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ToolButton : MonoBehaviour
    {
        public enum ToolType
        {
            Pen,
            Eraser,
            Bucket,
            Dropper,
            Clear
        }

        public Image backgroundImage;
        public Image foregroundImage;

        public ToolType toolType;
        public Color selectedBackgroundColor;
        public Color selectedForegroundColor;
        public Color unSelectedBackgroundColor;
        public Color unSelectedForegroundColor;

        private Action<ToolType> _selected;

        public void Initialize(Action<ToolType> selected)
        {
            _selected = selected;

            SetIsSelected(false);
        }

        public void SetIsSelected(bool isSelected)
        {
            backgroundImage.color = isSelected ? selectedBackgroundColor : unSelectedBackgroundColor;
            foregroundImage.color = isSelected ? selectedForegroundColor : unSelectedForegroundColor;
        }

        public void Button_OnClick()
        {
            if (_selected != null)
            {
                _selected(toolType);
            }
        }
    }
}
