using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPickerRotation : MonoBehaviour
{
    float desiredRotation = 0f;
    public GameObject planetTemplate;
    [SerializeField] SecondOrderDynamics yrotation;
    List<PlanetPickerScale> planets = new List<PlanetPickerScale>();


    private void Start()
    {
        desiredRotation = transform.eulerAngles.y;
        yrotation.Initialise(desiredRotation);

        for (int i = 0; i<GameData.availablePlanets.Count; i++)
        {
            GameObject planet = Instantiate(planetTemplate, transform);
            planet.transform.Rotate(0f, (360f / GameData.availablePlanets.Count) * (GameData.selectedPlanetId - i), 0f);
            planets.Add(planet.GetComponentInChildren<PlanetPickerScale>());

            if (i == GameData.selectedPlanetId)
                planets[i].Initialise(true, GameData.availablePlanets[i].planetAsset);
            else
                planets[i].Initialise(false, GameData.availablePlanets[i].planetAsset);
        }
    }

    public void Rotate (int increment = 1)
    {
        desiredRotation += (360f / GameData.availablePlanets.Count) * increment;
        for (int i = 0; i<planets.Count; i++)
        {
            if (i == GameData.selectedPlanetId)
                planets[i].Highlight(true);
            else
                planets[i].Highlight(false);
        }
    }

    private void Update()
    {
        transform.eulerAngles = new Vector3(0f, yrotation.Update(desiredRotation));
    }
}
