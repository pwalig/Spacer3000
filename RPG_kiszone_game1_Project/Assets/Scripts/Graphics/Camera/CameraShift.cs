using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShift : MonoBehaviour
{
    Camera cam;
    public float bounds = 500f;
    Vector3 startPosition;
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        startPosition = transform.position;
        cam.usePhysicalProperties = true;
    }

    void Update()
    {
        transform.position = Vector3.right * GameplayManager.playerTransform.position.x + startPosition;
        cam.lensShift = new Vector2(-GameplayManager.playerTransform.position.x / bounds, 0);
    }
}
