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
        yield return new WaitForSeconds(lifespan * GameData.GetDifficultyMulitplier(1f, true));
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.AddToScore((int)(10f * GameData.GetDifficultyMulitplier(0.2f)));
            other.gameObject.GetComponent<PlayerSpaceship>().Powerup();
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (start) StartCoroutine(Expire());
        //transform.position + transform.up * speed * Time.deltaTime;
        transform.Translate(-1 * GameData.GetDifficultyMulitplier(1f) * speed * Time.deltaTime * Vector3.up);
    }
}
