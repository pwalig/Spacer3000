using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPickerScale : MonoBehaviour
{
    float desiredScale = 0f;
    float smallScale = 0.1667f;
    float bigScale = 1f;
    [SerializeField] SecondOrderDynamics scale;
    [SerializeField] GameObject particleBig;
    [SerializeField] GameObject particleSmall;

    public void Initialise(bool highlight, GameObject asset)
    {
        Highlight(highlight);
        transform.localScale = Vector3.one * desiredScale;
        scale.Initialise(desiredScale);
        Instantiate(asset, transform);
    }

    public void Highlight(bool highlight)
    {
        if (highlight)
            desiredScale = bigScale;
        else
            desiredScale = smallScale;
        particleBig.SetActive(highlight);
        particleSmall.SetActive(!highlight);
    }

    private void Update()
    {
        transform.localScale = Vector3.one * scale.Update(desiredScale);
    }
}
