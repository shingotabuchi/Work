namespace echo17.EnhancedUI.EnhancedGrid.Example_08
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
	/// Example showing how you can use a grid with selection to create pixel art.
	///
	/// The Controller handling the grid and the logic for pixel painting are separated
	/// into two separate files for clarity, both with partial classes of Controller.
	///
	/// Note: the logic here isn't specific to EnhancedGrid, so the comments will be minimal.
	/// </summary>
    public partial class Controller
    {
        private enum Mode
        {
            Paint,
            Save,
            Open
        }

        public Vector2Int canvasSize;
        public TextAsset paletteFile;
        public Transform palettePanel;
        public GameObject paletteColorButton;
        public Image previewImage;
        public GameObject open;
        public GameObject save;
        public Dropdown zoomDropdown;
        public ToolButton[] toolButtons;

        private Pixel[,] _pixels;
        private Palette _palette;
        private List<PaletteColorButton> _paletteColorButtons;
        private Color _selectedColor;
        private ToolButton.ToolType _selectedToolType;
        private Vector2Int _lastPixelLocation;
        private RectTransform _scrollViewRectTransform;
        private Texture2D _previewTexture;
        private Mode _mode;

        void Awake()
        {
            Application.targetFrameRate = 60;

            _scrollViewRectTransform = grid.ScrollView.GetComponent<RectTransform>();

            _palette.LoadFromText(paletteFile.text);

            _paletteColorButtons = new List<PaletteColorButton>();
            for (var i = 0; i < _palette.colors.Count; i++)
            {
                var go = GameObject.Instantiate(paletteColorButton, palettePanel);
                var paletteButton = go.GetComponent<PaletteColorButton>();
                paletteButton.Initialize(i, _palette.colors[i], _PaletteButtonSelected);
                _paletteColorButtons.Add(paletteButton);

                if (i == 0)
                {
                    paletteButton.ToggleSelected(true);
                }
            }

            _PaletteButtonSelected(0);

            foreach (var btn in toolButtons)
            {
                btn.Initialize(_ToolButtonSelected);
            }

            _ToolButtonSelected(toolButtons[0].toolType);

            _previewTexture = new Texture2D(canvasSize.x, canvasSize.y);
            Sprite sprite = Sprite.Create(_previewTexture, new Rect(0, 0, canvasSize.x, canvasSize.y), Vector2.zero);
            previewImage.sprite = sprite;

            ZoomDropdown_OnValueChanged();

            _SetMode(Mode.Paint);
        }

        void Start()
        {
            _pixels = new Pixel[canvasSize.x, canvasSize.y];

            _ClearCanvas(Color.clear);

            grid.WrapCellCount = canvasSize.x;

            _SetupGrid();
        }

        void Update()
        {
            if (_mode != Mode.Paint) return;

            if (Input.GetMouseButton(0))
            {
                _SelectPixel(_CalculatePixelLocation(_TransformPosition(Input.mousePosition)));
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_selectedToolType == ToolButton.ToolType.Bucket)
                {
                    _FloodFill(_CalculatePixelLocation(_TransformPosition(Input.mousePosition)), _selectedColor);
                }
            }
        }

        public void ZoomDropdown_OnValueChanged()
        {
            if (_mode != Mode.Paint) return;

            previewImage.rectTransform.localScale = new Vector3(zoomDropdown.value + 1, zoomDropdown.value + 1, 1f);
        }

        public void OpenButton_OnClick()
        {
            _SetMode(Mode.Open);
        }

        public void SaveButton_OnClick()
        {
            _SetMode(Mode.Save);
        }

        public void SaveCancelButton_OnClick()
        {
            _SetMode(Mode.Paint);
        }

        public void OpenCancelButton_OnClick()
        {
            _SetMode(Mode.Paint);
        }

        private void _SetMode(Mode mode)
        {
            _mode = mode;

            switch (_mode)
            {
                case Mode.Paint:

                    save.SetActive(false);
                    open.SetActive(false);

                    break;

                case Mode.Save:

                    save.SetActive(true);
                    open.SetActive(false);

                    _InitializeSave();

                    break;

                case Mode.Open:

                    save.SetActive(false);
                    open.SetActive(true);

                    _InitializeOpen();

                    break;
            }
        }

        private void _ClearCanvas(Color color)
        {
            for (var x = 0; x < canvasSize.x; x++)
            {
                for (var y = 0; y < canvasSize.y; y++)
                {
                    if (_pixels[x, y] == null)
                    {
                        _pixels[x, y] = new Pixel();
                    }

                    _pixels[x, y].location = new Vector2Int(x, y);
                    _pixels[x, y].color = color;
                }
            }

            for (int y = 0; y < _previewTexture.height; y++)
            {
                for (int x = 0; x < _previewTexture.width; x++)
                {
                    _previewTexture.SetPixel(x, y, color);
                }
            }

            _previewTexture.Apply();

            grid.RefreshActiveCells();
        }

        private Vector2Int _CalculatePixelLocation(Vector2 location)
        {
            return new Vector2Int(Mathf.FloorToInt(location.x / pixelSize.x), -Mathf.FloorToInt(location.y / pixelSize.y) - 1);
        }

        private bool _PixelLocationIsValid(Vector2Int pixelLocation)
        {
            return !((pixelLocation.x < 0 || pixelLocation.x > canvasSize.x - 1 || pixelLocation.y < 0 || pixelLocation.y > canvasSize.y - 1));
        }

        private void _SelectPixel(Vector2Int pixelLocation)
        {
            if (!_PixelLocationIsValid(pixelLocation)) return;

            if (pixelLocation != _lastPixelLocation)
            {
                switch (_selectedToolType)
                {
                    case ToolButton.ToolType.Pen:

                        _pixels[pixelLocation.x, pixelLocation.y].color = _selectedColor;
                        _SetPreviewTexturePixel(pixelLocation.x, pixelLocation.y, _selectedColor);

                        break;

                    case ToolButton.ToolType.Eraser:

                        _pixels[pixelLocation.x, pixelLocation.y].color = Color.clear;
                        _SetPreviewTexturePixel(pixelLocation.x, pixelLocation.y, Color.clear);

                        break;

                    case ToolButton.ToolType.Dropper:

                        var colorIndex = _palette.GetColorIndex(_pixels[pixelLocation.x, pixelLocation.y].color);
                        if (colorIndex != -1)
                        {
                            _PaletteButtonSelected(colorIndex);
                        }

                        break;
                }

                grid.RefreshActiveCells();
            }

            _lastPixelLocation = pixelLocation;
        }

        private void _SetPreviewTexturePixel(int x, int y, Color color)
        {
            _previewTexture.SetPixel(x, canvasSize.y - y - 1, color);
            _previewTexture.Apply();
        }

        private void _PaletteButtonSelected(int colorIndex)
        {
            _selectedColor = _palette.colors[colorIndex];

            foreach (var btn in _paletteColorButtons)
            {
                btn.ToggleSelected(btn.ColorIndex == colorIndex);
            }
        }

        private void _ToolButtonSelected(ToolButton.ToolType toolType)
        {
            if (_mode != Mode.Paint) return;

            if (toolType == ToolButton.ToolType.Clear)
            {
                _ClearCanvas(Color.clear);
                return;
            }

            _selectedToolType = toolType;

            foreach (var btn in toolButtons)
            {
                btn.SetIsSelected(btn.toolType == toolType);
            }
        }

        private int _GetDataIndexFromLocation(Vector2Int location)
        {
            return (location.y * canvasSize.x) + location.x;
        }

        private Vector2Int _GetLocationFromDataIndex(int dataIndex)
        {
            return new Vector2Int(dataIndex % canvasSize.x, dataIndex / canvasSize.x);
        }

        private Vector2 _TransformPosition(Vector2 position)
        {
            Vector2 result;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_scrollViewRectTransform, position, null, out result);
            return result;
        }

        private void _FloodFill(Vector2Int pixelLocation, Color replacementColor)
        {
            if (!_PixelLocationIsValid(pixelLocation)) return;

            var targetColor = _pixels[pixelLocation.x, pixelLocation.y].color;

            if (targetColor == replacementColor) return;

            Stack<Vector2Int> pixels = new Stack<Vector2Int>();

            pixels.Push(pixelLocation);
            while (pixels.Count != 0)
            {
                Vector2Int temp = pixels.Pop();
                int y1 = temp.y;
                while (y1 >= 0 && _pixels[temp.x, y1].color == targetColor)
                {
                    y1--;
                }
                y1++;
                bool spanLeft = false;
                bool spanRight = false;
                while (y1 < canvasSize.y && _pixels[temp.x, y1].color == targetColor)
                {
                    _pixels[temp.x, y1].color = replacementColor;

                    if (!spanLeft && temp.x > 0 && _pixels[temp.x - 1, y1].color == targetColor)
                    {
                        pixels.Push(new Vector2Int(temp.x - 1, y1));
                        spanLeft = true;
                    }
                    else if (spanLeft && temp.x - 1 == 0 && _pixels[temp.x - 1, y1].color != targetColor)
                    {
                        spanLeft = false;
                    }
                    if (!spanRight && temp.x < canvasSize.x - 1 && _pixels[temp.x + 1, y1].color == targetColor)
                    {
                        pixels.Push(new Vector2Int(temp.x + 1, y1));
                        spanRight = true;
                    }
                    else if (spanRight && temp.x < canvasSize.x - 1 && _pixels[temp.x + 1, y1].color != targetColor)
                    {
                        spanRight = false;
                    }

                    y1++;
                }
            }

            for (int y = 0; y < _previewTexture.height; y++)
            {
                for (int x = 0; x < _previewTexture.width; x++)
                {
                    _previewTexture.SetPixel(x, canvasSize.y - y - 1, _pixels[x, y].color);
                }
            }

            _previewTexture.Apply();

            grid.RefreshActiveCells();
        }
    }
}