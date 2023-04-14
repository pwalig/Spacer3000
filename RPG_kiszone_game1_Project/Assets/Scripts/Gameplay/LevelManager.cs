using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelLayout level;
    public static List<GameObject> enemies = new();
    public GameObject BossHpBar;
    static List<GameObject> bossBars = new();

    public static void RemoveBossHpBar(GameObject bar)
    {
        bossBars.Remove(bar);
        Destroy(bar);
        for (int i = 0; i < bossBars.Count; i++)
        {
            bossBars[i].transform.localPosition = Vector3.up * 75f * i;
        }
    }

    IEnumerator Spawn(LevelLayout.Wave wave)
    {
        for (int i = 0; i < wave.iterations; i++)
        {
            GameObject enemy = Instantiate(wave.enemyPrefab, wave.position + (wave.deltaPosition * i), Quaternion.Euler(0f, 0f, 180f));
            BossSpaceship boss;
            if (enemy.TryGetComponent(out boss))
            {
                GameObject bossBar = Instantiate(BossHpBar, GameObject.Find("BossBarsAnchor").transform);
                bossBar.transform.localPosition += Vector3.up * 75f * bossBars.Count;
                bossBars.Add(bossBar);
                bossBar.transform.Find("BossHpBar").Find("BossName").gameObject.GetComponent<TMP_Text>().text = enemy.name;
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
            while (wave.waitMode == LevelLayout.Wave.WaitMode.untilAllKilled && enemies.Count > 0) yield return 0;
            yield return new WaitForSeconds(wave.delayAfterSpawn);
        }
        GameplayManager.GameWon();
    }
    void Start()
    {
        StartCoroutine(PlayLevel());
    }
}
