using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    static string saveFileName = "/mainSave.spacer";
    public static string SlotToPath(int saveSlot = 0)
    {
        return Application.persistentDataPath + "/save" + saveSlot;
    }
    public static void SaveGame(int saveSlot = -404)
    {
        if (saveSlot == -404) saveSlot = GameData.currentSave;
        BinaryFormatter formatter = new BinaryFormatter();
        string path = SlotToPath(saveSlot);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        path += saveFileName;
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, GameData.GetSaveableData());
        stream.Close();

        foreach (PlanetData planet in GameData.availablePlanets)
        {
            path = SlotToPath(saveSlot) + "/" + planet.name + ".planet";
            stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, planet.GetSaveableData());
            stream.Close();
        }
#if (UNITY_EDITOR)
        Debug.Log("Game Saved");
#endif
    }

    public static void LoadGame(int saveSlot = 0)
    {
        string path = SlotToPath(saveSlot) + saveFileName;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameSaveableData dat = formatter.Deserialize(stream) as GameSaveableData;
            GameData.ReadSaveableData(dat);
            stream.Close();

            foreach (PlanetData planet in GameData.availablePlanets)
            {
                path = SlotToPath(saveSlot) + "/" + planet.name + ".planet";
                if (File.Exists(path))
                {
                    stream = new FileStream(path, FileMode.Open);
                    PlanetSaveableData psd = formatter.Deserialize(stream) as PlanetSaveableData;
                    planet.ReadSaveableData(psd);
                    stream.Close();
                }
#if (UNITY_EDITOR)
                else Debug.LogError("Save file: " + path + " not existant!");
#endif
            }
            SceneManager.LoadScene("MainMenu");
#if (UNITY_EDITOR)
            Debug.Log("Game Loaded");
#endif
        }
#if (UNITY_EDITOR)
        else Debug.LogError("Save file: " + path + " not existant!");
#endif
    }

    public static void DeleteSave(int saveSlot = 0)
    {
        string path = SlotToPath(saveSlot);
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
#if (UNITY_EDITOR)
            Debug.Log("Save deleted");
#endif
        }
        else Debug.LogError("Save file: " + path + " not existant!");
    }
}
