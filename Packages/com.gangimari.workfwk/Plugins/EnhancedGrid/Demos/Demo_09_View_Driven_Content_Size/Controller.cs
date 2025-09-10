namespace echo17.EnhancedUI.EnhancedGrid.Demo_09
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
	/// Demo showing how you can use Unity's content size fitter on a template object
	/// to calculate the space needed for the text to fit inside a cell.
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject cellPrefab;

        /// <summary>
		/// The template object to reuse for each cell
		/// </summary>
        public Text templateText;
        public RectOffset cellPadding;

        private List<Model> _data;
        private RectTransform _templateRectTransform;
        private LayoutElement _templateLayoutElement;

        void Awake()
        {
            _templateRectTransform = templateText.rectTransform;
            _templateLayoutElement = templateText.GetComponent<LayoutElement>();

            Application.targetFrameRate = 60;
        }

        void Start()
        {
            templateText.gameObject.SetActive(true);

            grid.InitializeGrid(this);

            _data = new List<Model>();

            // add in some data with Lorum ipsum text

            _AddCell(300f, "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Id donec ultrices tincidunt arcu non sodales neque sodales ut. Nullam eget felis eget nunc lobortis mattis. Magnis dis parturient montes nascetur ridiculus. Integer eget aliquet nibh praesent. Commodo ullamcorper a lacus vestibulum sed. Tortor vitae purus faucibus ornare suspendisse sed. Mi proin sed libero enim sed faucibus turpis in eu. Aliquam faucibus purus in massa tempor nec feugiat nisl pretium. Nisi vitae suscipit tellus mauris a. Non diam phasellus vestibulum lorem sed risus ultricies. Egestas integer eget aliquet nibh praesent tristique magna sit amet. Faucibus vitae aliquet nec ullamcorper sit amet risus nullam. Commodo elit at imperdiet dui accumsan sit amet. Risus commodo viverra maecenas accumsan.");
            _AddCell(100f, "Et netus et malesuada fames ac turpis egestas. Auctor elit sed vulputate mi sit amet. Volutpat odio facilisis mauris sit amet. Ac placerat vestibulum lectus mauris ultrices. Volutpat lacus laoreet non curabitur gravida.");
            _AddCell(500f, "Volutpat ac tincidunt vitae semper quis lectus nulla. Ullamcorper morbi tincidunt ornare massa eget egestas purus viverra accumsan. Potenti nullam ac tortor vitae purus faucibus ornare suspendisse sed. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Mi eget mauris pharetra et.");
            _AddCell(300f, "Nec tincidunt praesent semper feugiat nibh sed pulvinar.");
            _AddCell(400f, "Massa ultricies mi quis hendrerit dolor magna. Dignissim diam quis enim lobortis scelerisque fermentum dui faucibus in. Convallis posuere morbi leo urna molestie at. Ultrices mi tempus imperdiet nulla malesuada. Eget duis at tellus at urna. Venenatis lectus magna fringilla urna.");
            _AddCell(600f, "Nibh sed pulvinar proin gravida hendrerit. Cum sociis natoque penatibus et magnis dis parturient montes. Lacus sed viverra tellus in hac habitasse platea dictumst vestibulum. Mauris a diam maecenas sed enim ut sem. Sed viverra tellus in hac. Odio facilisis mauris sit amet. Potenti nullam ac tortor vitae purus faucibus ornare suspendisse sed. Sit amet aliquam id diam maecenas ultricies mi eget. Donec pretium vulputate sapien nec sagittis. Posuere sollicitudin aliquam ultrices sagittis. Nisi vitae suscipit tellus mauris.");
            _AddCell(300f, "Adipiscing enim eu turpis egestas pretium aenean. Urna nunc id cursus metus aliquam eleifend mi. Sem integer vitae justo eget magna fermentum. Magna sit amet purus gravida quis blandit turpis cursus in. Id donec ultrices tincidunt arcu non sodales neque. Commodo elit at imperdiet dui accumsan sit. Ut enim blandit volutpat maecenas. Odio pellentesque diam volutpat commodo sed egestas egestas. Tincidunt vitae semper quis lectus. Eu mi bibendum neque egestas congue quisque. Malesuada fames ac turpis egestas sed. Dolor morbi non arcu risus quis varius. Euismod quis viverra nibh cras pulvinar mattis nunc. Integer malesuada nunc vel risus commodo viverra maecenas accumsan. Pharetra et ultrices neque ornare aenean. Dapibus ultrices in iaculis nunc sed augue lacus viverra vitae. Volutpat maecenas volutpat blandit aliquam etiam erat.");
            _AddCell(250f, "Posuere ac ut");

            grid.RecalculateGrid();

            templateText.gameObject.SetActive(false);
        }

        public int GetCellCount(EnhancedGrid grid)
        {
            return _data.Count;
        }

        public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
        {
            return cellPrefab;
        }

        public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
        {
            return new CellProperties(_data[dataIndex].cellSize, 0f);
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            (cell as View).UpdateCell(_data[dataIndex]);
        }

        /// <summary>
		/// Function that sets a template text object to the text, calculating its size.
		/// It then stores that size in the cell data
		/// </summary>
		/// <param name="preferredWidth">One of the axis has to be fixed, so we are fixing the width in this example</param>
		/// <param name="text">The text to calculate the size on</param>
        private void _AddCell(float preferredWidth, string text)
        {
            // set the preferred width of the template label's layout element
            _templateLayoutElement.preferredWidth = preferredWidth;

            // set the text of the template label
            templateText.text = text;

            // force Unity to calculate the size of the label. If this is not done here
            // then the calculation would not happen until later
            Canvas.ForceUpdateCanvases();

            // set the cell's size in its data. we add in some padding to match the cell prefab
            var modelElement = new Model()
            {
                cellSize = new Vector2(_templateRectTransform.sizeDelta.x + cellPadding.left + cellPadding.right, _templateRectTransform.sizeDelta.y + cellPadding.top + cellPadding.bottom),
                text = text
            };

            _data.Add(modelElement);
        }
    }
}