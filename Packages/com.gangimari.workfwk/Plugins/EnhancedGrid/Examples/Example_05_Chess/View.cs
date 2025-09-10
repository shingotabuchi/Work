namespace echo17.EnhancedUI.EnhancedGrid.Example_05
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
	/// A board square
	/// </summary>
    public class View : BasicGridCell
    {
        public Image backgroundImage;
        public Image pieceImage;
        public GameObject kingInCheck;

        public Color boardColor1;
        public Color boardColor2;

        public GameObject moveFrom;
        public Image moveToImage;

        public Color moveColor;
        public Color castleColor;
        public Color attackColor;

        private BoardSquare _boardSquare;
        private Action<int, BoardSquare> _squareSelected;

        public void UpdateCell(int currentPlayerID, BoardSquare boardSquare, string imagePath, Action<int, BoardSquare> squareSelected)
        {
            // board square data
            _boardSquare = boardSquare;

            // delegate to call when the square is selected
            _squareSelected = squareSelected;

            // alternate black and white squares
            if (boardSquare.location.y % 2 == 0)
            {
                backgroundImage.color = (boardSquare.location.x % 2 == 0) ? boardColor2 : boardColor1;
            }
            else
            {
                backgroundImage.color = boardSquare.location.x % 2 == 0 ? boardColor1 : boardColor2;
            }

            // show if the king is in check
            kingInCheck.SetActive(boardSquare.kingInCheck);

            // show the piece if there is one
            pieceImage.gameObject.SetActive(boardSquare.pieceType != PieceType.None);

            // set the piece's image if there is one
            pieceImage.sprite = Resources.Load<Sprite>($"{imagePath}/chess_{Enum.GetName(typeof(PieceType), boardSquare.pieceType).ToLower()}_{boardSquare.playerID}");

            // set up some color coding based on whether the square is selected
            moveFrom.SetActive(boardSquare.moveFromSelected);
            moveToImage.gameObject.SetActive(boardSquare.moveToHighlighted);

            // set up some color coding if the square is one that a piece can move to
            if (boardSquare.moveToHighlighted)
            {
                if (boardSquare.isCastlingMove)
                {
                    moveToImage.color = castleColor;
                }
                else
                {
                    moveToImage.color = (boardSquare.pieceType != PieceType.None && currentPlayerID != boardSquare.playerID) ? attackColor : moveColor;
                }
            }
        }

        /// <summary>
		/// Cell selected, so call the delegate on the controller
		/// </summary>
        public void Button_OnClick()
        {
            if (_squareSelected != null)
            {
                _squareSelected(DataIndex, _boardSquare);
            }
        }
    }
}
