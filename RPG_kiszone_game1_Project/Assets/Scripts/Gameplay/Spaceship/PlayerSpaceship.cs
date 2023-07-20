using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpaceship : Spaceship
{
    public int projectiles = 1;
    private float takenDmgMulti = 1f;
    RectTransform hpBar;

    private void Start()
    {
        hpBar = GameObject.Find("PlayerHpBar").GetComponent<RectTransform>();
    }

    public override IEnumerator Shoot()
    {
        StartCoroutine(attacks[Mathf.Clamp(projectiles-1, 0, attacks.Count-1)].Perform(transform));
        return base.Shoot();
    }

    IEnumerator PowerupExp(float time, int proj, float addSpeed, float shield)
    {
        yield return new WaitForSeconds(time * GameData.GetDifficultyMulitplier(0.4f, true));
        takenDmgMulti = Mathf.Clamp01(takenDmgMulti + shield);
        speed -= addSpeed * GameData.GetDifficultyMulitplier(0.2f, true);
        projectiles -= proj;
    }

    public void Powerup(float time, int proj = 1, float addSpeed = 0f, float shield = 0f)
    {
        projectiles += proj;
        takenDmgMulti = Mathf.Clamp01(takenDmgMulti - (shield * GameData.GetDifficultyMulitplier(0.2f, true)));
        speed += addSpeed * GameData.GetDifficultyMulitplier(0.2f, true);
        StartCoroutine(PowerupExp(time, proj, addSpeed, shield));
    }

    public override void DealDamage(float damage)
    {
        if (damage > 0f) damage *= takenDmgMulti;
        base.DealDamage(damage);
        hpBar.sizeDelta = new Vector2(250f * hp / maxHp, 50f);
    }
}
