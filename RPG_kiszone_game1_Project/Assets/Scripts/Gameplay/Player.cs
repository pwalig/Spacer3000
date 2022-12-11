using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SpaceshipController
{
    void Update()
    {
        moveDirection = Input.GetAxisRaw("Horizontal");
        shoot = Input.GetKeyDown(KeyCode.Space);
    }
}
