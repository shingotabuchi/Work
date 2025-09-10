namespace echo17.EnhancedUI.EnhancedGrid.Example_01
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
	/// Practical example showing one way to display a one-dimensional list of contacts
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid grid;
        public GameObject contactCellPrefab;
        public Vector2 contactCellSize;
        public GameObject contactGroupCellPrefab;
        public Vector2 contactGroupCellSize;

        /// <summary>
		/// Text file storing a list of contacts
		/// </summary>
        public TextAsset contactDatabase;

        private List<IModelBase> _contacts;

        void Awake()
        {
            Application.targetFrameRate = 60;
        }

        void Start()
        {
            // read in the contacts from a simple text file delimited by carriage returns
            var contactLines = contactDatabase.text.Split("\n"[0]);

            grid.InitializeGrid(this);

            _contacts = new List<IModelBase>();

            // read each contact from the text file, adding it to the data set
            for (var i = 0; i < contactLines.Length; i++)
            {
                // ignore empty lines or lines without commas
                if (!string.IsNullOrEmpty(contactLines[i]) && contactLines[i].Contains(","))
                {
                    // split the line into a name and phone number
                    var contactFields = contactLines[i].Split(","[0]);

                    // split the first and last name into seperate values
                    var names = contactFields[0].Split(" "[0]);

                    // add the contact
                    // note that contact and contact group both inherit from IModelBase, so they
                    // both be stored in the same data set.
                    _contacts.Add(new Contact()
                    {
                        firstName = names[0],
                        surname = names[1],
                        phoneNumber = contactFields[1]
                    });
                }
            }

            // sort the contacts by last name
            _contacts = _contacts.OrderBy(o => ((Contact)o).surname).ToList();

            // break the contacts into groups by the first letter of the last name

            var c = 0;
            var previousLetter = "";

            while (c < _contacts.Count)
            {
                var firstLetter = ((Contact)_contacts[c]).surname.Substring(0, 1);

                if (previousLetter == "" || previousLetter != firstLetter)
                {
                    // insert a contact group into the data set for this letter of the alphabet.
                    // note that contact and contact group both inherit from IModelBase, so they
                    // both be stored in the same data set.

                    _contacts.Insert(c, new ContactGroup()
                    {
                        groupDescription = firstLetter
                    });

                    previousLetter = firstLetter;
                }

                c++;
            }

            grid.RecalculateGrid();
        }

        public int GetCellCount(EnhancedGrid grid)
        {
            return _contacts.Count;
        }

        public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
        {
            // get the correct prefab based on whether the data element is a contact or a contact group

            if (_contacts[dataIndex].GetModelType() == ModelType.Contact)
            {
                return contactCellPrefab;
            }
            else
            {
                return contactGroupCellPrefab;
            }
        }

        public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
        {
            // return different cell properties based on whether the data element is a contact or a contact group

            if (_contacts[dataIndex].GetModelType() == ModelType.Contact)
            {
                return new CellProperties(contactCellSize, 0f);
            }
            else
            {
                return new CellProperties(contactGroupCellSize, 0f);
            }
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            // update the cell view based on whether the data element is a contact or contact group

            if (_contacts[dataIndex].GetModelType() == ModelType.Contact)
            {
                (cell as ContactCell).UpdateCell((Contact)_contacts[dataIndex]);
            }
            else
            {
                (cell as ContactGroupCell).UpdateCell((ContactGroup)_contacts[dataIndex]);
            }
        }
    }
}