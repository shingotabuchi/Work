namespace echo17.EnhancedUI.EnhancedGrid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using echo17.EnhancedUI.Helpers;

    /// <summary>
    /// The minimum size of the cell and its size mode.
    /// </summary>
    public struct CellProperties
    {
        /// <summary>
        /// The size requested by the user. This size will not
        /// necessarily equal the actual size in the cell layout,
        /// depending on the other sizes in the group.
        /// </summary>
        public Vector2 minSize;

        /// <summary>
        /// What percentage (typically 0..1) of the remaining space
        /// in a group to use when expanding. A weight of zero means
        /// that the cell will just use its minSize value. Note: you
        /// can exceed a value of one, but can't go below zero.
        /// </summary>
        public float expansionWeight;

        public CellProperties(Vector2 minSize, float expansionWeight)
        {
            this.minSize = minSize;
            this.expansionWeight = expansionWeight;
        }
    }
}
