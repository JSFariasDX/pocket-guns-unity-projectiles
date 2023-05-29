using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomCreationHelper
{
    public static HashSet<Vector2Int> CreateRoomFloorFromRectInt(RectInt rect)
    {
        // Padding from rect to prevent overlapping
        rect.size = new Vector2Int(rect.size.x - 4, rect.size.y - 4);
        rect.x = rect.x + 2;
        rect.y = rect.y + 2;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        foreach (var position in rect.allPositionsWithin)
        {
            floorPositions.Add(position);
        }
        return floorPositions;
    }

    public static HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
            corridor.Add(position + Direction2d.cardinalDirectionsList[1]);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
            corridor.Add(position + Direction2d.cardinalDirectionsList[0]);
        }
        return corridor;
    }

    public static int GetPaddingToNextRoom(int currentLimits, int nextRoomLimits)
    {
        return currentLimits + 4 + nextRoomLimits;
    }

    public static Vector2Int GetNextRoomCenter(Vector2Int currentRoomCenter, int paddingBetween, bool ignoreDirections = false)
    {
        Vector2Int direction;
        if (ignoreDirections)
            direction = Direction2d.upRightDirectionsList[Random.Range(0, Direction2d.upRightDirectionsList.Count)];
        else
            direction = Direction2d.cardinalDirectionsList[Random.Range(0, 3)];

        Vector2Int finalPoint = currentRoomCenter;
        for (int i = 1; i <= paddingBetween; i++)
        {
            finalPoint += direction;
        }
        return finalPoint;
    }

    public static Vector2Int GetRandomPoint(HashSet<Vector2Int> positions)
    {
        int sorted = Random.Range(0, positions.Count);
        int i = 0;
        Vector2Int pos = Vector2Int.zero;
        foreach (Vector2Int position in positions)
        {
            if (i == sorted)
            {
                pos = position;
            }
            i++;
        }
        return pos;
    }
}
