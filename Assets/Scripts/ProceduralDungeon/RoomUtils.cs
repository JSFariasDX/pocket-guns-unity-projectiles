using System.Collections.Generic;
using UnityEngine;

public class FloorStats
{
    public int minX = 9999999;
    public int minY = 9999999;
    public int maxX = -9999999;
    public int maxY = -9999999;

    public void GetFloorInfo(HashSet<Vector2Int> allPositions)
    {
        foreach (var position in allPositions)
        {
            if (position.x < minX)
            {
                minX = position.x;
            }
            else if (position.x > maxX)
            {
                maxX = position.x;
            }

            if (position.y < minY)
            {
                minY = position.y;
            }
            else if (position.y > maxY)
            {
                maxY = position.y;
            }
        }
    }
}

public static class RoomUtils
{
    public static Vector2Int GetGeometricCenter(HashSet<Vector2Int> allPositions)
    {
        int minX = 9999999, maxX = 0, minY = 999999999, maxY = 0;

        foreach (var position in allPositions)
        {
            if (position.x < minX)
            {
                minX = position.x;
            } else if (position.x > maxX)
            {
                maxX = position.x;
            }

            if (position.y < minY)
            {
                minY = position.y;
            } else if (position.y > maxY)
            {
                maxY = position.y;
            }
        }

        // Debug.Log($"minX {minX} maxX {maxX}");
        // Debug.Log($"minY {minY} maxY {maxY}");

        int centerX = Mathf.RoundToInt((maxX + minX) / 2);
        int centerY = Mathf.RoundToInt((maxY + minY) / 2);

        // Debug.Log($"centerX {centerX} centerY {centerY}");
        return new Vector2Int(centerX, centerY);
    }
}