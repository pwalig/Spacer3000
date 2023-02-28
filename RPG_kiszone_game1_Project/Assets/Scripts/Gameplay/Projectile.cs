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
    // Start is called before the first frame update
    public IEnumerator Expire()
    {
        start = false;
        yield return new WaitForSeconds(lifespan);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((CompareTag("Player_Projectile") && other.tag == "Enemy") || (CompareTag("Enemy_Projectile") && other.tag == "Player"))
        {
            VFXManager.CreateEffect(transform.position, 0, 0.3f);
            CameraShake.Shake(30f);
            other.gameObject.GetComponent<Spaceship>().DealDamage(damage);
            Destroy(gameObject);
        }else if (CompareTag("Player_Projectile") && other.tag == "Enemy_Projectile" && GameplayManager.projectile_destroy == true)
        {
            VFXManager.CreateEffect(transform.position, 0, 0.3f);
            VFXManager.CreateEffect(other.transform.position, 0, 0.3f);
            CameraShake.Shake(30f);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (start) StartCoroutine(Expire());
        //transform.position + transform.up * speed * Time.deltaTime;
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
