using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiEscape : SpaceshipController
{
    [HideInInspector] public bool escape = false; // wrap child classes update code in if(!escape){ }

    public IEnumerator FlyAway(float delay=0f)
    {
        escape = true;
        yield return new WaitForSeconds(delay);
        Vector2 dir = new Vector2(moveDirectionX, moveDirectionY).normalized;
        moveDirectionX = dir.x; moveDirectionY = dir.y;
        if (dir.magnitude < 0.1f)
            moveDirectionX = 1f;
        yield return new WaitForSeconds(20f);
        LevelManager.enemies.Remove(gameObject);
        Destroy(gameObject);
    }
}
