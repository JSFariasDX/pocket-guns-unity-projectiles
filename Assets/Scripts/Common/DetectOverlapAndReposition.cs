using UnityEngine;

public class DetectOverlapAndReposition : MonoBehaviour
{
    [Header("Overlap Settings")]
    [SerializeField] private Transform overlapPoint;
    [SerializeField] private Vector2 overlapSize;
    [SerializeField] private LayerMask wallLayer;

    [Header("Reposition Settings")]
    [SerializeField] private Transform transformToMove;
    [SerializeField] private Vector2 repositionDirection;
    [SerializeField] private float repositionDistance;

    private void Update()
    {
        if (Physics2D.OverlapBox(overlapPoint.position, overlapSize, 0f, wallLayer))
        {
            Reposition();
        }
    }

    private void Reposition()
    {
        var direction = repositionDirection.normalized * repositionDistance;
        transformToMove.position += (Vector3)direction;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(overlapPoint.position, (Vector3)overlapSize);
    }
}
