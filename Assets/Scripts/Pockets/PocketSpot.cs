using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PocketSpot : MonoBehaviour
{
    public string categoryName;
    [HideInInspector]
    public CinemachineTargetGroup.Target thisTarget;

    [Header("Pockets")]
    //[HideInInspector]
    public List<Pocket> pocketsAvailable = new List<Pocket>();
    List<Transform> possibleSpots = new();
    public GameObject spotPrefab;
    float randomThreshold = .1f;

    // Start is called before the first frame update
    void Start()
    {
        CreateRandomSpots(3);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddPocket(Pocket pocket)
    {
        if (!pocketsAvailable.Contains(pocket))
        {
            pocketsAvailable.Add(pocket);
            int index = Random.Range(0, possibleSpots.Count);
            pocket.SetTargetPosition(possibleSpots[index]);
            possibleSpots.RemoveAt(index);
        }
    }

    public void SortList()
    {
        pocketsAvailable = pocketsAvailable.OrderBy(p => p.pocketName).ThenBy(l => l.level).ToList();
        print("<color=cyan>LIST SORTED</color>");
    }

    void CreateRandomSpots(int radius)
    {
        for (int x = -radius; x < radius; x++)
        {
            for (int y = -radius; y < radius; y++)
            {
                Vector2 spot = new Vector2(x, y);
                GameObject newObject = new GameObject();
                newObject.transform.parent = transform;

                Vector2 desiredSpot = spot + new Vector2(Random.Range(-randomThreshold, randomThreshold), Random.Range(-randomThreshold, randomThreshold));
                newObject.transform.localPosition = desiredSpot;

                //GameObject prefab = Instantiate(spotPrefab, Vector2.zero, Quaternion.identity, transform);
                //prefab.transform.localPosition = desiredSpot;

                possibleSpots.Add(newObject.transform);
            }
        }
    }
}
