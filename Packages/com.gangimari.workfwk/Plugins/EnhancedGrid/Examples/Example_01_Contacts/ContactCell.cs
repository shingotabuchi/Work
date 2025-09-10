namespace echo17.EnhancedUI.EnhancedGrid.Example_01
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
	/// Cell view to show the contact information.
	/// Note: in this example, we are not inheriting from BasicGridCell to
	/// show you how to inherit directly from IEnhancedGridCell
	/// </summary>
    public class ContactCell : MonoBehaviour, IEnhancedGridCell
    {
        public int DataIndex { get; set; }
        public int RepeatIndex { get; set; }
        public Transform ContainerTransform { get; set; }

        public Text nameLabel;
        public Text phoneNumberLabel;

        public void UpdateCell(Contact contact)
        {
            nameLabel.text = contact.firstName + " " + contact.surname;
            phoneNumberLabel.text = contact.phoneNumber;
        }

        /// <summary>
		/// Contact cells differ from contact group cells, so we pass a unique identifier back here
		/// </summary>
		/// <returns></returns>
        public virtual string GetCellTypeIdentifier()
        {
            return "ContactCell";
        }

        public virtual float GetDrawPriority()
        {
            return 0f;
        }
    }
}
