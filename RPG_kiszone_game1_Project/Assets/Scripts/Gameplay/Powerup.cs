using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public float speed = 100;
    public float duration = 7f; // time in seconds when projectiles and shield will be granted
    public int projectiles = 0;
    public float heal = 0f;
    public float shield = 0f; // only values <0f; 1f> make sense
    public float shipSpeed = 0f;

    /* public float lifespan = 4f; old code for destroying powerup

    private void Start() 
    {
        StartCoroutine(Expire());
    }
    public IEnumerator Expire()
    {
        yield return new WaitForSeconds(lifespan * GameData.GetDifficultyMulitplier(1f, true));
        Destroy(gameObject);
    }*/

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.AddToScore((int)(10f * GameData.GetDifficultyMulitplier(0.2f)));
            PlayerSpaceship playerSpaceship = other.gameObject.GetComponent<PlayerSpaceship>();
            if (projectiles > 0) playerSpaceship.Powerup(duration, projectiles, shipSpeed, shield);
            if (heal > 0f) playerSpaceship.DealDamage(-heal);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        //transform.position + transform.up * speed * Time.deltaTime;
        transform.Translate(-1 * GameData.GetDifficultyMulitplier(0.2f) * speed * Time.deltaTime * Vector3.up);
        if (transform.position.y <= -GameplayManager.gameAreaSize.y - 25f) Destroy(gameObject); // if powerup goes out of screen destroy it
    }
}
