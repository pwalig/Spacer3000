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

    public override IEnumerator Shoot(int attack_id)
    {
        attack_id = Mathf.Clamp(projectiles-1, 0, attacks.Count-1);
        return base.Shoot(attack_id);
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
        if (!GameplayManager.immortality) base.DealDamage(damage);
        Powerup(1f * GameData.GetDifficultyMulitplier(0.2f, true), 0, 0, 0.5f * GameData.GetDifficultyMulitplier(0.2f, true)); // system preventing instant death when coliding with swarm of projectiles
        hpBar.sizeDelta = new Vector2(250f * hp / maxHp, 50f);
    }
}
