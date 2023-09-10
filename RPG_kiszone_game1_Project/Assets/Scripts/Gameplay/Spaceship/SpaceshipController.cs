using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class SpaceshipController : MonoBehaviour
{
    [HideInInspector] public float moveDirectionX = 0f;
    [HideInInspector] public float moveDirectionY = 0f;
    [HideInInspector] public bool shoot = false;
}
