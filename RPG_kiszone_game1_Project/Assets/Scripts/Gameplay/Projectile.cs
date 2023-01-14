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
                other.gameObject.GetComponent<Spaceship>().hp -= damage;
                if (other.gameObject.GetComponent<Spaceship>().hp <= 0)
                {
                    Destroy(other.gameObject);
                }
                Destroy(gameObject);
            }
        }
        if (gameObject.tag == "Enemy_Projectile")
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<Spaceship>().hp -= damage;
                if (other.gameObject.GetComponent<Spaceship>().hp <= 0)
                {
                    Destroy(other.gameObject);
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
