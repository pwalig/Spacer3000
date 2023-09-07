using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    LevelLayout level;
    public static List<GameObject> enemies = new();
    public static int score = 0;
    public GameObject BossHpBar;
    static List<GameObject> bossBars = new();
    static GameObject RewardMenu;
    static TMP_Text scoreText;
    private static bool signal = false;

    private void Awake()
    {
        RewardMenu = GameObject.Find("RewardMenu");
        RewardMenu.SetActive(false);
        scoreText = GameObject.Find("ScoreText").GetComponent<TMP_Text>();
    }
    void Start()
    {
        if (GameData.GetLevel() != null) level = GameData.GetLevel();
        score = 0;
        StartCoroutine(PlayLevel());
    }

    public static void RemoveBossHpBar(GameObject bar)
    {
        bossBars.Remove(bar);
        Destroy(bar);
        for (int i = 0; i < bossBars.Count; i++)
        {
            bossBars[i].transform.localPosition = 40f * i * Vector3.up;
        }
    }

    IEnumerator Spawn(List<LevelLayout.Wave.InWaveEnemySpawn> enemySpawns)
    {
        foreach (LevelLayout.Wave.InWaveEnemySpawn enemySpawn in enemySpawns)
        {
            yield return new WaitForSeconds(enemySpawn.delayBeforeSpawn);

            // translate position form spawn_side + position_float to vector3
            Vector3 pos = Vector3.up * (GameplayManager.gameAreaSize.y + enemySpawn.padding);
            switch (enemySpawn.spawnSide)
            {
                case LevelLayout.Wave.InWaveEnemySpawn.SpawnSide.top:
                    pos = new Vector3(enemySpawn.position * GameplayManager.gameAreaSize.x, GameplayManager.gameAreaSize.y + enemySpawn.padding);
                    break;
                case LevelLayout.Wave.InWaveEnemySpawn.SpawnSide.left:
                    pos = new Vector3(GameplayManager.gameAreaSize.x + enemySpawn.padding, enemySpawn.position * GameplayManager.gameAreaSize.y);
                    break;
                case LevelLayout.Wave.InWaveEnemySpawn.SpawnSide.right:
                    pos = new Vector3(-GameplayManager.gameAreaSize.x - enemySpawn.padding, enemySpawn.position * GameplayManager.gameAreaSize.y);
                    break;
                case LevelLayout.Wave.InWaveEnemySpawn.SpawnSide.bottom:
                    pos = new Vector3(-enemySpawn.position * GameplayManager.gameAreaSize.x, GameplayManager.gameAreaSize.y + enemySpawn.padding);
                    break;
            }

            // spawn
            GameObject enemy = Instantiate(enemySpawn.enemyPrefab, pos, Quaternion.Euler(0f, 0f, 180f));
            if (enemy.TryGetComponent(out BossSpaceship boss))
            {
                GameObject bossBar = Instantiate(BossHpBar, GameObject.Find("BossBarsAnchor").transform);
                bossBar.transform.localPosition += 40f * bossBars.Count * Vector3.up;
                bossBars.Add(bossBar);
                TMP_Text barText = bossBar.transform.Find("BossHpBar").Find("BossName").gameObject.GetComponent<TMP_Text>();
                barText.text = enemy.name;
                if (barText.text.EndsWith("(Clone)")) barText.text = barText.text.Remove(barText.text.Length - 7);
                boss.hpBar = bossBar.transform.Find("BossHpBar").gameObject.GetComponent<RectTransform>();
                boss.DealDamage(0f);
            }
            if (enemySpawn.screenTime > 0f) StartCoroutine(enemy.GetComponent<AiEscape>().FlyAway(enemySpawn.screenTime));
            if (enemySpawn.addToMustKillList) enemies.Add(enemy);
        }
    }

    IEnumerator PlayLevel()
    {
        enemies.Clear();
        foreach (LevelLayout.Wave wave in level.waves)
        {
            yield return new WaitForSeconds(wave.delayBeforeWave);
            GameCameraContoller.SetAngle(180f - wave.camAngle);
            GameCameraContoller.SetFieldOfView(wave.camFOV <= 0 ? 13f : wave.camFOV);
            GameplayManager.SetBounds(wave.gameBoundsScale);
            StartCoroutine(Spawn(wave.enemies));

            // wait until Spawn(wave.enemies) ends
            float totalTime = 0f;
            foreach (LevelLayout.Wave.InWaveEnemySpawn spawn in wave.enemies) totalTime += spawn.delayBeforeSpawn;
            if (wave.waitMode != LevelLayout.Wave.WaitMode.runNextInParallel) yield return new WaitForSeconds(totalTime); // by default PlayLevel() doesn't wait till Spawn(wave) ends, it keeps going. This line fixes that.

            while (wave.waitMode == LevelLayout.Wave.WaitMode.untilSignal && signal == false) yield return 0;
            signal = false;
            while (wave.waitMode == LevelLayout.Wave.WaitMode.untilAllKilled && enemies.Count > 0)
            {
                bool err = false;
                try
                {
                    CheckEnemyList();
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    Debug.LogError("Wave: " + wave + ". Failed to check the list at count: " + enemies.Count);
                    Debug.LogError("Original Error: " + e.Message);
#endif
                    err = true;
                    enemies.Clear();
                }
                if (err) yield return new WaitForSeconds(enemies.Count * 4f * GameData.GetDifficultyMulitplier(1f));
                else yield return 0;
            }
            yield return new WaitForSeconds(wave.delayAfterWave);
        }
        Rewards();
        AddToScore((int)(100f * GameData.GetDifficultyMulitplier(1f)));
        level.highScore = Mathf.Max(level.highScore, score);
        GameplayManager.GameWon();
    }

    void CheckEnemyList()
    {
        foreach (GameObject go in enemies)
            if (go == null)
                enemies.Remove(go);
    }

    public static void AddToScore(int _score)
    {
        score += _score;
        if (score < 0) score = 0;
        scoreText.text = "Score: " + score;
    }

    public static void SendSignal()
    {
        signal = true;
    }

    void Rewards()
    {
        GameData.GetPlanet().UnlockNextLevel(level);

        if (level.highScore < 0f) return; //don't reward if level was finished before

        string text = "rewards:";
        RectTransform tr = RewardMenu.transform.GetChild(0).GetComponent<RectTransform>();

        foreach (Material mat in level.materialRewards)
            if (!GameData.availableMaterials.Contains(mat))
            {
                GameData.availableMaterials.Add(mat);
                text += "\nnew material: " + mat.name;
                tr.sizeDelta = new Vector2(100f, tr.sizeDelta.y + 20f);
            }
        foreach (SpaceshipGeneratorPreset spaceship in level.spaceshipRewards)
            if (!GameData.availableSpaceships.Contains(spaceship))
            {
                GameData.availableSpaceships.Add(spaceship);
                text += "\nnew ship: " + spaceship.name;
                tr.sizeDelta = new Vector2(100f, tr.sizeDelta.y + 20f);
            }
        foreach (PlanetData planet in level.planetRewards)
            if (!GameData.availablePlanets.Contains(planet))
            {
                GameData.availablePlanets.Add(planet);
                text += "\nnew planet: " + planet.name;
                tr.sizeDelta = new Vector2(100f, tr.sizeDelta.y + 20f);
            }

        RewardMenu.SetActive(true);
        RewardMenu.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = text;
    }
}
