using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundaries : MonoBehaviour
{
    //public  Vector2 cameraBounds;

    // Start is called before the first frame update
    void Start()
    {
        //AddCollider();
        transform.parent = null;
        transform.position = new Vector3(0, 0, Camera.main.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        AddCollider();
    }

    void AddCollider()
    {
        if (Camera.main == null) { Debug.LogError("Camera.main not found, failed to create edge colliders"); return; }

        var cam = Camera.main;
        if (!cam.orthographic) { Debug.LogError("Camera.main is not Orthographic, failed to create edge colliders"); return; }

        var bottomLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, 0, cam.transform.position.z));
        var topLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, Screen.height, cam.transform.position.z));
        var topRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.transform.position.z));
        var bottomRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cam.transform.position.z));

        // add or use existing EdgeCollider2D
        var edge = GetComponent<EdgeCollider2D>() == null ? gameObject.AddComponent<EdgeCollider2D>() : GetComponent<EdgeCollider2D>();

        var edgePoints = new[] { bottomLeft, topLeft, topRight, bottomRight, bottomLeft };
        edge.points = edgePoints;
    }
}
