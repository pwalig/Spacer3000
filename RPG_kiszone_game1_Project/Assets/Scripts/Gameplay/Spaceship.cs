using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Spaceship : MonoBehaviour
{
    SpaceshipController controller; //determines what controls the shpaceship: player or ai
    public float hp = 100f;
    public float speed = 1f;
    void Awake()
    {
        controller = GetComponent<SpaceshipController>(); //get controller
        if (controller == null) controller = gameObject.AddComponent<SpaceshipController>(); //if controller is missing create empty one
    }

    //to implement:
    void Shoot()
    {
        Debug.Log("Ship: " + name + " has shot");
        controller.shoot = false;
    }

    void Update()
    {
        transform.position += Vector3.right * controller.moveDirection * speed * Time.deltaTime; //spaceship movement
        if (controller.shoot) Shoot();
    }
}
