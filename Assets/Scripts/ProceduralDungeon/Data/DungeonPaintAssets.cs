using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ProceduralGenerationTileAssets_", menuName = "PCG/Tile Assets")]
public class DungeonPaintAssets : ScriptableObject
{
    public List<TileBase> floorTile, wallTop, wallSideRight, wallSideLeft, wallBottom, wallInnerCornerDownLeft, wallInnerCornerDownRight, wallInnerCornerUpLeft, wallInnerCornerUpRight, wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;
}
