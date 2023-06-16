using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    List<GameObject> menus = new List<GameObject>();
    Vector3 lastMouseCoordinate = Vector3.zero;

    static MainMenuManager Manager;

    private void Awake()
    {
#if !UNITY_EDITOR
        SaveSystem.LoadGame();
#endif
        //ensure only one manager exists
        if (Manager == null)
            Manager = this;
        else
            Destroy(this);

        //setup
        menus.Add(GameObject.Find("MainGroup"));
        menus.Add(GameObject.Find("CustomizeGroup"));
        menus.Add(GameObject.Find("SettingsGroup"));
        menus.Add(GameObject.Find("CreditsGroup"));
        menus.Add(GameObject.Find("PlanetGroup"));
        menus.Add(GameObject.Find("GalleryGroup"));
        GameObject.Find("DifficultyDropdown").GetComponent<TMP_Dropdown>().value = (int)GameData.difficultyLevel;
        GameObject.Find("QualityDropdown").GetComponent<TMP_Dropdown>().value = (int)QualitySettings.GetQualityLevel();
        GameObject.Find("VolumeSlider").GetComponent<Slider>().value = GameData.volume;
        GameObject.Find("ShakeSlider").GetComponent<Slider>().value = CameraShake.globalIntensity;
    }
    private void Start()
    {
        ChangeMenu(0);
    }
    public void Play()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        SaveSystem.SaveGame();
        Application.Quit();
#endif
    }
    public void ChangeMenu(int menuId)
    {
        // reduce camera shake when in different settings
        if (menuId != 0)
            CameraShake.SetMuffle(true);
        else
            CameraShake.SetMuffle(false);

        for (int i = 0; i < menus.Count; i++)
        {
            if (i == menuId) menus[i].SetActive(true);
            else menus[i].SetActive(false);
        }

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(FindObjectOfType<Button>().gameObject);
        }
    }
    public void ChangeDifficultyLevel(int difficulty)
    {
        GameData.SetDifficultyLevel((GameData.Difficulty)difficulty);
    }
    public void ChangeQualityLevel(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
    }
    public void ChangeVolume (float volume)
    {
        GameData.volume = volume;
    }
    public void ChangeCameraShakeIntensity(float intensity)
    {
        CameraShake.globalIntensity = intensity;
    }

    private void Update()
    {
        // unhighlight menu buttons if mouse moved
        Vector3 mouseDelta = Input.mousePosition - lastMouseCoordinate;
        if (mouseDelta.magnitude > 0) EventSystem.current.SetSelectedGameObject(null);
        if (EventSystem.current.currentSelectedGameObject == null && (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f)) EventSystem.current.SetSelectedGameObject(FindObjectOfType<Button>().gameObject);
        lastMouseCoordinate = Input.mousePosition;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P)) // my favourite settings -Pawel
        {
            CameraShake.globalIntensity = 1f;
            GameplayManager.movementMode = true;
        }
        if (Input.GetKeyDown(KeyCode.Z)) SaveSystem.SaveGame();
        if (Input.GetKeyDown(KeyCode.C)) SaveSystem.LoadGame();
        if (Input.GetKeyDown(KeyCode.X)) SaveSystem.DeleteSave();
    }
    private void OnApplicationQuit()
    {
        GameData.PurgeScores();
        foreach (Material material in GameData.availableMaterials) material.color = Color.white;
#endif
    }
}
