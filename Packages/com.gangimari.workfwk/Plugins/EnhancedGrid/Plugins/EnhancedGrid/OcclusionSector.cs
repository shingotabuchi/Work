namespace echo17.EnhancedUI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using EnhancedUI.Helpers;
    using System.Text;

    /// <summary>
    /// Used to speed up processing of cell layouts. OcclusionSectors are divided into
    /// four quadrants recusively, depending on the depth set in the grid. If cells don't
    /// fall in the higher level quadrants, then sub quandrants will not be processed, cutting
    /// down on the number of checks against visibility.
    /// </summary>
    public class OcclusionSector
    {
        #region Public

        #region Public Properties

        /// <summary>
        /// Used for debugging.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// The upper left corner of the logical rect used in calculations of visibility.
        /// EnhancedGrid reverses the y coordinates for simplicity.
        /// </summary>
        public virtual Vector2 LogicPosition
        {
            get
            {
                return _logicRect.position;
            }
        }

        /// <summary>
        /// The upper left corner of the rect that Unity uses.
        /// </summary>
        public virtual Vector2 UIPosition
        {
            get
            {
                return _uiRect.position;
            }
        }

        /// <summary>
        /// The size of the logical rect.
        /// </summary>
        public virtual Vector2 Size
        {
            get
            {
                return _logicRect.size;
            }
        }

        /// <summary>
        /// The list of cell dataIndices that are contained in this sector.
        /// Used for debugging.
        /// </summary>
        public virtual string CellDataIndicesString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (var c = 0; c < _cells.Count; c++)
                {
                    sb.Append(_cells[c].dataIndex + (c < _cells.Count - 1 ? "," : ""));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// The list of cell repeatIndices that are contained in this sector.
        /// Used for debugging.
        /// </summary>
        public virtual string CellRepeatIndicesString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (var c = 0; c < _cells.Count; c++)
                {
                    sb.Append(_cells[c].repeatIndex + (c < _cells.Count - 1 ? "," : ""));
                }
                return sb.ToString();
            }
        }

        #endregion // Public Properties

        #region Public Methods

        /// <summary>
        /// Used for debugging.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var s = $"{_name} ({_isBaseSector}): sector count: {_subSectors.Count}  cell index count: {_cells.Count}  rect: {_logicRect}\n";

            if (_subSectors.Count > 0)
            {
                foreach (var sector in _subSectors)
                {
                    s += sector.ToString();
                }
            }

            return s;
        }

        /// <summary>
        /// Clears the data and all data in sub-sectors.
        /// </summary>
        /// <param name="name">The name of the sector</param>
        /// <param name="depth">The sub-sector recursion depth</param>
        /// <param name="rect">The rect that defines the extents of the sector</param>
        public virtual void Reset(string name, int depth, Rect rect)
        {
            _name = name;
            _logicRect = rect;
            _uiRect = _logicRect.ReverseY();

            _isBaseSector = depth == 0;

            _subSectors.Clear();
            _cells.Clear();

            if (depth > 0)
            {
                // each sector has four sub-sectors (four quadrants)

                _subSectors.Resize<OcclusionSector>(4);

                var halfWidth = rect.width / 2.0f;
                var halfHeight = rect.height / 2.0f;

                // set up the sub-sectors recursively

                for (var s = 0; s < _subSectors.Count; s++)
                {
                    var subSectorName = _name + (string.IsNullOrEmpty(_name) ? "" : "_") + s.ToString();

                    var subSectorRect = new Rect(
                                                    s == 0 || s == 2 ? rect.x : (rect.x + halfWidth),
                                                    s == 0 || s == 1 ? rect.y : (rect.y + halfHeight),
                                                    halfWidth,
                                                    halfHeight
                                                    );

                    _subSectors[s].Reset(subSectorName, depth - 1, subSectorRect);
                }
            }
        }

        /// <summary>
        /// Adds a cell to the sector
        /// </summary>
        /// <param name="dataIndex">The dataIndex of the cell</param>
        /// <param name="repeatIndex">The repeatIndex of the cell</param>
        /// <param name="cellRect">The cell's logical rect</param>
        public virtual void AddCell(int dataIndex, int repeatIndex, Rect cellRect)
        {
            // check if the sector contains the cell

            if (_logicRect.Overlaps(cellRect, false))
            {
                if (_isBaseSector)
                {
                    // add the cell to the base sector (highest level)

                    _cells.AddUnique(new OcclusionCell(dataIndex, repeatIndex, cellRect));
                }
                else
                {
                    // chech each sub-sector and add the cell there recursively if necessary

                    for (var s = 0; s < _subSectors.Count; s++)
                    {
                        _subSectors[s].AddCell(dataIndex, repeatIndex, cellRect);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of cells that fall within a visible rect.
        /// </summary>
        /// <param name="visibleRect">The rect to check against the cells</param>
        /// <param name="sort">Whether to sort the cells by repeatIndex</param>
        /// <returns></returns>
        public virtual EnhancedList<OcclusionCell> GetOcclusionCells(Rect visibleRect, bool sort = false)
        {
            var list = new EnhancedList<OcclusionCell>();

            // check if the cell falls withing the visibleRect

            if (visibleRect.Overlaps(_logicRect, false))
            {
                if (_isBaseSector)
                {
                    // if this is the highest level sector, then add the cells that are visible

                    for (var c = 0; c < _cells.Count; c++)
                    {
                        if (visibleRect.Overlaps(_cells[c].logicRect))
                        {
                            list.Add(_cells[c]);
                        }
                    }
                    return list;
                }
                else
                {
                    // check each sub-sector for visibility there

                    for (var s = 0; s < _subSectors.Count; s++)
                    {
                        var subList = _subSectors[s].GetOcclusionCells(visibleRect);
                        for (var i = 0; i < subList.Count; i++)
                        {
                            list.AddUnique(subList[i]);
                        }
                    }
                }
            }

            // sort the list based on repeatIndex if necessary

            if (sort)
            {
                var sortedList = new EnhancedList<OcclusionCell>();
                bool inserted;
                for (var i = 0; i < list.Count; i++)
                {
                    inserted = false;
                    for (var s = 0; s < sortedList.Count; s++)
                    {
                        if (list[i].repeatIndex < sortedList[s].repeatIndex)
                        {
                            sortedList.Insert(s, list[i]);
                            inserted = true;
                            break;
                        }
                    }
                    if (!inserted)
                    {
                        sortedList.Add(list[i]);
                    }
                }

                return sortedList;
            }
            else
            {
                return list;
            }
        }

        /// <summary>
        /// Gets the four base sectors
        /// </summary>
        /// <returns></returns>
        public virtual EnhancedList<OcclusionSector> GetBaseSectors()
        {
            if (_isBaseSector)
            {
                return new EnhancedList<OcclusionSector>() { this };
            }
            else
            {
                EnhancedList<OcclusionSector> list = new EnhancedList<OcclusionSector>();

                for (var s = 0; s < _subSectors.Count; s++)
                {
                    var subList = _subSectors[s].GetBaseSectors();
                    for (var i = 0; i < subList.Count; i++)
                    {
                        list.Add(subList[i]);
                    }
                }

                return list;
            }
        }

        #endregion // Public Methods

        #endregion // Public

        #region Protected

        #region Protected Members

        /// <summary>
        /// Name of the sector.
        /// </summary>
        protected string _name;

        /// <summary>
        /// Whether this is a highest level sector (no parents)
        /// </summary>
        protected bool _isBaseSector;

        /// <summary>
        /// The logical rect used for visibility calculations.
        /// EnhancedGrid reverses the y coordinates for simplicity.
        /// </summary>
        protected Rect _logicRect;

        /// <summary>
        /// The rect that Unity uses
        /// </summary>
        protected Rect _uiRect;

        /// <summary>
        /// The sub sectors (quadrants) of this sector.
        /// </summary>
        protected EnhancedList<OcclusionSector> _subSectors = new EnhancedList<OcclusionSector>();

        /// <summary>
        /// The cells that fall within this sector.
        /// </summary>
        protected EnhancedList<OcclusionCell> _cells = new EnhancedList<OcclusionCell>();

        #endregion // Protected Methods

        #endregion // Protected
    }
}