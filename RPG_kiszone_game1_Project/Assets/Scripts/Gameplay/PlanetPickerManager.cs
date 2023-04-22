using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlanetPickerManager : MonoBehaviour
{
    [SerializeField] List<PlanetData> avalilablePlanetsInspector;
    TMP_Text planetNameText;
    TMP_Text planetNoteText;
    TMP_Text levelNoteText;

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
        levelNoteText = GameObject.Find("LevelNote").GetComponent<TMP_Text>();
        if (GameData.availablePlanets == null)
            GameData.availablePlanets = avalilablePlanetsInspector;
        planetNameText.text = GameData.GetPlanet().name;
        planetNoteText.text = GameData.GetPlanet().GetNote();
        levelNoteText.text = GameData.GetLevel().GetNote();
        SetLevelDropdown(GameData.GetPlanet().unlockedLevels - 1);
    }

    void SetLevelDropdown(int levelId = 0)
    {
        TMP_Dropdown levelDropdown = GameObject.Find("LevelDropdown").GetComponent<TMP_Dropdown>();
        PlanetData planet = GameData.GetPlanet();

        levelDropdown.options.Clear();

        for (int i = 0; i < planet.unlockedLevels && i < planet.levels.Count; i++)
            levelDropdown.options.Add(new TMP_Dropdown.OptionData(planet.levels[i].name));

        levelDropdown.value = levelId;
        SelectLevel(levelId);
        levelDropdown.RefreshShownValue();
    }

    public void SelectLevel(int levelId)
    {
        GameData.selectedLevelId = levelId;
        levelNoteText.text = GameData.GetLevel().GetNote();
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
        planetNameText.text = GameData.GetPlanet().name;
        planetNoteText.text = GameData.GetPlanet().GetNote();
        levelNoteText.text = GameData.GetLevel().GetNote();
        SetLevelDropdown(GameData.GetPlanet().unlockedLevels - 1);
    }

    void UnlockLevels(int increment, bool all=false, bool set=false)
    {
        if (all)
        {
            foreach (PlanetData planet in GameData.availablePlanets)
            {
                if (set) planet.unlockedLevels = increment;
                else planet.unlockedLevels += increment;
                if (planet.unlockedLevels <= 0) planet.unlockedLevels = 1;
            }
        }
        else
        {
            PlanetData planet = GameData.GetPlanet();
            if (set) planet.unlockedLevels = increment;
            else planet.unlockedLevels += increment;
            if (planet.unlockedLevels <= 0) planet.unlockedLevels = 1;
        }
        SetLevelDropdown();
    }

    private void Update()
    {
#if (UNITY_EDITOR) // cheat codes for unlocking levels
        if (GameObject.Find("LevelDropdown") != null)
        {
            if (Input.GetKeyDown(KeyCode.U)) { UnlockLevels(1, Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.S)); }
            if (Input.GetKeyDown(KeyCode.L)) { UnlockLevels(-1, Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.S)); }
        }
#endif
    }
}
