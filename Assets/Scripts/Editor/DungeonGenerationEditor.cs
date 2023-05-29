using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractDungeonGenerator), true)]
public class DungeonGeneratorEditor : Editor
{
    AbstractDungeonGenerator generator;

    private void Awake()
    {
        generator = (AbstractDungeonGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Create Dungeon"))
        {
            if (Application.isPlaying)
            {
                GameplayManager.Instance.StartNextDungeon();
                return;
            }
            generator.RunProceduralGeneration(generator.testConfig);
        }
    }
}
