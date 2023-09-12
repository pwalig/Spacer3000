using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiStationary : AiEscape
{
    public Vector2 targetPosition;
    void Update()
    {
        if (!escape)
        {
            Vector3 pos = targetPosition * GameplayManager.gameAreaSize;
            Vector3 distance = Quaternion.Inverse(transform.rotation) * (pos - transform.position);
            moveDirectionX = distance.normalized.x;
            moveDirectionY = distance.normalized.y;
            if (Mathf.Abs(distance.magnitude) < 1f)
            {
                moveDirectionX = 0;
                moveDirectionY = 0;
            }
            shoot = true;
        }
    }
}
