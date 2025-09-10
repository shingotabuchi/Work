namespace echo17.EnhancedUI.EnhancedGrid.Example_05
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using EnhancedUI.Helpers;

    /// Demo showing one way to use a grid for board games. The controller
    /// and the chess logic are separated into separate files for clarity,
    /// both using the partial class of Controller.
	///
    /// This code is not specific to EnhancedGrid and were custom-made
	/// to suit the rules of chess, so comments will be minimal.
	///
    /// Note that this controller stores a 2-dimensional matrix of board data,
	/// unlike the other demos and examples which store a linear set of data.
	/// The _GetXYFromDataIndex method translates a data index into a X and Y
	/// index value.
    public partial class Controller
    {
        private enum GameState
        {
            Waiting,
            PieceSelected,
            PawnPromotionSelection,
            Stalemate,
            Checkmate
        }

        public Text turnLabel;
        public GameObject kingInCheckText;
        public Transform player1CapturedPieces;
        public Transform player2CapturedPieces;
        public GameObject pawnPromotion;
        public GameObject stalemate;
        public GameObject checkmate;
        public string imagePath = "EnhancedGridExamples/Images";

        private BoardSquare[,] _boardSquares;
        private int _currentPlayerID;
        private GameState _gameState;
        private BoardSquare _selectedBoardSquare;
        private BoardSquare _pawnPromotionSquare;

        public void PawnPromotionButton_OnClick(int pieceTypeIndex)
        {
            _pawnPromotionSquare.pieceType = (PieceType)pieceTypeIndex;

            _TogglePawnPromotion(null);

            _RefreshBoard();
        }

        /// <summary>
		/// Translate a data index into an x and y value to look up
		/// in the 2D matrix.
		/// </summary>
		/// <param name="dataIndex">The data index to translate</param>
		/// <returns></returns>
        private Vector2Int _GetXYFromDataIndex(int dataIndex)
        {
            return new Vector2Int(dataIndex % 8, dataIndex / 8);
        }

        private void _RefreshBoard()
        {
            grid.RefreshActiveCells();
        }

        private void _SetupBoard()
        {
            _boardSquares = new BoardSquare[8, 8];

            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    var boardSquare = new BoardSquare()
                    {
                        moveFromSelected = false,
                        moveToHighlighted = false,
                        pieceHasMoved = false,
                        pieceType = PieceType.None,
                        isCastlingMove = false,
                        kingInCheck = false
                    };

                    boardSquare.location = new Vector2Int(x, y);

                    if (y == 0 || y == 1)
                    {
                        boardSquare.playerID = 1;
                    }
                    else if (y == 6 || y == 7)
                    {
                        boardSquare.playerID = 2;
                    }

                    if (y == 1 || y == 6)
                    {
                        boardSquare.pieceType = PieceType.Pawn;
                    }
                    else if (y == 0 || y == 7)
                    {
                        switch (x)
                        {
                            case 0:
                            case 7:

                                boardSquare.pieceType = PieceType.Rook;
                                break;

                            case 1:
                            case 6:

                                boardSquare.pieceType = PieceType.Knight;
                                break;

                            case 2:
                            case 5:

                                boardSquare.pieceType = PieceType.Bishop;
                                break;

                            case 3:

                                boardSquare.pieceType = PieceType.King;
                                break;

                            case 4:

                                boardSquare.pieceType = PieceType.Queen;
                                break;
                        }
                    }

                    _boardSquares[x, y] = boardSquare;
                }
            }

            _currentPlayerID = 1;

            _SetTurnLabel();
        }

        private void _SetTurnLabel()
        {
            if (_currentPlayerID == 1)
            {
                turnLabel.text = "White's turn";
            }
            else
            {
                turnLabel.text = "Black's turn";
            }
        }

        private void _SetSquare(int x, int y, int playerID, bool pieceHasMoved, PieceType pieceType)
        {
            _boardSquares[x, y].playerID = playerID;
            _boardSquares[x, y].pieceHasMoved = pieceHasMoved;
            _boardSquares[x, y].pieceType = pieceType;
        }

        private void _ClearSquare(int x, int y)
        {
            _boardSquares[x, y].playerID = 0;
            _boardSquares[x, y].pieceHasMoved = false;
            _boardSquares[x, y].pieceType = PieceType.None;
        }

        private void _MovePiece(BoardSquare fromSquare, BoardSquare toSquare)
        {
            _SetSquare(toSquare.location.x, toSquare.location.y, fromSquare.playerID, true, fromSquare.pieceType);
            _ClearSquare(fromSquare.location.x, fromSquare.location.y);
        }

        private void _SquareSelected(int dataIndex, BoardSquare boardSquare)
        {
            if (
                    _gameState == GameState.PawnPromotionSelection
                    ||
                    _gameState == GameState.Stalemate
                    ||
                    _gameState == GameState.Checkmate
                )
            {
                return;
            }

            if (boardSquare.pieceType != PieceType.None && boardSquare.playerID == _currentPlayerID)
            {
                for (var x = 0; x < 8; x++)
                {
                    for (var y = 0; y < 8; y++)
                    {
                        _boardSquares[x, y].moveFromSelected = (boardSquare.location.x == x && boardSquare.location.y == y);
                        _boardSquares[x, y].moveToHighlighted = false;
                        _boardSquares[x, y].isCastlingMove = false;
                        _boardSquares[x, y].kingInCheck = false;
                    }
                }

                _SetValidMoves(boardSquare);

                _gameState = GameState.PieceSelected;
                _selectedBoardSquare = boardSquare;

                _RefreshBoard();
            }
            else if (_gameState == GameState.PieceSelected && boardSquare.moveToHighlighted)
            {
                if (_IsOpponent(_currentPlayerID, boardSquare.location.x, boardSquare.location.y))
                {
                    _CapturePiece(boardSquare.playerID, boardSquare.pieceType);
                }

                _MovePiece(_selectedBoardSquare, boardSquare);

                if (boardSquare.isCastlingMove)
                {
                    if (boardSquare.location.x == 1)
                    {
                        _MovePiece(_boardSquares[0, boardSquare.location.y], _boardSquares[2, boardSquare.location.y]);
                    }
                    else
                    {
                        _MovePiece(_boardSquares[7, boardSquare.location.y], _boardSquares[4, boardSquare.location.y]);
                    }
                }

                for (var x = 0; x < 8; x++)
                {
                    for (var y = 0; y < 8; y++)
                    {
                        _boardSquares[x, y].moveFromSelected = false;
                        _boardSquares[x, y].moveToHighlighted = false;
                        _boardSquares[x, y].isCastlingMove = false;
                        _boardSquares[x, y].kingInCheck = false;
                    }
                }

                if (!_CheckPawnPromotion(boardSquare))
                {
                    _SwitchPlayers();
                }

                _selectedBoardSquare = null;

                _RefreshBoard();
            }
        }

        private void _CheckInCheck()
        {
            if (_IsInCheck(_currentPlayerID))
            {
                var kingSquare = _GetKingSquare(_currentPlayerID);

                Debug.Assert(kingSquare != null, $"no king found for player {_currentPlayerID}");

                kingSquare.kingInCheck = true;
                kingInCheckText.SetActive(true);
            }
            else
            {
                kingInCheckText.SetActive(false);
            }
        }

        private bool _CheckEndCondition()
        {
            if (!_AnyPieceCanMove(_currentPlayerID))
            {
                if (_IsInCheck(_currentPlayerID))
                {
                    _SetCheckmate();
                    return true;
                }
                else
                {
                    _SetStalemate();
                    return true;
                }
            }

            return false;
        }

        private bool _AnyPieceCanMove(int playerID)
        {
            EnhancedList<BoardSquare> validMoves = new EnhancedList<BoardSquare>();

            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    if (_boardSquares[x, y].playerID == playerID)
                    {
                        validMoves.Clear();

                        _CheckValidMoves(_boardSquares[x, y], ref validMoves);

                        if (validMoves.Count > 0) return true;
                    }
                }
            }

            return false;
        }

        private void _SetStalemate()
        {
            _gameState = GameState.Stalemate;

            turnLabel.text = "";

            stalemate.SetActive(true);
        }

        private void _SetCheckmate()
        {
            _gameState = GameState.Checkmate;

            turnLabel.text = "";

            var text = "";
            if (_currentPlayerID == 1)
            {
                text = "Checkmate. Black wins!";
            }
            else
            {
                text = "Checkmate. White wins!";
            }

            checkmate.GetComponentInChildren<Text>().text = text;

            checkmate.SetActive(true);
        }

        private void _SetValidMoves(BoardSquare boardSquare)
        {
            EnhancedList<BoardSquare> validMoves = new EnhancedList<BoardSquare>();

            _CheckValidMoves(boardSquare, ref validMoves);

            foreach (var validMove in validMoves)
            {
                validMove.moveToHighlighted = true;
            }
        }

        private void _CheckValidMoves(BoardSquare boardSquare, ref EnhancedList<BoardSquare> validMoves)
        {
            _CheckPiecePotentialMoves(boardSquare, ref validMoves);
            _PruneValidMoves(boardSquare, ref validMoves);
        }

        private void _CheckPiecePotentialMoves(BoardSquare boardSquare, ref EnhancedList<BoardSquare> validMoves)
        {
            var playerID = boardSquare.playerID;
            var pieceType = boardSquare.pieceType;
            var x = boardSquare.location.x;
            var y = boardSquare.location.y;

            if (pieceType == PieceType.Pawn)
            {
                _CheckPawnMoves(playerID, x, y, ref validMoves);
            }
            else if (pieceType == PieceType.Rook)
            {
                _CheckRookMoves(playerID, x, y, ref validMoves);
            }
            else if (pieceType == PieceType.Knight)
            {
                _CheckKnightMoves(playerID, x, y, ref validMoves);
            }
            else if (pieceType == PieceType.Bishop)
            {
                _CheckBishopMoves(playerID, x, y, ref validMoves);
            }
            else if (pieceType == PieceType.Queen)
            {
                _CheckRookMoves(playerID, x, y, ref validMoves);
                _CheckBishopMoves(playerID, x, y, ref validMoves);
            }
            else if (pieceType == PieceType.King)
            {
                _CheckKingMoves(playerID, x, y, ref validMoves);

                var i = 0;
                var kingHasMoved = _boardSquares[x, y].pieceHasMoved;

                _ClearSquare(x, y);

                while (i < validMoves.Count)
                {
                    var checkX = validMoves[i].location.x;
                    var checkY = validMoves[i].location.y;
                    var previousPlayerID = validMoves[i].playerID;
                    var previousPieceHasMoved = validMoves[i].pieceHasMoved;
                    var previousPieceType = validMoves[i].pieceType;

                    _SetSquare(checkX, checkY, playerID, false, PieceType.King);

                    if (_CanBoardSquareBeAttacked(checkX, checkY))
                    {
                        validMoves.RemoveAt(i, ignoreOrder: true);
                    }
                    else
                    {
                        i++;
                    }

                    _SetSquare(checkX, checkY, previousPlayerID, previousPieceHasMoved, previousPieceType);
                }

                _SetSquare(x, y, playerID, kingHasMoved, PieceType.King);
            }
        }

        private void _PruneValidMoves(BoardSquare boardSquare, ref EnhancedList<BoardSquare> validMoves)
        {
            var i = 0;
            var pieceX = boardSquare.location.x;
            var pieceY = boardSquare.location.y;
            var playerID = boardSquare.playerID;
            var pieceHasMoved = boardSquare.pieceHasMoved;
            var pieceType = boardSquare.pieceType;

            _ClearSquare(pieceX, pieceY);

            while (i < validMoves.Count)
            {
                var checkX = validMoves[i].location.x;
                var checkY = validMoves[i].location.y;
                var previousPlayerID = validMoves[i].playerID;
                var previousPieceHasMoved = validMoves[i].pieceHasMoved;
                var previousPieceType = validMoves[i].pieceType;

                _SetSquare(checkX, checkY, playerID, pieceHasMoved, pieceType);

                if (_IsInCheck(_currentPlayerID))
                {
                    validMoves.RemoveAt(i, ignoreOrder: true);
                }
                else
                {
                    i++;
                }

                _SetSquare(checkX, checkY, previousPlayerID, previousPieceHasMoved, previousPieceType);
            }

            _SetSquare(pieceX, pieceY, playerID, pieceHasMoved, pieceType);
        }

        private void _CheckPawnMoves(int playerID, int x, int y, ref EnhancedList<BoardSquare> validMoves)
        {
            if (playerID == 1)
            {
                if (y < 7 && !_IsOccupied(x, y + 1)) validMoves.Add(_boardSquares[x, y + 1]);
                if (y == 1 && !_IsOccupied(x, y + 2)) validMoves.Add(_boardSquares[x, y + 2]);

                if (x > 0 && y < 7 && _IsOpponent(playerID, x - 1, y + 1)) validMoves.Add(_boardSquares[x - 1, y + 1]);
                if (x < 7 && y < 7 && _IsOpponent(playerID, x + 1, y + 1)) validMoves.Add(_boardSquares[x + 1, y + 1]);
            }
            else if (playerID == 2)
            {
                if (y > 0 && !_IsOccupied(x, y - 1)) validMoves.Add(_boardSquares[x, y - 1]);
                if (y == 6 && !_IsOccupied(x, y - 2)) validMoves.Add(_boardSquares[x, y - 2]);

                if (x > 0 && y > 0 && _IsOpponent(playerID, x - 1, y - 1)) validMoves.Add(_boardSquares[x - 1, y - 1]);
                if (x < 7 && y > 0 && _IsOpponent(playerID, x + 1, y - 1)) validMoves.Add(_boardSquares[x + 1, y - 1]);
            }
        }

        private void _CheckRookMoves(int playerID, int x, int y, ref EnhancedList<BoardSquare> validMoves)
        {
            for (var i = x + 1; i < (x + 8); i++)
            {
                if (i > 7) break;

                if (!_IsOccupied(i, y))
                {
                    validMoves.Add(_boardSquares[i, y]);
                }
                else
                {
                    if (_IsOpponent(playerID, i, y))
                    {
                        validMoves.Add(_boardSquares[i, y]);
                    }

                    break;
                }
            }

            for (var i = x - 1; i > (x - 8); i--)
            {
                if (i < 0) break;

                if (!_IsOccupied(i, y))
                {
                    validMoves.Add(_boardSquares[i, y]);
                }
                else
                {
                    if (_IsOpponent(playerID, i, y))
                    {
                        validMoves.Add(_boardSquares[i, y]);
                    }

                    break;
                }
            }

            for (var i = y + 1; i < (y + 8); i++)
            {
                if (i > 7) break;

                if (!_IsOccupied(x, i))
                {
                    validMoves.Add(_boardSquares[x, i]);
                }
                else
                {
                    if (_IsOpponent(playerID, x, i))
                    {
                        validMoves.Add(_boardSquares[x, i]);
                    }

                    break;
                }
            }

            for (var i = y - 1; i > (y - 8); i--)
            {
                if (i < 0) break;

                if (!_IsOccupied(x, i))
                {
                    validMoves.Add(_boardSquares[x, i]);
                }
                else
                {
                    if (_IsOpponent(playerID, x, i))
                    {
                        validMoves.Add(_boardSquares[x, i]);
                    }

                    break;
                }
            }
        }

        private void _CheckKnightMoves(int playerID, int x, int y, ref EnhancedList<BoardSquare> validMoves)
        {
            _CheckKnightMove(playerID, x - 1, y - 2, ref validMoves);
            _CheckKnightMove(playerID, x + 1, y - 2, ref validMoves);
            _CheckKnightMove(playerID, x - 1, y + 2, ref validMoves);
            _CheckKnightMove(playerID, x + 1, y + 2, ref validMoves);

            _CheckKnightMove(playerID, x - 2, y - 1, ref validMoves);
            _CheckKnightMove(playerID, x + 2, y - 1, ref validMoves);
            _CheckKnightMove(playerID, x - 2, y + 1, ref validMoves);
            _CheckKnightMove(playerID, x + 2, y + 1, ref validMoves);
        }

        private void _CheckKnightMove(int playerID, int x, int y, ref EnhancedList<BoardSquare> validMoves)
        {
            if (!_InBounds(x, y)) return;

            if (!_IsOccupied(x, y) || _IsOpponent(playerID, x, y))
            {
                validMoves.Add(_boardSquares[x, y]);
            }
        }

        private void _CheckBishopMoves(int playerID, int x, int y, ref EnhancedList<BoardSquare> validMoves)
        {
            _CheckBishopMove(playerID, x, y, -1, -1, ref validMoves);
            _CheckBishopMove(playerID, x, y, 1, -1, ref validMoves);
            _CheckBishopMove(playerID, x, y, -1, 1, ref validMoves);
            _CheckBishopMove(playerID, x, y, 1, 1, ref validMoves);
        }

        private void _CheckBishopMove(int playerID, int x, int y, int xMultiplier, int yMultiplier, ref EnhancedList<BoardSquare> validMoves)
        {
            for (var i = 1; i < 8; i++)
            {
                var x2 = i * xMultiplier;
                var y2 = i * yMultiplier;

                if (!_InBounds(x + x2, y + y2)) break;

                if (!_IsOccupied(x + x2, y + y2))
                {
                    validMoves.Add(_boardSquares[x + x2, y + y2]);
                }
                else
                {
                    if (_IsOpponent(playerID, x + x2, y + y2))
                    {
                        validMoves.Add(_boardSquares[x + x2, y + y2]);
                    }

                    break;
                }
            }
        }

        private void _CheckKingMoves(int playerID, int x, int y, ref EnhancedList<BoardSquare> validMoves)
        {
            for (var i = x - 1; i <= x + 1; i++)
            {
                for (var j = y - 1; j <= y + 1; j++)
                {
                    if (_InBounds(i, j))
                    {
                        if (_IsOpponent(playerID, i, j) || !_IsOccupied(i, j))
                        {
                            validMoves.Add(_boardSquares[i, j]);
                        }
                    }
                }
            }

            var row = (playerID == 1) ? 0 : 7;

            if (_boardSquares[3, row].pieceType == PieceType.King && _boardSquares[3, row].pieceHasMoved == false)
            {
                _CheckCastlingMovesRook(playerID, row, 0, ref validMoves);
                _CheckCastlingMovesRook(playerID, row, 7, ref validMoves);
            }
        }

        private void _CheckCastlingMovesRook(int playerID, int row, int rookPosition, ref EnhancedList<BoardSquare> validMoves)
        {
            var pathStart = rookPosition == 0 ? 1 : 4;
            var pathEnd = rookPosition == 0 ? 2 : 6;
            var finalPosition = rookPosition == 0 ? 1 : 5;

            if (_boardSquares[rookPosition, row].pieceType == PieceType.Rook && _boardSquares[rookPosition, row].pieceHasMoved == false)
            {
                var safePath = true;

                for (var i = pathStart; i <= pathEnd; i++)
                {
                    if (_IsOccupied(i, row))
                    {
                        safePath = false;
                        break;
                    }
                    else
                    {
                        _SetSquare(i, row, playerID, false, PieceType.King);

                        if (_CanBoardSquareBeAttacked(i, row))
                        {
                            safePath = false;

                            _ClearSquare(i, row);

                            break;
                        }

                        _ClearSquare(i, row);
                    }
                }

                if (safePath)
                {
                    _boardSquares[finalPosition, row].isCastlingMove = true;
                    validMoves.Add(_boardSquares[finalPosition, row]);
                }
            }
        }

        private bool _IsInCheck(int playerID)
        {
            var kingSquare = _GetKingSquare(playerID);

            Debug.Assert(kingSquare != null, $"king not found for player {playerID}");

            if (_CanBoardSquareBeAttacked(kingSquare.location.x, kingSquare.location.y)) return true;

            return false;
        }

        private BoardSquare _GetKingSquare(int playerID)
        {
            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    if (_boardSquares[x, y].playerID == playerID && _boardSquares[x, y].pieceType == PieceType.King)
                    {
                        return _boardSquares[x, y];
                    }
                }
            }

            return null;
        }

        private bool _CanBoardSquareBeAttacked(int checkX, int checkY)
        {
            if (!_IsOccupied(checkX, checkY)) return false;

            var victimPlayerID = _boardSquares[checkX, checkY].playerID;

            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    if (_IsOpponent(victimPlayerID, x, y) && _boardSquares[x, y].pieceType != PieceType.King)
                    {
                        var validMoves = new EnhancedList<BoardSquare>();

                        _CheckPiecePotentialMoves(_boardSquares[x, y], ref validMoves);

                        if (_ValidMovesContainLocation(ref validMoves, checkX, checkY))
                        {
                            return true;
                        }
                    }
                }
            }

            var kingSquare = _GetKingSquare(_OtherPlayerID(victimPlayerID));

            Debug.Assert(kingSquare != null, $"No king found for player {_OtherPlayerID(victimPlayerID)}");

            return (Mathf.Abs(kingSquare.location.x - checkX) <= 1 && Mathf.Abs(kingSquare.location.y - checkY) <= 1);
        }

        private bool _IsOccupied(int x, int y)
        {
            if (!_InBounds(x, y)) return false;

            return _boardSquares[x, y].pieceType != PieceType.None;
        }

        private bool _IsOpponent(int playerID, int x, int y)
        {
            if (!_InBounds(x, y)) return false;

            return _IsOccupied(x, y) && _boardSquares[x, y].playerID != playerID;
        }

        private int _OtherPlayerID(int playerID)
        {
            return 3 - playerID;
        }

        private bool _InBounds(int x, int y)
        {
            return (x >= 0 && x <= 7 && y >= 0 && y <= 7);
        }

        private bool _ValidMovesContainLocation(ref EnhancedList<BoardSquare> validMoves, int x, int y)
        {
            foreach (var validMove in validMoves)
            {
                if (validMove.location.x == x && validMove.location.y == y) return true;
            }

            return false;
        }

        private void _CapturePiece(int playerID, PieceType pieceType)
        {
            var parent = (playerID == 1 ? player2CapturedPieces : player1CapturedPieces);

            var go = new GameObject(Enum.GetName(typeof(PieceType), pieceType));
            go.transform.SetParent(parent);

            var image = go.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>($"{imagePath}/chess_{Enum.GetName(typeof(PieceType), pieceType).ToLower()}_{playerID}");
        }

        private bool _CheckPawnPromotion(BoardSquare boardSquare)
        {
            if (boardSquare.pieceType == PieceType.Pawn)
            {
                if (
                        (boardSquare.playerID == 1 && boardSquare.location.y == 7)
                        ||
                        (boardSquare.playerID == 2 && boardSquare.location.y == 0)
                    )
                {
                    _TogglePawnPromotion(boardSquare);

                    return true;
                }
            }

            return false;
        }

        private void _TogglePawnPromotion(BoardSquare boardSquare)
        {
            _pawnPromotionSquare = boardSquare;

            if (boardSquare != null)
            {
                pawnPromotion.SetActive(true);

                _gameState = GameState.PawnPromotionSelection;
            }
            else
            {
                pawnPromotion.SetActive(false);

                _SwitchPlayers();
            }
        }

        private void _SwitchPlayers()
        {
            _currentPlayerID = _OtherPlayerID(_currentPlayerID);
            _gameState = GameState.Waiting;

            _SetTurnLabel();

            _CheckInCheck();

            _CheckEndCondition();
        }
    }
}