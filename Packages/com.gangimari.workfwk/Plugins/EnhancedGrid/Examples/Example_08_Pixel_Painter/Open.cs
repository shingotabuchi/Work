namespace echo17.EnhancedUI.EnhancedGrid.Example_08
{
    using System;
    using System.IO;
    using UnityEngine;
    using UnityEngine.UI;

    public partial class Controller
    {
        public Text openLocationLabel;
        public GameObject openFilePrefab;
        public Transform openFileContent;

        private void _InitializeOpen()
        {
            if (string.IsNullOrEmpty(savePath))
            {
                savePath = Application.persistentDataPath;
            }

            openLocationLabel.text = savePath;

            var dir = new DirectoryInfo(savePath);

            foreach (var file in dir.GetFiles("*.png"))
            {
                var go = GameObject.Instantiate(openFilePrefab, openFileContent);
                var openFile = go.GetComponent<OpenFile>();

                openFile.Initialize(_OpenFileSelected, file.Name);
            }
        }

        private void _OpenFileSelected(string fileName)
        {
            var fullPath = Path.Combine(savePath, fileName);

            var data = File.ReadAllBytes(fullPath);
            var tempTexture = new Texture2D(2, 2);
            tempTexture.LoadImage(data);

            for (var x = 0; x < Mathf.Min(canvasSize.x, tempTexture.width); x++)
            {
                for (var y = 0; y < Mathf.Min(canvasSize.y, tempTexture.height); y++)
                {
                    _pixels[x, canvasSize.y - y - 1].color = tempTexture.GetPixel(x, y);
                }
            }

            for (var x = Mathf.Min(canvasSize.x, tempTexture.width); x < canvasSize.x; x++)
            {
                for (var y = Mathf.Min(canvasSize.y, tempTexture.height); y < canvasSize.y; y++)
                {
                    _pixels[x, canvasSize.y - y - 1].color = Color.clear;
                }
            }

            for (int y = 0; y < _previewTexture.height; y++)
            {
                for (int x = 0; x < _previewTexture.width; x++)
                {
                    _previewTexture.SetPixel(x, y, _pixels[x, canvasSize.y - y - 1].color);
                }
            }

            _previewTexture.Apply();

            grid.RefreshActiveCells();

            OpenCancelButton_OnClick();
        }
    }
}
