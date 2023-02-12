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
        if (gameObject.tag == "Player_Projectile")
        {
            if (other.gameObject.tag == "Enemy")
            {
                VFXManager.CreateEffect(transform.position, 0.3f, 0);
                CameraShake.Shake(30f);
                other.gameObject.GetComponent<Spaceship>().hp -= damage;
                if (other.gameObject.GetComponent<Spaceship>().hp <= 0)
                {
                    Destroy(other.gameObject);
                    VFXManager.CreateEffect(other.transform.position, 1f, 1);
                    CameraShake.Shake(400f);
                }
                Destroy(gameObject);
            }
        }
        if (gameObject.tag == "Enemy_Projectile")
        {
            if (other.gameObject.tag == "Player")
            {
                VFXManager.CreateEffect(transform.position, 0.3f, 0);
                CameraShake.Shake(30f);
                other.gameObject.GetComponent<Spaceship>().hp -= damage;
                if (other.gameObject.GetComponent<Spaceship>().hp <= 0)
                {
                    Destroy(other.gameObject);
                    VFXManager.CreateEffect(other.transform.position, 1f, 1);
                    CameraShake.Shake(400f);
                }
                Destroy(gameObject);
            }
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
