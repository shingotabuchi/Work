namespace echo17.EnhancedUI.EnhancedGrid.Example_01
{
    using UnityEngine;

    public enum ModelType
    {
        ContactGroup,
        Contact
    }

    /// <summary>
	/// Base interface for both contacts and contact groups.
	/// This allows us to store contacts and contact groups in the same data set.
	/// </summary>
    public interface IModelBase
    {
        ModelType GetModelType();
    }
}
