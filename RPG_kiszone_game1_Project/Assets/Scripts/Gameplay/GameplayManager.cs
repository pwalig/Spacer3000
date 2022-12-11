using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    static GameplayManager Manager;
    public static Transform playerTransform;
    public static bool paused;
    static GameObject PauseMenu;
    void Awake()
    {
        //ensure only one manager exists
        if (Manager == null)
            Manager = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        PauseMenu = GameObject.Find("PauseMenu");
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        UnPause();
    }
    void Pause()
    {
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
        paused = true;
    }
    void UnPause()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
        paused = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (paused) UnPause();
            else Pause();
        }
    }
}
