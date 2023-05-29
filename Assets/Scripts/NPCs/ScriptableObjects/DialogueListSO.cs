using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue List", menuName = "DialogueList")]
public class DialogueListSO : ScriptableObject
{
    public List<Dialogue> List;

    public Dialogue GetRandomDialogueFromList()
    {
        var randomDialogue = Random.Range(0, List.Count);
        return List[randomDialogue];
    }
}