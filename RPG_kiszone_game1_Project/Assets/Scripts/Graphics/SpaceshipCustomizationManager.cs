using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipCustomizationManager : MonoBehaviour
{
    public Material material;
    float hue = 0f;
    float saturation = 0f;
    float value = 1f;
    private void Awake()
    {
        ChangeColor();
    }

    public void ChangeColor()
    {
        material.color = Color.HSVToRGB(hue, saturation, value);
    }
    public void ChangeHue(float h)
    {
        hue = h;
        ChangeColor();
    }
    public void ChangeSaturation(float sat)
    {
        saturation = sat;
        ChangeColor();
    }
    public void ChangeValue(float val)
    {
        value = val;
        ChangeColor();
    }
}
