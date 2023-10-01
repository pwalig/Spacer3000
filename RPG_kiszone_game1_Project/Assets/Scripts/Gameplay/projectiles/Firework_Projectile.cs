using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework_Projectile : Projectile
{
    public GameObject projectile;
    public int amount = 8;
    public float time = 2f;

    void Explode()
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject clonedProjectile = Instantiate(projectile, transform.position, Quaternion.Euler(0f, 0f, i * 360f / amount));
            VFXManager.CreateEffect(clonedProjectile.transform.position, 2, 0.5f);
            if (clonedProjectile.GetComponent<Projectile>() != null) clonedProjectile.tag = tag;
        }
        VFXManager.CreateEffect(transform.position, 0, 0.3f);
        CameraShake.Shake(30f);
        Destroy(gameObject);
    }
    IEnumerator ExplosionCountDown()
    {
        yield return new WaitForSeconds(time);
        Explode();
    }
    private void Start()
    {
        StartCoroutine(ExplosionCountDown());
    }
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.up);
    }
}
