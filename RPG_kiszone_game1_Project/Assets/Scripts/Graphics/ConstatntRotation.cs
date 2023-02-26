using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstatntRotation : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 axisOfRotation;
    void Update()
    {
        transform.Rotate(axisOfRotation, rotationSpeed * Time.deltaTime);
    }
}
