using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public enum Difficulty {VeryEasy, Easy, Normal, Hard, VeryHard, UltraHard }
    public static Difficulty difficultyLevel = Difficulty.Normal;
    public static float volume = 1f;
    public static List<Material> availableMaterials = null;
    public static List<SpaceshipGeneratorPreset> availableSpaceships = null;
    public static List<PlanetData> availablePlanets = null;
    public static int selectedMaterialId = 0;
    public static int selectedSpaceshipId = 0;
    public static int selectedPlanetId = 0;
    public static int selectedLevelId = 0;

    public static Material GetMaterial()
    {
        return availableMaterials[selectedMaterialId];
    }

    public static SpaceshipGeneratorPreset GetSpaceshipPreset()
    {
        return availableSpaceships[selectedSpaceshipId];
    }
    public static PlanetData GetPlanet()
    {
        if (availablePlanets != null)
            return availablePlanets[selectedPlanetId];
        return null;
    }
    public static LevelLayout GetLevel()
    {
        if (GetPlanet() != null && GetPlanet().levels != null)
            return GetPlanet().levels[Mathf.Clamp(selectedLevelId, 0, GetPlanet().levels.Count-1)];
        return null;
    }
    /*static GameData gameData;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //ensure only one gameData exists
        if (gameData == null)
            gameData = this;
        else
            Destroy(gameObject);
    }*/
}
