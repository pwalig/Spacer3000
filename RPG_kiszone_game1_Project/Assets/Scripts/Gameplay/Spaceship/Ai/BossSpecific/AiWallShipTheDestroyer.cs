using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiWallShipTheDestroyer : AiBehaviour
{
    public Vector2 targetPosition;
    void Start()
    {
        attack = -1;
        shoot = true;
    }
    void Update()
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
    }

    private void OnDestroy()
    {
        GameObject.Find("Wall_The_Left_One(Clone)").GetComponent<Spaceship>().Die();
        GameObject.Find("Wall_The_Right_One(Clone)").GetComponent<Spaceship>().Die();
    }
}
