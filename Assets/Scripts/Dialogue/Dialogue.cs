using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [Tooltip("Only if it needs a name")]
    public string Name;

    [TextArea(3, 10)]
    public string[] Sentences;
}
