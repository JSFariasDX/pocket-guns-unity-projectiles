using System.Collections.Generic;
using UnityEngine;

public class Explorer : NPC
{
    [Header("Explorer Settings")]
    [SerializeField] private int maxDungeonDialogue = 1;
    [SerializeField] private int maxGeneralDialogue = 2;

    [Header("Dialogue Settings")]
    [SerializeField] private DialogueListSO labDialogue;
    [SerializeField] private DialogueListSO forestDialogue;
    [SerializeField] private DialogueListSO caveDialogue;
    [SerializeField] private DialogueListSO glacierDialogue;
    [SerializeField] private DialogueListSO volcanoDialogue;
    [SerializeField] private DialogueListSO swampDialogue;
    [SerializeField] private DialogueListSO sinisterLabDialogue;
    [SerializeField] private DialogueListSO lobbyDialogue;

    private List<Dialogue> _allGeneralDialogue;
    private List<Dialogue> _explorerSelectedDialogue;
    private int _explorerDialogueIndex;

    protected override void Start()
    {
        _explorerSelectedDialogue = new();
        _explorerDialogueIndex = 0;

        SetupDungeonDialogue();
        SetupGeneralDialogue();
        base.Start();
    }

    private void SetupDungeonDialogue()
    {
        var currentDungeonDialogueList = GetCurrentDungeonDialogue();
        for (int i = 0; i < maxDungeonDialogue; i++)
        {
            var randomDialogue = Random.Range(0, currentDungeonDialogueList.Count);
            _explorerSelectedDialogue.Add(currentDungeonDialogueList[randomDialogue]);
            currentDungeonDialogueList.RemoveAt(randomDialogue);
        }
    }

    private void SetupGeneralDialogue()
    {
        _allGeneralDialogue = new();

        foreach (Dialogue dialogue in npcSettings.BuySuccessDialogue.List)
            _allGeneralDialogue.Add(dialogue);

        for (int i = 0; i < maxGeneralDialogue; i++)
        {
            var randomDialogue = Random.Range(0, _allGeneralDialogue.Count);
            _explorerSelectedDialogue.Add(_allGeneralDialogue[randomDialogue]);
            _allGeneralDialogue.RemoveAt(randomDialogue);
        }
    }

    private List<Dialogue> GetCurrentDungeonDialogue()
    {
        List<Dialogue> currentDungeonDialogue = new();
        DialogueListSO dungeonDialogue = null;

        switch (GameplayManager.Instance.currentDungeonType)
        {
            case DungeonType.Lab: dungeonDialogue = labDialogue; break;
            case DungeonType.Forest: dungeonDialogue = forestDialogue; break;
            case DungeonType.Cave: dungeonDialogue = caveDialogue; break;
            case DungeonType.Glacier: dungeonDialogue = glacierDialogue; break;
            case DungeonType.Volcano: dungeonDialogue = volcanoDialogue; break;
            case DungeonType.Swamp: dungeonDialogue = swampDialogue; break;
            case DungeonType.SinisterLab: dungeonDialogue = sinisterLabDialogue; break;
        }

        foreach (Dialogue dialogue in dungeonDialogue.List)
            currentDungeonDialogue.Add(dialogue);
            
        return currentDungeonDialogue;
    }

    public override void OnItemBought()
    {
        if (ScreenManager.currentScreen == Screens.Lobby)
        {
            int dialogueIndex = 0;
            int howManyPockets = FindObjectOfType<PlayerEntryPanel>().unlockedPockets.Count;

            if (howManyPockets > 0 && howManyPockets <= 10) dialogueIndex = 0;
            else if (howManyPockets > 10 && howManyPockets <= 20) dialogueIndex = 1;
            else if (howManyPockets > 20 && howManyPockets <= 30) dialogueIndex = 2;
            else if (howManyPockets > 30 && howManyPockets <= 40) dialogueIndex = 3;
            else if (howManyPockets > 40 && howManyPockets <= 50) dialogueIndex = 4;
            else dialogueIndex = 5;

            TriggerDialogue(lobbyDialogue.List[dialogueIndex], true);
        }
        else
        {
            TriggerDialogue(_explorerSelectedDialogue[_explorerDialogueIndex]);

            _explorerDialogueIndex++;
            if (_explorerDialogueIndex >= maxDungeonDialogue + maxGeneralDialogue)
                _explorerDialogueIndex = 0;
        }

        AbortBreakIdle();
        string buyAnimation = Random.value > .5 ? "buyOne" : "buyTwo";
        PlaySuccessSellSound();
        animator.SetTrigger(buyAnimation);
    }
}
