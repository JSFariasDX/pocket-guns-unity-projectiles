using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowStorm : MonoBehaviour
{
    bool isActive = false;
    [SerializeField]
    GameObject snowStorm;

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.Instance.currentDungeonType == DungeonType.Glacier && !isActive)
        {
            isActive = true;
            snowStorm.SetActive(true);
        } else if (GameplayManager.Instance.currentDungeonType != DungeonType.Glacier && isActive)
        {
            isActive = false;
            snowStorm.SetActive(false);
        }
    }
}
