using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Pass
{
    public string passName;
    public enum PartPlacementMethod { Loose, Interpolate, Scatter };
    public PartPlacementMethod placementMethod;
    public enum PartRotationMethod { Set, Outward, Normal };
    public PartRotationMethod rotationMethod;
    public int partsGroup;
    public bool mirror;
    public int seed;
    public bool randomizeSeed;
    public bool addToRenderers;
    public int amount;
    public AnimationCurve scaleDistribution;
    public Vector3 randomPostRotation;
    public List<int> assignToLayers;
    [Tooltip("Loose: parameters 0, 1, 2 (-inf, inf) dimentions x, y, z. ||| Interpolate: parameter 0 <0, inf) layer to interpolate between. ||| Scatter: parameter 0 <0, inf) layer to scatter on. ||| Set: parameters 3, 4, 5 (-inf, inf) localEulerAngles of a part. ||| Outward: parameter 3 (0f, 1f) gradually blends origin between root and direct parent, parameter 4 (0f, 1f) flattens gradually rotation to y axis only.")]
    public List<float> parameters;
    public float qualityInfluence;
}

[System.Serializable]
public struct MeshGroup
{
    public string name;
    public List<Mesh> meshes;
}

[CreateAssetMenu(fileName = "Spaceship1", menuName = "SpaceshipGeneratorPreset")]
public class SpaceshipGeneratorPreset : ScriptableObject
{
    public List<string> notes;
    public bool randomizeSeeds;
    public List<Pass> passes;
    public List<Material> materials;
    public List<MeshGroup> meshes;

    public void CopyPreset(SpaceshipGeneratorPreset preset)
    {
        name = preset.name;
        notes = new List<string>();
        foreach (string note in preset.notes) notes.Add(note);
        randomizeSeeds = preset.randomizeSeeds;
        passes = new List<Pass>();
        foreach (Pass pass in preset.passes) passes.Add(pass);
        materials = new List<Material>();
        foreach (Material material in preset.materials) materials.Add(material);
        meshes = new List<MeshGroup>();
        foreach (MeshGroup mesh in preset.meshes) meshes.Add(mesh);
    }
}