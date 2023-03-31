using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpaceship : Spaceship
{
    public int projectiles = 1;
    RectTransform hpBar;

    private void Start()
    {
        hpBar = GameObject.Find("PlayerHpBar").GetComponent<RectTransform>();
    }

    public override IEnumerator Shoot()
    {
        StartCoroutine(attacks[projectiles-1].Perform(transform, ProjectilePrefab));
        return base.Shoot();
    }

    public void Powerup()
    {
        projectiles += 1;
    }

    public override void DealDamage(float damage)
    {
        hpBar.sizeDelta = new Vector2(250f * (hp - damage) / maxHp, 50f);
        base.DealDamage(damage);
    }
}
