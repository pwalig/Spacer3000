using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

[System.Serializable]
public struct OneShoot
{
    public GameObject projectile;
    public Vector3 position;
    public float rotation;
    public float time;
}

[System.Serializable]
public class AttackPattern //class describing one attack
{
    public string name;
    public int iterations; //how may times to repeat the attack pattern
    public List<OneShoot> projectiles;
    public IEnumerator Perform(Transform host)
    {
        for (int i = 0; i < iterations; i++)
        {
            foreach (OneShoot shoot in projectiles)
            {
                CameraShake.Shake(30f);
                GameObject clonedProjectile = GameObject.Instantiate(shoot.projectile, host.position + shoot.position, host.rotation);
                VFXManager.CreateEffect(clonedProjectile.transform.position, 2, 0.5f);
                clonedProjectile.transform.Rotate(Vector3.forward * shoot.rotation);
                clonedProjectile.gameObject.tag = host.tag + "_Projectile";
                yield return new WaitForSeconds(shoot.time);
            }
        }
    }
}
public class Spaceship : MonoBehaviour
{
    SpaceshipController controller; //determines what controls the shpaceship: player or ai
    public float maxHp = 100f;
    public float hp = 100f;
    public float speed = 20f;
    public float responsiveness = 1000f; //Spaceship's acceleration and deacceleration. Works only if GameplayManager.movement_mode == true;
    public float shootDelay = 1f;
    public List<AttackPattern> attacks;

    bool canShoot = true;
    //[SerializeField]
    //private float shootSpawn = 10f;

    ParticleSystem smoke = null;

    Vector3 currentSpeed = Vector3.zero; 

    void Awake()
    {
        controller = GetComponent<SpaceshipController>(); //get controller
        if (controller == null) controller = gameObject.AddComponent<SpaceshipController>(); //if controller is missing create empty one
        shootDelay *= GameData.GetDifficultyMulitplier(0.2f, !CompareTag("Player"));
    }

    public virtual IEnumerator Shoot()
    {
        // in child class override this member and implement shooting logic before return base.Shoot();
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }

    public virtual void DealDamage(float damage)
    {
        hp -= damage;

        //add smoke effect: less hp left ---> more smoke
        if (smoke == null)
            smoke = Instantiate(VFXManager.effects[4], transform).GetComponentInChildren<ParticleSystem>();
        var emm = smoke.emission;
        emm.rateOverTime = (maxHp - hp) / maxHp * 10f; //z jakiegos glupiego powodu nie mozna po prostu zrobic: smoke.emission.rateOverTime = (maxHp - hp) / maxHp * 10f; - bo unity wyrzuca blad
        LevelManager.AddToScore((int)(Mathf.Clamp(damage, 0f, maxHp) * GameData.GetDifficultyMulitplier(0.1f) * (CompareTag("Player") ? -0.5f : 1f)));

        if (hp <= 0) Die();
    }

    public virtual void Die()
    {
        //destroy ship with fancy effects
        if (CompareTag("Player")) GameplayManager.GameOver();
        VFXManager.CreateEffect(transform.position, 1);
        VFXManager.CreateEffect(transform.position, 3);
        Destroy(gameObject);
        CameraShake.Shake(400f);
    }


    void Update()
    {
        Vector3 moveDirection = new Vector3(controller.moveDirectionX, controller.moveDirectionY, 0);
        if (GameplayManager.movementDirectionNormalize || moveDirection.magnitude > 1f) moveDirection.Normalize(); //disable ability to move slowly granted by gamepads and joysticks. To activate press N while GameplayManager is present.

        if (GameplayManager.movementMode) //Alternative way to move ship involving acceleration and deacceleration. To activate press M while GameplayManager is present.
        {
            if (moveDirection.magnitude > 0.05f)
            {
                currentSpeed += moveDirection * responsiveness * Time.deltaTime;
            }
            else
            {
                if (responsiveness * Time.deltaTime <= (currentSpeed - Vector3.zero).magnitude) currentSpeed -= currentSpeed.normalized * responsiveness * Time.deltaTime;
                else currentSpeed = Vector3.zero;
            }
            if (currentSpeed.magnitude > speed) currentSpeed = currentSpeed.normalized * speed;
            transform.position += currentSpeed * Time.deltaTime;
        }
        else transform.position += moveDirection * speed * Time.deltaTime; //spaceship movement

        if (controller.shoot && canShoot)
        {
            canShoot = false;
            StartCoroutine(Shoot());
        }
    }

}
