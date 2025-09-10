namespace echo17.EnhancedUI.EnhancedGrid
{
    using System;
    using UnityEngine;
    using Helpers;

    /// <summary>
    /// Helper struct to load a list of color values from a file or text string, with
    /// lookups on those colors. File format should be similar to:
    ///
    /// REM https://lospec.com/palette-list/cl8uds
    ///
    /// fcb08c
    /// ef9d7f
    /// d6938a
    /// b48d92
    /// a597a1
    /// 8fa0bf
    /// 9aabc9
    /// a5b7d4
    ///
    /// Note: REM and black lines are ignored and are not necessary.
    /// </summary>
    public struct Palette
    {
        /// <summary>
        /// List of colos in the palette
        /// </summary>
        public EnhancedList<Color> colors;

        /// <summary>
        /// Create and loads the palette from a resource
        /// </summary>
        /// <param name="resourcePath">The path of the text file to load</param>
        public Palette(string resourcePath)
        {
            colors = null;
            LoadFromResource(resourcePath);
        }

        /// <summary>
        /// Loads a palette from a resource
        /// </summary>
        /// <param name="resourcePath">The path of the text file to load</param>
        public void LoadFromResource(string resourcePath)
        {
            TextAsset paletteFile = Resources.Load<TextAsset>(resourcePath);
            LoadFromText(paletteFile.text);
        }

        /// <summary>
        /// Loads a palette from a text string
        /// </summary>
        /// <param name="text">the text to load</param>
        public void LoadFromText(string text)
        {
            colors = new EnhancedList<Color>();

            Color color;

            // files can be delimited by Linux / Mac or Windows line endings

            string[] paletteLines = text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < paletteLines.Length; i++)
            {
                // trim off extra spaces and has tags

                var line = paletteLines[i].Trim();
                line.Replace("#", "");

                // ignore empty lines, lines beginning with "REM" (remark),
                // or lines that don't have enough characters for the color

                if (string.IsNullOrEmpty(line)) continue;
                if (line.Length >= 3 && line.Substring(0, 3).ToLower() == "rem") continue;
                if (line.Length < 6) continue;

                // try to get a color from the line

                ColorUtility.TryParseHtmlString("#" + paletteLines[i], out color);

                colors.Add(color);
            }
        }

        /// <summary>
        /// Gets a color based on an index. If the color is outside of the bounds of the palette,
        /// it will pull the modulus of the color index, so no out of bounds errors occur and you
        /// don't have to worry about the actual size of the palette.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Color GetColor(int index)
        {
            return colors != null && colors.Count > 0 ? colors[index % colors.Count] : Color.magenta;
        }

        /// <summary>
        /// Get the index of the color
        /// </summary>
        /// <param name="color">The color to check</param>
        /// <returns>Index, or -1 if no color was found</returns>
        public int GetColorIndex(Color color)
        {
            for (var i = 0; i < colors.Count; i++)
            {
                if (colors[i] == color) return i;
            }

            return -1;
        }
    }
}