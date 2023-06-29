using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    static SettingsManager Manager;

    private void Awake()
    {
        //ensure only one manager exists
        if (Manager == null)
            Manager = this;
        else
            Destroy(this);

        //setup
        GameObject.Find("DifficultyDropdown").GetComponent<TMP_Dropdown>().value = (int)GameData.difficultyLevel;
        GameObject.Find("QualityDropdown").GetComponent<TMP_Dropdown>().value = (int)QualitySettings.GetQualityLevel();
        GameObject.Find("VolumeSlider").GetComponent<Slider>().value = GameData.volume;
        GameObject.Find("ShakeSlider").GetComponent<Slider>().value = CameraShake.globalIntensity;
    }

    public void SaveGame()
    {
        SaveSystem.SaveGame();
    }

    public void ChangeDifficultyLevel(int difficulty)
    {
        GameData.SetDifficultyLevel((GameData.Difficulty)difficulty);
    }
    public void ChangeQualityLevel(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
    }
    public void ChangeVolume(float volume)
    {
        GameData.volume = volume;
    }
    public void ChangeCameraShakeIntensity(float intensity)
    {
        CameraShake.globalIntensity = intensity;
    }
}
