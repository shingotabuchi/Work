namespace echo17.EnhancedUI.Helpers
{
    using System.Globalization;
    using UnityEngine;

    /// <summary>
    /// Some helper extensions for various types.
    /// </summary>
    public static class EnhancedExtensions
    {
        #region Rect Extensions

        /// <summary>
        /// Reverses just the y component of a rect
        /// </summary>
        /// <param name="r">This rect</param>
        /// <returns>Modified rect</returns>
        public static Rect ReverseY(this Rect r)
        {
            return new Rect(r.x, -r.y, r.width, r.height);
        }

        /// <summary>
        /// Upper left corner of the rect
        /// </summary>
        /// <param name="r">This rect</param>
        /// <returns>Coordinates</returns>
        public static Vector2 UpperLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }

        /// <summary>
        /// Upper right corner of the rect
        /// </summary>
        /// <param name="r">This rect</param>
        /// <returns>Coordinates</returns>
        public static Vector2 UpperRight(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.yMin);
        }

        /// <summary>
        /// Lower left corner of the rect
        /// </summary>
        /// <param name="r">This rect</param>
        /// <returns>Coordinates</returns>
        public static Vector2 LowerLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMax);
        }

        /// <summary>
        /// Lower right corner of the rect
        /// </summary>
        /// <param name="r">This rect</param>
        /// <returns>Coordinates</returns>
        public static Vector2 LowerRight(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.yMax);
        }

        /// <summary>
        /// Returns whether the rect is above and to the left of a coordinate
        /// </summary>
        /// <param name="rect">This rect</param>
        /// <param name="point">Coordinate to check</param>
        /// <returns>Whether the test passes</returns>
        public static bool IsAboveAndToTheLeft(this Rect rect, Vector2 point)
        {
            return (point.x > rect.xMax && point.y > rect.yMax);
        }

        /// <summary>
        /// Returns whether the rect is above and to the right of a coordinate
        /// </summary>
        /// <param name="rect">This rect</param>
        /// <param name="point">Coordinate to check</param>
        /// <returns>Whether the test passes</returns>
        public static bool IsAboveAndToTheRight(this Rect rect, Vector2 point)
        {
            return (point.x < rect.xMin && point.y > rect.yMax);
        }

        /// <summary>
        /// Returns whether the rect is below and to the left of a coordinate
        /// </summary>
        /// <param name="rect">This rect</param>
        /// <param name="point">Coordinate to check</param>
        /// <returns>Whether the test passes</returns>
        public static bool IsBelowAndToTheLeft(this Rect rect, Vector2 point)
        {
            return (point.x > rect.xMax && point.y < rect.yMin);
        }

        /// <summary>
        /// Returns whether the rect is below and to the right of a coordinate
        /// </summary>
        /// <param name="rect">This rect</param>
        /// <param name="point">Coordinate to check</param>
        /// <returns>Whether the test passes</returns>
        public static bool IsBelowAndToTheRight(this Rect rect, Vector2 point)
        {
            return (point.x < rect.xMin && point.y < rect.yMin);
        }

        /// <summary>
        /// Returns whether the rect is directly above a coordinate, meaning the
        /// coordinate falls in the x bounds of the rect, but is more than the rect's
        /// y bounds.
        /// </summary>
        /// <param name="rect">This rect</param>
        /// <param name="point">Coordinate to check</param>
        /// <returns>Whether the test passes</returns>
        public static bool IsDirectlyAbove(this Rect rect, Vector2 point)
        {
            return (point.x >= rect.xMin && point.x <= rect.xMax && point.y > rect.yMax);
        }

        /// <summary>
        /// Returns whether the rect is directly below a coordinate, meaning the
        /// coordinate falls in the x bounds of the rect, but is less than the rect's
        /// y bounds.
        /// </summary>
        /// <param name="rect">This rect</param>
        /// <param name="point">Coordinate to check</param>
        /// <returns>Whether the test passes</returns>
        public static bool IsDirectlyBelow(this Rect rect, Vector2 point)
        {
            return (point.x >= rect.xMin && point.x <= rect.xMax && point.y < rect.yMin);
        }

        /// <summary>
        /// Returns whether the rect is directly to the left of a coordinate, meaning the
        /// coordinate falls in the y bounds of the rect, but is more than the rect's
        /// x bounds.
        /// </summary>
        /// <param name="rect">This rect</param>
        /// <param name="point">Coordinate to check</param>
        /// <returns>Whether the test passes</returns>
        public static bool IsDirectlyToTheLeft(this Rect rect, Vector2 point)
        {
            return (point.x > rect.xMax && point.y >= rect.yMin && point.y <= rect.yMax);
        }

        /// <summary>
        /// Returns whether the rect is directly to the right of a coordinate, meaning the
        /// coordinate falls in the y bounds of the rect, but is less than the rect's
        /// x bounds.
        /// </summary>
        /// <param name="rect">This rect</param>
        /// <param name="point">Coordinate to check</param>
        /// <returns>Whether the test passes</returns>
        public static bool IsDirectlyToTheRight(this Rect rect, Vector2 point)
        {
            return (point.x < rect.xMin && point.y >= rect.yMin && point.y <= rect.yMax);
        }

        /// <summary>
        /// The square root of the DistanceToSquared method
        /// </summary>
        /// <param name="rect">This rect</param>
        /// <param name="point">Coordinate to check</param>
        /// <returns>The linear distance to the coordinate</returns>
        public static float DistanceTo(this Rect rect, Vector2 point)
        {
            return Mathf.Sqrt(DistanceToSquared(rect, point));
        }

        /// <summary>
        /// Returns the squared distance from a rect to a coordinate. If the coordinate is completely inside
        /// the rect, then the distance will be zero. If it is directly to a side of the rect, then
        /// the distance will be from the coordinate to the side. If it is outside of a corner of the
        /// rect, then the distance will be the square magnitude of the vector offset.
        /// This method is faster than DistanceTo, so if you are just comparing and don't need the actual
        /// distance, use this method instead.
        /// </summary>
        /// <param name="rect">This rect</param>
        /// <param name="point">Coordinate to check</param>
        /// <returns>The squared distance to the coordinate</returns>
        public static float DistanceToSquared(this Rect rect, Vector2 point)
        {
            // inside

            if (rect.Contains(point)) return 0;

            // corners

            if (rect.IsBelowAndToTheRight(point)) return (point - rect.UpperLeft()).sqrMagnitude;
            if (rect.IsBelowAndToTheLeft(point)) return (point - rect.UpperRight()).sqrMagnitude;
            if (rect.IsAboveAndToTheRight(point)) return (point - rect.LowerLeft()).sqrMagnitude;
            if (rect.IsAboveAndToTheLeft(point)) return (point - rect.LowerRight()).sqrMagnitude;

            // edges

            if (rect.IsDirectlyBelow(point)) return Mathf.Pow((point.y - rect.yMin), 2);
            if (rect.IsDirectlyAbove(point)) return Mathf.Pow((point.y - rect.yMax), 2);
            if (rect.IsDirectlyToTheRight(point)) return Mathf.Pow((point.x - rect.xMin), 2);
            if (rect.IsDirectlyToTheLeft(point)) return Mathf.Pow((point.x - rect.xMax), 2);

            return Mathf.Infinity;
        }

        /// <summary>
        /// Checks whether another rect falls completely inside this rect (not just overlaps).
        /// </summary>
        /// <param name="rect">This rect</param>
        /// <param name="otherRect">Rect to check against</param>
        /// <returns>Test result</returns>
        public static bool FullyContainsRect(this Rect rect, Rect otherRect)
        {
            return (
                    otherRect.xMin >= rect.xMin
                    &&
                    otherRect.xMax <= rect.xMax
                    &&
                    otherRect.yMin >= rect.yMin
                    &&
                    otherRect.yMax <= rect.yMax
                    );
        }

        /// <summary>
        /// Returns a coordinate based on the rect's minimum point (upper left)
        /// and a factor. Useful for place coordinates inside the rect based on
        /// a normalized amount.
        /// </summary>
        /// <param name="rect">This rect</param>
        /// <param name="factor">The factor to multiple against the rect's size</param>
        /// <returns>New coordinate based on this rect's position</returns>
        public static Vector2 PointInRect(this Rect rect, Vector2 factor)
        {
            return new Vector2(rect.xMin + (factor.x * rect.width), rect.yMin + (factor.y * rect.height));
        }

        #endregion

        #region String Extensions

        /// <summary>
        /// Capitalizes every word in a sentence.
        /// </summary>
        /// <param name="s">string to check</param>
        /// <param name="cultureInfoName">The culture rules to use, defaulting to US english</param>
        /// <returns>New capitalized string</returns>
        public static string CapitalizeFirstLetterOfWords(this string s, string cultureInfoName = "en-US")
        {
            TextInfo textInfo = new CultureInfo(cultureInfoName, false).TextInfo;
            return textInfo.ToTitleCase(s);
        }

        #endregion
    }
}
