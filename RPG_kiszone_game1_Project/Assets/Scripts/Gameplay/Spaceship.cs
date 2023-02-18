using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
public class Spaceship : MonoBehaviour
{
    SpaceshipController controller; //determines what controls the shpaceship: player or ai
    public float maxHp = 100f;
    public float hp = 100f;
    public float speed = 1f;
    public float shootDelay = 1f;

    private bool canShoot = true;
    [SerializeField]
    private GameObject ProjectilePrefab;
    [SerializeField]
    private float shootSpawn = 10f;

    ParticleSystem smoke = null;

    void Awake()
    {
        controller = GetComponent<SpaceshipController>(); //get controller
        if (controller == null) controller = gameObject.AddComponent<SpaceshipController>(); //if controller is missing create empty one
    }

    public IEnumerator Shoot()
    {
        //UnityEngine.Debug.Log("Ship: " + name + " has shot");
        GameObject clonedProjectile = Instantiate(ProjectilePrefab, transform.position + transform.up * shootSpawn, transform.rotation);

        //Create smoke effect and camera shake
        VFXManager.CreateEffect(clonedProjectile.transform.position, 2, 0.5f);
        CameraShake.Shake(30f);

        if (gameObject.tag == "Player") clonedProjectile.gameObject.tag = "Player_Projectile";
        if (gameObject.tag == "Enemy") clonedProjectile.gameObject.tag = "Enemy_Projectile";
        canShoot = false;
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }

    public void DealDamage(float damage)
    {
        hp -= damage;

        //add smoke effect: less hp left ---> more smoke
        if (smoke == null)
            smoke = Instantiate(VFXManager.effects[4], transform).GetComponentInChildren<ParticleSystem>();
        var emm = smoke.emission;
        emm.rateOverTime = (maxHp - hp) / maxHp * 10f; //z jakiegos glupiego powodu nie mozna po prostu zrobic: smoke.emission.rateOverTime = (maxHp - hp) / maxHp * 10f; - bo unity wyrzuca blad

        //destroy ship with fancy effects
        if (hp <= 0)
        {
            if (CompareTag("Player")) GameplayManager.GameOver();
            VFXManager.CreateEffect(transform.position, 1);
            VFXManager.CreateEffect(transform.position, 3);
            Destroy(gameObject);
            CameraShake.Shake(400f);
        }
    }

    void Update()
    {
        transform.position += new Vector3(controller.moveDirectionX, controller.moveDirectionY, 0).normalized * speed * Time.deltaTime; //spaceship movement
        if (controller.shoot && canShoot) StartCoroutine(Shoot());
    }

}
