namespace echo17.EnhancedUI.EnhancedGrid.Example_08
{
    using System;
    using System.IO;
    using UnityEngine;
    using UnityEngine.UI;

    public partial class Controller
    {
        public string savePath = "";

        public InputField saveNameInputField;
        public Text willBeSavedToLabel;

        private void _InitializeSave()
        {
            if (string.IsNullOrEmpty(savePath))
            {
                savePath = Application.persistentDataPath;
            }

            willBeSavedToLabel.text = $"Will be saved to: {savePath}";
        }

        public void SaveConfirmButton_OnClick()
        {
            if (saveNameInputField.text.Trim() == "")
            {
                saveNameInputField.text = "MyImage";
            }

            File.WriteAllBytes(savePath + "/" + saveNameInputField.text.Trim() + ".png", _previewTexture.EncodeToPNG());
            SaveCancelButton_OnClick();
        }

        public void OpenLocationButton_OnClick()
        {
            System.Diagnostics.Process.Start(savePath);
        }
    }
}
