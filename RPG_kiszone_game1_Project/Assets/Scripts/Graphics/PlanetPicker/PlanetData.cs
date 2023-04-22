using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="planet", menuName ="Planet")]
public class PlanetData : ScriptableObject
{
    public GameObject planetAsset;
    public GameObject terrainAsset;
    [SerializeField] string[] notes;
    public List<LevelLayout> levels;
    public int unlockedLevels;

    public void UnlockNextLevel(LevelLayout levelPassed)
    {
        if (levelPassed == levels[unlockedLevels - 1]) unlockedLevels = Mathf.Clamp(unlockedLevels + 1, 1, levels.Count);
    }
    public string GetNote()
    {
        string text = "Planet Information:";
        foreach (string note in notes)
            text += "\n- " + note;
        return text;
    }
}
