using System.Collections.Generic;
using UnityEngine;
using Minimax;
using Position = UnityEngine.Vector2Int;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


// BoardModel has the responsibility of keeping track of the current state of the play, know the rules of the game and
// send updates to all listeners when the state changes.

//Specifically, The methods are used un this script exist for various functionalities like game state management, player moves, board validation, and game win conditions.
//An alternative for some methods might be using different algorithms for move validation, game win check, or board management, depending on the specific game's requirements and design preferences.
//For example, the Minimax algorithm in MakeMove could be replaced with other AI algorithms like Alpha-Beta Pruning or Monte Carlo Tree Search for improved performance and better AI decision-making.
//Additionally, game state management could be implemented using different design patterns, and board validation could be optimized based on the game rules.
//Overall, the alternatives depend on the specific requirements and design choices of the game being developed.I've basically worked in a group and at this point its very hard to put my finger on who did exactly what,
//I've tried commenting as much as possible to show that I understand everything.
public class BoardModel : IBoardModel
{
    // the BoardModel class represents the model of the game board and implements core game functionalities. 
    // it follows the singleton design pattern to ensure only one instance of the class is created

    private static BoardModel instance; // the singleton instance of the BoardModel class
    private List<IBoardListener> listenerslist = new List<IBoardListener>(); // list of board listeners to notify about changes on the board
    private Piece[,] board = Utility.MakeMatrix<Piece>(Utility.xMin, Utility.yMin, Utility.xMax, Utility.yMax); // the game board represented as a 2D array of pieces
    private List<Player> listOfPlayers = new List<Player>(); // list of players participating in the game

    private int playerAmount; // the number of players in the game
    private int playerToMove; // the index of the current player whose turn it is
    private int difficulty; // the difficulty level of the game
    private bool gameOver; // flag indicating if the game is over

    public List<int> playerNumList = new List<int>(); // a list that keeps track of which player's turn it is and will be updated during the game
    public List<Position> legalMoves = new List<Position>(); // a list of potential moves for the current player

    private BoardModel() { } // private constructor to enforce the singleton pattern

    public static BoardModel Instance() // method to get the singleton instance of the BoardModel class
    {
        if (instance == null)
            instance = new BoardModel();

        return instance;
    }

    public void StartGame(int numPlayers) // method to start a new game with the specified number of players. it initializes the game state, players, and the game board
    {
        playerAmount = numPlayers; // store the number of players in the local variable

        RestartGame(); // reset the game board and other variables
        NewPlayersForTheGame(playerAmount); // create new players for the game
        SetNewGame(); // set up the initial game state
        SetUpGameBoard(); // set up the game board
        SetDifficulty(2); // set the game difficulty to 2 (or any default value)
    }


    private void NewPlayersForTheGame(int players)
    {


        for (int i = 0; i < players; i++)// loop which adds the numbers of players in the list with a special piece color
        {
            listOfPlayers.Add(new Player(PieceInfo.setups[players][i], i == 0)); //!!IMPORTANT!! switch between bot vs bot (false), human vs human (true) and human vs bot (i == 0)
        }
    }

    private void SetAmountOfPlayers() // this method sets up the number of players on the list
    {


        foreach (Player player in listOfPlayers)
        {
            playerNumList.Add((int)player.Value());
        }
    }


    //this GetBoard() method is a getter function, a 2D array reflecting the game boards current state, is returned by the GetBoard method,
    //which is a getter function. By using this technique, the game board can be accessed by other portions of
    //the code for scoring or presentation purposes.

    public Piece[,] GetBoard()  //return the game board used for the scoring
    {
        return board;
    }

    private void RestartGame()  //method used to completely restart the game
    {
        gameOver = false;   // resets the variable to false, indicating that the game is not over.
        playerNumList.Clear(); // clears the list with amount of players, which stores the number of players for scoring purposes.
        listOfPlayers.Clear();         // clears the list list of players, which chontains the players participating in the game.
        InvalidPositions();           // calls the method InvalidPartOfTheBoard(), which sets up the invalid part of the game board, possibly to mark areas that are not accessible during gameplay.
    }

    private void SetNewGame() //set the new game
    {
        foreach (IBoardListener listener in listenerslist) //this loop iterates through each element (listener) in the listeners list. The listeners are objects that implement the inteface, allowing them to respond to changes on the game board
        {
            listener.NewGame(listOfPlayers); //call the NewGame method for each listener, passing the players list
        }
    }


    //i've added a new variable boardSize to calculate the size of the game board based on Utility.xMin and Utility.xMax. Since the game board is square, we only need to calculate the size once.
    //the loops have been adjusted to iterate from 0 to boardSize - 1 for both x and y.This ensures that we cover the valid range of coordinates on the game board.
    //the gameBoard elements are updated by adding Utility.xMin ad Utility.yMin to the loop indices x and y, respectively, to properly access the correct positions on the game board.
    private void InvalidPositions() //set invalid part of the board
    {
        //int boardSize = Utility.xMax - Utility.xMin + 1; //calculate the size of the game board

        for (int x = Utility.xMin; x <= Utility.xMax; x++) // iterate from -8 to 8
        {
            for (int y = Utility.yMin; y <= Utility.yMax; y++)
            {
                board[x,y] = Piece.Invalid; //set invalid places on the board
            }
        }
    }


    private void EmptyPlacesOnTheBoard()  // Empty places on the board for the spawn pieces theere
    {

        for (int i = 0; i < Utility.playerStartPos.Length; i++) // Going all piec start positions 
        {
            foreach (Position position in Utility.playerStartPos[i]) // Set posiion in all start positions 
            {
                board[position.x, position.y] = Piece.Empty;  // Set all start positions to empty
            }
        }
    }


    public void SetUpGameBoard() // Set Up the game board with the all piece for current game 
    {
        Piece[] pieceSetUps = PieceInfo.setups[playerAmount];  // Which piece color will spawn in the game 

        EmptyPlacesOnTheBoard();//Method whit set empty spaaces on the board 
        SetAmountOfPlayers(); //Create a new players for the game which will be add in the List <Int>



        for (int i = 0; i < playerAmount; i++)// going amount of the plaers 
        {
            for (int j = 0; j < PieceInfo.numPieces[playerAmount]; j++)  // how many piece will be set ups on the board
            {
                board[Utility.playerStartPos[playerNumList[i]][j][0], Utility.playerStartPos[playerNumList[i]][j][1]] = pieceSetUps[i]; // instantiate pieces on the board
                SetPiece(Utility.playerStartPos[playerNumList[i]][j], pieceSetUps[i]); // Method which set pieces on the board.

            }
        }

    }

    // this method checks if a player will try to put a piece out of the board
    private bool CheckIfOutOfBound(Position posCheck)
    {
        // return true if any of the following conditions are true, indicating that the position is out of bounds:

        return Utility.xMin > posCheck.x //the x-coordinate (posCheck.x) is less than the minimum x-coordinate (Utiliy.xMin)
            || Utility.xMax < posCheck.x //the x-coordinate (posCheck.x) is greater than the maximum x-coordinate (Utility.xMax)
            || Utility.yMin > posCheck.y //the y-coordinate (posCheck.y) is les than the minimum y-coordinate (Utility.yMin)
            || Utility.yMax < posCheck.y; //the y-coordinate (posCheck.y) is greater than the maximum y-coordinate (Utility.yMax)
    }

    //in summary, this method explores all possible directions from the startPos position and finds valid moves for the player
    // this method looks for all possible moves for the player starting from a given position
    public List<Position> AllPossiblePosstions(Position startPos, bool isHuman)
    {
        legalMoves.Clear(); // clear the list of legal moves before finding new ones

        // loop through all possible directions from the current position to explore possible moves
        for (int i = 0; i < Utility.legalDir.Length; i++)
        {
            Position checkPos = startPos + Utility.legalDir[i]; // calculate the new position where the player can choose to move

            // check if the new position is out of the board, if so, skip it and coninue with the next direction
            if (CheckIfOutOfBound(checkPos))
                continue;

            // if the new position is empty and not invalid, it's a valid move, so add it to the list of legalMoves
            if (board[checkPos.x, checkPos.y] == Piece.Empty
                && board[checkPos.x, checkPos.y] != Piece.Invalid)
            {
                legalMoves.Add(checkPos);
            }
            // if the new position is not empty and not invalid, check if the piece can jump over it
            else if (board[checkPos.x, checkPos.y] != Piece.Empty
                     && board[checkPos.x, checkPos.y] != Piece.Invalid)
            {
                JumpOver(startPos, i, isHuman); // call the JumpOver method to handle the possibility of jumping over pieces
            }
        }

        return legalMoves; // return the list containing all new possible moves
    }


    // calculate the new position where the player can choose to move (two steps ahead in the direction indicated by bounceIndex)
    private void JumpOver(Position startPos, int bounceIndex, bool isHuman)
    {
        Position currentNewPos = (startPos + 2 * (Utility.legalDir[bounceIndex]));

        // check if the new position is already in the list of legal moves, if so, return (no need to process it again)
        if (legalMoves.Contains(currentNewPos))
            return;

        // check if the new position is out of the board, if so, return (the piece cannot jump outside the board)
        if (CheckIfOutOfBound(currentNewPos))
            return;

        // if the new position is empty, the piece can be placed there
        if (board[currentNewPos.x, currentNewPos.y] == Piece.Empty)
        {
            legalMoves.Add(currentNewPos); // add the new position to the list of legal moves
            JumpMoreThanOneTime(currentNewPos, isHuman); // check if the piece can jump more than once from the new position
        }
        return;
    }




    // method that checks if the player can move piece again
    private void JumpMoreThanOneTime(Position currentNewPos, bool isHuman)
    {
        // loop through all possible directions to check if the piece can jump more than once
        for (int i = 0; i < Utility.legalDir.Length; i++)
        {
            // check if the piece will jump more than once and the new position is not outside of the board
            if (!CheckIfOutOfBound(new Position((currentNewPos.x + Utility.legalDir[i].x), (currentNewPos.y + Utility.legalDir[i].y))))
            {
                // check if the new position is not empty or invalid
                if (board[(currentNewPos + Utility.legalDir[i]).x, (currentNewPos + Utility.legalDir[i]).y] != Piece.Empty
                && board[(currentNewPos + Utility.legalDir[i]).x, (currentNewPos + Utility.legalDir[i]).y] != Piece.Invalid)
                {
                    JumpOver(currentNewPos, i, isHuman); // call the JumpOver method to handle the possibility of jumping again
                }
            }
        }
    }


    // check if the next position is on the board; if not, return false
    private bool CheckIfPossible(Position endPos)
    {
        // iterate through the list of legalMoves to check if the endPos is present and if the position is empty on the board
        foreach (Position position in legalMoves)
        {
            if (endPos == position && board[endPos.x, endPos.y] == Piece.Empty)
            {
                return true; // if the endPos is present and empty on the board, return true
            }
        }

        return false; // if the endPos is not found or not empty on the board, return false
    }

    // method to add a listener to the board
    public void AddListener(IBoardListener listener)
    {
        this.listenerslist.Add(listener);
    }

    // method to set the difficulty for the game
    public void SetDifficulty(int difficulty)
    {
        this.difficulty = difficulty;
    }

    // method to set a piece on the board at the specified position if possible
    public bool SetPiece(Position pos, Piece piece)
    {
        // Placing the piece on the board by notifying all board listeners
        foreach (IBoardListener listener in listenerslist)
        {
            listener.PlacePiece(pos, piece);
        }

        return false; // placeholder return value, needs to be updated as per the actual implementation
    }

    // method to get the piece at the specified position on the board
    public Piece GetPiece(Position pos)
    {
        legalMoves.Clear(); // clear the list of all possibe moves

        AllPossiblePosstions(pos, true); // calculate all possible moves for the player from the given position

        return board[pos.x, pos.y]; // return the piece at the specified position on the board
    }

    // method to retrieve the current piece position on the board for the given position
    public Piece GetCurrentPiecePosition(Position position)
    {
        return board[position.x, position.y]; // return the piece at the specified position on the board
    }

    // method to move a piece from the starting position (startPos) to the ending position (endPos) if possible
    public bool MovePiece(Position startPos, Position endPos)
    {
        if (gameOver) // check if the game is over, and if so, players cannot make any more moves
            return false;

        if (CheckIfOutOfBound(endPos)) // check if the ending position is out of bounds
            return false;

        if (board[endPos.x, endPos.y] == Piece.Invalid || endPos == startPos) // check if the ending positiion is invalid or the same as the starting position
            return false;

        if (CheckIfPossible(endPos)) // check if the move to the ending position is possible and valid
        {
            // Move the piece from the starting position to the ending position on the board
            board[endPos.x, endPos.y] = board[startPos.x, startPos.y];
            board[startPos.x, startPos.y] = Piece.Empty;

            // Notify all board listeners about the move by calling their MovePiece method
            foreach (IBoardListener listener in listenerslist)
            {
                listener.MovePiece(startPos, endPos);
            }

            // Check win condition after the move
            winCondition(board[endPos.x, endPos.y]);

            legalMoves.Clear(); // clear the list of possible moves
            return true; // move was successful
        }
        else
        {
            legalMoves.Clear(); // clear the list of possible moves
            return false; // move is not possible
        }
    }


    // method to make a moe for a player at the specified index
    public void MakeMove(Player player, int index)
    {
        if (gameOver) // check if the game is over, and if so, players cannot make any more moves
            return;

        playerToMove = index; // set the current player to the given index (used for win check and setting the win player correctly)

        Player otherPlayers = listOfPlayers[index]; // get the player at the speciied index from the list of players

        // initialize start and end positions for moving a piece
        Position startPos = new Position(100, 100);
        Position endPos = new Position(100, 100);

        // create a new state with a copy of the game board to use in Minimax
        State state = new State(board);

        if (winCondition(player.Value())) // check if the current player has won the game
            return;

        // use Minimax to select the next move (new state)
        State newState = (State)MiniMax.Select(state, otherPlayers, listOfPlayers, difficulty, true);

        // loop through the entire board to find the start and end positions for moving a piece
        for (int x = Utility.xMin; x <= Utility.xMax; x++) // iterate from -8 to 8
        {
            for (int y = Utility.yMin; y <= Utility.yMax; y++) // iterate from -8 to 8
            {
                // check player's position on the current board and update the new end position
                if (board[x, y] != player.Value() && newState.takeBoard[x, y] == player.Value())
                {
                    endPos = new Position(x, y);
                }
                // check player's position on the current board and update the new start position
                if (board[x, y] == player.Value() && newState.takeBoard[x, y] != player.Value())
                {
                    startPos = new Position(x, y);
                }
            }
        }

        // calculate all possible positions for moving the piece from the start position (no need to update the legalMoves list)
        AllPossiblePosstions(startPos, false);

        // move the piece from the start position to the end position
        MovePiece(startPos, endPos);
    }



    //This part to my knowledge was made by Bella, all props to her, I have tried to comment as much as possible to show that I understand how it works.

    //These methods are a part of a system for saving and loading the game. The LoadGame() function de-serializes this data from the binary file to restore the game state when loading a saved game.
    //The SaveGame() method serializes pertinent gae data (board state, player turn, and player amount) into a binary file.
    // Save game method
    public void SaveGame()
    {
        // create a 2D integer array (_saveBoard) using the Utility.MakeMatrix method to save the game state.
        int[,] _saveBoard = Utility.MakeMatrix<int>(Utility.xMin, Utility.yMin, Utility.xMax, Utility.yMax);

        // iterate through each position on the board and save its corresponding value in _saveBoard.
        for (int i = Utility.xMin; i <= Utility.xMax; i++)
        {
            for (int j = Utility.yMin; j <= Utility.yMax; j++)
            {
                _saveBoard[i, j] = (int)board[i, j];
            }
        }

        // define the file path where the game data will be saved.
        string path = Application.persistentDataPath + "/SaveGameChinaChess.dat";

        // create a BinaryFormatter to serialize the data.
        BinaryFormatter formatter = new BinaryFormatter();

        // create a FileStream to write the serialized data to the specified file.
        FileStream stream = File.Create(path);

        // serialize and save the game board (_saveBoard), the playerToMove variable, and the playerAount variable to the file.
        formatter.Serialize(stream, _saveBoard);
        formatter.Serialize(stream, playerToMove);
        formatter.Serialize(stream, playerAmount);

        // close the FileStream after saving the data.
        stream.Close();
    }

    // Load game method
    public void LoadGame()
    {
        // call the RestartGame method to reset the gae before starting to load.
        RestartGame();

        // define the file path from where the game data will be loaded.
        string path = Application.persistentDataPath + "/SaveGameChinaChess.dat";

        // check if the file exists at the given path.
        if (File.Exists(path))
        {
            // create a BinaryFormatter to deserialize the data.
            BinaryFormatter formatter = new BinaryFormatter();

            // create a FileStream to read the serialized data from the file.
            FileStream stream = new FileStream(path, FileMode.Open);

            // deserialize the data from the file and store it in the intBoardToSave array,
            // playerToMove variable, and playerAmount variable.
            int[,] intBoardToSave = (int[,])formatter.Deserialize(stream);
            playerToMove = (int)formatter.Deserialize(stream);
            playerAmount = (int)formatter.Deserialize(stream);

            // copy the values from intBoardToSave back to the main board (board).
            for (int i = Utility.xMin; i <= Utility.xMax; i++)
            {
                for (int j = Utility.yMin; j <= Utility.yMax; j++)
                {
                    board[i, j] = (Piece)intBoardToSave[i, j];
                }
            }

            // notify listeners that a new game has started.
            foreach (IBoardListener listener in listenerslist)
            {
                listener.NewGame(listOfPlayers);

                // place the pieces on the board based on the loaded data.
                for (int x = Utility.xMin; x <= Utility.xMax; x++)
                {
                    for (int y = Utility.yMin; y <= Utility.yMax; y++)
                    {
                        if (board[x, y] != Piece.Invalid && board[x, y] != Piece.Empty)
                        {
                            Position pos = new Position(x, y);
                            listener.PlacePiece(pos, board[x, y]);
                        }
                    }
                }
            }

            // create new players based on the loaded playerAmount value.
            NewPlayersForTheGame(playerAmount);

            // close the FileStream after loading the data.
            stream.Close();
        }
    }


    // method to check if a player has won the game
    public bool winCondition(Piece playerColourPiece)
    {
        List<Position> oppositePositions = new List<Position>(); // list with the opposite positions 
        int counter = 0; // counter to keep track of how many pieces are in win posiion for the player

        int oppsiteIndex = (int)PieceInfo.opposites[(int)playerColourPiece]; // get the index of the opposite side for the player's color

        // set opposite positions to the list based on the player's color
        for (int i = 0; i < PieceInfo.numPieces[playerAmount]; i++)
        {
            oppositePositions.Add(Utility.playerStartPos[oppsiteIndex][i]);
        }

        // check if the player's pieces are in win positions
        foreach (Position pos in oppositePositions)
        {
            if (board[pos.x, pos.y] == playerColourPiece)
            {
                counter++;
            }
        }

        // If the number of player's pieces in win positions is qual to the total number of win positions for that player,
        // then the player has won the game
        if ((counter) == WinningPositions(playerColourPiece).Count)
        {
            // notify all board listeners that the current player is the winner
            foreach (IBoardListener listener in listenerslist)
            {
                listener.SetNewWinner(listOfPlayers[playerToMove]);

                // remove the current player from the list of players
                listOfPlayers.Remove(listOfPlayers[playerToMove]);

                // if there's only one player left in the list, the game is over
                if (listOfPlayers.Count == 1)
                    gameOver = true;

                return true; // return true indicating the player has won
            }
        }

        return false; // return false indicating the player has not won yet
    }

    // method to get the list of winning positions for a given piece
    public List<Position> WinningPositions(Piece piece)
    {
        List<Position> winPositions = new List<Position>();
        int opposite = (int)PieceInfo.opposites[(int)piece]; // get the integer value of the opposite piece for the given piece

        // determine the total number of pieces based on the number of players in the game
        int totalPiece = playerAmount == 2 ? 15 : 10;

        // populate the winPositions list with the positions of the opposite pieces
        for (int j = 0; j < totalPiece; j++)
        {
            winPositions.Add(Utility.playerStartPos[opposite][j]);
        }

        return winPositions; // return the list of winning positions
    }
}
