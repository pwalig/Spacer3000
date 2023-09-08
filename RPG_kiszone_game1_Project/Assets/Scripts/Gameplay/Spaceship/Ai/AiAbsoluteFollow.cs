using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAbsoluteFollow : AiEscape
{
    public FF distanceToKeep;
    public FF shootingDistance;
    void Update()
    {
        Vector3 distance = GameplayManager.GetPlayerPosition() - transform.position;
        moveDirectionX = Mathf.Clamp(distance.x, -1f, 1f);
        moveDirectionY = Mathf.Clamp(distance.y, -1f, 1f);
        if (Mathf.Abs(distance.x) < 1) moveDirectionX = 0;
        if (Mathf.Abs(distance.y) < 1) moveDirectionX = 0;
        if (distance.magnitude < distanceToKeep.F())
        {
            moveDirectionX *= -1f;
            moveDirectionY *= -1f;
        }
        if (shootingDistance.F() > distance.magnitude) shoot = true;
        else shoot = false;
    }
}
