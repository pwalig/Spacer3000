using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
public class TerrainGenerator : MonoBehaviour
{
    [System.Serializable] public struct GeneratorPass
    {
        public enum Function { Random, Perlin, SineX, TangentX, SineY, TangentY, Constatnt };
        public enum Mode { Add, Multiply, Subtract, Divide, Min, Max, Average };
        public Function function;
        public Mode overlayMode;
        public Vector3 direction;
        public float amplitude;
        public float frequency;
    }

    [SerializeField] List<GeneratorPass> passes;

    [SerializeField] Vector2 distance;
    [SerializeField] Vector2 resolution;
    [SerializeField] Material material;

    void Awake()
    {
        if(GetComponent<MeshFilter>() == null)
        {
            gameObject.AddComponent<MeshFilter>().mesh = Distort(MakeBasePlane(distance, resolution), passes, resolution);
            gameObject.AddComponent<MeshRenderer>().material = material;
            GetComponent<Renderer>().material.mainTexture = GenerateTexture(new Vector2Int(256, 256));
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
        return mesh;
    }

    Vector3 EvaluatePass(Vector3 _base, GeneratorPass pass, Vector3 position)
    {
        Vector3 functionResult = pass.direction;
        switch (pass.function)
        {
            case GeneratorPass.Function.Random:
                functionResult *= Random.Range(-1f, 1f);
                break;
            case GeneratorPass.Function.Perlin:
                functionResult *= PerliNoise.Perlin(position.x * pass.frequency, position.y * pass.frequency);
                break;
            case GeneratorPass.Function.SineX:
                functionResult *= Mathf.Sin(position.x * pass.frequency);
                break;
            case GeneratorPass.Function.SineY:
                functionResult *= Mathf.Sin(position.y * pass.frequency);
                break;
            case GeneratorPass.Function.TangentX:
                functionResult *= Mathf.Tan(position.x * pass.frequency);
                break;
            case GeneratorPass.Function.TangentY:
                functionResult *= Mathf.Tan(position.y * pass.frequency);
                break;
        }
        functionResult *= pass.amplitude;
        switch (pass.overlayMode)
        {
            case GeneratorPass.Mode.Add:
                _base += functionResult;
                break;
            case GeneratorPass.Mode.Multiply:
                _base = new Vector3(_base.x * functionResult.x, _base.y * functionResult.y, _base.z * functionResult.z);
                break;
            case GeneratorPass.Mode.Subtract:
                _base -= functionResult;
                break;
            case GeneratorPass.Mode.Divide:
                _base = new Vector3(_base.x / functionResult.x, _base.y / functionResult.y, _base.z / functionResult.z);
                break;
            case GeneratorPass.Mode.Min:
                _base = new Vector3(Mathf.Min(_base.x, functionResult.x), Mathf.Min(_base.y, functionResult.y), Mathf.Min(_base.z, functionResult.z));
                break;
            case GeneratorPass.Mode.Max:
                _base = new Vector3(Mathf.Max(_base.x, functionResult.x), Mathf.Max(_base.y, functionResult.y), Mathf.Max(_base.z, functionResult.z));
                break;
            case GeneratorPass.Mode.Average:
                _base = new Vector3((_base.x + functionResult.x)/2f, (_base.y + functionResult.y) / 2f, (_base.z + functionResult.z) / 2f);
                break;
        }
        return _base;
    }

    Mesh Distort (Mesh mesh, List<GeneratorPass> passes, Vector2 resolution)
    {
        Vector3[] newVertices = mesh.vertices;
        for (int x = 0; x < resolution.x; x++)
        {
            for (int y = 0; y < resolution.y; y++)
            {
                int vertId = y + (int)(x * resolution.y);
                Vector3 transformation = Vector3.zero;
                foreach (GeneratorPass pass in passes)
                    transformation = EvaluatePass(transformation, pass, newVertices[vertId] + transform.position);

                newVertices[vertId] += transformation;
            }
        }
        mesh.vertices = newVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        return mesh;
    }

    Texture2D GenerateTexture(Vector2Int resolution)
    {
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        Texture2D texture = new Texture2D(resolution.x, resolution.y, TextureFormat.ARGB32, false);

        // set the pixel values
        for (int i = 0; i<resolution.x; i++)
            for (int j = 0; j<resolution.y; j++)
                texture.SetPixel(i, j, new Color((float)i/resolution.x, (float)j/resolution.y, PerliNoise.Perlin(i*0.02f, j*0.02f)*2f, 1f));

        // Apply all SetPixel calls
        texture.Apply();
        return texture;
    }
}
