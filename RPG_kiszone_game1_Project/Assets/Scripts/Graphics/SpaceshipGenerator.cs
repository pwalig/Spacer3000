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
    public int amount;
    public AnimationCurve scaleDistribution;
    public Vector3 randomPostRotation;
    public List<int> assignToLayers;
    public List<float> parameters;
}

[System.Serializable]
public struct MeshGroup
{
    public List<Mesh> meshes;
}

public class SpaceshipGenerator : MonoBehaviour
{
    [SerializeField] List<string> notes;
    [SerializeField] bool randomizeSeeds;
    GameObject partTempalte;
    [SerializeField] List<Pass> passes;
    [SerializeField] List<Material> materials;
    [SerializeField] List<MeshGroup> meshes;
    List<List<GameObject>> customLayers = new();
    List<GameObject> parts = new();

    void Start()
    {
        Generate();
    }

    void RunPass(Pass pass)
    {
        if (!pass.randomizeSeed && !randomizeSeeds)
            Random.InitState(pass.seed);

        for (int i = 0; i < pass.amount; i++)
        {
            // instatiate new part
            GameObject part = Instantiate(partTempalte, transform);

            //SET POSTION
            switch (pass.placementMethod)
            {
                case Pass.PartPlacementMethod.Loose:
                    part.transform.localPosition = new Vector3(Random.Range(-pass.parameters[0] / 2f, pass.parameters[0] / 2f), Random.Range(-pass.parameters[1] / 2f, pass.parameters[1] / 2f), Random.Range(-pass.parameters[2] / 2f, pass.parameters[2] / 2f));
                    break;

                case Pass.PartPlacementMethod.Interpolate:
                    // pick two different parts to interpolate between
                    int partid1 = Random.Range(0, customLayers[(int)pass.parameters[0]].Count);
                    GameObject prevpart1 = customLayers[(int)pass.parameters[0]][partid1];
                    int partid2 = Random.Range(0, customLayers[(int)pass.parameters[0]].Count);
                    while (partid2 == partid1)
                        partid2 = Random.Range(0, customLayers[(int)pass.parameters[0]].Count);
                    GameObject prevpart2 = customLayers[(int)pass.parameters[0]][partid2];

                    // set position somewhere in between
                    float split = Random.Range(0f, 1f);
                    part.transform.localPosition = (prevpart1.transform.localPosition * split) + (prevpart2.transform.localPosition * (1f - split));

                    // set rotation to poit at one of chosen parts to interpolate between
                    if (pass.rotationMethod == Pass.PartRotationMethod.Normal)
                        part.transform.rotation = Quaternion.LookRotation(prevpart1.transform.position - part.transform.position);
                        break;

                case Pass.PartPlacementMethod.Scatter:
                    // pick part to scatter on
                    GameObject prevpart = customLayers[(int)pass.parameters[0]][Random.Range(0, customLayers[(int)pass.parameters[0]].Count)];

                    // reset chosen parts rotation
                    Vector3 tempEuler = prevpart.transform.eulerAngles;
                    //prevpart.transform.SetParent(null);
                    prevpart.transform.eulerAngles = Vector3.zero;

                    // set position on the surface of chosen part
                    Mesh premesh = prevpart.GetComponent<MeshFilter>().mesh;
                    int ind = Random.Range(0, Mathf.FloorToInt(premesh.triangles.Length / 3));
                    Vector3 pos = (premesh.vertices[premesh.triangles[ind * 3]] + premesh.vertices[premesh.triangles[ind * 3 + 1]] + premesh.vertices[premesh.triangles[ind * 3 + 2]]) / 3f;
                    part.transform.localPosition = (pos * prevpart.transform.localScale.z) + prevpart.transform.localPosition;

                    // allign new parts rotation it to surface normal
                    if (pass.rotationMethod == Pass.PartRotationMethod.Normal)
                        part.transform.rotation = Quaternion.LookRotation(Vector3.Cross(premesh.vertices[premesh.triangles[ind * 3 + 1]] - premesh.vertices[premesh.triangles[ind * 3]], premesh.vertices[premesh.triangles[ind * 3 + 2]] - premesh.vertices[premesh.triangles[ind * 3]]));

                    // parent new part to previous one and rotate back into place
                    part.transform.SetParent(prevpart.transform);
                    prevpart.transform.eulerAngles = tempEuler;
                    //prevpart.transform.SetParent(transform);
                    part.transform.SetParent(transform);
                    break;

                default:
                    // do nothing
                    break;
            }

            // SET ROTATION
            switch (pass.rotationMethod)
            {
                case Pass.PartRotationMethod.Set:
                    // fill remaining parameters with default values
                    while (pass.parameters.Count < 6)
                        pass.parameters.Add(0f);

                    part.transform.localEulerAngles = new Vector3(pass.parameters[3], pass.parameters[4], pass.parameters[5]);
                    break;

                case Pass.PartRotationMethod.Outward:
                    // fill remaining parameters with default values
                    while (pass.parameters.Count < 5)
                        pass.parameters.Add(0f);

                    //set rotation
                    Vector3 origin = (transform.position * (1f - pass.parameters[3])) + (part.transform.parent.position * pass.parameters[3]); // parameter 3 (0f, 1f) gradually blends origin between root and direct parent
                    origin.y += (part.transform.position.y - origin.y) * pass.parameters[4]; // parameter 4 (0f, 1f) flattens gradually rotation to y axis only
                    part.transform.rotation = Quaternion.LookRotation(part.transform.position - origin);
                    break;

                default:
                    // do nothing
                    break;
            }

            // add mesh and material
            part.AddComponent<MeshFilter>().mesh = meshes[pass.partsGroup].meshes[Random.Range(0, meshes[pass.partsGroup].meshes.Count)];
            part.AddComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Count)];

            // set random rotation and scale
            part.transform.Rotate(new Vector3(Random.Range(-pass.randomPostRotation.x, pass.randomPostRotation.x), Random.Range(-pass.randomPostRotation.y, pass.randomPostRotation.y), Random.Range(-pass.randomPostRotation.z, pass.randomPostRotation.z)));
            part.transform.localScale = Vector3.one * pass.scaleDistribution.Evaluate(Random.Range(0f, 1f));

            // assign to groups
            parts.Add(part);
            if (pass.assignToLayers.Count > -1)
                foreach (int layer in pass.assignToLayers)
                    customLayers[layer].Add(part);

            // MAKE MIRRORED PART
            if (pass.mirror) {
                GameObject part2 = Instantiate(part, part.transform.parent);
                i++;

                // adjust transforms
                part2.transform.position = new Vector3(part.transform.position.x * -1f, part.transform.position.y, part.transform.position.z);
                part2.transform.localScale = new Vector3(-1f, 1f, 1f) * part.transform.localScale.z;
                part2.transform.eulerAngles = new Vector3(part2.transform.eulerAngles.x, -part2.transform.eulerAngles.y, -part2.transform.eulerAngles.z);

                // assign to groups
                parts.Add(part2);
                if (pass.assignToLayers.Count > -1)
                    foreach (int layer in pass.assignToLayers)
                        customLayers[layer].Add(part2);
            }
        }
    }

    public void Generate()
    {
        Clear();
        // clear spaceship parenting and rotation
        partTempalte = transform.GetChild(0).gameObject;
        Transform postParent = transform.parent;
        transform.SetParent(null);
        Vector3 postRotation = transform.eulerAngles;
        transform.eulerAngles = Vector3.zero;
        Vector3 postPosition = transform.position;
        transform.position = Vector3.zero;

        // initialise custom layers
        foreach (Pass pass in passes)
            foreach (int count in pass.assignToLayers)
                while (customLayers.Count <= count)
                    customLayers.Add(new List<GameObject>());

        // run passes
        foreach (Pass p in passes)
        {
            RunPass(p);
        }

        // rotate ship and parent it back
        transform.position = postPosition;
        transform.eulerAngles = postRotation;
        transform.SetParent(postParent);
    }

    public void Clear()
    {
        foreach (List<GameObject> _parts in customLayers)
            _parts.Clear();
        customLayers.Clear();

        foreach (GameObject part in parts)
        {
            Destroy(part);
        }
        parts.Clear();
    }
}
