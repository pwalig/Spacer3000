using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    static GameplayManager Manager;
    public static Transform playerTransform;
    public static bool paused;
    public static bool movementMode = true;
    public static bool immortality = false;
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

        if (GameData.currentSave < 0) SceneManager.LoadScene("TitleScreen");
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
        if (GameData.GetPlanet().terrainAsset != null)
            Instantiate(GameData.GetPlanet().terrainAsset).transform.position = new Vector3(0f, -200f, 300f);
        playerTransform = playerGameObject.transform;

        // generate same spaceship as in main menu
        GameObject psv = GameObject.Find("PlayerSpaceshipVisuals");
        if (psv != null) psv.AddComponent<SpaceshipGenerator>().SetPreset(GameData.GetSpaceshipPreset());

        UnPause();
        GameOverMenu.SetActive(false);
        YouWonMenu.SetActive(false);
    }
    public static Vector3 GetPlayerPosition(Vector3 requestPosition = default)
    {
        if (playerTransform == null) return requestPosition;
        return playerTransform.position;
    }
    void Pause()
    {
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
        paused = true;
        if (EventSystem.current.currentSelectedGameObject != null && FindObjectOfType<Button>() != null)
        {
            EventSystem.current.SetSelectedGameObject(FindObjectOfType<Button>().gameObject);
        }
    }
    public void UnPause()
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
        SaveSystem.SaveGame();
        YouWonMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(FindObjectOfType<Button>().gameObject);
        EventSystem.current.SetSelectedGameObject(null);
    }

    void ChangeBounds(float delta)
    {
        gameAreaSize += new Vector2(1.6f, 0.9f) * delta;
        GameCameraContoller.ChangeFieldOfView(0f);
    }
    
    public static void SetBounds(float val=1f)
    {
        gameAreaSize = 100f * val * new Vector2(1.6f, 0.9f);
        GameCameraContoller.ChangeFieldOfView(0f);
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7)) && !YouWonMenu.activeInHierarchy && !GameOverMenu.activeInHierarchy)
        {
            if (!paused) Pause();
            else if (PauseMenu.activeInHierarchy) UnPause();
        }

#if UNITY_EDITOR // cheat codes
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Joystick1Button8)) { movementMode = !movementMode; Debug.Log("movementMode: " + movementMode); }
        if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.Joystick1Button9)) { movementDirectionNormalize = !movementDirectionNormalize; Debug.Log("movementDirectionNormalize: " + movementDirectionNormalize); }
        if (Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.Joystick1Button4)) { playerTransform.gameObject.GetComponent<PlayerSpaceship>().projectiles += 1; }
        if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Joystick1Button5)) { playerTransform.gameObject.GetComponent<Spaceship>().DealDamage(-100f); }
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Joystick1Button1)) { immortality = !immortality; }
        if (Input.GetKey(KeyCode.B)) { ChangeBounds(-Input.mouseScrollDelta.y * 5f); Debug.Log("game Bounds: " + gameAreaSize); }
        if (Input.GetKey(KeyCode.Joystick1Button2)) { ChangeBounds(-(Input.GetAxis("JoystickScroll") * Time.deltaTime) * 50f); Debug.Log("game Bounds: " + gameAreaSize); }
        if (Input.GetKey(KeyCode.J) && Input.mouseScrollDelta.y != 0f && Time.timeScale + Input.mouseScrollDelta.y * 0.1f >= 0f) { Time.timeScale += Input.mouseScrollDelta.y * 0.1f; Debug.Log("gameSpeed: " + (Mathf.Round(Time.timeScale * 100)) + "%"); }
        if (Input.GetKey(KeyCode.Joystick1Button3) && Input.GetAxis("JoystickScroll") != 0f && Time.timeScale + (Input.GetAxis("JoystickScroll") * Time.deltaTime) >= 0f) { Time.timeScale += Input.GetAxis("JoystickScroll") * Time.deltaTime; Debug.Log("gameSpeed: " + (Mathf.Round(Time.timeScale * 100)) + "%"); }
#endif
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
