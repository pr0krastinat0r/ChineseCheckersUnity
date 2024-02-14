using System;
using System.Collections.Generic;
using Position = UnityEngine.Vector2Int;
// The interface for using Minimax
// It has its own namespace to be reusable

namespace Minimax 
{
    public interface IState
    {
        List<IState> Expand(IPlayer player);  // Generate a list of the states reachable from the current state by player
        int Score(IPlayer player, IState state);      // The heuristic score for player in the current state
    }

    public interface IPlayer  // The IPlayer interface is just for identity, does not require any methods
    {
        Piece Value();
    }

    //The method Select is a recursive function that evaluates possible game states and chooses the move that maximizes the AI player's score while minimizing the opponent's score.
    //It considers different game states at a specified depth and returns the state with the best move based on the evaluation function Score for the given player.
    //The algorithm allows the AI to make strategic decisions in turn-based chinese checkers and helps find an optimal solution within a limited search depth. Note: This script is pretty much a group effort, so I commented each step myself.


    public class MiniMax  // Method to select the best move using the MiniMax algorithm

    {
        public static IState Select(IState state, IPlayer otherPlayer, List<Player> playersList, int depth, bool maximising)
        {
            //variable to store the piece of the current player
            Piece currentPlayerPiece = new Piece();
            //variable to store the current value of the state
            int currentValue;

            //check if the search depth is reached or if the state is a terminal state
            if (depth == 0 || state.Score(otherPlayer, state) == Int32.MaxValue || state.Score(otherPlayer, state) == Int32.MinValue)
                return state;

            IState childState;
            IState nextState;

            if (maximising) // AI move
            {
                currentPlayerPiece = otherPlayer.Value(); //get the piece of the current player
                currentValue = Int32.MinValue;  //initialize the current value as the lowest possible value
                nextState = null; //initialize the next state as null

                 
                List<IState> childStates = state.Expand(otherPlayer); //find all possible moves the player can have

                if (childStates.Count == 0) //if no child states are available, return the current state
                    return state;

                foreach (IState s in childStates) //for each found state, choose the move that will give the highest score
                {
                    childState = Select(s, otherPlayer, playersList, depth - 1, false); 
   
                    if (childState != null && childState.Score(otherPlayer, childState) > currentValue)  //check if this move is better than any previous and update the nextState and currentValue
                    {
                        nextState = s;
                        currentValue = childState.Score(otherPlayer, childState);
                    }
                }
            }
            else //the opponent's move (similar to above, but choosing the lowest score for the player)
            {
                nextState = null; //initialize the next state as null
                currentValue = Int32.MaxValue;  //initialize the current value as the highest possible value


                List<IState> childStates = state.Expand(otherPlayer); //find all possible moves the player can have


                if (childStates.Count == 0) //if no child states are available, return the current state

                    return state;

                Player nextPlayer = new Player();
                for (int i = 0; i < playersList.Count; i++)  //find the next player in the list of players

                {
                    if (playersList[i].Value() == otherPlayer.Value() && i != playersList.Count - 1)
                    {
                        nextPlayer = playersList[i + 1];
                        break;
                    }
                    else if (playersList[i].Value() == otherPlayer.Value() && i == playersList.Count - 1)
                    {
                        nextPlayer = playersList[0];
                        break;
                    }
                }

                foreach (IState s in childStates)
                {
                    if (nextPlayer.Value() == currentPlayerPiece) //determine whether the next player is the current player's piece

                        childState = Select(s, nextPlayer, playersList, depth - 1, true);
                    else
                        childState = Select(s, nextPlayer, playersList, depth - 1, false);

                    if (childState != null && childState.Score(otherPlayer, childState) < currentValue)  //check if this move is better than any previous and update the nextState and currentValue

                    {
                        nextState = s;
                        currentValue = childState.Score(otherPlayer, childState);
                    }
                }
            }
            return nextState; //return the next state with the best move
        }
    }

}