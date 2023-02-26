using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct PlanetData
{
    public string name;
    public Material material;
    public GameObject planetAsset;
    public GameObject terrainAsset;
    public string note;
}

public class PlanetPickerManager : MonoBehaviour
{
    [SerializeField] List<PlanetData> avalilablePlanetsInspector;
    TMP_Text planetNameText;
    TMP_Text planetNoteText;

    static PlanetPickerManager Manager;
    private void Awake()
    {
        //ensure only one manager exists
        if (Manager == null)
            Manager = this;
        else
            Destroy(this);

        planetNameText = GameObject.Find("PlanetName").GetComponent<TMP_Text>();
        planetNoteText = GameObject.Find("PlanetNote").GetComponent<TMP_Text>();
        if (GameData.availablePlanets == null)
            GameData.availablePlanets = avalilablePlanetsInspector;
        planetNameText.text = GameData.availablePlanets[GameData.selectedPlanetId].name;
        planetNoteText.text = GameData.availablePlanets[GameData.selectedPlanetId].note;
    }
    public void IteratePlanet(int increment)
    {
        for (int i = 0; i < Mathf.Abs(increment); i++)
        {
            GameData.selectedPlanetId += (int)Mathf.Sign(increment);
            if (GameData.selectedPlanetId >= GameData.availablePlanets.Count)
                GameData.selectedPlanetId = 0;
            else if (GameData.selectedPlanetId < 0)
                GameData.selectedPlanetId = GameData.availablePlanets.Count - 1;
        }
        planetNameText.text = GameData.availablePlanets[GameData.selectedPlanetId].name;
        planetNoteText.text = GameData.availablePlanets[GameData.selectedPlanetId].note;
    }
}
