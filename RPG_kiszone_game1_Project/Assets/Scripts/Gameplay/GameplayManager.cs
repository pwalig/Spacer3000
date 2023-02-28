using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    static GameplayManager Manager;
    public static Transform playerTransform;
    public static bool paused;
    public static bool projectile_destroy = true;
    static GameObject PauseMenu;
    static GameObject GameOverMenu;
    void Awake()
    {
        //ensure only one manager exists
        if (Manager == null)
            Manager = this;
        else
            Destroy(this);

        if (GameData.availablePlanets == null)
            SceneManager.LoadScene("MainMenu");
    }
    private void Start()
    {
        PauseMenu = GameObject.Find("PauseMenu");
        GameOverMenu = GameObject.Find("GameOverMenu");
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (GameData.availableMaterials != null) playerGameObject.GetComponentInChildren<MeshRenderer>().material = GameData.availableMaterials[GameData.selectedMaterialId];
        if (GameData.availablePlanets[GameData.selectedPlanetId].terrainAsset != null)
            Instantiate(GameData.availablePlanets[GameData.selectedPlanetId].terrainAsset).transform.position = new Vector3(0f, -200f, 300f);
        playerTransform = playerGameObject.transform;
        UnPause();
    }
    public static Vector3 GetPlayerPosition(Vector3 requestPosition = default(Vector3))
    {
        if (playerTransform == null) return requestPosition;
        return playerTransform.position;
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
        GameOverMenu.SetActive(false);
    }
    public static void GameOver()
    {
        GameOverMenu.SetActive(true);
    }
    public void QuitToMainMenu()
    {
        UnPause();
        SceneManager.LoadScene("MainMenu");
    }
    public void Quit()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        //Application.Quit(); //change to this on build
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (paused) UnPause();
            else Pause();
        }
    }
}
