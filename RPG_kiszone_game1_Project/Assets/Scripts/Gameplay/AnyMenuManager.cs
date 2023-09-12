using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AnyMenuManager : MonoBehaviour
{
    public List<GameObject> menus = new List<GameObject>();
    Vector3 lastMouseCoordinate = Vector3.zero;

    static AnyMenuManager Manager;

    private void Awake()
    {
        //ensure only one manager exists
        if (Manager == null)
            Manager = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        ChangeMenu(0);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        GameData.PurgeScores();
        foreach (Material material in GameData.availableMaterials) material.color = Color.white;
#else
        if (SceneManager.GetActiveScene().name != "TitleScreen") SaveSystem.SaveGame();
        Application.Quit();
#endif
    }

    public void ChangeMenu(int menuId)
    {
        // reduce camera shake when not in main submenu
        if (menuId != 0)
            CameraShake.SetMuffle(true);
        else
            CameraShake.SetMuffle(false);

        for (int i = 0; i < menus.Count; i++)
        {
            if (i == menuId) menus[i].SetActive(true);
            else menus[i].SetActive(false);
        }

        if (EventSystem.current.currentSelectedGameObject != null && FindObjectOfType<Button>() != null)
        {
            EventSystem.current.SetSelectedGameObject(FindObjectOfType<Button>().gameObject);
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        if (sceneName == "TitleScreen")
            foreach (Material material in GameData.availableMaterials) material.color = Color.white;
    }

    private void Update()
    {
        // unhighlight menu buttons if mouse moved
        Vector3 mouseDelta = Input.mousePosition - lastMouseCoordinate;
        if (mouseDelta.magnitude > 0) EventSystem.current.SetSelectedGameObject(null);
        if (EventSystem.current.currentSelectedGameObject == null && FindObjectOfType<Button>() != null && (Input.GetAxisRaw("Vertical") != 0f || Input.GetAxisRaw("Horizontal") != 0f))
            EventSystem.current.SetSelectedGameObject(FindObjectOfType<Button>().gameObject);
        lastMouseCoordinate = Input.mousePosition;
    }

    private void OnDestroy()
    {
        Manager = null;
    }
}
