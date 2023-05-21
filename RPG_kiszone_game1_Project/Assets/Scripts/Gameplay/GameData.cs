using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public enum Difficulty {Peaceful, UltraEasy, VeryEasy, Easy, Normal, Hard, VeryHard, UltraHard, Impossible }
    public static Difficulty difficultyLevel = Difficulty.Normal;
    static float difficultyFloat = 1f;
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

    public static void SetDifficultyLevel(Difficulty _difficulty)
    {
        difficultyLevel = _difficulty;
        switch (_difficulty)
        {
            case Difficulty.Peaceful:
                difficultyFloat = -1f;
                break;
            case Difficulty.UltraEasy:
                difficultyFloat = -0.5f;
                break;
            case Difficulty.VeryEasy:
                difficultyFloat = -(2f/3f);
                break;
            case Difficulty.Easy:
                difficultyFloat = -0.2f;
                break;
            case Difficulty.Normal:
                difficultyFloat = 0f;
                break;
            case Difficulty.Hard:
                difficultyFloat = 0.25f;
                break;
            case Difficulty.VeryHard:
                difficultyFloat = 0.5f;
                break;
            case Difficulty.UltraHard:
                difficultyFloat = 1f;
                break;
            case Difficulty.Impossible:
                difficultyFloat = 9f;
                break;
        }
    }

    public static float GetDifficultyMulitplier(float influence = 1f, bool inverse = false)
    {
        influence = Mathf.Clamp01(influence);
        float newValue = Mathf.Clamp(1f + (difficultyFloat * influence), 0.01f, 100f);
        if (inverse) return 1f / newValue;
        else return newValue;
    }

    public static void PurgeScores()
    {
        foreach (PlanetData planet in availablePlanets)
            foreach (LevelLayout level in planet.levels)
                level.highScore = 0f;
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
