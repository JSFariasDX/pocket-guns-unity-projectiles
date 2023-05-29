using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCoins : MonoBehaviour
{
    [Header("Drops")]
    public GameObject coinPrefab;
    public int amountToDrop = 1;
    public float dropForce = 1;
    bool dropped = false;

    public void DropCoin(Vector3 dropPosition, bool force = false)
    {
        if (!dropped)
        {
            if (Random.value <= .4f || force)
            {
                for (int i = 0; i < amountToDrop; i++)
                {
                    GameObject coin = Instantiate(coinPrefab, dropPosition, Quaternion.identity);
                    Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();

                    float randomAngle = Random.Range(0f, 6.28319f); //radians
                    Vector2 randomVector = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));

                    rb.AddForce(randomVector.normalized * dropForce, ForceMode2D.Impulse);
                }
            }
            dropped = true;
        }
    }
}
