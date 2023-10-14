using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OneShoot
{
    public GameObject projectile;
    public Vector3 position;
    public float rotation;
    public Texture2D pattern;
    [Tooltip("Delay after in seconds")]
    public float time;
    public float TotalTime()
    {
        if (pattern != null)
        {
            float sum = 0f;
            for (int k = 0; k < pattern.height; k++)
            {
                for (int j = 0; j < pattern.width; j++)
                {
                    sum += pattern.GetPixel(j, k).g;
                }
            }
            return sum * time;
        }
        else
        {
            return time;
        }
    }

    public IEnumerator ReadPattern(Transform host)
    {
        for (int i = 0; i < pattern.height; i++)
        {
            for(int j = 0; j < pattern.width; j++)
            {
                Color data = pattern.GetPixel(j, i);
                if (data.b > 0.5f)
                {
                    CameraShake.Shake(30f);
                    GameObject clonedProjectile = GameObject.Instantiate(projectile, host.position + (host.transform.rotation * new Vector3((j - ((pattern.width - 1) / 2f)) * position.x * Mathf.Sign(host.localScale.x), position.y, position.z)), host.rotation);
                    VFXManager.CreateEffect(clonedProjectile.transform.position, 2, 0.5f);
                    clonedProjectile.transform.Rotate((data.r - 0.5f) * Mathf.Sign(host.localScale.x) * rotation * Vector3.forward);
                    if (clonedProjectile.GetComponent<Projectile>() != null) clonedProjectile.tag = host.tag + "_Projectile";
                }
                if (data.g > 0f) yield return new WaitForSeconds(data.g * time);
            }
        }
    }
}

[System.Serializable]
public class AttackPattern //class describing one attack
{
    public string name;
    public int iterations; //how may times to repeat the attack pattern

    public List<OneShoot> projectiles;
    public float TotalTime()
    {
        float sum = 0f;
        foreach (OneShoot os in projectiles) sum += os.TotalTime();
        return sum * iterations;
    }
    public IEnumerator Perform(Transform host)
    {
        for (int i = 0; i < iterations; i++)
        {
            foreach (OneShoot shoot in projectiles)
            {
                if (shoot.pattern != null)
                {
                    host.gameObject.GetComponent<Spaceship>().StartCoroutine(shoot.ReadPattern(host));
                }
                else
                {
                    CameraShake.Shake(30f);
                    GameObject clonedProjectile = GameObject.Instantiate(shoot.projectile, host.position + (host.transform.rotation * shoot.position), host.rotation);
                    VFXManager.CreateEffect(clonedProjectile.transform.position, 2, 0.5f);
                    clonedProjectile.transform.Rotate(Mathf.Sign(host.localScale.x) * shoot.rotation * Vector3.forward);
                    if (clonedProjectile.GetComponent<Projectile>() != null) clonedProjectile.tag = host.tag + "_Projectile";
                }
                yield return new WaitForSeconds(shoot.TotalTime());
            }
        }
    }
}
public class Spaceship : MonoBehaviour
{
    SpaceshipController controller; //determines what controls the shpaceship: player or ai
    public float maxHp = 100f;
    [HideInInspector] public float hp;
    public float speed = 20f;
    public float responsiveness = 1000f; //Spaceship's acceleration and deacceleration. Works only if GameplayManager.movement_mode == true;
    public AnimationCurve escapeForce;
    public float shootDelay = 1f;
    public List<AttackPattern> attacks;

    [HideInInspector] public bool canShoot = true;
    //[SerializeField]
    //private float shootSpawn = 10f;

    ParticleSystem smoke = null;

    Vector3 currentSpeed = Vector3.zero;

    void Awake()
    {
        hp = maxHp;
        controller = GetComponent<SpaceshipController>(); //get controller
        if (controller == null) controller = gameObject.AddComponent<SpaceshipController>(); //if controller is missing create empty one
        shootDelay *= GameData.GetDifficultyMulitplier(0.2f, !CompareTag("Player"));
    }

    public virtual IEnumerator Shoot(int attack_id)
    {
        // in child class override this member and implement shooting logic and return base.Shoot(); at the end
        if (attack_id < 0) attack_id = Random.Range(0, attacks.Count);
        AttackPattern attack = attacks[attack_id];
        StartCoroutine(attack.Perform(transform));
        yield return new WaitForSeconds(attack.TotalTime());
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }

    public virtual void DealDamage(float damage)
    {
        hp -= damage;
        if (hp > maxHp) hp = maxHp;

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
        // ANTI COLISION SYSTEM
        //setup: find spaceships, determine max distance, declare AddMove
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float maxDist = escapeForce.keys[escapeForce.keys.Length - 1].time;
        Vector3 AddMove = Vector3.zero;

        //check player proximity
        Vector3 dist = Quaternion.Inverse(transform.rotation) * (GameplayManager.GetPlayerPosition() - transform.position);
        if (dist.magnitude > 0f && dist.magnitude < maxDist) AddMove -= escapeForce.Evaluate(dist.magnitude) * dist.normalized;

        //check enemies proximity
        for (int i = 0; i < enemies.Length; i++)
        {
            dist = Quaternion.Inverse(transform.rotation) * (enemies[i].transform.position - transform.position);
            if (dist.magnitude > 0f && dist.magnitude < maxDist) AddMove -= escapeForce.Evaluate(dist.magnitude) * dist.normalized;
        }
        //speed influence
        float factor = Mathf.Clamp(new Vector2(controller.moveDirectionX, controller.moveDirectionY).magnitude / 2f + 0.5f, 0.01f, 1f); //slower moving ships should recieve less effect to prevent jittering
        //add effect
        controller.moveDirectionX += AddMove.x * factor;
        controller.moveDirectionY += AddMove.y * factor;

        // MOVEMENT
        Vector3 moveDirection = transform.rotation * new Vector3(controller.moveDirectionX, controller.moveDirectionY, 0);

        if (GameplayManager.movementDirectionNormalize || moveDirection.magnitude > 1f) moveDirection.Normalize(); //disable ability to move slowly granted by gamepads and joysticks. To activate press N while GameplayManager is present.

        if (GameplayManager.movementMode) //Alternative way to move ship involving acceleration and deacceleration. To deactivate press M while GameplayManager is present.
        {
            if (moveDirection.magnitude > 0.05f)
            {
                currentSpeed += responsiveness * Time.deltaTime * moveDirection;
            }
            else
            {
                if (responsiveness * Time.deltaTime <= (currentSpeed - Vector3.zero).magnitude) currentSpeed -= responsiveness * Time.deltaTime * currentSpeed.normalized;
                else currentSpeed = Vector3.zero;
            }
            if (currentSpeed.magnitude > speed) currentSpeed = currentSpeed.normalized * speed;
            transform.position += currentSpeed * Time.deltaTime;
        }
        else transform.position += speed * Time.deltaTime * moveDirection; //spaceship movement

        if (controller.shoot && canShoot)
        {
            canShoot = false;
            StartCoroutine(Shoot(controller.attack));
        }
    }

}
