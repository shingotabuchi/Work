namespace echo17.EnhancedUI.EnhancedGrid.Example_01
{
    using UnityEngine;

    /// <summary>
	/// Contact group record.
	/// Note: both ContactGroup and Contact inherit from the same base of IModelBase
	/// so they can be stored in the same data set in the controller
	/// </summary>
    public class ContactGroup : IModelBase
    {
        public string groupDescription;

        public ModelType GetModelType()
        {
            return ModelType.ContactGroup;
        }
    }
}
