using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    static TitleScreenManager Manager;
    string fileName = "/Last.tit";

    int saveSlots = 7;

    [SerializeField] List<Material> allMaterials;
    [SerializeField] List<SpaceshipGeneratorPreset> allSpaceships;
    [SerializeField] List<PlanetData> allPlanets;

    private void Awake()
    {
        //ensure only one manager exists
        if (Manager == null)
            Manager = this;
        else
            Destroy(this);

        GameData.Clear();
        GameData.allMaterials = allMaterials;
        GameData.allSpaceships = allSpaceships;
        GameData.allPlanets = allPlanets;
    }

    public void NewGame(int saveSlot = -1)
    {
        //int saveSlot = Directory.GetDirectories(Application.persistentDataPath).Length;
        if (saveSlot == -1)
        {
            saveSlot = 0;
            while (Directory.Exists(SaveSystem.SlotToPath(saveSlot))) saveSlot++;
        }
        
        string path = Application.persistentDataPath + fileName;
        FileStream stream = new FileStream(path, FileMode.Create);
        stream.WriteByte((byte)(char)(saveSlot + '0'));
        stream.Close();
        GameData.currentSave = saveSlot;
        SceneManager.LoadScene("MainMenu");
    }

    public void Continue()
    {
        int saveSlot = 0;
        string path = Application.persistentDataPath + fileName;
        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            saveSlot = (stream.ReadByte() - '0');
            stream.Close();
        }
        SaveSystem.LoadGame(saveSlot);
        GameData.currentSave = saveSlot;
    }

    public void LoadGame(int saveSlot)
    {
        string path = Application.persistentDataPath + fileName;
        FileStream stream = new FileStream(path, FileMode.Create);
        stream.WriteByte((byte)(char)(saveSlot + '0'));
        GameData.currentSave = saveSlot;
        SaveSystem.LoadGame(saveSlot);
    }

    public void DeleteSave(int saveSlot)
    {
        SaveSystem.DeleteSave(saveSlot);
    }

    public void ShowSaves(bool invert = false)
    {
        for (int i = 0; i < saveSlots; i++)
        {
            GameObject button = GameObject.Find("SaveButton (" + i + ")");
            if (button == null) break;
            if (Directory.Exists(SaveSystem.SlotToPath(i)))
            {
                button.GetComponentsInChildren<Button>()[0].interactable = !invert;
                button.GetComponentsInChildren<Button>()[1].interactable = true;
            }
            else
            {
                button.GetComponentsInChildren<Button>()[0].interactable = invert;
                button.GetComponentsInChildren<Button>()[1].interactable = false;
            }
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P)) // my favourite settings -Pawel
        {
            CameraShake.globalIntensity = 1f;
            GameplayManager.movementMode = true;
        }
        if (Input.GetKeyDown(KeyCode.Z)) SaveSystem.SaveGame();
        if (Input.GetKeyDown(KeyCode.C)) SaveSystem.LoadGame();
        if (Input.GetKeyDown(KeyCode.X)) SaveSystem.DeleteSave();
#endif
    }
}
