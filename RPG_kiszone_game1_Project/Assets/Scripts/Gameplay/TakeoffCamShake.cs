using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeoffCamShake : MonoBehaviour
{
    [SerializeField] bool shake = false;
    void FixedUpdate()
    {
        if (shake)
        {
            shake = false;
            CameraShake.Shake(5f);
        }
    }
}
