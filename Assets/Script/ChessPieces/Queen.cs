using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override List<Vector2Int> GetAvailableMove(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        // Define all 8 possible directions (rook + bishop)
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),   // Right
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(1, 1),   // Top-right
            new Vector2Int(-1, 1),  // Top-left
            new Vector2Int(1, -1),  // Bottom-right
            new Vector2Int(-1, -1)  // Bottom-left
        };

        // Iterate through each direction
        foreach (Vector2Int dir in directions)
        {
            int x = currentX;
            int y = currentY;

            while (true)
            {
                x += dir.x;
                y += dir.y;

                // Check if the new position is within bounds
                if (x < 0 || x >= tileCountX || y < 0 || y >= tileCountY)
                    break;

                // If the tile is occupied
                if (board[x, y] != null)
                {
                    // Add as a valid move if occupied by an enemy
                    if (board[x, y].team != team)
                        r.Add(new Vector2Int(x, y));

                    // Stop moving further in this direction
                    break;
                }

                // Add the valid empty tile
                r.Add(new Vector2Int(x, y));
            }
        }

        return r;
    }
}
