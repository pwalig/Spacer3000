using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class PerliNoise
{
    public static float Interpolate(float a0, float a1, float w)
    {
        /* // You may want clamping by inserting:
         * if (0.0 > w) return a0;
         * if (1.0 < w) return a1;
         */
        return (a1 - a0) * w + a0;
        /* // Use this cubic interpolation [[Smoothstep]] instead, for a smooth appearance:
         * return (a1 - a0) * (3.0 - w * 2.0) * w * w + a0;
         *
         * // Use [[Smootherstep]] for an even smoother result with a second derivative equal to zero on boundaries:
         * return (a1 - a0) * ((w * (w * 6.0 - 15.0) + 10.0) * w * w * w) + a0;
         */
    }

    /* Create pseudorandom direction vector
     */
    private static Vector2 RandomGradient(int ix, int iy)
    {
        // No precomputed gradients mean this works for any number of grid coordinates
        int w = 8 * sizeof(uint);
        int s = w / 2; // rotation width
        uint a = (uint)ix, b = (uint)iy;
        a *= 3284157443; b ^= a << s | a >> w - s;
        b *= 1911520717; a ^= b << s | b >> w - s;
        a *= 2048419325;
        float random = (float)(a * (3.14159265 / ~(~0u >> 1))); // in [0, 2*Pi]
        Vector2 v;
        v.x = Mathf.Cos(random); v.y = Mathf.Sin(random);
        return v;
    }

    // Computes the dot product of the distance and gradient vectors.
    private static float DotGridGradient(int ix, int iy, float x, float y)
    {
        // Get gradient from integer coordinates
        Vector2 gradient = RandomGradient(ix, iy);

        // Compute the distance vector
        float dx = x - (float)ix;
        float dy = y - (float)iy;

        // Compute the dot-product
        return (dx * gradient.x + dy * gradient.y);
    }

    // Compute Perlin noise at coordinates x, y
    public static float Perlin(float x, float y)
    {
        // Determine grid cell coordinates
        int x0 = (int)Mathf.Floor(x);
        int x1 = x0 + 1;
        int y0 = (int)Mathf.Floor(y);
        int y1 = y0 + 1;

        // Determine interpolation weights
        // Could also use higher order polynomial/s-curve here
        float sx = x - (float)x0;
        float sy = y - (float)y0;

        // Interpolate between grid point gradients
        float n0, n1, ix0, ix1, value;

        n0 = DotGridGradient(x0, y0, x, y);
        n1 = DotGridGradient(x1, y0, x, y);
        ix0 = Interpolate(n0, n1, sx);

        n0 = DotGridGradient(x0, y1, x, y);
        n1 = DotGridGradient(x1, y1, x, y);
        ix1 = Interpolate(n0, n1, sx);

        value = Interpolate(ix0, ix1, sy);
        return value; // Will return in range -1 to 1. To make it in range 0 to 1, multiply by 0.5 and add 0.5
    }
}
[System.Serializable]
public class Operation
{
    public enum Operator { Constant_0, X_0, Y_0, Z_0, Add_2, Subtract_2, Multiply_2, Divide_2, Power_2, Min_inf, Max_inf, Sin_1, Cos_1, Tan_1, Ctg_1, Perlin_2, Random_0 }
    public Operator _operator;
    public List<Operation> operands;
    public float value = 1f; // works only if Constant_0 is selected
    public float Evaluate(Vector3 vertexPosition)
    {
        switch (_operator)
        {
            case Operator.Constant_0:
                return value;
            case Operator.X_0:
                return vertexPosition.x;
            case Operator.Y_0:
                return vertexPosition.y;
            case Operator.Z_0:
                return vertexPosition.z;
            case Operator.Add_2:
                return operands[0].Evaluate(vertexPosition) + operands[1].Evaluate(vertexPosition);
            case Operator.Subtract_2:
                return operands[0].Evaluate(vertexPosition) - operands[1].Evaluate(vertexPosition);
            case Operator.Multiply_2:
                return operands[0].Evaluate(vertexPosition) * operands[1].Evaluate(vertexPosition);
            case Operator.Divide_2:
                return operands[0].Evaluate(vertexPosition) / operands[1].Evaluate(vertexPosition);
            case Operator.Power_2:
                return Mathf.Pow(operands[0].Evaluate(vertexPosition), operands[1].Evaluate(vertexPosition));
            case Operator.Min_inf:
                float min_res = operands[0].Evaluate(vertexPosition);
                for (int i = 1; i<operands.Count; i++)
                {
                    float n = operands[i].Evaluate(vertexPosition);
                    if (n < min_res) min_res = n;
                }
                return min_res;
            case Operator.Max_inf:
                float max_res = operands[0].Evaluate(vertexPosition);
                for (int i = 1; i < operands.Count; i++)
                {
                    float n = operands[i].Evaluate(vertexPosition);
                    if (n > max_res) max_res = n;
                }
                return max_res;
            case Operator.Sin_1:
                return Mathf.Sin(operands[0].Evaluate(vertexPosition));
            case Operator.Cos_1:
                return Mathf.Cos(operands[0].Evaluate(vertexPosition));
            case Operator.Tan_1:
                return Mathf.Tan(operands[0].Evaluate(vertexPosition));
            case Operator.Ctg_1:
                return 1f / Mathf.Tan(operands[0].Evaluate(vertexPosition));
            case Operator.Perlin_2:
                return PerliNoise.Perlin(operands[0].Evaluate(vertexPosition), operands[1].Evaluate(vertexPosition));
            case Operator.Random_0:
                return Random.Range(0f, 1f);
        }
        return 0f;
    }
}

[System.Serializable]
public class Transformation
{
    public Operation operation;
    public Vector3 direction;
    public bool active = true;
    public void Translate(ref Vector3 vert, Vector3 offset, Vector3 normal)
    {
        if (active)
        {
            if (direction.magnitude == 0f) vert += normal * operation.Evaluate(vert + offset);
            else vert += direction * operation.Evaluate(vert + offset);
        }
    }
}
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] List<Transformation> transformations;

    [SerializeField] Vector2 distance;
    [SerializeField] Vector2 resolution;
    [SerializeField] Material material;

    void Awake()
    {
        if (GetComponent<MeshFilter>() == null)
        {
            Vector2 _resolution = new Vector2(Mathf.Round(resolution.x * GameData.GetQualityMultiplier()), Mathf.Round(resolution.y * GameData.GetQualityMultiplier()));
            Vector2Int textureResolution = Vector2Int.one * (int)Mathf.Pow(2f, QualitySettings.GetQualityLevel() + 4);

            gameObject.AddComponent<MeshFilter>().mesh = Distort(MakeBasePlane(distance / GameData.GetQualityMultiplier(), _resolution), transformations, _resolution);
            gameObject.AddComponent<MeshRenderer>().material = material;
            GetComponent<Renderer>().material.mainTexture = GenerateTexture(textureResolution);
        }
    }

    Mesh MakeBasePlane(Vector2 distance, Vector2 resolution)
    {
        Mesh mesh = new Mesh();
        Vector3[] newVertices = new Vector3 [(int)(resolution.x * resolution.y)];
        Vector2[] newUV = new Vector2[(int)(resolution.x * resolution.y)];
        int[] newTriangles = new int[(int)(6 * (resolution.x-1) * (resolution.y-1))];

        for (int x = 0; x < resolution.x; x++)
        {
            for (int y = 0; y < resolution.y; y++)
            {
                newVertices[y + (int)(x * resolution.y)] = new Vector3(x * distance.x, y * distance.y);
                newUV[y + (int)(x * resolution.y)] = new Vector3(x / (resolution.x - 1), y / (resolution.y - 1));
            }
        }
        for (int x = 0; x < resolution.x - 1; x++)
        {
            for (int y = 0; y < resolution.y - 1; y ++)
            {
                int trianId = y + (int)(x * (resolution.y - 1));
                newTriangles[trianId * 3] = y + (int)(x * resolution.y);
                newTriangles[trianId * 3 + 1] = y + (int)((x + 1) * resolution.y);
                newTriangles[trianId * 3 + 2] = y + 1 + (int)((x + 1) * resolution.y);
            }
        }
        for (int x = 0; x < resolution.x - 1; x++)
        {
            for (int y = 0; y < resolution.y - 1; y++)
            {
                int trianId = y + (int)(x * (resolution.y - 1));
                newTriangles[trianId * 3 + 1 + (newTriangles.Length/2)] = y + (int)(x * resolution.y);
                newTriangles[trianId * 3 + (newTriangles.Length/2)] = y + 1 + (int)(x * resolution.y);
                newTriangles[trianId * 3 + 2 + (newTriangles.Length/2)] = y + 1 + (int)((x + 1) * resolution.y);
            }
        }
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
        mesh.name = "terrainMesh";
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        return mesh;
    }

    Mesh Distort (Mesh mesh, List<Transformation> transformations, Vector2 resolution)
    {
        foreach (Transformation transformation in transformations)
        {
            Vector3[] newVertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            for (int x = 0; x < resolution.x; x++)
            {
                for (int y = 0; y < resolution.y; y++)
                {
                    int vertId = y + (int)(x * resolution.y);
                    transformation.Translate(ref newVertices[vertId], transform.position, normals[vertId]);
                }
            }
            mesh.vertices = newVertices;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
        }
        
        return mesh;
    }

    Texture2D GenerateTexture(Vector2Int resolution)
    {
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        Texture2D texture = new Texture2D(resolution.x, resolution.y, TextureFormat.ARGB32, false);

        // set the pixel values
        for (int i = 0; i<resolution.x; i++)
            for (int j = 0; j<resolution.y; j++)
                texture.SetPixel(i, j, new Color((float)i/resolution.x, (float)j/resolution.y, PerliNoise.Perlin((float)i / resolution.x * 0.02f, (float)j / resolution.y * 0.02f)*2f, 1f));

        // Apply all SetPixel calls
        texture.Apply();
        return texture;
    }
}
