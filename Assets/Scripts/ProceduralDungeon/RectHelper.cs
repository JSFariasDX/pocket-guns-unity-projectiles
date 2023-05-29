using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectHelper
{
    public static bool CheckOverlap(RectInt rectToCheck, List<RectInt> placedRects)
    {
        foreach (var rect in placedRects)
        {
            if (rectToCheck.Overlaps(rect))
            {
                return true;
            }
        }
        return false;
    }

    public static RectInt DrawRectFromCorner(Vector2Int center, int sizeX, int sizeY)
    {
        int drawPointX = center.x - Mathf.FloorToInt(sizeX / 2);
        int drawPointY = center.y - Mathf.FloorToInt(sizeY / 2);
        return new RectInt(drawPointX, drawPointY, sizeX, sizeY);
    }

    public static RectInt DrawRectFromPoint(Vector2Int point, int sizeX, int sizeY)
    {
        return new RectInt(point.x, point.y, sizeX, sizeY);
    }

    public static RectInt GetNextRoomRect(Vector2Int center, int nextLimits)
    {
        Vector2Int corner = GetCorner(center, nextLimits);
        int rectLimit = nextLimits * 2;
        return new RectInt(corner.x, corner.y, rectLimit, rectLimit);
    }

    private static Vector2Int GetCorner(Vector2Int nextRoomCenter, int limit)
    {
        Vector2Int corner = nextRoomCenter;
        for (int i = 1; i <= limit; i++)
        {
            corner += Direction2d.diagonalDirectionsList[2];
        }
        return corner;
    }

    public static bool CheckRectIsOutOfBounds(RectInt rect, HashSet<Vector2Int> bounds)
    {
        foreach (var position in rect.allPositionsWithin)
        {
            if (!bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }
}
