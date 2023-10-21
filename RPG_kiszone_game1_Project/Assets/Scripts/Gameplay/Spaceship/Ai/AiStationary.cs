using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiStationary : AiBehaviour
{
    public Vector2 targetPosition;
    void Start()
    {
        attack = -1;
        shoot = true;
    }
    public override void Behave()
    {
        Vector3 pos = (targetPosition + levelOffset) * GameplayManager.gameAreaSize;
        Vector3 distance = Quaternion.Inverse(transform.rotation) * (pos - transform.position);
        moveDirectionX = distance.normalized.x;
        moveDirectionY = distance.normalized.y;
        if (Mathf.Abs(distance.magnitude) < 1f)
        {
            moveDirectionX = 0;
            moveDirectionY = 0;
        }
    }
}
