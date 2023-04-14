using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class LevelLayout : ScriptableObject
{
    [System.Serializable]
    public class Wave
    {
        public string name; // Name of the spawn. Does nothing practically. Only helps not to get lost while creating a level.
        public GameObject enemyPrefab;
        public Vector3 position = new Vector3(0f, 150f, 0f);
        public int iterations = 1;
        public float delayAfterIteration = 2f;
        public Vector3 deltaPosition; // Shift position of next enemies in the spawn.
        public float screenTime; // Time until enemy flies away. Set to 0 or below to make enemy stay until it is killed.
        public bool addToMustKillList;
        public enum WaitMode { untilSpawnEnd, untilAllKilled, runNextInParallel }
        public WaitMode waitMode;
        public float delayAfterSpawn; // Time to next spawn in seconds.
    }
    public List<Wave> waves;
}
