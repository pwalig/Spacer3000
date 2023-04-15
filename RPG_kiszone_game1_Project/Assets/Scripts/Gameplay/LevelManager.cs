using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    LevelLayout level;
    public static List<GameObject> enemies = new();
    public GameObject BossHpBar;
    static List<GameObject> bossBars = new();

    private void Awake()
    {
        if (GameData.GetLevel() != null) level = GameData.GetLevel();
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
            StartCoroutine(Spawn(wave));
            if (wave.waitMode != LevelLayout.Wave.WaitMode.runNextInParallel) yield return new WaitForSeconds(wave.delayAfterIteration * wave.iterations);
            while (wave.waitMode == LevelLayout.Wave.WaitMode.untilAllKilled && enemies.Count > 0)
            {
                CheckEnemyList();
                yield return 0;
            }
            yield return new WaitForSeconds(wave.delayAfterSpawn);
        }
        GameplayManager.GameWon();
    }
    void Start()
    {
        StartCoroutine(PlayLevel());
    }

    void CheckEnemyList()
    {
        foreach (GameObject go in enemies)
            if (go == null)
                enemies.Remove(go);
    }
}
