using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TentSelectionManager : MonoBehaviour
{
    [Header("Components")]
    public List<TentSelection> tents = new List<TentSelection>();
    [SerializeField] TentSelection selectedTent;
    Ray2D ray;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Navigation(InputAction.CallbackContext ctx)
    {
        ray = new Ray2D(selectedTent.transform.position, ctx.ReadValue<Vector2>() * 100);
        RaycastHit2D[] hit = Physics2D.RaycastAll(ray.origin, ray.direction, 100, LayerMask.GetMask("Tents"));

        List<RaycastHit2D> newHit = new List<RaycastHit2D>(hit);
        newHit.RemoveAll(h => h.collider.gameObject == selectedTent.gameObject);

        if (newHit[0].collider.gameObject.GetComponent<TentSelection>())
        {
            selectedTent = newHit[0].collider.gameObject.GetComponent<TentSelection>();
        }
    }
}
