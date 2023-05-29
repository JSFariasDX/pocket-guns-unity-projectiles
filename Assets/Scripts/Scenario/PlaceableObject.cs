using UnityEngine;

public enum PlaceableOrientation
{
    Horizontal = 1,
    Vertical = 2,
}

[CreateAssetMenu(fileName = "Placeable_", menuName = "Procedural Gen/Placeable Object")]
public class PlaceableObject : ScriptableObject
{
    public int sizeX = 1, sizeY = 1;
    public GameObject prefab;
    public PlaceableOrientation defaultOrientation;
    public bool ignoreWallsOnPlacement = false;
}
