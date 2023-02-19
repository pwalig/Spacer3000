using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Place
{
    public Vector3 position;
    public Vector3 rotation;
}

public class MainMenuCameraMovement : MonoBehaviour
{
    [SerializeField] List<SecondOrderDynamics> systems = new List<SecondOrderDynamics>(); //requires 6 systems
    [SerializeField] List<Place> cameraPlacements;
    [HideInInspector] public Vector3 desiredPosition;
    [HideInInspector] public Vector3 desiredRotation;

    public void GoTo(int placeID)
    {
        if (placeID >= cameraPlacements.Count) placeID = cameraPlacements.Count - 1;
        desiredPosition = cameraPlacements[placeID].position;
        desiredRotation = cameraPlacements[placeID].rotation;
    }

    void Awake()
    {
        desiredRotation = cameraPlacements[0].position;
        desiredRotation = cameraPlacements[0].rotation;
        systems[0].Initialise(transform.position.x);
        systems[1].Initialise(transform.position.y);
        systems[2].Initialise(transform.position.z);
        systems[3].Initialise(transform.eulerAngles.x);
        systems[4].Initialise(transform.eulerAngles.y);
        systems[5].Initialise(transform.eulerAngles.z);
    }

    private void Start()
    {
        //z jakiegos powodu system ustawia sie na srodek bez ponizszego kodu
        GoTo(0); //set initial position
        for (int i = 0; i<200; i++) //stabilize systems before game begins
        {
            systems[0].Update(desiredPosition.x);
            systems[1].Update(desiredPosition.y);
            systems[2].Update(desiredPosition.z);
            systems[3].Update(desiredRotation.x);
            systems[4].Update(desiredRotation.y);
            systems[5].Update(desiredRotation.z);
        }
    }

    void Update()
    {
        transform.position = new Vector3(systems[0].Update(desiredPosition.x), systems[1].Update(desiredPosition.y), systems[2].Update(desiredPosition.z));
        transform.eulerAngles = new Vector3(systems[3].Update(desiredRotation.x), systems[4].Update(desiredRotation.y), systems[5].Update(desiredRotation.z));
    }
}
