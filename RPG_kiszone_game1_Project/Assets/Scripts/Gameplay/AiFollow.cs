using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFollow : SpaceshipController
{
    void Update()
    {
        float distance = GameplayManager.playerTransform.position.x - transform.position.x;
        moveDirection = Mathf.Clamp(distance/10f, -1f, 1f);
    }
}
