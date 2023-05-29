using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldParticleController : MonoBehaviour
{
    public AnimationCurve sizeCurve;
    public AnimationCurve alphaCurve;
    float curveTime;
    Transform shieldIcon;

    bool started = false;

    public PickUpPopUp popUpPrefab;

    // Start is called before the first frame update
    void Start()
    {
        shieldIcon = transform.GetChild(0);
        //shieldIcon.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //shieldIcon.GetComponent<SpriteRenderer>().color = Color.white;

        PickUpPopUp popUp = Instantiate(popUpPrefab, transform.position, Quaternion.identity);
        popUp.Setup(shieldIcon.GetComponent<SpriteRenderer>().sprite, .4f);

        //started = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
            BeginEffect();
    }

    void BeginEffect()
    {
        if (shieldIcon.GetComponent<SpriteRenderer>().color.a > 0)
        {
            if (curveTime < 1) curveTime += Time.deltaTime;

            shieldIcon.localScale = Vector3.Lerp(new Vector3(0.25f, 0.25f, 0.25f), Vector3.one, sizeCurve.Evaluate(curveTime));
            shieldIcon.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, alphaCurve.Evaluate(curveTime)));
        }
    }
}
