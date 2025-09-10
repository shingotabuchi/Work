namespace Fwk.UI
{
    public class StackableView : View
    {
        public bool IsAboveBlur { get; set; } = false;
        public virtual bool BlurBackground { get; } = false;

        public virtual void OnFront()
        {
            Show();
        }

        public virtual void OnRemoveFromFront()
        {
            Hide();
        }

        public virtual void OnRemoveFromBack()
        {
            Hide();
        }

        public virtual void OnAddToFront()
        {
        }

        public virtual void OnAddToBack()
        {
        }

        public virtual void OnCovered(StackableView coveringView)
        {
        }

        public virtual void UpdateViewLoop(float deltaTime)
        {
        }
    }
}