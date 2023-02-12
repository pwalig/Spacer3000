using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SecondOrderDynamics
{
    [SerializeField] float f = 1f; //system speed: (0, oo) reasonable values
    [SerializeField] float z = 0.5f; //damping: {0} undamped, (0;1) underdamped, {1} critical damping, (1; oo) overdamped
    [SerializeField] float r = 0f; //initial response: (-oo, 0) anticipation, {0} rest, (0;1> response towards goal, (1, oo) overshoot
    [SerializeField] float amplitude = 1f; //output input ratio
    [SerializeField] float offset = 0f;
    float xp;
    float y, yd;
    float k1, k2, k3;
    public void Initialise (float x0 = 0)
    {
        //compute constants
        k1 = z / (Mathf.PI * f);
        k2 = 1 / (4 * Mathf.PI * Mathf.PI * f * f);
        k3 = r * z / (2 * Mathf.PI * f);

        xp = x0;
        y = x0;
        yd = 0;
    }

    ///<summary>
    ///Computes physics based output interpolation
    ///</summary>
    public float Update (float x)
    {
        float xd = (x - xp) * Time.deltaTime; //calculate input acceleration
        xp = x; //update input position
        float k2_stable = Mathf.Max(k2, 1.1f * (Time.deltaTime * Time.deltaTime / 4 + Time.deltaTime * k1 / 2));
        y += Time.deltaTime * yd; //update output position
        yd += Time.deltaTime * (x + k3 * xd - y - k1 * yd) / k2_stable; //update output acceleration
        return y * amplitude + offset;
    }
}