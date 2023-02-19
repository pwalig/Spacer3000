using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour
{
    List<GameObject> menus = new List<GameObject>();
    private void Awake()
    {
        menus.Add(GameObject.Find("MainGroup"));
        menus.Add(GameObject.Find("CustomizeGroup"));
        menus.Add(GameObject.Find("SettingsGroup"));
        menus.Add(GameObject.Find("CreditsGroup"));
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
}
