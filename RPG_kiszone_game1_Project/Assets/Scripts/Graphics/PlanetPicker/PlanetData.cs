using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlanetSaveableData
{
    public int unlockedLevels;
    public float[] highScores;
}

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

    public PlanetSaveableData GetSaveableData()
    {
        PlanetSaveableData dat = new();
        dat.unlockedLevels = unlockedLevels;
        dat.highScores = new float[levels.Count];
        for(int i = 0; i<levels.Count; i++)
        {
            dat.highScores[i] = levels[i].highScore;
        }
        return dat;
    }
    public void ReadSaveableData(PlanetSaveableData dat)
    {
        unlockedLevels = dat.unlockedLevels;
        for (int i = 0; i < levels.Count && i < dat.highScores.Length; i++)
        {
             levels[i].highScore = dat.highScores[i];
        }
    }
}
