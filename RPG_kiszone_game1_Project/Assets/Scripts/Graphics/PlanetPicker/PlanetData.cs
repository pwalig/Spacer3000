using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="planet", menuName ="Planet")]
public class PlanetData : ScriptableObject
{
    public GameObject planetAsset;
    public GameObject terrainAsset;
    public string note;
    public List<LevelLayout> levels;
    public int unlockedLevels;

    public void UnlockNextLevel()
    {
        unlockedLevels = Mathf.Clamp(unlockedLevels + 1, 1, levels.Count);
    }
}