using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SpaceshipSway : MonoBehaviour
{
    SpaceshipController controller;
    private float rotationSpeed = 120f;
    private float angle = 0f;
    private float maxAngle = 25f;
    void Awake()
    {
        controller = GetComponent<SpaceshipController>(); //get controller
        if (controller == null) controller = gameObject.AddComponent<SpaceshipController>(); //if controller is missing create empty one
    }

    void Update()
    {
        //spaceship sway
        if (controller.moveDirectionX < 0)
        {
            float moveAngle = Math.Min(rotationSpeed * Time.deltaTime, maxAngle - angle);
            angle += moveAngle;
            transform.Rotate(transform.up*moveAngle);
        }
        if (controller.moveDirectionX > 0)
        {
            float moveAngle = Math.Min(rotationSpeed * Time.deltaTime, angle + maxAngle);
            angle -= moveAngle;
            transform.Rotate(transform.up * -moveAngle);
        }
        if (controller.moveDirectionX == 0)
        {
            if (angle > 0)
            {
                float moveAngle = Math.Min(rotationSpeed * Time.deltaTime, angle);
                angle -= moveAngle;
                transform.Rotate(transform.up * -moveAngle);
            }
            if (angle < 0)
            {
                float moveAngle = Math.Min(rotationSpeed * Time.deltaTime, -angle);
                angle += moveAngle;
                transform.Rotate(transform.up * moveAngle);
            }
        }
    }
}
