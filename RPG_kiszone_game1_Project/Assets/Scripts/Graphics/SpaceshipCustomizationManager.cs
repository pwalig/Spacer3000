using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceshipCustomizationManager : MonoBehaviour
{
    public List<Material> startMaterials;
    [SerializeField] MeshRenderer spaceshipMeshRenderer;
    float hue = 0f;
    float saturation = 0f;
    float value = 1f;
    private void Awake()
    {
        if (GameData.availableMaterials == null)
            GameData.availableMaterials = startMaterials;
        SetMaterial(GameData.selectedMaterialId);
        SetDropdown(GameData.selectedMaterialId);
        SetSliders();
    }

    void SetSliders()
    {
        Color.RGBToHSV(GameData.availableMaterials[0].color, out hue, out saturation, out value);
        GameObject.Find("HueSlider").GetComponent<Slider>().value = hue;
        GameObject.Find("SaturationSlider").GetComponent<Slider>().value = saturation;
        GameObject.Find("ValueSlider").GetComponent<Slider>().value = value;
    }

    void SetDropdown(int matId = 0)
    {
        TMP_Dropdown materialDropdown = GameObject.Find("MaterialDropdown").GetComponent<TMP_Dropdown>();
        materialDropdown.options.Clear();
        foreach(Material mat in GameData.availableMaterials)
            materialDropdown.options.Add(new TMP_Dropdown.OptionData(mat.name));
        materialDropdown.value = matId;
    }

    public void SetMaterial(int matId = 0)
    {
        GameData.selectedMaterialId = matId;
        spaceshipMeshRenderer.material = GameData.availableMaterials[matId];
    }

    void ChangeColor()
    {
        foreach (Material mat in GameData.availableMaterials)
        {
            mat.color = Color.HSVToRGB(hue, saturation, value);
        }
    }

    public void ChangeHue(float h)
    {
        hue = h;
        ChangeColor();
    }
    public void ChangeSaturation(float sat)
    {
        saturation = sat;
        ChangeColor();
    }
    public void ChangeValue(float val)
    {
        value = val;
        ChangeColor();
    }
}
