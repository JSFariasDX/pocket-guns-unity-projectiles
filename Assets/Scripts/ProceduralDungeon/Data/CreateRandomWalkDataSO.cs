using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleRandomWalkParameters_",menuName = "PCG/SimpleRandomWalkData")]
public class CreateRandomWalkDataSO : ScriptableObject
{
    public Size size;
    public int iterations = 10, walkLength = 10, limitDistanceFromCenter = 0;
    public bool startRandomlyEachIteration = true;
}

public enum Size
{
    Small, Medium, Big
}
