using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SRWDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    protected CreateRandomWalkDataSO smallRoomParams, mediumRoomParams, bigRoomParams;

    public override void RunProceduralGeneration(DungeonConfig dungeonConfig)
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(mediumRoomParams, startPosition);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalk(CreateRandomWalkDataSO parameters, Vector2Int position)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGeneration.SimpleRandomWalk(currentPosition, parameters.walkLength, parameters.limitDistanceFromCenter);
            floorPositions.UnionWith(path);
            if (parameters.startRandomlyEachIteration == true)
            {
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }
        return floorPositions;
    }
}
