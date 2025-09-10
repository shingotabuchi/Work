using System;
using UnityEngine;
using UnityEngine.UI;
using Fwk.Sound;

namespace Fwk.UI
{
    public class CoolButton : Button
    {
        [SerializeField] private string _seName = "click1";
        private Action _onClickAction;

        protected override void Awake()
        {
            base.Awake();
            onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if (!string.IsNullOrEmpty(_seName))
            {
                SoundManager.Instance.PlaySeOneShot(_seName);
            }
            _onClickAction?.Invoke();
        }

        public void SetOnClickAction(Action action)
        {
            _onClickAction = action;
        }

        public virtual void SetInteractable(bool interactable)
        {
            base.interactable = interactable;
        }
    }
}