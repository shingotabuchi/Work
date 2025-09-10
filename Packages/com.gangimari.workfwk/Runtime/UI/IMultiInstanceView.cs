namespace Fwk.UI
{
    /// <summary>
    /// Interface to mark views that support multiple instances in the view stack.
    /// Views implementing this interface will be pooled and can have multiple instances active simultaneously.
    /// </summary>
    public interface IMultiInstanceView
    {
        /// <summary>
        /// Called when the view is returned to the pool. Use this to reset the view state.
        /// </summary>
        void OnReturnToPool();
        
        /// <summary>
        /// Called when the view is taken from the pool. Use this to initialize the view.
        /// </summary>
        void OnTakeFromPool();
    }
}