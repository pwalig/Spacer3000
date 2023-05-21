using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpaceship : Spaceship
{
    public int powerupChance = 0; //0-100;
    [SerializeField]
    private GameObject powerupPrefab;
    public override IEnumerator Shoot()
    {
        StartCoroutine(attacks[Random.Range(0, attacks.Count)].Perform(transform, ProjectilePrefab));
        return base.Shoot();
    }

    public override void Die()
    {
        System.Random rnd = new System.Random();
        if (rnd.Next(1, 100) * GameData.GetDifficultyMulitplier(0.5f) <= powerupChance)
        {
            UnityEngine.Debug.Log("powerup dropped");
            GameObject clonedPowerup = Instantiate(powerupPrefab, transform.position, Quaternion.Euler(0f, 0f, 0f));
        }
        LevelManager.enemies.Remove(gameObject);
        LevelManager.AddToScore((int)(maxHp * GameData.GetDifficultyMulitplier(0.5f)));
        base.Die();
    }
}
