using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveableData
{
    public int difficulty;
    public float volume;
    public float cameraShake;
    public bool[] availableMaterials;
    public bool[] availableSpaceships;
    public bool[] availablePlanets;
    public int selectedMaterialId;
    public int selectedSpaceshipId;
    public int selectedPlanetId;
    public int selectedLevelId;
    public float[] color;
}

public static class GameData
{
    public enum Difficulty {Peaceful, UltraEasy, VeryEasy, Easy, Normal, Hard, VeryHard, UltraHard, Impossible }
    public static Difficulty difficultyLevel = Difficulty.Normal;
    static float difficultyFloat = 1f;
    public static float volume = 1f;

    public static List<Material> availableMaterials = null;
    public static List<SpaceshipGeneratorPreset> availableSpaceships = null;
    public static List<PlanetData> availablePlanets = null;

    static List<Material> allMaterials = new();
    static List<SpaceshipGeneratorPreset> allSpaceships = new();
    static List<PlanetData> allPlanets = new();

    public static int selectedMaterialId = 0;
    public static int selectedSpaceshipId = 0;
    public static int selectedPlanetId = 0;
    public static int selectedLevelId = 0;

    public static GameSaveableData GetSaveableData()
    {
        GameSaveableData dat = new();
        dat.difficulty = (int)difficultyLevel;
        dat.volume = volume;
        dat.cameraShake = CameraShake.globalIntensity;

        UpdateAllLists();
        dat.availableMaterials = new bool[allMaterials.Count];
        dat.availablePlanets = new bool[allPlanets.Count];
        dat.availableSpaceships = new bool[allSpaceships.Count];
        for (int i = 0; i < allMaterials.Count; i++) dat.availableMaterials[i] = availableMaterials.Contains(allMaterials[i]);
        for (int i = 0; i < allPlanets.Count; i++) dat.availablePlanets[i] = availablePlanets.Contains(allPlanets[i]);
        for (int i = 0; i < allSpaceships.Count; i++) dat.availableSpaceships[i] = availableSpaceships.Contains(allSpaceships[i]);

        dat.selectedMaterialId = selectedMaterialId;
        dat.selectedSpaceshipId = selectedSpaceshipId;
        dat.selectedPlanetId = selectedPlanetId;
        dat.selectedLevelId = selectedLevelId;

        dat.color = new float[3];
        dat.color[0] = GetMaterial().color.r;
        dat.color[1] = GetMaterial().color.g;
        dat.color[2] = GetMaterial().color.b;

        return dat;
    }
    public static List<PlanetSaveableData> GetPlanetSaveableData()
    {
        List<PlanetSaveableData> psd = new();
        foreach(PlanetData pd in availablePlanets)
        {
            psd.Add(pd.GetSaveableData());
        }
        return psd;
    }

    public static void ReadSaveableData(GameSaveableData dat)
    {
        difficultyLevel = (Difficulty)dat.difficulty;
        volume = dat.volume;
        CameraShake.globalIntensity = dat.cameraShake;

        UpdateAllLists();
        for (int i = 0; i < dat.availableMaterials.Length; i++) if(dat.availableMaterials[i] && !availableMaterials.Contains(allMaterials[i])) availableMaterials.Add(allMaterials[i]);
        for (int i = 0; i < dat.availablePlanets.Length; i++) if (dat.availablePlanets[i] && !availablePlanets.Contains(allPlanets[i])) availablePlanets.Add(allPlanets[i]);
        for (int i = 0; i < dat.availableSpaceships.Length; i++) if (dat.availableSpaceships[i] && !availableSpaceships.Contains(allSpaceships[i])) availableSpaceships.Add(allSpaceships[i]);

        selectedMaterialId = dat.selectedMaterialId;
        selectedSpaceshipId = dat.selectedSpaceshipId;
        selectedPlanetId = dat.selectedPlanetId;
        selectedLevelId = dat.selectedLevelId;

        foreach (Material material in availableMaterials) material.color = new Color(dat.color[0], dat.color[1], dat.color[2]);
    }
    static void UpdateAllListsRecursive(PlanetData planet)
    {
        if (!allPlanets.Contains(planet))
        {
            allPlanets.Add(planet);
            foreach (LevelLayout level in planet.levels)
            {
                foreach (Material mat in level.materialRewards)
                    if (!allMaterials.Contains(mat))
                        allMaterials.Add(mat);
                foreach (SpaceshipGeneratorPreset spaceship in level.spaceshipRewards)
                    if (!allSpaceships.Contains(spaceship))
                        allSpaceships.Add(spaceship);
                foreach (PlanetData planet1 in level.planetRewards)
                    UpdateAllListsRecursive(planet1);
            }
        }

    }
    static void UpdateAllLists()
    {
        allMaterials.Clear();
        allPlanets.Clear();
        allSpaceships.Clear();
        foreach(Material mat in availableMaterials) allMaterials.Add(mat);
        foreach(SpaceshipGeneratorPreset spaceship in availableSpaceships) allSpaceships.Add(spaceship);
        foreach(PlanetData planet in availablePlanets)
        {
            UpdateAllListsRecursive(planet);
        }
    }

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

    public static float GetQualityMultiplier(float influence = 1f, bool accept0 = false, bool inverse = false, bool roundToInt = false)
    {
        float qualityLevels = accept0 ? 5f : 6f;
        float res = ((QualitySettings.GetQualityLevel() + (accept0 ? 0f : 1f)) / qualityLevels * influence) + (1f * (1f - influence));
        if (inverse) res = 1f / res;
        if (roundToInt) res = Mathf.Round(res);
        return res;
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
