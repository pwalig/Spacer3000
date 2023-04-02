using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Player : SpaceshipController
{
    void Update()
    {
        moveDirectionX = Input.GetAxisRaw("Horizontal");
        moveDirectionY = Input.GetAxisRaw("Vertical");

        if((transform.position.y>= 0 && moveDirectionY == 1) || (transform.position.y <= -GameplayManager.gameAreaSize.y && moveDirectionY == -1))
        {
            moveDirectionY = 0;
        }
        if ((transform.position.x >= GameplayManager.gameAreaSize.x && moveDirectionX == 1) || (transform.position.x <= -GameplayManager.gameAreaSize.x && moveDirectionX == -1))
        {
            moveDirectionX = 0;
        }

        shoot = Input.GetKey(KeyCode.Space);
    }
}
