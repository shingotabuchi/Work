namespace echo17.EnhancedUI.EnhancedGrid.Example_04
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
	/// Demo showing one way to set up an image carousel that changes the images'
	/// size, draw order, and rotation based on how close to the center of the screen they are.
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject cellPrefab;
        public Vector2 cellSize;

        public Sprite[] sprites;

        void Awake()
        {
            Application.targetFrameRate = 60;
        }

        void Start()
        {
            grid.InitializeGrid(this);

            // set up the grid scrolled delegate so that we can resort the cells based on their draw priority
            grid.gridScrolled = _GridScrolled;

            grid.RecalculateGrid();
        }

        public int GetCellCount(EnhancedGrid grid)
        {
            return sprites.Length;
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
            // base the grid's viewport rect so that the image can do some transformations based on its position
            (cell as View).UpdateCell(sprites[dataIndex], grid.Viewport);
        }

        /// <summary>
		/// The grid was scrolled
		/// </summary>
		/// <param name="grid">The grid that was scrolled</param>
		/// <param name="scrollPosition">Grid's scroll position</param>
		/// <param name="normalizedScrollPosition">Grid's normalized scroll position (0..1)</param>
        private void _GridScrolled(EnhancedGrid grid, Vector2 scrollPosition, Vector2 normalizedScrollPosition)
        {
            // resort the cells based on their draw priority.
            // this will push cells near the center to the top (higher priority)
            grid.ResortDrawPriority();
        }
    }
}