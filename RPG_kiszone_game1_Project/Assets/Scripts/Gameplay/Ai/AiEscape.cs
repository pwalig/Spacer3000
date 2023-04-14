using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiEscape : SpaceshipController
{
    [HideInInspector] public bool escape = false; // wrap child classes update code in if(!escape){ }

    public IEnumerator FlyAway(float delay=0f)
    {
        yield return new WaitForSeconds(delay);
        escape = true;
        Vector2 dir = new Vector2(moveDirectionX, moveDirectionY).normalized;
        moveDirectionX = dir.x; moveDirectionY = dir.y;
        if (dir.magnitude < 0.1f)
            moveDirectionX = 1f;
        while(Mathf.Abs(transform.position.x) < GameplayManager.gameAreaSize.x + 40f && Mathf.Abs(transform.position.y) < GameplayManager.gameAreaSize.y + 40f) yield return 0;
        LevelManager.enemies.Remove(gameObject);
        Destroy(gameObject);
    }
}
