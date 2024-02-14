using System;
using System.Collections.Generic;
using UnityEngine;
using Minimax;

using Position = UnityEngine.Vector2Int;

// The script handles the game's state, including the game board and scoring mechanism.
// The Expand method is used to generate possible moves for a given player,
// while the Score method calculates the score for each potential move in a Minimax algorithm to find the best move.
// this script like other were created through a group effort, i've added my changes explanations
public class State : IState
{
    public List<IState> Expand(IPlayer player)    // Expand creates a list of all states that can be reached in one move
    {
        List<IState> newState = new List<IState>();

        //scan the board and add the new state to the list of possible states
        for (int x = Utility.xMin; x <= Utility.xMax; x++)
        {
            for (int y = Utility.yMin; y <= Utility.yMax; y++)
            {
                //check if the current cell on the board conains a piece belonging to the current player
                if (takeBoard[x, y] == player.Value())
                {
                    //iterate over all the possible positions that this piece can move to
                    foreach (Position pos in boardModel.AllPossiblePosstions(new Position(x, y), false))
                    {
                        //create a new state object as a copy of the current state
                        State stateToAdd = new State(takeBoard);

                        //set the current position on the copied board as empty (remove the piece)
                        stateToAdd.takeBoard[x, y] = Piece.Empty;

                        //set the new position on the copied board with the player's piece
                        stateToAdd.takeBoard[pos.x, pos.y] = player.Value();

                        //add the new state (copied board) to the list of possible states
                        newState.Add(stateToAdd);
                    }
                }
            }
        }
        return newState;
    }

    //this 2D array holds the current state of the game board, with instances of the 'Piece' enum.
    public Piece[,] takeBoard = Utility.MakeMatrix<Piece>(Utility.xMin, Utility.yMin, Utility.xMax, Utility.yMax);

    //this private read-only variable stores an instance of the 'BoardModel' class, presumably implemented as a Singleton pattern.
    private readonly BoardModel boardModel = BoardModel.Instance();

    //constructor to create a new 'State' object by copying the provided game board.
    //the 'board' parameter is a 2D array representing the current state of the game board.
    public State(Piece[,] board)
    {
        //iterate through each cell of 'takeBoard' and copy the correspondig value from 'board' into it,
        //creating a deep copy of the 'board' array into 'takeBoard'.
        for (int x = Utility.xMin; x <= Utility.xMax; x++)
        {
            for (int y = Utility.yMin; y <= Utility.yMax; y++)
            {
                takeBoard[x, y] = board[x, y];
            }
        }
    }


    //calculate the score for Minimax, which looks for the best possible move.
    //the 'player' parameter represents the current player making the move.
    //the 'state' parameter is the current state of the game.
    public int Score(IPlayer player, IState state)
    {
        //create a list to store positions of the current player's pieces on the board.
        List<Position> currentPos = new List<Position>();

        //cast the 'state' parameter to the 'State' class to access its properties and methods.
        State nextState = (State)state;

        //initialize the score to 0.
        int score = 0;

        //loop through each cell of the game board.
        for (int x = Utility.xMin; x <= Utility.xMax; x++)
        {
            for (int y = Utility.yMin; y <= Utility.yMax; y++)
            {
                //check if the cell contains a piece belonging to the current player.
                if (nextState.takeBoard[x, y] == player.Value())
                {
                    //add the position of the current player's piece to the 'currentPos' list.
                    currentPos.Add(new Position(x, y));
                }
            }
        }

        //get the current player's clor (piece type).
        Piece currentPlayerColour = ((Player)player).Value();

        //get the other player's color (opposite piece type).
        Piece otherPlayerColour = PieceInfo.opposites[(int)currentPlayerColour];

        //loop through the positions of the current player's pieces.
        foreach (Position position in currentPos)
        {
            //initialize variables to keep track of win positions and spawn positions.
            int winIndex = 0;
            int spawnIndex = 0;

            //loop through the positions of the current player's win positions on the board.
            foreach (Position winPosition in boardModel.WinningPositions(currentPlayerColour))
            {
                //check if the current player's piece is in a win position.
                if (position == winPosition)
                {
                    //adjust the score based on the win position's index.
                    score -= 1000 - (50 * winIndex);
                    continue;
                }
                winIndex++;
            }

            //loop through the positions of the other player's win positions on the board.
            foreach (Position spawnPosition in boardModel.WinningPositions(otherPlayerColour))
            {
                //check if the current player's piece is in an opposing player's win position (spawn position).
                if (position == spawnPosition)
                {
                    //adjust the score based on the spawn position's index.
                    score += 2000 - (50 * spawnIndex);
                }
                spawnIndex++;
            }

            //calculate score based on the distancefrom the current player's pieces to the win positions.
            foreach (Position targetPosition in boardModel.WinningPositions(currentPlayerColour))
            {
                //skip if the target position is occupied by the current player's piece.
                if (boardModel.GetCurrentPiecePosition(targetPosition) == currentPlayerColour)
                {
                    continue;
                }

                //calculate the distance in points from the current position to the target position.
                Vector2 positionCoordinates = Utility.PositionToCoordinates(position);
                Vector2 targetCoordinates = Utility.PositionToCoordinates(targetPosition);
                int distancePoints = Convert.ToInt32(Vector2.Distance(positionCoordinates, targetCoordinates));
                distancePoints *= 8 * distancePoints;
                score += distancePoints;
            }
        }

        //return the negated score, as Minmax algorithm maximizes the negative scores.
        return -score;
    }
}
