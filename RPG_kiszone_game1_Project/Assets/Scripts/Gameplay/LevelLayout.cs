using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class LevelLayout : ScriptableObject
{
    [System.Serializable]
    public class Wave
    {
        public string name; // Name of the wave. Does nothing practically. Only helps not to get lost while creating a level.
        public GameObject enemyPrefab;
        public Vector3 position = new Vector3(0f, 150f, 0f);
        public int iterations = 1;
        public float delayAfterIteration = 2f;
        public Vector3 deltaPosition; // Shift position of next enemies in the wave.
        public float screenTime; // Time until enemy flies away. Set to 0 or below to make enemy stay until it is killed.
        public bool addToMustKillList;
        public enum WaitMode { untilSpawnEnd, untilAllKilled, runNextInParallel }
        public WaitMode waitMode;
        public float delayAfterSpawn; // Time to next wave in seconds.
    }
    public List<Wave> waves;

    [SerializeField] string[] notes;

    [Header("Rewards")]
    //public List<dynamic> rewards;
    public List<Material> materialRewards;
    public List<SpaceshipGeneratorPreset> spaceshipRewards;
    public List<PlanetData> planetRewards;

    public List<string> GetRewardList()
    {
        List<string> rewList = new();
        foreach (Material mat in materialRewards)
            rewList.Add(mat.name + " [material]");
        foreach (SpaceshipGeneratorPreset spaceship in spaceshipRewards)
            rewList.Add(spaceship.name + " [ship]");
        foreach (PlanetData planet in planetRewards)
            rewList.Add(planet.name + " [planet]");
        return rewList;
    }

    public string GetNote()
    {
        string text = "Mission Details:";
        foreach(string note in notes)
            text += "\n- " + note;
        text += "\n\nRewards:";
        foreach (string reward in GetRewardList())
            text += "\n- " + reward;
        return text;
    }
}
