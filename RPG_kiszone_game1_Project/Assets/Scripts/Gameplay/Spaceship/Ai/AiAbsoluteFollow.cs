using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAbsoluteFollow : AiEscape
{
    public float turnSpeed;
    public Vector3 offset;
    public FF distanceToKeep;
    public FF shootingDistance;
    void Update()
    {
        Vector3 distance = Quaternion.Inverse(transform.rotation) * (GameplayManager.GetPlayerPosition() + offset - transform.position);
        moveDirectionX = distance.normalized.x;
        moveDirectionY = distance.normalized.y;
        if (distance.magnitude < distanceToKeep.F())
        {
            moveDirectionX = -distance.normalized.x;
            moveDirectionY = -distance.normalized.y;
        }
        if (Mathf.Abs(distance.magnitude - distanceToKeep.value) < 1f)
        {
            moveDirectionX = 0;
            moveDirectionY = 0;
        }
        transform.Rotate(Vector3.forward, -distance.normalized.x * Time.deltaTime * turnSpeed);
        if (shootingDistance.F() > distance.magnitude) shoot = true;
        else shoot = false;
    }
}
