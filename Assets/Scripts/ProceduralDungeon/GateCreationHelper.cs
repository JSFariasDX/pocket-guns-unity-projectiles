using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GateCreationHelper
{
    public static Quaternion GetDoorRotation(RelativePosition relativePosition)
    {
        switch (relativePosition)
        {
            case RelativePosition.Up:
                return Quaternion.Euler(0, 0, 180);
            case RelativePosition.Right:
                return Quaternion.Euler(0, 0, 90);
            case RelativePosition.Left:
                return Quaternion.Euler(0, 0, -90);
            default:
                return Quaternion.Euler(0, 0, 0);
        }
    }

    public static Vector2Int GetBestDoorPosition(HashSet<Vector2Int> corridor, RelativePosition relativePosition, Vector2Int roomCenter, HashSet<Vector2Int> currentGlobalFloorPositions, RectInt preventGateArea)
    {
        // Debug.Log("Looking for best door position");
        List<Vector2Int> potentialPositions;
        if (relativePosition == RelativePosition.Up || relativePosition == RelativePosition.Down)
        {
            potentialPositions = GetPotentialVerticalPositions(corridor, currentGlobalFloorPositions);
        }
        else
        {
            potentialPositions = GetPotentialHorizontalPositions(corridor, currentGlobalFloorPositions);
        }
        
        return GetClosestPosition(potentialPositions, roomCenter, preventGateArea);
    }

    public static List<Vector2Int> GetPotentialHorizontalPositions(HashSet<Vector2Int> corridor, HashSet<Vector2Int> currentGlobalFloorPositions)
    {
        List<Vector2Int> potentialPositions = new List<Vector2Int>();
        foreach (var position in corridor)
        {
            Vector2Int toUp = position + Direction2d.cardinalDirectionsList[0];
            bool toUpIsWall = !currentGlobalFloorPositions.Contains(toUp);
            Vector2Int toDown = position + Direction2d.cardinalDirectionsList[2];
            Vector2Int twoToDown = toDown + Direction2d.cardinalDirectionsList[2];
            bool twoDownIsWall = !currentGlobalFloorPositions.Contains(twoToDown);

            if (toUpIsWall && twoDownIsWall)
            {
                potentialPositions.Add(position);
            }
        }
        return potentialPositions;
    }

    public static List<Vector2Int> GetPotentialVerticalPositions(HashSet<Vector2Int> corridor, HashSet<Vector2Int> currentGlobalFloorPositions)
    {
        List<Vector2Int> potentialPositions = new List<Vector2Int>();
        foreach (var position in corridor)
        {
            Vector2Int toRight = position + Direction2d.cardinalDirectionsList[1];
            bool toRightIsWall = !currentGlobalFloorPositions.Contains(toRight);
            Vector2Int toLeft = position + Direction2d.cardinalDirectionsList[3];
            Vector2Int twoToLeft = toLeft + Direction2d.cardinalDirectionsList[3];
            bool twoToLeftIsWall = !currentGlobalFloorPositions.Contains(twoToLeft);

            if (twoToLeftIsWall && toRightIsWall)
            {
                potentialPositions.Add(position);
            }
        }
        return potentialPositions;
    }

    public static Vector2Int GetClosestPosition(List<Vector2Int> positions, Vector2Int target, RectInt preventGateArea)
    {
        float minDistance = 999999;
        Vector2Int closest = positions[0];
        foreach (var position in positions)
        {
            float distance = Vector2Int.Distance(position, target);
            if (!preventGateArea.Contains(position) && distance < minDistance)
            {
                minDistance = distance;
                closest = position;
            }
        }
        return closest;
    }
}
