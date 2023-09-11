using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSidewaysFollow : AiEscape
{
    [Tooltip("value from -1 to +1. relative to gameAreaSize.y")]
    public FF goalHeight;
    void Update()
    {
        if (!escape)
        {
            float distance = (Quaternion.Inverse(transform.rotation) * (GameplayManager.GetPlayerPosition(transform.position) - transform.position)).x;
            moveDirectionX = Mathf.Clamp(distance, -1f, 1f);
            moveDirectionY = Mathf.Clamp(-goalHeight.F() * GameplayManager.gameAreaSize.y - (Quaternion.Inverse(transform.rotation) * transform.position).y, -1f, 1f);
            if (Mathf.Abs(distance) < 1) moveDirectionX = 0;
            shoot = true;
        }
    }
}
