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
        GameObject.Find("DifficultyDropdown").GetComponent<TMP_Dropdown>().value = (int)GameData.difficultyLevel;
        GameObject.Find("VolumeSlider").GetComponent<Slider>().value = GameData.volume;
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
        UnityEditor.EditorApplication.isPlaying = false;
        //Application.Quit(); change to this on build
    }
    public void ChangeMenu(int menuId)
    {
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
    public void ChangeVolume (float volume)
    {
        GameData.volume = volume;
    }
}
