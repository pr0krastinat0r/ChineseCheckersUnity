using System;
using UnityEngine;

// A *Position* is where a piece is in terms of the game board
// A *Coordinate* is the pixel coordinates on the screen.
// We rename Vector2Int and Vector2 for convenience
using Position = UnityEngine.Vector2Int;
using Coordinate = UnityEngine.Vector2;

public class Utility
{
    // The board representation is expected to be scaled so that each hexagon has a diameter of 1
    // and the origin of the board is a the centre of the window.

    // These are used to convert a position to coordinates.
    private const float deltaX = 0.5f;
    private const float deltaY = 0.8660254f; // √3/2

    // The board is at the lowest level, the pieces placed above it.  Moving pieces are above static pieces.
    public const float boardLevel = 0.0f;
    public const float pieceLevel = -1.0f;
    public const float movingPieceLevel = -2.0f;

    public const int xMin = -8;
    public const int yMin = -8;
    public const int xMax = 8;
    public const int yMax = 8;

    // Convert a board position into world coordinates
    public static Coordinate PositionToCoordinates(Position pos)
    {
        return (new Coordinate(pos.x + deltaX * pos.y, pos.y * deltaY));
    }

    // Convert world coordinates into a matching board position
    // The coordinate system sets the centre hexagon as (0, 0);
    // the x axis goes from left to right; the y axis goes at 60° from left to right.
    public static Position CoordinatesToPosition(Coordinate coords)
    {
        int y = Mathf.FloorToInt((coords.y + deltaY / 2.0f) / deltaY);
        int x = Mathf.FloorToInt((coords.x + deltaX) - (deltaX * y));

        return new Position(x, y);
    }

    public static T[,] MakeMatrix<T>(int xMin, int yMin, int xMax, int yMax)
    {
        return (T[,])Array.CreateInstance(typeof(T), new int[] { xMax - xMin + 1, yMax - yMin + 1 }, new int[] { xMin, yMin });
    }

    //The all player start position and empty positions on the board
    public static readonly Position[][] playerStartPos = new Position[][]
    {
        //Red 
        new Position[15] //Red
        {
            new Position(-4, 8),
            new Position(-4, 7),
            new Position(-3, 7),
            new Position(-4, 6),
            new Position(-3, 6),
            new Position(-2, 6),
            new Position(-4, 5),
            new Position(-3, 5),
            new Position(-2, 5),
            new Position(-1, 5),

            new Position(-4, 4),
            new Position(-3, 4),
            new Position(-2, 4),
            new Position(-1, 4),
            new Position(0, 4)
        },
        // Yellow
        new Position[15]
        {
            new Position (4,4),
            new Position (3,4),
            new Position (4,3),
            new Position (2,4),
            new Position (3,3),
            new Position (4,2),
            new Position (1,4),
            new Position (2,3),
            new Position (3,2),
            new Position (4,1),

            new Position(0,4),
            new Position(1,3),
            new Position(2,2),
            new Position(3,1),
            new Position(4,0)

        },
        // Green
        new Position[15]
        {
            new Position (8,-4),
            new Position (7,-3),
            new Position (7,-4),
            new Position (6,-2),
            new Position (6,-3),
            new Position (6,-4),
            new Position (5,-1),
            new Position (5,-2),
            new Position (5,-3),
            new Position (5,-4),

            new Position (4,0),
            new Position (4,-1),
            new Position (4,-2),
            new Position (4,-3),
            new Position (4,-4),
        },

       new Position[15] //Cyan
        {
            new Position(4, -8),
            new Position(4, -7),
            new Position(3, -7),
            new Position(4, -6),
            new Position(3, -6),
            new Position(2, -6),
            new Position(4, -5),
            new Position(3, -5),
            new Position(2, -5),
            new Position(1, -5),

            new Position(4, -4),
            new Position(3, -4),
            new Position(2, -4),
            new Position(1, -4),
            new Position(0, -4)
        },
         // Blue
        new Position[15]
        {
            new Position(-4, -4),
            new Position(-3, -4),
            new Position(-4, -3),
            new Position(-2, -4),
            new Position(-3, -3),
            new Position(-4, -2),
            new Position(-1, -4),
            new Position(-2, -3),
            new Position(-3, -2),
            new Position(-4, -1),

            new Position(0, -4),
            new Position(-1, -3),
            new Position(-2, -2),
            new Position(-3, -1),
            new Position(-4, 0)
        },
         // Magenta
       new Position[15]
        {
            new Position(-8, 4),
            new Position(-7, 3),
            new Position(-7, 4),
            new Position(-6, 2),
            new Position(-6, 3),
            new Position(-6, 4),
            new Position(-5, 1),
            new Position(-5, 2),
            new Position(-5, 3),
            new Position(-5, 4),

            new Position(-4, 0),
            new Position(-4, 1),
            new Position(-4, 2),
            new Position(-4, 3),
            new Position(-4, 4)
        },
         // Empty 
       new Position[]
        {
            //Row 1
            new Position(-4, 4),
            new Position(-3, 4),
            new Position(-2, 4),
            new Position(-1, 4),
            new Position(0, 4),

            //Row 2
            new Position(-4, 3),
            new Position(-3, 3),
            new Position(-2, 3),
            new Position(-1, 3),
            new Position(0, 3),
            new Position(1, 3),

            new Position(-4, 2),
            new Position(-3, 2),
            new Position(-2, 2),
            new Position(-1, 2),
            new Position(0, 2),
            new Position(1, 2),
            new Position(2, 2),

            new Position(-4, 1),
            new Position(-3, 1),
            new Position(-2, 1),
            new Position(-1, 1),
            new Position(0, 1),
            new Position(1, 1),
            new Position(2, 1),
            new Position(3, 1),

            new Position(-4, 0),
            new Position(-3, 0),
            new Position(-2, 0),
            new Position(-1, 0),
            new Position(0, 0),
            new Position(1, 0),
            new Position(2, 0),
            new Position(3, 0),
            new Position(4, 0),

            new Position(-3, -1),
            new Position(-2, -1),
            new Position(-1, -1),
            new Position(0, -1),
            new Position(1, -1),
            new Position(2, -1),
            new Position(3, -1),
            new Position(4, -1),

            new Position(-2, -2),
            new Position(-1, -2),
            new Position(0, -2),
            new Position(1, -2),
            new Position(2, -2),
            new Position(3, -2),
            new Position(4, -2),

            new Position(-1, -3),
            new Position(-0, -3),
            new Position(1, -3),
            new Position(2, -3),
            new Position(3, -3),
            new Position(4, -3),

            new Position(0, -4),
            new Position(1, -4),
            new Position(2, -4),
            new Position(3, -4),
            new Position(4, -4)
        },

    };

    //All possible direction which player can move
    public static readonly Position[] legalDir = new Position[]
    {
        new Position(0,1),// +y
        new Position(1,0),//+x
        new Position(1,-1),//+x,-y
        new Position(0,-1),//-y
        new Position(-1,0),//-x
        new Position(-1,1)//-x,-y

    };
    //All win position which players need to move their piece
    public static Position[] winPositions = new Position[]
    {
        new Position(-4, 8), //Red
        new Position(4, 4), //Yellow
        new Position(8, -4), //Green
        new Position(4, -8), //Cyan
        new Position(-4, -4), //Blue
        new Position(-8, 4) //Magenta
    };



}