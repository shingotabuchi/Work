namespace echo17.EnhancedUI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using EnhancedUI.Helpers;
    using System.Text;

    /// <summary>
    /// Used to speed up processing of cells. OcclusionCells are pointers to cellLayouts,
    /// and store the occlusion information in the OcclusionSector.
    /// </summary>
    public struct OcclusionCell
    {
        /// <summary>
        /// The dataIndex of the cell. See cellLayout for more information.
        /// </summary>
        public int dataIndex;

        /// <summary>
        /// The repeatIndex of the cell. See cellLayout for more information.
        /// </summary>
        public int repeatIndex;

        /// <summary>
        /// The logicRect of the cell. See cellLayout for more information.
        /// </summary>
        public Rect logicRect;

        public OcclusionCell(int dataIndex, int repeatIndex, Rect logicRect)
        {
            this.dataIndex = dataIndex;
            this.repeatIndex = repeatIndex;
            this.logicRect = logicRect;
        }
    }
}
