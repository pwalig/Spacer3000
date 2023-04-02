using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    static float temporaryIntensity = 1f;
    public static float globalIntensity = 0f; //<--- tutaj mozesz sobie domyslna intensywnosc wstrzasow ustawic
    public static Vector3 startPosition;
    [SerializeField] List<SecondOrderDynamics> systemsInspector = new List<SecondOrderDynamics>();
    [SerializeField] static List<SecondOrderDynamics> systems = new List<SecondOrderDynamics>();
    void Awake()
    {
        startPosition = transform.localPosition;

        systems = systemsInspector;
        foreach (SecondOrderDynamics system in systems)
        {
            system.Initialise();
        }
    }

    public static void Shake(float radius)
    {
        foreach (SecondOrderDynamics system in systems)
        {
            system.Update(Random.Range(-radius, radius));
        }
    }

    public static void SetMuffle(bool on_off, float temp_intensity = 0.1f)
    {
        if (on_off)
            temporaryIntensity = temp_intensity;
        else
            temporaryIntensity = 1f;
    }

    void Update()
    {
        transform.localPosition = startPosition + (new Vector3(systems[0].Update(0f), systems[1].Update(0f), systems[2].Update(0f)) * temporaryIntensity * globalIntensity);
    }
}
