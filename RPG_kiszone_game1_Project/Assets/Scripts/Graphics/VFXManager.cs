using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    static VFXManager Manager;
    public List<GameObject> effectsInspector;
    public static List<GameObject> effects;
    void Awake()
    {
        //ensure only one manager exists
        if (Manager == null)
            Manager = this;
        else
            Destroy(this);
        effects = effectsInspector;
    }

    public static void CreateEffect(Vector3 position, int effectId = -1, float scale = 1)
    {
        if (effectId == -1)
        {
            effectId = Random.Range(0, effects.Count);
        }
        Transform t = Instantiate(effects[effectId]).transform;
        t.position = position;
        t.localScale = Vector3.one * scale;
    }
}
