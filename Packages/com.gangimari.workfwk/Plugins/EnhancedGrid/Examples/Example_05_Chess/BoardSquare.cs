namespace echo17.EnhancedUI.EnhancedGrid.Example_05
{
    using UnityEngine;

    public enum PieceType
    {
        None = 0,
        Pawn = 1,
        Rook = 2,
        Knight = 3,
        Bishop = 4,
        Queen = 5,
        King = 6
    }

    /// <summary>
	/// One square of the board
	/// </summary>
    public class BoardSquare
    {
        /// <summary>
		/// x, y location
		/// </summary>
        public Vector2Int location;

        /// <summary>
		/// Which player owns the square
		/// </summary>
        public int playerID;

        /// <summary>
		/// Whether the piece has moved.
		/// This is useful for castling.
		/// </summary>
        public bool pieceHasMoved;

        /// <summary>
		/// The type of piece in the square
		/// </summary>
        public PieceType pieceType;


        // The values below are just to change visible aspects of the cell view

        public bool moveFromSelected;
        public bool moveToHighlighted;
        public bool isCastlingMove;
        public bool kingInCheck;
    }
}
