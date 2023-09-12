using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class waypointFollowerLoop : AiBehaviour
{
    public GameObject waypoints;
    public int counter;

    void Start()
    {
        counter = 0;
        attack = -1;
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, waypoints.GetComponent<waypoints>().wp[counter].transform.position) < 0.2f)
        {
            counter++;
            counter %= waypoints.GetComponent<waypoints>().wp.Length;
        }
        Vector3 moveDirection = new Vector3(waypoints.GetComponent<waypoints>().wp[counter].transform.position.x - transform.position.x, waypoints.GetComponent<waypoints>().wp[counter].transform.position.y - transform.position.y, 0);
        if (moveDirection.magnitude > 1f) moveDirection.Normalize();
        moveDirectionX = moveDirection.x;
        moveDirectionY = moveDirection.y;
    }
}
