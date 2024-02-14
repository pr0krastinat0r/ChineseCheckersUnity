﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Position = UnityEngine.Vector2Int;
using Coordinate = UnityEngine.Vector2;

public class BoardView : MonoBehaviour, IBoardListener
{
    [SerializeField]
    private GameObject boardObject;  // Set in Inspector, this is the visual representation of the board
    [SerializeField]
    private GameObject[] piecePrefab = new GameObject[(int)Piece.ColourMax]; // Set in Inspector, keeps track of piece prototypes
    [SerializeField]
    private Text currentPlayerText;  // Set in Inspector, shows whose turn it is
    [SerializeField]
    private Text winnersText;        // Set in Inspector, shows what players have already won

    private readonly IBoardModel boardModel = BoardModel.Instance();

    public const string pieceTag = "Ball";  // All piece prefabs are tagged

    // Keep track of tile representations on board
    // We create a matrix [-xMin: xMax, -yMin: yMax].
    private readonly GameObject[,] board = Utility.MakeMatrix<GameObject>(Utility.xMin, Utility.yMin, Utility.xMax, Utility.yMax);

    // NewGame is required by IBoardListener
    // When a new game is started we remove all pieces;
    // setting up a new game is assumed to be done with PlacePiece.
    public void NewGame(List<Player> players)
    {
        // Remove all pieces from the board.  
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(pieceTag))
            Destroy(go);
    }

    // Add player to the list of winners.
    public void SetNewWinner(Player player)
    {
        winnersText.text += PieceInfo.pieceNames[(int)player.Value()];
    }

    // Place piece at position (pos.x, pos.y)
    public void PlacePiece(Position pos, Piece piece)
    {

        // Convert a position to coordinates.
        Coordinate coords = Utility.PositionToCoordinates(pos);

        // A prefab representing the piece is instantiated at the given coordinates as a subobject of the board.
        if (piecePrefab[(int)piece])
            board[pos.x, pos.y] = Instantiate(piecePrefab[(int)piece], new Vector3(coords.x, coords.y, Utility.pieceLevel), Quaternion.identity, boardObject.transform);
    }

    // Move the piece at position (startPos.x, startPos.y) to (endPos.x, endPos.y)
    // Note that the move will go straight from the start position to the end position
    // even if it logically consists of multiple jumps.  This could be animated by supplying a list of positions instead,
    // moving through these in turn.  This would require MovePiece to be a coroutine,
    // but that would be a fairly simple change to make.

    public void MovePiece(Position startPos, Position endPos)
    {
        //convert the end position to coordinates in the game world
        Coordinate endCoords = Utility.PositionToCoordinates(endPos);

        //move the piece from the start position to the end position in the game world
        board[startPos.x, startPos.y].transform.position = new Vector3(endCoords.x, endCoords.y, Utility.pieceLevel);

        //update the board to reflect the moved piece
        board[endPos.x, endPos.y] = board[startPos.x, startPos.y];
        board[startPos.x, startPos.y] = null;
    }

    public void Start()
    {
        //make the view a listener of the BoardModel
        boardModel.AddListener(this);
    }

    public GameObject GetPiece(Position pos)
    {
        //return the game object (piece) at the given position on the board
        return board[pos.x, pos.y];
    }

    //indicate whose turn it is by updating the UI text
    public void SetCurrentPlayer(Player player)
    {
        int playerValue = (int)player.Value();

        //check if the player's value is within the pieceNames array bounds
        if (playerValue >= 0 && playerValue < PieceInfo.pieceNames.Length)
        {
            //update the currentPlayerText to display the current player's name
            currentPlayerText.text = $"{PieceInfo.pieceNames[playerValue]}";
        }
    }

}
