namespace echo17.EnhancedUI.EnhancedGrid.Example_04
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
	/// This cell view will change its transformation and draw priority based on its location on screen
	/// </summary>
    public class View : BasicGridCell
    {
        public Image image;
        public Vector2 actualCellSize;
        public float scaleFactor;

        private RectTransform _rectTransform;
        private RectTransform _viewportRectTransform;
        private float _drawPriority;

        private void Awake()
        {
            // cache the rect transform to be used later
            _rectTransform = GetComponent<RectTransform>();
        }

        public void UpdateCell(Sprite sprite, RectTransform viewportRectTransform)
        {
            // cache the viewport rect from the grid
            _viewportRectTransform = viewportRectTransform;

            image.sprite = sprite;

            _CalculateSize();
        }

        private void Update()
        {
            _CalculateSize();
        }

        /// <summary>
		/// Return the draw priority of the cell, used to sort the draw order
		/// </summary>
		/// <returns></returns>
        public override float GetDrawPriority()
        {
            return _drawPriority;
        }

        /// <summary>
		/// Calculates the size, rotation, and draw priority
		/// </summary>
        private void _CalculateSize()
        {
            if (_viewportRectTransform == null) return;

            // calculate how far the cell is from the center of the viewport rect
            var distanceFromCenter = (_viewportRectTransform.rect.width / 2.0f) - _rectTransform.position.x;

            // calculate the scale of the cell, making the cells larger as they get near the center
            var scale = (1f - (Mathf.Abs(distanceFromCenter) / _viewportRectTransform.rect.width)) * scaleFactor;

            // calculate the rotation of the cell, making them skew as they go toward the edges of the grid.
            var rotation = (-distanceFromCenter / _viewportRectTransform.rect.width) * 45f;

            // set the rect transform's size and rotation
            _rectTransform.sizeDelta = actualCellSize * scale;
            _rectTransform.localEulerAngles = new Vector3(rotation, Mathf.Abs(rotation), 0f);

            // set the draw priority.
			// in this example, larger sizes get drawn on top,
            // making cells in the center draw on top of others
            _drawPriority = scale;
        }
    }
}
