namespace echo17.EnhancedUI.EnhancedGrid.Example_08
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class PixelView : BasicGridCell
    { 
        public Image image;
        public Sprite transparentSprite;

        public void UpdateCell(Pixel data)
        {
            if (data.color == Color.clear)
            {
                image.sprite = transparentSprite;
                image.color = Color.white;
            }
            else
            {
                image.sprite = null;
                image.color = data.color;
            }
        }
    }
}
