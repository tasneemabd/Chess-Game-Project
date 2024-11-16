using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMove(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        // Define all possible king moves
        Vector2Int[] moveOffsets = new Vector2Int[]
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

        // Iterate through all possible moves
        foreach (Vector2Int offset in moveOffsets)
        {
            int newX = currentX + offset.x;
            int newY = currentY + offset.y;

            // Check if the new position is within bounds
            if (newX >= 0 && newX < tileCountX && newY >= 0 && newY < tileCountY)
            {
                // Add move if the target tile is empty or occupied by an enemy piece
                if (board[newX, newY] == null || board[newX, newY].team != team)
                {
                    r.Add(new Vector2Int(newX, newY));
                }
            }
        }

        return r;
    }
}
