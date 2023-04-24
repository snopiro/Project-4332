using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTranslator
{
    //Enum to evaulate the directional arrow according to the player's movement.
    public enum ArrowDirection
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        TopRight = 5,
        BottomRight = 6,
        TopLeft = 7,
        BottomLeft = 8,
        UpFinished = 9,
        DownFinished = 10,
        LeftFinished = 11,
        RightFinished = 12
    }

    //Figure out the direction the character is and will be moving in, therefore what tiles need to be displayed.
    public ArrowDirection TranslateDirection(OverlayTile previousTile, OverlayTile currentTile, OverlayTile futureTile)
    {
        //If there is not a future tile, define the last tile as the final.
        bool isFinal = futureTile == null;

        //If the previous tile isn't the first tile that is used, get the current tile. Otherwise, create a new vector. Same for future tile.
        Vector2Int pastDirection = previousTile != null ? currentTile.gridLocation2D - previousTile.gridLocation2D : new Vector2Int(0, 0);

        Vector2Int futureDirection = futureTile != null ? futureTile.gridLocation2D - currentTile.gridLocation2D : new Vector2Int(0, 0);

        //Get the current direction if the beginning tile does not equate above. 
        Vector2Int direction = pastDirection != futureDirection ? pastDirection + futureDirection : futureDirection;

        //If statements for associating the proper arrows to the direction the character is going.
        //If we go up in the y direction, and that location is not the final tile, show the appropriate arrow.
        if(direction == new Vector2Int(0,1) && !isFinal)
        {
            return ArrowDirection.Up;
        }
        //If we go down in the y direction, and that location is not the final tile, show the appropriate arrow.
        if (direction == new Vector2Int(0, -1) && !isFinal)
        {
            return ArrowDirection.Down;
        }
        //If we go right in the x direction, and that location is not the final tile, show the appropriate arrow.
        if (direction == new Vector2Int(1, 0) && !isFinal)
        {
            return ArrowDirection.Right;
        }
        //If we go left in the y direction, and that location is not the final tile, show the appropriate arrow.
        if (direction == new Vector2Int(-1, 0) && !isFinal)
        {
            return ArrowDirection.Left;
        }

        if(direction == new Vector2Int(1, 1))
        {
            //If we are moving up and away, return the appropriate arrow depending on where the character was initially.
            if(pastDirection.y < futureDirection.y)
            {
                return ArrowDirection.BottomLeft;
            }
            else
            {
                return ArrowDirection.TopRight;
            }
        }

        //If we are moving up and in, return the appropriate arrow depending on where the character was initially.
        if (direction == new Vector2Int(-1, 1))
        {
            //If we are moving up,
            if (pastDirection.y < futureDirection.y)
            {
                return ArrowDirection.BottomRight;
            }
            else
            {
                return ArrowDirection.TopLeft;
            }
        }

        //If we are moving down and away, return the appropriate arrow depending on where the character was initially.
        if (direction == new Vector2Int(1, -1))
        {
            //If we are moving up,
            if (pastDirection.y > futureDirection.y)
            {
                return ArrowDirection.TopLeft;
            }
            else
            {
                return ArrowDirection.BottomRight;
            }
        }

        //If we are moving down and in, return the appropriate arrow depending on where the character was initially.
        if (direction == new Vector2Int(-1, -1))
        {
            //If we are moving up,
            if (pastDirection.y > futureDirection.y)
            {
                return ArrowDirection.TopRight;
            }
            else
            {
                return ArrowDirection.BottomLeft;
            }
        }
        //If we go up in the y direction, and that location IS the final tile, show the appropriate arrow.
        if (direction == new Vector2Int(0, 1) && isFinal)
        {
            return ArrowDirection.UpFinished;
        }
        //If we go down in the y direction, and that location IS the final tile, show the appropriate arrow.
        if (direction == new Vector2Int(0, -1) && isFinal)
        {
            return ArrowDirection.DownFinished;
        }
        //If we go right in the x direction, and that location IS the final tile, show the appropriate arrow.
        if (direction == new Vector2Int(1, 0) && isFinal)
        {
            return ArrowDirection.RightFinished;
        }
        //If we go left in the y direction, and that location IS the final tile, show the appropriate arrow.
        if (direction == new Vector2Int(-1, 0) && isFinal)
        {
            return ArrowDirection.LeftFinished;
        }

        return ArrowDirection.None;
    }
}
