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

    // Update is called once per frame
    void Update()
    {
        if (start) StartCoroutine(Expire());
        //transform.position + transform.up * speed * Time.deltaTime;
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
