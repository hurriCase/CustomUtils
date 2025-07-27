namespace CustomUtils.Runtime.UI.CustomComponents
{
    /// <summary>
    /// Interface for components that need to be notified when text size changes
    /// </summary>
    public interface ITextSizeNotifiable
    {
        /// <summary>
        /// Called when the text component's size has changed
        /// </summary>
        void OnTextSizeChanged();
    }
}