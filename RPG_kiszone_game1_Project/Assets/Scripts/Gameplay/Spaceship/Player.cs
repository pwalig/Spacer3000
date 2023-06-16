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

        // comes back automatically if game area shrinks
        if (transform.position.y <= -GameplayManager.gameAreaSize.y - 10f) moveDirectionY = 1f;
        if (transform.position.x >= GameplayManager.gameAreaSize.x + 10f) moveDirectionX = -1;
        if (transform.position.x <= -GameplayManager.gameAreaSize.x - 10f) moveDirectionX = 1f;

        shoot = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0);
    }
}
