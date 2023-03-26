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
