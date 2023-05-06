using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFollow : AiEscape
{
    public float goalHeight;
    void Update()
    {
        if (!escape)
        {
            float distance = GameplayManager.GetPlayerPosition(transform.position).x - transform.position.x;
            moveDirectionX = Mathf.Clamp(distance, -1f, 1f);
            moveDirectionY = Mathf.Clamp(goalHeight * GameplayManager.gameAreaSize.y / 80f - transform.position.y, -1f, 1f);
            if (Mathf.Abs(distance) < 1) moveDirectionX = 0;
            shoot = true;
        }
    }
}
