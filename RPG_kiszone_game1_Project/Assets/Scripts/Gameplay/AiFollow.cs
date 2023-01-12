using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFollow : SpaceshipController
{
    void Update()
    {
        float distance = GameplayManager.playerTransform.position.x - transform.position.x;
        moveDirectionX = Mathf.Clamp(distance, -1f, 1f);
        shoot = true;
    }
}
