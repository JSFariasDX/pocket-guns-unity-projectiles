using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGeneration
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength, float limit)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2d.GetRandomCardinalDirection();
            if (limit == 0f || Vector2Int.Distance(startPosition, newPosition) <= limit)
            {
                path.Add(newPosition);
                foreach (var direction in Direction2d.eightDirectionsList)
                {
                    path.Add(newPosition + direction);
                }
                previousPosition = newPosition;
            }
        }

        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        corridor.Add(startPosition);
        var direction = Direction2d.GetRandomCardinalDirection();
        var currentPosition = startPosition;
        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
            if (direction == Direction2d.cardinalDirectionsList[0] || direction == Direction2d.cardinalDirectionsList[2])
            {
                corridor.Add(currentPosition + Direction2d.cardinalDirectionsList[1]);
            } else
            {
                corridor.Add(currentPosition + Direction2d.cardinalDirectionsList[0]);
            }
        }
        return corridor;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value < 0.5f)
                {
                    if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    } else if (room.size.x >= minWidth * 2)
                    {
                        SplitVetically(minWidth, roomsQueue, room);
                    } else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                } else
                {
                    
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVetically(minWidth, roomsQueue, room);
                    } else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }

    private static void SplitVetically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(
            new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z)
        );
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(
            new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z)
        );
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}

public static class Direction2d
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int> {
        new Vector2Int(0, 1), // UP
        new Vector2Int(1, 0), // RIGHT
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(-1, 0), // LEFT
    };

    public static List<Vector2Int> upRightDirectionsList = new List<Vector2Int> {
        new Vector2Int(0, 1), // UP
        new Vector2Int(1, 0) // RIGHT
    };

    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int> {
        new Vector2Int(1, 1), // UP-RIGHT
        new Vector2Int(1, -1), // RIGHT-DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 1), // LEFT-UP
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // UP
        new Vector2Int(1, 1), // UP-RIGHT
        new Vector2Int(1, 0), // RIGHT
        new Vector2Int(1, -1), // RIGHT-DOWN
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 0), // LEFT
        new Vector2Int(-1, 1), // LEFT-UP
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}
