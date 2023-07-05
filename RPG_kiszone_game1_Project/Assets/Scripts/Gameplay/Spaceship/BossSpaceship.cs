using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossSpaceship : Spaceship
{
    [HideInInspector] public RectTransform hpBar;
    public override IEnumerator Shoot()
    {
        StartCoroutine(attacks[Random.Range(0, attacks.Count)].Perform(transform));
        return base.Shoot();
    }
    public override void DealDamage(float damage)
    {
        hpBar.sizeDelta = new Vector2(1500f * (hp - damage) / maxHp, 40f);
        base.DealDamage(damage);
    }
    public override void Die()
    {
        LevelManager.RemoveBossHpBar(hpBar.parent.gameObject);
        LevelManager.enemies.Remove(gameObject);
        LevelManager.AddToScore((int)(maxHp * GameData.GetDifficultyMulitplier(0.5f)));
        base.Die();
    }
}
