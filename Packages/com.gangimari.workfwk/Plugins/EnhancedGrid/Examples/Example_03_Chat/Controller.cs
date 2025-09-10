namespace echo17.EnhancedUI.EnhancedGrid.Example_03
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
	/// Example showing a chat between two devices, each with their own grid.
	/// The cells' sizes are calculated based on the text contents using Unity's
	/// content size fitter component.
	///
	/// Note: check the inspector for the EnhancedGrid components, noting they
	/// both have their content minimum size set to a value that pushes the content
	/// down to the bottom. They also have their flow set to BottomToTopLeftToRight
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        public EnhancedGrid person1Grid;
        public EnhancedGrid person2Grid;
        public GameObject chatFromMeCellPrefab;
        public GameObject chatFromOtherPersonCellPrefab;
        public float chatCellWidth;
        public RectOffset chatCellPadding;

        /// <summary>
		/// Template text label to calculate the text size based on Unity's
		/// content size fitter component
		/// </summary>
        public Text chatTemplateText;

        public InputField person1ChatInputField;
        public InputField person2ChatInputField;

        private List<Chat> _chats;
        private RectTransform _chatTemplateRectTransform;

        void Awake()
        {
            Application.targetFrameRate = 60;

            _chatTemplateRectTransform = chatTemplateText.GetComponent<RectTransform>();
        }

        void Start()
        {
            person1Grid.InitializeGrid(this);
            person2Grid.InitializeGrid(this);

            _chats = new List<Chat>();

            // set up some initial chats 

            _AddChat(1, "Hey");
            _AddChat(2, "What's up?");
            _AddChat(1, "Nothing, just wanted to see how you are doing.");
            _AddChat(1, "Everything going well there?");
            _AddChat(2, "Yep, all is well.");
            _AddChat(2, "How are you?");

            // recalculate the grids, setting their scroll position to the bottom.
            // Note: the y position in EnhancedGrid is reversed from Unity's UI y values,
            // where EnhancedGrid uses 0 for the top and 1 for the bottom.

            person1Grid.RecalculateGrid(scrollNormalizedPositionY: 1f);
            person2Grid.RecalculateGrid(scrollNormalizedPositionY: 1f);

            person1ChatInputField.ActivateInputField();
        }

        void Update()
        {
            // if the enter key is pressed, send the chat

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (person1ChatInputField.isFocused)
                {
                    PersonSendButton_OnClick(1);
                }
                else if (person2ChatInputField.isFocused)
                {
                    PersonSendButton_OnClick(2);
                }
            }
        }

        public int GetCellCount(EnhancedGrid grid)
        {
            return _chats.Count;
        }

        /// <summary>
		/// Set different cell prefabs based on whether the person using the device sent the chat
		/// </summary>
		/// <param name="grid">There are two grids in this example, so this will be the requesting grid</param>
		/// <param name="dataIndex">Data index of the cell</param>
		/// <returns></returns>
        public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
        {
            // check which grid sent the request and return the appropriate cell prefab

            if (grid == person1Grid ? _chats[dataIndex].fromPersonID == 1 : _chats[dataIndex].fromPersonID == 2)
            {
                return chatFromMeCellPrefab;
            }
            else
            {
                return chatFromOtherPersonCellPrefab;
            }
        }

        public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
        {
            return new CellProperties()
            {
                minSize = new Vector2(chatCellWidth, _chats[dataIndex].cellHeight),
                expansionWeight = 0f
            };
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            (cell as ChatCell).UpdateCell(_chats[dataIndex], _chatTemplateRectTransform.sizeDelta.x + chatCellPadding.left + chatCellPadding.right);
        }

        /// <summary>
		/// Send the chat
		/// </summary>
		/// <param name="personID">Which device is sending</param>
        public void PersonSendButton_OnClick(int personID)
        {
            InputField chatInputField = personID == 1 ? person1ChatInputField : person2ChatInputField;

            _AddChat(personID, chatInputField.text.Trim());

            chatInputField.text = "";

            // recalculate the grids, scrolling to the bottom

            person1Grid.RecalculateGrid(scrollNormalizedPositionY: 1f);
            person2Grid.RecalculateGrid(scrollNormalizedPositionY: 1f);
        }

        private void _AddChat(int personID, string text)
        {
            // turn on the template text UI element and set its text
            chatTemplateText.gameObject.SetActive(true);
            chatTemplateText.text = text;

            // recalculate the size of the text template object
            Canvas.ForceUpdateCanvases();

            // insert the chat into the data at the beginning (since the flow is BottomToTopLeftToRight)
            // the cell's height is based on the calculated value from the template label object
            _chats.Insert(0, new Chat()
            {
                fromPersonID = personID,
                date = DateTime.Now,
                text = text,
                cellHeight = _chatTemplateRectTransform.sizeDelta.y + chatCellPadding.top + chatCellPadding.bottom
            });

            // hide the template label object now that we are done with it
            chatTemplateText.gameObject.SetActive(false);
        }
    }
}