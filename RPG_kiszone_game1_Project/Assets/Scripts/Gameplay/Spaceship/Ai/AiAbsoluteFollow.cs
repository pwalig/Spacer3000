using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAbsoluteFollow : AiEscape
{
    public float distanceToKeep = 150f;
    void Update()
    {
        Vector3 distance = GameplayManager.GetPlayerPosition() - transform.position;
        moveDirectionX = Mathf.Clamp(distance.x, -1f, 1f);
        moveDirectionY = Mathf.Clamp(distance.y, -1f, 1f);
        if (Mathf.Abs(distance.x) < 1) moveDirectionX = 0;
        if (Mathf.Abs(distance.y) < 1) moveDirectionX = 0;
        if (distance.magnitude < distanceToKeep)
        {
            moveDirectionX *= -1f;
            moveDirectionY *= -1f;
        }
    }
}
