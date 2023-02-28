using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
        public float speed = 100;
        public float lifespan = 4f;
        private bool start = true;


    public IEnumerator Expire()
    {
        start = false;
        yield return new WaitForSeconds(lifespan);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Spaceship>().Powerup();
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (start) StartCoroutine(Expire());
        //transform.position + transform.up * speed * Time.deltaTime;
        transform.Translate(Vector3.up * speed * Time.deltaTime * -1);
    }
}
