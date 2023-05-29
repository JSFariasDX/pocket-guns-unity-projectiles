using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrocaHud : MonoBehaviour
{
    public GameObject[] disableHuds;
    public GameObject[] huds;
    public float delay = 20;
    int currentIndex;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(Troca(delay));
	}

    // Update is called once per frame
    IEnumerator Troca(float delay)
    {
        for (int i = 0; i < huds.Length; i++)
		{
            huds[i].SetActive(false);
		}
        currentIndex++;
        if (currentIndex >= huds.Length) currentIndex = 0;

        huds[currentIndex].SetActive(true);

        yield return new WaitForSeconds(delay);

        StartCoroutine(Troca(delay));
    }
}
