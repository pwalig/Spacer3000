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

        if((transform.position.y>= 0 && moveDirectionY == 1) || (transform.position.y <= -70 && moveDirectionY == -1))
        {
            moveDirectionY = 0;
        }
        if ((transform.position.x >= 65 && moveDirectionX == 1) || (transform.position.x <= -65 && moveDirectionX == -1))
        {
            moveDirectionX = 0;
        }

        shoot = Input.GetKey(KeyCode.Space);
    }
}
