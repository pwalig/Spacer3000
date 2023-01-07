using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScroll : MonoBehaviour
{
    public GameObject next;
    public float objectSize = 1000f;
    public Vector3 speed = Vector3.up * -100f;
    bool spawn = false;
    static Vector3 startPosition = Vector3.zero;
    private void Start()
    {
        if (startPosition == Vector3.zero)
            startPosition = transform.position - (speed.normalized * objectSize);
        if (next == null)
            next = gameObject;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime;
        if (!spawn && ((transform.position - startPosition).magnitude > objectSize))
        {
            Instantiate(next, transform.position - (speed.normalized * objectSize), transform.rotation);
            spawn = true;
        }
        if ((transform.position - startPosition).magnitude >= 2f * objectSize)
        {
            Destroy(gameObject);
        }
    }
}
