using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayContent : MonoBehaviour
{
    [SerializeField] GameObject elementToFocus;
    public TMPro.TextMeshProUGUI topScoreText;

    void Update()
    {
        //GameObject s = GameObject.Find("TopScore");
        //s.GetComponent<TMPro.TextMeshProUGUI>().text = GlobalData.Instance.topScore.ToString("D7");
        topScoreText.text = GlobalData.Instance.topScore.ToString("D7");
    }
    private void OnEnable()
    {
        if (Gamepad.current != null)
        {
            EventSystem.current.SetSelectedGameObject(elementToFocus);
        }
    }
}
