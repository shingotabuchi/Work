namespace echo17.EnhancedUI.EnhancedGrid.Example_01
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Cell view to show the contact group information.
    /// Note: in this example, we are not inheriting from BasicGridCell to
    /// show you how to inherit directly from IEnhancedGridCell
    /// </summary>
    public class ContactGroupCell : MonoBehaviour, IEnhancedGridCell
    {
        public int DataIndex { get; set; }
        public int RepeatIndex { get; set; }
        public Transform ContainerTransform { get; set; }

        public Text groupDescriptionLabel;

        public void UpdateCell(ContactGroup contactGroup)
        {
            groupDescriptionLabel.text = contactGroup.groupDescription;
        }

        /// <summary>
		/// Contact group cells differ from contact cells, so we pass a unique identifier back here
		/// </summary>
		/// <returns></returns>
        public virtual string GetCellTypeIdentifier()
        {
            return "ContactGroupCell";
        }

        public virtual float GetDrawPriority()
        {
            return 0f;
        }
    }
}
