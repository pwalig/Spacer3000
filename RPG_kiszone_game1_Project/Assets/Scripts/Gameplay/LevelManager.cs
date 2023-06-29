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
            bossBars[i].transform.localPosition = Vector3.up * 40f * i;
        }
    }

    IEnumerator Spawn(LevelLayout.Wave wave)
    {
        for (int i = 0; i < wave.iterations; i++)
        {
            GameObject enemy = Instantiate(wave.enemyPrefab, wave.position + (wave.deltaPosition * i), Quaternion.Euler(0f, 0f, 180f));
            if (enemy.TryGetComponent(out BossSpaceship boss))
            {
                GameObject bossBar = Instantiate(BossHpBar, GameObject.Find("BossBarsAnchor").transform);
                bossBar.transform.localPosition += Vector3.up * 40f * bossBars.Count;
                bossBars.Add(bossBar);
                TMP_Text barText = bossBar.transform.Find("BossHpBar").Find("BossName").gameObject.GetComponent<TMP_Text>();
                barText.text = enemy.name;
                if (barText.text.EndsWith("(Clone)")) barText.text = barText.text.Remove(barText.text.Length - 7);
                boss.hpBar = bossBar.transform.Find("BossHpBar").gameObject.GetComponent<RectTransform>();
                boss.DealDamage(0f);
            }
            if (wave.screenTime > 0f) StartCoroutine(enemy.GetComponent<AiEscape>().FlyAway(wave.screenTime));
            if (wave.addToMustKillList) enemies.Add(enemy);
            yield return new WaitForSeconds(wave.delayAfterIteration);
        }
    }

    IEnumerator PlayLevel()
    {
        foreach (LevelLayout.Wave wave in level.waves)
        {
            GameCameraContoller.SetAngle(180f - wave.camAngle);
            GameCameraContoller.SetFieldOfView(wave.camFOV == 0 ? 13f : wave.camFOV);
            GameplayManager.SetBounds(wave.gameBoundsScale);
            StartCoroutine(Spawn(wave));
            if (wave.waitMode != LevelLayout.Wave.WaitMode.runNextInParallel) yield return new WaitForSeconds(wave.delayAfterIteration * wave.iterations);
            while (wave.waitMode == LevelLayout.Wave.WaitMode.untilAllKilled && enemies.Count > 0)
            {
                bool err = false;
                try
                {
                    CheckEnemyList();
                }
                catch
                {
#if UNITY_EDITOR
                    Debug.LogError("Wave: " + wave + ". Failed to check the list at count: " + enemies.Count);
#endif
                    err = true;
                    enemies.Clear();
                }
                if (err) yield return new WaitForSeconds(enemies.Count * 3f * GameData.GetDifficultyMulitplier(1f));
                else yield return 0;
            }
            yield return new WaitForSeconds(wave.delayAfterSpawn);
        }
        Rewards();
        AddToScore((int)(100f * GameData.GetDifficultyMulitplier(2f)));
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
