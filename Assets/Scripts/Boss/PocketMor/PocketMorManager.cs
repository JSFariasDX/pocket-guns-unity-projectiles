using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PocketMorManager : MonoBehaviour
{
    public static PocketMorManager Instance;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

}
