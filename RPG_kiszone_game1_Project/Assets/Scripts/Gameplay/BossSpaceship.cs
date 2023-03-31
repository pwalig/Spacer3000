using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossSpaceship : Spaceship
{
    [HideInInspector] public RectTransform hpBar;
    public override IEnumerator Shoot()
    {
        StartCoroutine(attacks[Random.Range(0, attacks.Count)].Perform(transform, ProjectilePrefab));
        return base.Shoot();
    }
    public override void DealDamage(float damage)
    {
        hpBar.sizeDelta = new Vector2(1500f * (hp - damage) / maxHp, 75f);
        base.DealDamage(damage);
    }
    public override void Die()
    {
        hpBar.parent.gameObject.SetActive(false);
        base.Die();
    }
}
