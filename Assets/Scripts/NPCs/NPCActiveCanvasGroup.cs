using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class NPCActiveCanvasGroup : Interactable
{
    [Header("NPC Settings")]
    [SerializeField] CanvasGroup canvas;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField, TextArea] string popupTitle;
    [SerializeField, TextArea] string popupDescription;
    [SerializeField, TextArea] string title;

    [SerializeField] DialogueController dialogueBalloon;
    [SerializeField] DialogueListSO dialogues;

    [SerializeField] RectTransform focusObject;

	protected override void Start()
	{
		base.Start();
        itemName.text = popupTitle;
        itemDescription.text = popupDescription;
    }

	public override void OnInteract(Player player)
	{
        ShowCanvas();
    }

    public void ShowCanvas()
	{
        titleText.text = title;
        canvas.gameObject.SetActive(true);
        _currentPlayer.GetInputController().SetMapInput("UI");
        EventSystem.current.SetSelectedGameObject(focusObject.gameObject);
    }

    public virtual void HideCanvas()
	{
        canvas.gameObject.SetActive(false);
        _currentPlayer.GetInputController().SetMapInput("Gameplay");
    }

    void TriggerDialogue(DialogueListSO dialogueList)
	{
        if (dialogueList.List.Count == 0)
            return;

        var dialogue = dialogueList.GetRandomDialogueFromList();
        TriggerDialogue(dialogue);
    }

    protected void TriggerDialogue(Dialogue dialogue, bool lobby = false)
    {
        if (dialogueBalloon == null)
            return;

        dialogueBalloon.gameObject.SetActive(true);
        dialogueBalloon.StartDialogue(dialogue, lobby);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            if (!players.Contains(collision.GetComponentInParent<Player>()))
                players.Add(collision.GetComponentInParent<Player>());

            _currentPlayer = collision.GetComponentInParent<Player>();
            _currentPlayer.SetInteractable(this);
            //popUp.alpha = 1;
            SetIcon();

            TriggerDialogue(dialogues);
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            _currentPlayer = collision.GetComponentInParent<Player>();
            _currentPlayer.SetInteractable(null);

            if (players.Contains(collision.GetComponentInParent<Player>()))
                players.Remove(collision.GetComponentInParent<Player>());
            popUp.alpha = 0;
            
            _currentPlayer = null;
        }
    }
}
