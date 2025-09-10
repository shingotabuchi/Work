using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace Fwk.UI
{
    public class View : MonoBehaviour
    {
        private readonly List<View> _childViews = new();
        private CancellationTokenSource _cancellationTokenSource = new();

        protected virtual void Awake()
        {
            GetChildViews();
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new();
        }

        protected void GetChildViews()
        {
            _childViews.Clear();
            _childViews.AddRange(GetComponentsInChildren<View>());
            _childViews.Remove(this);
        }

        public virtual void Show()
        {
            gameObject.SetActiveFast(true);
            foreach (var childView in _childViews)
            {
                childView.OnParentShow();
            }
        }

        public virtual void Hide()
        {
            gameObject.SetActiveFast(false);
            foreach (var childView in _childViews)
            {
                childView.OnParentHide();
            }
        }

        public virtual void OnParentShow()
        {
        }

        public virtual void OnParentHide()
        {
        }

        public async UniTask SetActiveWithDelay(float delay, bool active, CancellationToken cancellationToken = default)
        {
            Cancel();
            using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: linkedCancellationTokenSource.Token);
            if (linkedCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }
            if (this == null)
            {
                return;
            }
            gameObject.SetActiveFast(active);
        }
    }
}