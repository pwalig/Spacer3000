using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpaceship : Spaceship
{
    [System.Serializable]
    public class PowerupDrop
    {
        [Tooltip("chance of getting powerup represented as % (value from 0 to 100). Chances should add up to 100")]
        public float chance = 0;
        [Tooltip("can be left empty to reflect chance of getting nothing")]
        public GameObject prefab;
    }
    [SerializeField] List<PowerupDrop> powerups;

    public override void Die()
    {
        // Powerup Drop
        float sum = 0f;
        foreach (PowerupDrop pd in powerups) sum += pd.chance; // check if chances add up to 100
        if (sum > 100.00001f) foreach (PowerupDrop pd in powerups) pd.chance *= 100f / sum; //normalize chances
        float rand = Random.Range(0f, 100f);
        if (rand <= sum)
        {
            int powerupId = 0;
            rand -= powerups[powerupId].chance;
            while (rand > 0f)
            {
                powerupId++;
                rand -= powerups[powerupId].chance;
            }
            if (powerups[powerupId].prefab != null) Instantiate(powerups[powerupId].prefab, transform.position, Quaternion.identity);
        }

        LevelManager.enemies.Remove(gameObject);
        LevelManager.AddToScore((int)(maxHp * GameData.GetDifficultyMulitplier(0.5f)));
        base.Die();
    }
}
