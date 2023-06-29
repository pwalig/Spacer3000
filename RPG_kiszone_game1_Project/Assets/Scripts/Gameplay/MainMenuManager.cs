using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    static MainMenuManager Manager;

    private void Awake()
    {
        //ensure only one manager exists
        if (Manager == null)
            Manager = this;
        else
            Destroy(this);

        if (GameData.currentSave < 0) SceneManager.LoadScene("TitleScreen");
    }

    private void Start()
    {
        SaveSystem.SaveGame();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // my favourite settings -Pawel
        {
            CameraShake.globalIntensity = 1f;
            GameplayManager.movementMode = true;
        }
    }
#endif
}
