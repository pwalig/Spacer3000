using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScroll : MonoBehaviour
{
    public GameObject nextUp;
    public GameObject nextSides;
    public Vector2 objectSize = Vector2.one * 500f;
    public Vector3 speed = Vector3.up * -100f;

    Camera gameCam;
    bool spawn = false;
    bool spawnSides = false;
    private void Start()
    {
        if (nextUp == null)
            nextUp = gameObject;
        if (nextSides == null)
            nextSides = gameObject;

        gameCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        spawn = false;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime;

        //spawn up
        if (!spawn && Mathf.Abs(transform.position.x) < objectSize.x / 4f && gameCam.WorldToViewportPoint(objectSize.y / 2f * Vector3.up + transform.position).y < 2f && transform.position.y < objectSize.y)
        {
            Instantiate(nextUp, transform.position - (speed.normalized * objectSize.y), transform.rotation);
            spawn = true;
        }

        //goaway
        if (gameCam.WorldToViewportPoint(objectSize.y / 2f * Vector3.up + transform.position).y < -1f && transform.position.y < -objectSize.y)
        {
            if (!spawn && Mathf.Abs(transform.position.x) < objectSize.x / 4f) Instantiate(nextUp, transform.position - (speed.normalized * objectSize.y), transform.rotation);
            Destroy(gameObject);
        }

        //spawn sides
        if (!spawnSides && Mathf.Abs(transform.position.x) < objectSize.x / 4f && gameCam.WorldToViewportPoint(objectSize.x / 2f * Vector3.right + transform.position).x < 1.1f && transform.position.y < objectSize.y)
        {
            Instantiate(nextSides, transform.position - (Vector3.right * objectSize.x), transform.rotation);
            Instantiate(nextSides, transform.position + (Vector3.right * objectSize.x), transform.rotation);
            spawnSides = true;
        }
    }
}
