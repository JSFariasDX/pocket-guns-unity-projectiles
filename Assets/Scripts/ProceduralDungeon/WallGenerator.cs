using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static HashSet<Vector2Int> CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        var basicWallPositions = FindWallDirections(floorPositions, Direction2d.cardinalDirectionsList);
        wallPositions.UnionWith(basicWallPositions);
        var cornerWallPositions = FindWallDirections(floorPositions, Direction2d.diagonalDirectionsList);
        wallPositions.UnionWith(cornerWallPositions);
        PaintWalls(floorPositions, tilemapVisualizer, basicWallPositions, cornerWallPositions);
        return wallPositions;
    }

    public static void PaintWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> cornerWallPositions)
    {
        CreateBasicWall(tilemapVisualizer, basicWallPositions, floorPositions);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPositions);
    }

    private static void CreateCornerWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in cornerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2d.eightDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                } else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType);
        }
    }

    private static void CreateBasicWall(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2d.cardinalDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                } else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemapVisualizer.PaintBasicWallPosition(position, neighboursBinaryType);
        }
    }

    private static HashSet<Vector2Int> FindWallDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionsList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition) == false)
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }
        return wallPositions;
    }

    public static bool IsCorner(HashSet<Vector2Int> wallPositions, Vector2Int positionToCheck)
    {
        bool verticalNeighbours = false;
        bool horizontalNeighbours = false;
        foreach (var direction in Direction2d.cardinalDirectionsList)
        {
            Vector2Int pos = positionToCheck + direction;
            bool isHorizontal = direction == Vector2Int.up || direction == Vector2Int.down;
            bool isVertical = direction == Vector2Int.left || direction == Vector2Int.right;
            if (!horizontalNeighbours && isHorizontal && wallPositions.Contains(pos))
            {
                horizontalNeighbours = true;
            } else if (!verticalNeighbours && isVertical && wallPositions.Contains(pos))
            {
                verticalNeighbours = true;
            }        
        }
        return horizontalNeighbours && verticalNeighbours;
    }
}
