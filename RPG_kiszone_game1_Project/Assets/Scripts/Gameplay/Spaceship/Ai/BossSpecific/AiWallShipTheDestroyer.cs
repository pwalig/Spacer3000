using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiWallShipTheDestroyer : AiStationary
{
    private void OnDestroy()
    {
        try
        {
            GameObject.Find("Wall_The_Left_One(Clone)").GetComponent<Spaceship>().Die();
            GameObject.Find("Wall_The_Right_One(Clone)").GetComponent<Spaceship>().Die();
        }
        catch (Exception) { }
    }
}
