using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScroll : MonoBehaviour
{
    public GameObject nextUp;
    public Vector2 objectSize = Vector2.one * 500f;
    public Vector3 speed = Vector3.up * -100f;

    Camera gameCam;
    bool spawn = false;
    private void Start()
    {
        if (nextUp == null)
            nextUp = gameObject;

        gameCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        spawn = false;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime;

        if (!spawn && gameCam.WorldToViewportPoint(objectSize.y / 2f * Vector3.up + transform.position).y < 2f && transform.position.y < objectSize.y)
        {
            Instantiate(nextUp, transform.position - (speed.normalized * objectSize.y), transform.rotation);
            spawn = true;
        }

        if (gameCam.WorldToViewportPoint(objectSize.y / 2f * Vector3.up + transform.position).y < -1f && transform.position.y < -objectSize.y)
        {
            if (!spawn) Instantiate(nextUp, transform.position - (speed.normalized * objectSize.y), transform.rotation);
            Destroy(gameObject);
        }
    }
}
