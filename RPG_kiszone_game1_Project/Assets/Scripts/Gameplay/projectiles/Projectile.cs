using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100;
    public float damage = 10;
    Camera gameCam;
    public enum ProjDestrMod { Destructable, Ghost, Indestructable };
    public ProjDestrMod destructionMode;

    public IEnumerator Expire()
    {
        yield return new WaitForSeconds(2f); // projectile has 2 seconds to enter game area

        // wait until projectile exits the screen
        Vector2 scrPos = gameCam.WorldToViewportPoint(transform.position);
        bool apeared = false;
        while (scrPos.x > 0f && scrPos.x < 1f && scrPos.y > 0f && scrPos.y < 1f)
        {
            yield return 0;
            scrPos = gameCam.WorldToViewportPoint(transform.position);
            apeared = true;
        }
        if (apeared) yield return new WaitForSeconds(0.5f); // wait additional to let it leave entirely
        Destroy(gameObject);
    }
    void Awake()
    {
        gameCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        speed *= GameData.GetDifficultyMulitplier(0.2f, CompareTag("Player_Projectile"));
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
        }
        else if (CompareTag("Player_Projectile") && other.CompareTag("Enemy_Projectile"))
        {
            ProjDestrMod otherDestr = other.gameObject.GetComponent<Projectile>().destructionMode;
            if (otherDestr == ProjDestrMod.Destructable && destructionMode != ProjDestrMod.Ghost)
            {
                VFXManager.CreateEffect(other.transform.position, 0, 0.3f);
                Destroy(other.gameObject);
                CameraShake.Shake(19f);
            }
            if (destructionMode == ProjDestrMod.Destructable && otherDestr != ProjDestrMod.Ghost)
            {
                VFXManager.CreateEffect(transform.position, 0, 0.3f);
                CameraShake.Shake(19f);
                Destroy(gameObject);
            }
        }
    }

}
