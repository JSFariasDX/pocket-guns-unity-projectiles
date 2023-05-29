using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class Tutorial : MonoBehaviour
{
    [Header("Components")]
    TutorialManager tutorial;
    PlayerControls control;

    [Header("Tutorial")]
    public bool hasAction = true;
    public string actionName;
    public string actionPrefix;
    [TextArea]
    public string description;
    string tutorialText;
    public bool needToBeCompleted = false;
    public bool needAction = true;
    public bool instant = false;
    public bool autoComplete = false;
    public float autoCompleteTime = 3;
    [SerializeField] private bool repeatable = false;
    float ac_timer;

    [Header("Positions")]
    public GameObject tutorialPosition;

    [Header("Slow Mo")]
    public bool isSlowMo = false;

    [Header("Time")]
    public float maxTime = 2; // Se for 0 a a��o conclui instant�neo. �til pra a��es que precissam s� de um toque
    float timer;
    bool actionPerformed;
    bool tapAction = false;
    bool completed = false;

    float timeScale = 1;

    [Header("UI")]
    public TextMeshProUGUI text;
    public Image indicationImage;
    public Image completionBar;

    [Header("Sprites")]
    public Sprite blackFill;
    public Sprite whiteFill;
    public Sprite whiteBorderFill;

    [Header("Effects")]
    public GameObject objectToPoint;
    public string targetName;
    Transform initialPoint;
    public Transform circle;
    LineRenderer line;

    private void Awake()
    {
        control = new PlayerControls();

        if (needToBeCompleted)
        {
            control.FindAction(actionName).performed += c => PerformAction(true);
            control.FindAction(actionName).canceled += c => PerformAction(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tutorial = FindObjectOfType<TutorialManager>();
        line = GetComponent<LineRenderer>();

        initialPoint = transform;

        timer = 0;
        ac_timer = 0;

        completed = false;

        //circle = transform.GetChild(1);
    }

    void PerformAction(bool perform)
    {
        if (PauseManager.Instance.IsGamePaused()) return;

        actionPerformed = perform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSlowMo)
        {
            if (!PauseManager.Instance.IsGamePaused())
            {
                timeScale = Mathf.Clamp(timeScale, 0, 1);

                Time.timeScale = timeScale;
                Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1);

                Time.fixedDeltaTime = Time.timeScale * .02f;

                if (!completed)
                    CauseSlowMo();
                else
                    LeaveSlowMo();
            }
        }

        if (tutorial.deviceSet && hasAction)
        {
            if (actionName == "Move" || actionName == "Walk")
                DisplayMoveIcon(false);
            else if (actionName == "Arrows")
                DisplayMoveIcon(true);
            else if (actionName == "Tab Left")
                DisplayTabsIcon(true);
            else if (actionName == "Tab Right")
                DisplayTabsIcon(false);
            else
                DisplayIcon(actionName);
        } 
        else if(!hasAction)
            DisplayIcon(actionName);

        if (maxTime <= 0)
            tapAction = true;

        if (targetName != "")
        {
            if (targetName == "GunSlot")
            {
                objectToPoint = GameObject.Find(targetName).transform.GetChild(0).GetChild(3).gameObject;
            }
            else if (targetName == "Pocket")
            {
                Transform objParent = GameplayManager.Instance.GetPlayers(true)[0].currentPocket.transform;

                foreach (Transform item in objParent)
                {
                    if (item.name == "PocketCenter")
                    {
                        objectToPoint = item.gameObject;
                        break;
                    }
                }
            }
            else
            {
                objectToPoint = GameObject.Find(targetName);
            }
        }

        if (tutorialText != null)
            text.text = tutorialText;


        PointToTarget();

        if (needToBeCompleted)
        {
            if (needAction && !autoComplete)
            {
                if (tapAction)
                {
                    if (actionPerformed)
                    {
                        CompleteTutorial();
                    }
                }
                else
                {
                    if (actionPerformed)
                    {
                        if (timer < maxTime)
                            timer += Time.unscaledDeltaTime;
                        else
                        {
                            CompleteTutorial();
                        }
                    }

                    if (completionBar)
                        completionBar.fillAmount = timer / maxTime;
                }
            } else if (autoComplete)
            {
                if (indicationImage)
                {
                    indicationImage.color = Color.clear;
                    completionBar.color = Color.clear;
                }

                if (ac_timer < autoCompleteTime)
                    ac_timer += Time.unscaledDeltaTime;
                else
                {
                    if (timer < maxTime)
                        timer += Time.unscaledDeltaTime;
                    else
                    {
                        CompleteTutorial();
                    }
                }
            }

            if(!completed)
                indicationImage.sprite = whiteBorderFill;
        }
        else
        {
            if (!completed)
            {
                if (needAction)
                    indicationImage.sprite = whiteBorderFill;
                else
                    indicationImage.sprite = blackFill;
            }
        }
    }

    void DisplayIcon(string action)
    {
        string buttonName = InputControlPath.ToHumanReadableString(control.FindAction(action).bindings[tutorial.isGamepad ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        //string buttonName = InputControlPath.ToHumanReadableString(ctx.action.bindings[tutorial.isGamepad ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        //print(buttonName);
        if (hasAction)
        {
            if (tutorial.isGamepad)
            {
                for (int i = 0; i < tutorial.gamepadButtons.Count; i++)
                {
                    if (tutorial.gamepadButtons[i].name == buttonName)
                    {
                        switch (tutorial.type)
                        {
                            case ControllerType.DualShock:
                                text.spriteAsset = tutorial.DSSprites;
                                tutorialText = actionPrefix + " <sprite name=\"" + tutorial.gamepadButtons[i].PlayStationIcon.name + "\"> to " + (description == "" ? actionName : description);
                                break;
                            case ControllerType.Xbox:
                                text.spriteAsset = tutorial.XboxSprites;
                                tutorialText = actionPrefix + " <sprite name=\"" + tutorial.gamepadButtons[i].XboxIcon.name + "\"> to " + (description == "" ? actionName : description);
                                break;
                            case ControllerType.Switch:
                                text.spriteAsset = tutorial.SwitchSprites;
                                tutorialText = actionPrefix + " <sprite name=\"" + tutorial.gamepadButtons[i].SwitchIcon.name + "\"> to " + (description == "" ? actionName : description);
                                break;
                            case ControllerType.Keyboard:
                                text.spriteAsset = tutorial.DSSprites;
                                tutorialText = actionPrefix + " <sprite name=\"" + tutorial.gamepadButtons[i].PlayStationIcon.name + "\"> to " + (description == "" ? actionName : description);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        continue;
                }
            }
            else
            {
                for (int i = 0; i < tutorial.keyboard.Count; i++)
                {
                    if (tutorial.keyboard[i].name == buttonName)
                    {
                        text.spriteAsset = tutorial.KeyboardSprites;
                        tutorialText = actionPrefix + " <sprite name=\"" + tutorial.keyboard[i].icon.name + "\"> to " + (description == "" ? actionName : description);
                        break;
                    }
                    else
                        continue;
                }
            }
        }
        else
        {
            if (!autoComplete)
                tutorialText = description;
            else
                tutorialText = "";
        }
    }

    void DisplayMoveIcon(bool arrows)
    {
        if (hasAction)
        {
            if (tutorial.isGamepad)
            {
                switch (tutorial.type)
                {
                    case ControllerType.DualShock:
                        text.spriteAsset = tutorial.DSSprites;
                        tutorialText = actionPrefix + " <sprite name=\"PS_L3\"> to " + (description == "" ? actionName : description);
                        break;
                    case ControllerType.Xbox:
                        text.spriteAsset = tutorial.XboxSprites;
                        tutorialText = actionPrefix + " <sprite name=\"Xbox_LeftStick\"> to " + (description == "" ? actionName : description);
                        break;
                    case ControllerType.Switch:
                        text.spriteAsset = tutorial.SwitchSprites;
                        tutorialText = actionPrefix + " <sprite name=\"Switch_LS\"> to " + (description == "" ? actionName : description);
                        break;
                    case ControllerType.Keyboard:
                        text.spriteAsset = tutorial.DSSprites;
                        tutorialText = actionPrefix + " <sprite name=\"PS_L3\"> to " + (description == "" ? actionName : description);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (arrows)
                {
                    text.spriteAsset = tutorial.KeyboardSprites;
                    tutorialText = actionPrefix + " <sprite name=\"ARROW UP\"><sprite name=\"ARROW LEFT\"><sprite name=\"ARROW DOWN\"><sprite name=\"ARROW RIGHT\"> to " + (description == "" ? actionName : description);
                }
                else
                {
                    text.spriteAsset = tutorial.KeyboardSprites;
                    tutorialText = actionPrefix + " <sprite name=\"W\"><sprite name=\"A\"><sprite name=\"S\"><sprite name=\"D\"> to " + (description == "" ? actionName : description);
                }
            }
        }
        else
        {
            tutorialText = description;
        }
    }

    void DisplayTabsIcon(bool left)
    {
        if (hasAction)
        {
            if (tutorial.isGamepad)
            {
                switch (tutorial.type)
                {
                    case ControllerType.DualShock:
                        text.spriteAsset = tutorial.DSSprites;
                        if (left)
                            tutorialText = actionPrefix + " <sprite name=\"PS_flat_L1\">";
                        else
                            tutorialText = actionPrefix + " <sprite name=\"PS_flat_R1\">";
                        break;
                    case ControllerType.Xbox:
                        text.spriteAsset = tutorial.XboxSprites;
                        if (left)
                            tutorialText = actionPrefix + " <sprite name=\"Xbox_LB\">";
                        else
                            tutorialText = actionPrefix + " <sprite name=\"Xbox_RB\">";
                        break;
                    case ControllerType.Switch:
                        text.spriteAsset = tutorial.SwitchSprites;
                        if (left)
                            tutorialText = actionPrefix + " <sprite name=\"Switch_flat_L\">";
                        else
                            tutorialText = actionPrefix + " <sprite name=\"Switch_flat_R\">";
                        break;
                    case ControllerType.Keyboard:
                        text.spriteAsset = tutorial.DSSprites;
                        tutorialText = actionPrefix + " <sprite name=\"PS_L3\"> to " + (description == "" ? actionName : description);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (left)
                {
                    text.spriteAsset = tutorial.KeyboardSprites;
                    tutorialText = actionPrefix + " <sprite name=\"Q\">";
                }
                else
                {
                    text.spriteAsset = tutorial.KeyboardSprites;
                    tutorialText = actionPrefix + " <sprite name=\"E\">";
                }
            }
        }
    }

    void PointToTarget()
    {
        if (objectToPoint != null)
        {
            if (circle)
                circle.gameObject.SetActive(true);

            GetComponent<Image>().enabled = true;

            line.enabled = true;

            if (GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceCamera)
            {
                if(tutorialPosition)
                    GetComponent<RectTransform>().position = tutorialPosition.transform.position;

                line.SetPosition(0, new Vector3(objectToPoint.transform.position.x, objectToPoint.transform.position.y, 0));
                circle.position = new Vector3(objectToPoint.transform.position.x, objectToPoint.transform.position.y, 0);

                line.SetPosition(1, new Vector3(initialPoint.position.x, initialPoint.position.y, 0));
            }
            else if(GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay)
            {

                if (objectToPoint.GetComponent<RectTransform>())
                {
                    //line.SetPosition(0, new Vector3(Camera.main.ScreenToWorldPoint(objectToPoint.transform.position).x, Camera.main.ScreenToWorldPoint(objectToPoint.transform.position).y, 0));
                    line.SetPosition(0, new Vector3(objectToPoint.transform.position.x, objectToPoint.transform.position.y, 0));
                    circle.position = new Vector3(objectToPoint.transform.position.x, objectToPoint.transform.position.y, 0);
                }
                else
                {
                    Vector3 targetPos = new Vector3(objectToPoint.GetComponent<Renderer>().bounds.center.x, objectToPoint.GetComponent<Renderer>().bounds.center.y, 0);

                    line.SetPosition(0, targetPos);
                    circle.position = targetPos;
                }

                line.SetPosition(1, new Vector3(Camera.main.ScreenToWorldPoint(initialPoint.position).x, Camera.main.ScreenToWorldPoint(initialPoint.position).y, 0));
            }
        }
        else
        {
            if (circle)
                circle.gameObject.SetActive(false);

            if(GetComponent<Image>())
                GetComponent<Image>().enabled = false;

            line.enabled = false;
        }
    }

    void CauseSlowMo()
    {
        if (timeScale > 0)
            timeScale -= Time.unscaledDeltaTime;
        else
            timeScale = 0;
    }

    void LeaveSlowMo()
    {
        if (timeScale < 1)
            timeScale += Time.unscaledDeltaTime;
        else
            timeScale = 1;
    }

    public void CompleteTutorial()
    {
        if (completed) return;

        if (!autoComplete)
        {
            if (indicationImage)
            {
                indicationImage.color = Color.green;
                completionBar.color = Color.green;
            }
        }

        if (!completed)
            StartCoroutine(CallNextScreen(instant ? 0 : 0.75f));

        completed = true;
    }

    IEnumerator CallNextScreen(float time)
    {
        if(indicationImage)
            indicationImage.sprite = whiteFill;

        yield return new WaitForSecondsRealtime(time);

        if (repeatable)
        {
            actionPerformed = false;
            completed = false;
            ac_timer = 0f;
            timer = 0f;
        }

        GetComponentInParent<TutorialsController>().NextScreen();
        //print("Next screen");

        if (indicationImage)
        {
            indicationImage.sprite = whiteBorderFill;

            indicationImage.color = new Color32(96, 215, 252, 255);
            completionBar.color = Color.white;
        }

        completed = false;
    }

    private void OnEnable()
    {
        control.Enable();
    }

    private void OnDisable()
    {
        control.Disable();
    }

    public bool GetIsComplete()
    {
        return completed;
    }
}
