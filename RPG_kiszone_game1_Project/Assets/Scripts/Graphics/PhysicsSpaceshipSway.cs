using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSpaceshipSway : MonoBehaviour
{
    SpaceshipController controller;
    [SerializeField] List<SecondOrderDynamics> systems = new List<SecondOrderDynamics>();
    void Awake()
    {
        controller = GetComponentInParent<SpaceshipController>(); //get controller
        if (controller == null) controller = gameObject.AddComponent<SpaceshipController>(); //if controller is missing create empty one
        foreach (SecondOrderDynamics system in systems)
        {
            system.Initialise();
        }
    }

    void Update()
    {
        transform.eulerAngles = new Vector3(systems[1].Update(controller.moveDirectionY), systems[0].Update(-controller.moveDirectionX), 0f);
    }
}
