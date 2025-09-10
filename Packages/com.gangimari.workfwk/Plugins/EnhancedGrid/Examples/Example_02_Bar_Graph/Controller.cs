namespace echo17.EnhancedUI.EnhancedGrid.Example_02
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using EnhancedUI.Helpers;

    /// <summary>
	/// Example showing one way to display a chart like a bar graph
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public ViewportChangeWatcher viewportChangeWatcher;
        public GameObject salesMonthCellPrefab;
        public Vector2 salesMonthCellMaxSize;

        /// <summary>
		/// Text file storing records of sales
		/// </summary>
        public TextAsset salesDatabase;

        /// <summary>
		/// Text file storing colors for a palette
		/// </summary>
        public TextAsset paletteFile;

        private List<SalesMonth> _salesMonths;
        private Palette _palette;

        void Awake()
        {
            Application.targetFrameRate = 60;

            // load the palette from the palette file
            _palette = new Palette();
            _palette.LoadFromText(paletteFile.text);
        }

        void Start()
        {
            // load the sales data
            var salesMonthLines = salesDatabase.text.Split("\n"[0]);

            viewportChangeWatcher.viewportChanged = _ViewportChanged;

            grid.InitializeGrid(this);

            _salesMonths = new List<SalesMonth>();

            for (var i = 0; i < salesMonthLines.Length; i++)
            {
                // only read lines that are not empty and contain a comma
                if (!string.IsNullOrEmpty(salesMonthLines[i]) && salesMonthLines[i].Contains(","))
                {
                    // split the fields in the line by the comma delimiter
                    var salesMonthFields = salesMonthLines[i].Split(","[0]);

                    // add the sales record
                    _salesMonths.Add(new SalesMonth()
                    {
                        date = Convert.ToDateTime(salesMonthFields[0]),
                        numberOfSales = Convert.ToInt32(salesMonthFields[1]),
                        color = _palette.GetColor(i)
                    });
                }
            }

            // order the sales by month
            _salesMonths = _salesMonths.OrderBy(o => o.date).ToList();

            grid.RecalculateGrid();
        }

        public int GetCellCount(EnhancedGrid grid)
        {
            return _salesMonths.Count;
        }

        public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
        {
            return salesMonthCellPrefab;
        }

        public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
        {
            return new CellProperties()
            {
                minSize = salesMonthCellMaxSize,
                expansionWeight = 0f
            };
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            (cell as SalesMonthCell).UpdateCell(_salesMonths[dataIndex], cellLayout);
        }

        private void _ViewportChanged(EnhancedGrid grid, Vector2 newSize)
        {
            grid.RecalculateGrid();
        }
    }
}