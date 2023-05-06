using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    static GameplayManager Manager;
    public static Transform playerTransform;
    public static bool paused;
    public static bool movementMode = false;
    public static bool movementDirectionNormalize = false;
    public static Vector2 gameAreaSize = new Vector2(160f, 90f);
    static GameObject PauseMenu;
    static GameObject GameOverMenu;
    static GameObject YouWonMenu;
    void Awake()
    {
        //ensure only one manager exists
        if (Manager == null)
            Manager = this;
        else
            Destroy(this);

        if (GameData.availablePlanets == null || GameData.availableSpaceships == null)
            SceneManager.LoadScene("MainMenu");
    }

    private void Start()
    {
        PauseMenu = GameObject.Find("PauseMenu");
        GameOverMenu = GameObject.Find("GameOverMenu");
        YouWonMenu = GameObject.Find("YouWonMenu");
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        //if (GameData.availableMaterials != null) playerGameObject.GetComponentInChildren<MeshRenderer>().material = GameData.availableMaterials[GameData.selectedMaterialId];

        // make terrain
        if (GameData.availablePlanets[GameData.selectedPlanetId].terrainAsset != null)
            Instantiate(GameData.availablePlanets[GameData.selectedPlanetId].terrainAsset).transform.position = new Vector3(0f, -200f, 300f);
        playerTransform = playerGameObject.transform;

        // generate same spaceship as in main menu
        GameObject psv = GameObject.Find("PlayerSpaceshipVisuals");
        if (psv != null) psv.AddComponent<SpaceshipGenerator>().SetPreset(GameData.availableSpaceships[GameData.selectedSpaceshipId]);

        UnPause();
        GameOverMenu.SetActive(false);
        YouWonMenu.SetActive(false);
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
    }
    public static void GameOver()
    {
        GameOverMenu.SetActive(true);
    }
    public static void GameWon()
    {
        YouWonMenu.SetActive(true);
    }
    public void QuitToMainMenu()
    {
        UnPause();
        SceneManager.LoadScene("MainMenu");
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
    void ChangeBounds(float delta)
    {
        gameAreaSize += new Vector2(1.6f, 0.9f) * delta;
        GameCameraContoller.ChangeFieldOfView(0f);
    }
    
    public static void SetBounds(float val=1f)
    {
        gameAreaSize = new Vector2(1.6f, 0.9f) * val * 100f;
        GameCameraContoller.ChangeFieldOfView(0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !YouWonMenu.activeInHierarchy && !GameOverMenu.activeInHierarchy) {
            if (paused) UnPause();
            else Pause();
        }

#if (UNITY_EDITOR)
        if (Input.GetKeyDown(KeyCode.M)) { movementMode = !movementMode; Debug.Log("movementMode: " + movementMode); }
        if (Input.GetKeyDown(KeyCode.N)) { movementDirectionNormalize = !movementDirectionNormalize; Debug.Log("movementDirectionNormalize: " + movementDirectionNormalize); }
        if (Input.GetKeyDown(KeyCode.O)) { playerTransform.gameObject.GetComponent<PlayerSpaceship>().projectiles += 1; }
        if (Input.GetKeyDown(KeyCode.H)) { playerTransform.gameObject.GetComponent<Spaceship>().DealDamage(-100f); }
        if (Input.GetKey(KeyCode.B)) { ChangeBounds(-Input.mouseScrollDelta.y * 5f); Debug.Log("game Bounds: " + gameAreaSize); }
        if (Input.GetKey(KeyCode.J) && Input.mouseScrollDelta.y != 0f && Time.timeScale + Input.mouseScrollDelta.y * 0.1f >= 0f) { Time.timeScale += Input.mouseScrollDelta.y * 0.1f; Debug.Log("gameSpeed: " + (Mathf.Round(Time.timeScale * 100)) + "%"); }
#endif
    }
}
