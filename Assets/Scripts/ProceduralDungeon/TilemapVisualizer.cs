using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public static class AssetId {
    public const string floorTile = "floorTile";
    public const string wallSideRight = "wallSideRight";
    public const string wallSideLeft = "wallSideLeft";
    public const string wallBottom = "wallBottom";
    public const string wallInnerCornerUpLeft = "wallInnerCornerUpLeft";
    public const string wallInnerCornerUpRight = "wallInnerCornerUpRight";
    public const string wallTop = "wallTop";
    public const string wallInnerCornerDownLeft = "wallInnerCornerDownLeft";
    public const string wallInnerCornerDownRight = "wallInnerCornerDownRight";
    public const string wallDiagonalCornerDownLeft = "wallDiagonalCornerDownLeft";
    public const string wallDiagonalCornerDownRight = "wallDiagonalCornerDownRight";
    public const string wallDiagonalCornerUpRight = "wallDiagonalCornerUpRight";
    public const string wallDiagonalCornerUpLeft = "wallDiagonalCornerUpLeft";
};

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    public Tilemap floorTilemap, wallTilemap;
    DungeonPaintAssets currentAsset;

    [SerializeField]
    DungeonPaintAssets basicAsset;

    [SerializeField]
    DungeonPaintAssets destroyedAsset;

    public Dictionary<Vector2Int, string> assetsPositions = new Dictionary<Vector2Int, string>();

    int destroyChance = 10;

    public int minX = int.MaxValue;
    public int minY = int.MaxValue;
    public int maxX = -9999999;
    public int maxY = -9999999;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, AssetId.floorTile);   
    }

    public Direction GetWallDirection(Vector2Int wallPosition)
	{
        string wallId = assetsPositions[wallPosition];
        if (wallId.Equals(AssetId.wallTop)) return Direction.Up;
        else if (wallId.Equals(AssetId.wallSideRight)) return Direction.Right;
        else if (wallId.Equals(AssetId.wallBottom)) return Direction.Down;
        else if (wallId.Equals(AssetId.wallSideLeft)) return Direction.Left;
        else return Direction.None;
	}

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, string tileId)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tileId, position);
        }
    }

    internal void PaintBasicWallPosition(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        string tile = null;

        if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = AssetId.wallSideRight;
        } else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = AssetId.wallSideLeft;
        } else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
        {
            tile = AssetId.wallBottom;
        } else if (WallTypesHelper.wallInnerCornerUpLeft.Contains(typeAsInt))
        {
            tile = AssetId.wallInnerCornerUpLeft; //custom
        } else if (WallTypesHelper.wallInnerCornerUpRight.Contains(typeAsInt))
        {
            tile = AssetId.wallInnerCornerUpRight; // custom
        } else if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = AssetId.wallTop;
        }
        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }    
    }

    private void PaintSingleTile(Tilemap tilemap, string tileId, Vector2Int position)
    {
        if (currentAsset == null)
        {
            currentAsset = basicAsset;
        }

        assetsPositions[position] = tileId;

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
        }
        else if (position.y > maxY)
        {
            maxY = position.y;
        }

        TileBase tile = GetAssetById(tileId);
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        // Debug.Log("Clearing tilemap");
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        assetsPositions.Clear();
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        string tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = AssetId.wallInnerCornerDownLeft;
        } else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = AssetId.wallInnerCornerDownRight;
        } else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = AssetId.wallDiagonalCornerDownLeft;
        } else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = AssetId.wallDiagonalCornerDownRight;
        } else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = AssetId.wallDiagonalCornerUpRight;
        } else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = AssetId.wallDiagonalCornerUpLeft;
        } else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
        {
            tile = AssetId.wallBottom;
        }

        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }
    }

    public void SetAsset(DungeonPaintAssets asset)
    {
        Debug.Log("Setup paint asset");
        currentAsset = asset;
    }

    private TileBase GetAssetById(string id)
    {
        switch (id)
        {
            case "wallSideRight":
                int sort = Random.Range(0, currentAsset.wallSideRight.Count - 1);
                return currentAsset.wallSideRight[sort];
            case "wallSideLeft":
                int sort2 = Random.Range(0, currentAsset.wallSideLeft.Count - 1);
                return currentAsset.wallSideLeft[sort2];
            case "wallBottom":
                int sort3 = Random.Range(0, currentAsset.wallBottom.Count - 1);
                return currentAsset.wallBottom[sort3];
            case "wallInnerCornerUpLeft":
                int sort4 = Random.Range(0, currentAsset.wallInnerCornerUpLeft.Count - 1);
                return currentAsset.wallInnerCornerUpLeft[sort4];
            case "wallInnerCornerUpRight":
                int sort5 = Random.Range(0, currentAsset.wallInnerCornerUpRight.Count - 1);
                return currentAsset.wallInnerCornerUpRight[sort5];
            case "wallTop":
                int sort6 = Random.Range(0, currentAsset.wallTop.Count - 1);
                return currentAsset.wallTop[sort6];
            case "wallInnerCornerDownLeft":
                int sort7 = Random.Range(0, currentAsset.wallInnerCornerDownLeft.Count - 1);
                return currentAsset.wallInnerCornerDownLeft[sort7];
            case "wallInnerCornerDownRight":
                int sort8 = Random.Range(0, currentAsset.wallInnerCornerDownRight.Count - 1);
                return currentAsset.wallInnerCornerDownRight[sort8];
            case "wallDiagonalCornerDownLeft":
                int sort9 = Random.Range(0, currentAsset.wallDiagonalCornerDownLeft.Count - 1);
                return currentAsset.wallDiagonalCornerDownLeft[sort9];
            case "wallDiagonalCornerDownRight":
                int sort10 = Random.Range(0, currentAsset.wallDiagonalCornerDownRight.Count - 1);
                return currentAsset.wallDiagonalCornerDownRight[sort10];
            case "wallDiagonalCornerUpRight":
                int sort11 = Random.Range(0, currentAsset.wallDiagonalCornerUpRight.Count - 1);
                return currentAsset.wallDiagonalCornerUpRight[sort11];
            case "wallDiagonalCornerUpLeft":
                int sort12 = Random.Range(0, currentAsset.wallDiagonalCornerUpLeft.Count - 1);
                return currentAsset.wallDiagonalCornerUpLeft[sort12];
            default:
                int sort13 = Random.Range(0, currentAsset.floorTile.Count - 1);
                return currentAsset.floorTile[sort13];
        }
    }

    public void SetTilemapRendererMaterial(Material material)
    {
        floorTilemap.GetComponent<TilemapRenderer>().material = material;
        wallTilemap.GetComponent<TilemapRenderer>().material = material;
    }
}

public enum Direction
{
    None, Up, Right, Down, Left, Corner
}