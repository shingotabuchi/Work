namespace echo17.EnhancedUI.EnhancedGrid.Example_08
{
    using System;
    using System.IO;
    using UnityEngine;
    using UnityEngine.UI;

    public class OpenFile : MonoBehaviour
    {
        public Text label;

        private Action<string> _openAction;
        private string _fileName;

        public void Initialize(Action<string> openAction, string fileName)
        {
            label.text = fileName;

            _openAction = openAction;
            _fileName = fileName;
        }

        public void Button_OnClick()
        {
            if (_openAction != null)
            {
                _openAction(_fileName);
            }
        }
    }
}
