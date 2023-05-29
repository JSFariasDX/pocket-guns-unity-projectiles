using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField]
    public DungeonConfig testConfig;

    public void GenerateDungeon(DungeonConfig dungeonConfig)
    {
        tilemapVisualizer.Clear();
        RunProceduralGeneration(dungeonConfig);
    }

    public abstract void RunProceduralGeneration(DungeonConfig dungeonConfig);
}
