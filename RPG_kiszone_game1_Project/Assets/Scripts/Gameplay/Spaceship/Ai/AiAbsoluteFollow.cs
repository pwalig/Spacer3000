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
        moveDirectionX = Mathf.Clamp(distance.x, -1f, 1f);
        moveDirectionY = Mathf.Clamp(distance.y, -1f, 1f);
        if (distance.magnitude < distanceToKeep.F())
        {
            moveDirectionX *= -1f;
            moveDirectionY *= -1f;
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
