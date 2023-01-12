using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SpaceshipController
{
    void Update()
    {
        moveDirectionX = Input.GetAxisRaw("Horizontal");
        moveDirectionY = Input.GetAxisRaw("Vertical");

        shoot = Input.GetKey(KeyCode.Space);
    }
}
