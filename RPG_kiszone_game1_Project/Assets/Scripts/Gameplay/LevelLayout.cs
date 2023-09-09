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
        public float delayBeforeWave = 0f;
        public float gameBoundsScale = 1f;
        public float camFOV = 13f;
        public float camAngle = 0f;

        [System.Serializable]
        public class InWaveEnemySpawn
        {
            public float delayBeforeSpawn;
            public GameObject enemyPrefab;
            public enum SpawnSide { top, left, right, bottom };
            public SpawnSide spawnSide;
            public float position; // value between -1 and 1 adjusts to gameAreaSize automatically
            public float padding = 30f; // increase if enenemy spawns within camera range
            public float rotation;
            public bool flipHorizontal;
            public float screenTime; // Time until enemy flies away. Set to 0 or below to make enemy stay until it is killed.
            public bool addToMustKillList = true;
        }
        public List<InWaveEnemySpawn> enemies;

        public enum WaitMode { untilSpawnEnd, untilMustKillListEmpty, runNextInParallel, untilNoEnemies, untilSignal }
        public WaitMode waitMode;
        public float delayAfterWave; // Time to next wave in seconds.
    }
    public List<Wave> waves;

    public float highScore = 0f;

    [SerializeField] string[] notes; // level description displayed in level selection menu

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
        text += "\n\nHigh Score: " + highScore;
        return text;
    }
}
