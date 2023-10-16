using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAttackToBehaviour : AiBehaviour
{
    public List<AiBehaviour> behaviours;
    Spaceship ship;
    private void Awake()
    {
        ship = GetComponent<Spaceship>();
    }

    void Start()
    {
        attack = Random.Range(0, availableAttacks);
    }


    void Update()
    {
        // shootin'
        shoot = false;
        if (ship.canShoot)
        {
            attack = Random.Range(0, availableAttacks);
            behaviours[attack].levelOffset = levelOffset;
            shoot = true;
        }

        //movin'
        moveDirectionX = behaviours[attack].moveDirectionX;
        moveDirectionY = behaviours[attack].moveDirectionY;
    }
}
