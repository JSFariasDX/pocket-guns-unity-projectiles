using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [Header("Components")]
    PlayerControls controls;
    TutorialManager tutorial;

    [Header("Dialogue")]
    public TextMeshProUGUI dialogueText;
    bool isTyping = false;
    [SerializeField] private float typingDelay = 0.03f;

    [Header("Interactible")]
    public bool isInteractible = false;
    [SerializeField] private bool isSkippable = true;
    public float delay;
    float delayTimer;
    public TextMeshProUGUI iconText;

    public Queue<string> sentences = new Queue<string>();

    string currentSentence;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.UI.Any.performed += ctx => DisplayIcon("Interact");
        controls.Gameplay.Interact.performed += ctx => NextButton();
    }

    // Start is called before the first frame update
    void Start()
    {
        tutorial = FindObjectOfType<TutorialManager>();

        delayTimer = 0;

        //StartDialogue(thisDialogue);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInteractible)
        {
            if (delayTimer < delay)
            {
                if (!isTyping)
                    delayTimer += Time.unscaledDeltaTime;
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }

    void DisplayIcon(string action)
    {
        if (isInteractible)
        {
            string buttonName = InputControlPath.ToHumanReadableString(controls.FindAction(action).bindings[tutorial.isGamepad ? 0 : 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

            if (tutorial.isGamepad)
            {
                for (int i = 0; i < tutorial.gamepadButtons.Count; i++)
                {
                    if (tutorial.gamepadButtons[i].name == buttonName)
                    {
                        switch (tutorial.type)
                        {
                            case ControllerType.DualShock:
                                iconText.spriteAsset = tutorial.DSSprites;
                                iconText.text = "<sprite name=\"" + tutorial.gamepadButtons[i].PlayStationIcon.name + "\"> >>>";
                                break;
                            case ControllerType.Xbox:
                                iconText.spriteAsset = tutorial.XboxSprites;
                                iconText.text = "<sprite name=\"" + tutorial.gamepadButtons[i].XboxIcon.name + "\"> >>>";
                                break;
                            case ControllerType.Switch:
                                iconText.spriteAsset = tutorial.SwitchSprites;
                                iconText.text = "<sprite name=\"" + tutorial.gamepadButtons[i].SwitchIcon.name + "\"> >>>";
                                break;
                            case ControllerType.Keyboard:
                                iconText.spriteAsset = tutorial.DSSprites;
                                iconText.text = "<sprite name=\"" + tutorial.gamepadButtons[i].PlayStationIcon.name + "\"> >>>";
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
                        iconText.spriteAsset = tutorial.KeyboardSprites;
                        iconText.text = "<sprite name=\"" + tutorial.keyboard[i].icon.name + "\"> >>>";
                        break;
                    }
                    else
                        continue;
                }
            }
        }
    }

    public void StartDialogue(Dialogue dialogue, bool lobby = false)
    {
        sentences.Clear();

        foreach (string sentence in dialogue.Sentences)
        {
            string newString;

            if (lobby) newString = sentence.Replace("[X]", FindObjectOfType<PlayerEntryPanel>().unlockedPockets.Count.ToString());
            else newString = sentence;

            sentences.Enqueue(newString);
        }

        DisplayNextSentence();

        delayTimer = 0;
    }

    public void NextButton()
    {
        if (!isSkippable)
            return;
            
        if (isTyping)
            CompleteSentence();
        else
            DisplayNextSentence();
    }

    void DisplayNextSentence()
    {
        if (sentences.Count == 0) 
        {
            EndDialogue();
            return; 
        }

        string sentence = sentences.Dequeue();
        StopCoroutine("TypeSentence");
        StartCoroutine(TypeSentence(sentence, false));

        delayTimer = 0;
    }

    void CompleteSentence()
    {
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence, true));
    }

    void EndDialogue()
    {
        currentSentence = null;
        gameObject.SetActive(false);
    }

    IEnumerator TypeSentence(string sentence, bool complete)
    {
        dialogueText.text = sentence;
        dialogueText.maxVisibleCharacters = 0;
        currentSentence = sentence;

        if (!complete)
        {
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.maxVisibleCharacters++;

                if (dialogueText.text.Length < sentence.Length)
                {
                    isTyping = true;
                    yield return new WaitForSecondsRealtime(typingDelay);
                }
                else
                {
                    isTyping = false;
                    yield return new WaitForSecondsRealtime(typingDelay);
                }
            }
        }
        else
        {
            dialogueText.text = sentence;
            dialogueText.maxVisibleCharacters = sentence.Length;

            if (dialogueText.text.Length < sentence.Length)
            {
                isTyping = true;
                yield return null;
            }
            else
            {
                isTyping = false;
                yield return null;
            }
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
}
