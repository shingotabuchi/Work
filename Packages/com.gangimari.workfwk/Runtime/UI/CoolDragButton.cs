using System;
using UnityEngine.EventSystems;

namespace Fwk.UI
{
    public class CoolDragButton : CoolButton, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Action<PointerEventData> _onDragBeginAction;
        private Action<PointerEventData> _onDragAction;
        private Action<PointerEventData> _onDragEndAction;

        public void SetOnDragBeginAction(Action<PointerEventData> action)
        {
            _onDragBeginAction = action;
        }

        public void SetOnDragAction(Action<PointerEventData> action)
        {
            _onDragAction = action;
        }

        public void SetOnDragEndAction(Action<PointerEventData> action)
        {
            _onDragEndAction = action;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!interactable) return;
            _onDragBeginAction?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!interactable) return;
            _onDragAction?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!interactable) return;
            _onDragEndAction?.Invoke(eventData);
        }
    }
}