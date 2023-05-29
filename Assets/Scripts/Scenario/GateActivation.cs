using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateActivation : MonoBehaviour
{
    // Os codigos comentados poderão ser utilizados posteriormente,
    // quando definirmos como vai ser a interação com as portas

    [SerializeField]
    Gate gate;
    //List<Collider2D> collidingPlayers = new List<Collider2D>();


    private void OnTriggerStay2D(Collider2D other)
    {
        if (!gate.IsOpen() && other.CompareTag("PlayerCollider") && !gate.isLocked)
        {
   //         if (!collidingPlayers.Contains(other))
   //         {
   //             collidingPlayers.Add(other);
			//}

   //         if (collidingPlayers.Count == GameplayManager.Instance.GetPlayers(true).Count)
			//{
                gate.Open();
            other.GetComponentInParent<Player>().SetIsInGateActivationArea(true);
			//}
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (gate.IsOpen() && other.CompareTag("PlayerCollider"))
        {
            //collidingPlayers.Remove(other);
            gate.Close();
            other.GetComponentInParent<Player>().SetIsInGateActivationArea(false);
        }
    }
}
