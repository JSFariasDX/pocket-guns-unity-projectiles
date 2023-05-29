using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialBar : MonoBehaviour
{
    public Image specialBar;
    public GameObject toTrack;
    Special special;
    Image[] indicators;
    bool isDisabled = false;

    public Sprite rechargeSprite;
    public Sprite baseSprite;
    public Sprite flashSprite;

    public void Setup(GameObject toTrack)
	{
        this.toTrack = toTrack;
        special = toTrack.GetComponent<Special>();
    }

    // Start is called before the first frame update
    void Start()
    {
        specialBar = GetComponent<Image>();
        indicators = GetComponentsInParent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (special == null)
        {
            return;
        }
        if (toTrack == null && !isDisabled)
        {
            toggleIndicators(false);
            isDisabled = true;
        } else if (toTrack != null && isDisabled)
        {
            toggleIndicators(true);
            isDisabled = false;
        }
        specialBar.fillAmount = special.GetCharge() / special.GetTotalTime();
    }

    void toggleIndicators(bool value)
    {
        foreach (Image indicator in indicators)
        {
            indicator.enabled = value;
        }
    }
}
