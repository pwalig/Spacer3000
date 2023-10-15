using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAbsoluteFollow : AiBehaviour
{
    public float turnSpeed;
    public Vector3 offset;
    public FF distanceToKeep;
    public FF shootingDistance;
    void Update()
    {
        Vector3 distance = Quaternion.Inverse(transform.rotation) * (GameplayManager.GetPlayerPosition() + (transform.rotation * offset) - transform.position);
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

        // out of bounds checks
        distance = Vector3.zero;
        if (transform.position.x < -GameplayManager.gameAreaSize.x) distance += Quaternion.Inverse(transform.rotation) * Vector3.right;
        if (transform.position.x > GameplayManager.gameAreaSize.x) distance += Quaternion.Inverse(transform.rotation) * Vector3.left;
        if (transform.position.y > GameplayManager.gameAreaSize.y) distance += Quaternion.Inverse(transform.rotation) * Vector3.down;
        if (transform.position.y < -GameplayManager.gameAreaSize.y) distance += Quaternion.Inverse(transform.rotation) * Vector3.up;
        if (distance.magnitude >= 0.1f)
        {
            moveDirectionX = distance.normalized.x;
            moveDirectionY = distance.normalized.y;
        }
    }
}
