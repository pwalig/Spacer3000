using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100;
    public float damage = 10;
    public float lifespan = 4f;
    private bool start = true;
    public bool projectile_destroy = false;
    // Start is called before the first frame update
    public IEnumerator Expire()
    {
        start = false;
        yield return new WaitForSeconds(lifespan);
        Destroy(gameObject);
    }
    void Start()
    {
        speed *= GameData.GetDifficultyMulitplier(0.2f, CompareTag("Player_Projectile"));
        lifespan *= GameData.GetDifficultyMulitplier(0.2f, !CompareTag("Player_Projectile"));
        StartCoroutine(Expire());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((CompareTag("Player_Projectile") && other.CompareTag("Enemy")) || (CompareTag("Enemy_Projectile") && other.CompareTag("Player")))
        {
            VFXManager.CreateEffect(transform.position, 0, 0.3f);
            CameraShake.Shake(30f);
            if (other.CompareTag("Player")) other.gameObject.GetComponent<Spaceship>().DealDamage(damage * GameData.GetDifficultyMulitplier(2f));
            else other.gameObject.GetComponent<Spaceship>().DealDamage(damage * GameData.GetDifficultyMulitplier(2f, true));
            Destroy(gameObject);
        }else if (CompareTag("Player_Projectile") && other.CompareTag("Enemy_Projectile") && (projectile_destroy == true || other.gameObject.GetComponent<Projectile>().projectile_destroy == true))
        {
            VFXManager.CreateEffect(transform.position, 0, 0.3f);
            VFXManager.CreateEffect(other.transform.position, 0, 0.3f);
            CameraShake.Shake(30f);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

}
