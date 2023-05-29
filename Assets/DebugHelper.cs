using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    public Player player;
    public static DebugHelper Instance;
    public bool isActivated = false;
    public GameObject textPrefab;
    public GameObject imagePrefab;
    public GameObject text;
    public GameObject image;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

	public GameObject SetupImage(Sprite img, bool flip = false)
    {
        image.GetComponent<UnityEngine.UI.Image>().sprite = img;
        image.GetComponent<UnityEngine.UI.Image>().SetNativeSize();
        if (flip)
        {
            image.GetComponent<UnityEngine.UI.Image>().transform.localScale = new Vector3(-1, 1, 1);
        }
        return image;
    }

    public GameObject SetupText(string text)
    {
        this.text.GetComponent<TMPro.TextMeshProUGUI>().text = text;
        return this.text;
    }

    public GameObject AddImage(Sprite img, bool flip = false)
    {
        GameObject t = Instantiate(imagePrefab, transform);
        t.GetComponent<UnityEngine.UI.Image>().sprite = img;
        t.GetComponent<UnityEngine.UI.Image>().SetNativeSize();
        if (flip)
        {
            t.GetComponent<UnityEngine.UI.Image>().transform.localScale = new Vector3(-1, 1, 1);
        }
        return t;
    }

    public GameObject AddText(string text)
    {
        GameObject t = Instantiate(textPrefab, transform);
        t.GetComponent<TMPro.TextMeshProUGUI>().text = text;
        return t;
    }
}
