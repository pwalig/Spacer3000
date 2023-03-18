using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class MainMenuManager : MonoBehaviour
{
    List<GameObject> menus = new List<GameObject>();

    static MainMenuManager Manager;

    private void Awake()
    {
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
#if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
#endif
#if(UNITY_STANDALONE)
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
    }
    public void ChangeDifficultyLevel(int difficulty)
    {
        GameData.difficultyLevel = (GameData.Difficulty)difficulty;
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
}
