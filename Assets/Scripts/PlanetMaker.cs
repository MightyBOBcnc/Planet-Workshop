using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMaker : MonoBehaviour {

    public ComputeShader craterShader;

    public Texture3D tex3d;

    public void CreatePlanet (PlanetOptions options) {

        GameObject planetParent = new GameObject ("PlanetParent");

        options.material.mainTexture = CreateTexture (options, options.texRes);

        if (options.craters.Length > 0)
            tex3d = CreateCraterMap (options);

        for (byte i = 1; i <= 6; i++) {
            GameObject g = new GameObject ("Plane " + i);
            StartCoroutine (CreatePlane (i, options, g));
            g.transform.parent = planetParent.transform;
        }

        planetParent.transform.localScale = 1f / options.radius * Vector3.one;
    }

    public Texture3D CreateCraterMap (PlanetOptions options) {
    //public float[,,] CreateCraterMap (PlanetOptions options) {

        int r = 256;

        Texture3D t = new Texture3D (r, r, r, TextureFormat.Alpha8, false);
        //float[,,] t = new float[r, r, r];
        //RenderTexture t = new RenderTexture (r, r, r, RenderTextureFormat.Depth);
        //t.enableRandomWrite = true;

        float[] values = new float[(int) Mathf.Pow (r, 3)];
        ComputeBuffer buffer = new ComputeBuffer (values.Length, sizeof (float));
        craterShader.SetBuffer (0, "Result", buffer);

        Vector4[] craterVectors = new Vector4[options.craters.Length];
        for (int i = 0; i < craterVectors.Length; i++) {
            craterVectors[i] = (Vector4) options.craters[i].position + new Vector4 (0, 0, 0, 1) * options.craters[i].size;
        }

        craterShader.SetVectorArray ("_Craters", craterVectors);

        craterShader.SetInt ("_NumCraters", options.craters.Length);
        craterShader.SetInt ("_R", r);
        craterShader.SetFloat ("_SizeParameter", options.radius * 1.25f);
        //craterShader.SetTexture (0, "Result", t);

        int numGroups = Mathf.CeilToInt (r / 8f); // texture size in one dimension divided by the numthreads bit from the shader

        craterShader.Dispatch (0, numGroups, numGroups, numGroups);

        buffer.GetData (values);
        buffer.Release ();

        Color[] colours = new Color[(int) Mathf.Pow (r, 3)];

        float min = Mathf.Min (values);
        float max = Mathf.Max (values);

        print ("Min: " + min);
        print ("Max: " + max);

        for (int z = 0; z < t.depth; z++) {
            for (int y = 0; y < t.height; y++) {
                for (int x = 0; x < t.width; x++) {
                    int idx = x + (y * r) + (z * (r * r));
                    colours[idx] = Color.black * Map(values[idx], min, max, 0f, 1f);
                    //print (idx + ", " + values[idx]);
                }
            }
        }

        t.SetPixels (colours);
        t.Apply ();

        return t;
    }

    public Texture2D CreateHeightmap (PlanetOptions options, int width) {
        float[,] heights = new float[width, width / 2];
        float max = float.MinValue;
        float min = float.MaxValue;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < width / 2; y++) {
                float longitude = (-180 + x * 360f / width) * Mathf.Deg2Rad;
                float latitude = (-90 + y * 180f / (width / 2)) * Mathf.Deg2Rad;

                Vector3 point3d = options.radius * new Vector3 (Mathf.Cos (longitude) * Mathf.Cos (latitude), Mathf.Sin (latitude), Mathf.Sin (longitude) * Mathf.Cos (latitude));
                float v = FinalWorldHeight (point3d, options);
                heights[x, y] = v;
                if (v < min) min = v;
                if (v > max) max = v;
            }
        }

        Texture2D t = new Texture2D (width, width / 2);
        Color[] pixels = new Color[width * width / 2];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < width / 2; y++) {
                pixels[x + y * width] = Color.white * Mathf.InverseLerp (min, max, heights[x, y]) + new Color(0, 0, 0, 1);
            }
        }

        t.SetPixels (pixels);
        t.Apply ();
        
        return t;
    }

    public static float CraterMapCoordToWorldCoord (float x, PlanetOptions o) => (2f * x - 1f) * (o.radius * 1.25f);

    public static float WorldCoordToCraterMapCoord (float x, PlanetOptions o) => (x / (o.radius * 1.25f)) / 2f + 0.5f;

    public float SampleCraterMap (float x, float y, float z, PlanetOptions o) {

        if (o.craters.Length == 0) return 0f;

        float mx = WorldCoordToCraterMapCoord (x, o);
        float my = WorldCoordToCraterMapCoord (y, o);
        float mz = WorldCoordToCraterMapCoord (z, o);

        return -3f * tex3d.GetPixelBilinear (mx, my, mz).a;
    }

    public IEnumerator CreatePlane (byte idx, PlanetOptions options, GameObject g) {

        // IDX:
        // 1=top, 2=bottom, 3=left, 4=right, 5=back, 6=front;

        switch (idx) {
            case 0:
                break;
            case 1:
                // Top plane
                break;
            case 2:
                // Bottom plane
                g.transform.Rotate (g.transform.right * 180f);
                break;
            case 3:
                // Left plane
                g.transform.Rotate (g.transform.forward * 90f);
                break;
            case 4:
                // Right plane
                g.transform.Rotate (g.transform.forward * -90f);
                break;
            case 5:
                // Back plane
                g.transform.Rotate (g.transform.right * -90f);
                break;
            case 6:
                // Front plane
                g.transform.Rotate (g.transform.right * 90f);
                break;
        }

        Vector3[] vertices = new Vector3[options.resolution * options.resolution];
        int[] triangles = new int[(int) Mathf.Pow (options.resolution - 1, 2) * 6];
        int ti = 0;
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < options.resolution; i++) {
            for (int j = 0; j < options.resolution; j++) {

                Vector3 v = (new Vector3 (i - (options.resolution - 1) / 2f, (options.resolution - 1) / 2f, j - (options.resolution - 1) / 2f) / (float) options.resolution).normalized * options.radius;

                vertices[i + j * options.resolution] = v;

                if (i < options.resolution - 1 && j < options.resolution - 1) {
                    // create quad
                    if ((i + j) % 2 == 0) {
                        triangles[ti] = i + j * options.resolution;
                        triangles[ti + 1] = i + (j + 1) * options.resolution;
                        triangles[ti + 2] = (i + 1) + (j + 1) * options.resolution;
                        triangles[ti + 3] = (i + 1) + j * options.resolution;
                        triangles[ti + 4] = i + j * options.resolution;
                        triangles[ti + 5] = (i + 1) + (j + 1) * options.resolution;
                    } else {
                        triangles[ti] = i + j * options.resolution;
                        triangles[ti + 1] = i + (j + 1) * options.resolution;
                        triangles[ti + 2] = (i + 1) + j * options.resolution;
                        triangles[ti + 3] = (i + 1) + j * options.resolution;
                        triangles[ti + 4] = i + (j + 1) * options.resolution;
                        triangles[ti + 5] = (i + 1) + (j + 1) * options.resolution;
                    }
                    ti += 6;
                }

                Vector3 worldPoint = Vector3.Scale (g.transform.TransformPoint (v), new Vector3 (1f, 1f, -1f));

                float longitude = CalcSignedCentralAngle (Vector3.right, worldPoint, Vector3.up);

                if (longitude > 180f) longitude -= 360f;
                if (longitude < 180f) longitude += 360f;

                //Vector3 longVector = new Vector3 (Mathf.Cos (Mathf.Deg2Rad * longitude), 0f, Mathf.Sin (Mathf.Deg2Rad * longitude));
                Vector3 longVector = Vector3.ProjectOnPlane (worldPoint, Vector3.up).normalized;
                float latitude = Vector3.SignedAngle (longVector, worldPoint, Vector3.Cross (longVector, worldPoint));
                //float latitude = CalcSignedCentralAngle (longVector, worldPoint, Vector3.Cross (longVector, worldPoint));
                //float latitude = (worldPoint.y / options.radius / 2f + 0.5f) * 90f;
                //Debug.Log (worldPoint + "," + latitude + "," + longVector);

                if (worldPoint.y < 0) latitude *= -1f;

                uvs[i + j * options.resolution] = new Vector2 (longitude / 360f + 0.5f, latitude / 180f + 0.5f);
            }
        }

        for (int i = 0; i < vertices.Length; i++) {
            Vector3 wp = g.transform.TransformPoint (vertices[i]);
            float a = 1f;
            //vertices[i] *= 1f + (2*a * per3d (wp.x, wp.y, wp.z, options) - a) / options.radius;
            Vector3 norm = vertices[i].normalized;
            //vertices[i] += WorldNoise (wp.x, wp.y, wp.z, options) * norm * options.radius;
            //vertices[i] += SampleCraterMap (wp.x, wp.y, wp.z, options) * norm * 0.013f * options.radius;
            vertices[i] += FinalWorldHeight (wp.x, wp.y, wp.z, options) * norm;
        }

        Mesh m = new Mesh ();
        m.vertices = vertices;
        m.triangles = triangles;
        m.uv = uvs;

        m.RecalculateNormals ();
        m.RecalculateTangents ();

        g.AddComponent<MeshFilter> ().sharedMesh = m;
        g.AddComponent<MeshRenderer> ().material = options.material;

        //Debug.Log ("Done");

        yield return null;

        //return g;
    }

    public static Texture2D CreateTexture (PlanetOptions p, int xRes) {

        //int xRes = 512;
        int yRes = xRes / 2; //256;

        Texture2D t = new Texture2D (xRes, yRes);
        Color[] pixels = new Color[xRes * yRes];

        for (int x = 0; x < xRes; x++) {
            for (int y = 0; y < yRes; y++) {
                float longitude = (-180 + x * 360f / xRes) * Mathf.Deg2Rad;
                float latitude = (-90 + y * 180f / yRes) * Mathf.Deg2Rad;

                Vector3 point3d = p.radius * new Vector3 (Mathf.Cos (longitude) * Mathf.Cos (latitude), Mathf.Sin (latitude), Mathf.Sin (longitude) * Mathf.Cos (latitude));

                //pixels[x + y * xRes] = new Color (Mathf.Pow (point3d.x / p.radius / 2f + 0.5f, 7), Mathf.Pow (point3d.y / p.radius / 2f + 0.5f, 7), Mathf.Pow (point3d.z / p.radius / 2f + 0.5f, 7));
                pixels[x + y * xRes] = Color.Lerp (p.col1, p.col2, (scaleNoise (-point3d + Vector3.one * p.seed, p.colourScale * p.radius) / 2f / (p.colourBlending + 0.0001f)) + 0.5f);
            }
        }

        t.SetPixels (pixels);
        t.Apply ();

        return t;
    }

    float FinalWorldHeight(float x, float y, float z, PlanetOptions p) {
        //return WorldNoise (x, y, z, p) * p.radius + SampleCraterMap (x, y, z, p) * 0.013f * p.radius;

        float craters = SampleCraterMap (x, y, z, p) * 0.013f * p.radius;

        return craters;
    }

    float FinalWorldHeight (Vector3 x, PlanetOptions p) => FinalWorldHeight (x.x, x.y, x.z, p);

    static float noisef (float x, float y) {
        return Mathf.PerlinNoise (x, y);
    }

    static float noisef (float x, float y, float z) {
        return Perlin.Noise (x, y, z);
    }

    static float scaleNoise (Vector3 p, float scale) => noisef (p.x / scale, p.y / scale, p.z / scale);

    #region NOISE TRANSFORMATIONS
    public static float TransformRidge (float x) => 1f - 2f * Mathf.Abs (x - 0.5f);
    public static float TransformPlateau (float x) => (2f * x - 1f) / (1 + Mathf.Pow (2f * x - 1f, 2)) - 0.5f;
    #endregion NOISE TRANSFORMATIONS

    static public float CalcSignedCentralAngle (Vector3 dir1, Vector3 dir2, Vector3 normal) // https://forum.unity.com/threads/is-vector3-signedangle-working-as-intended.694105/#post-5546026
        => Mathf.Rad2Deg * Mathf.Atan2 (Vector3.Dot (Vector3.Cross (dir1, dir2), normal), Vector3.Dot (dir1, dir2));

    // https://forum.unity.com/threads/mapping-or-scaling-values-to-a-new-range.180090/#post-2241099
    public static float Map (float x, float in_min, float in_max, float out_min, float out_max) {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
}

// OBSOLETE
/*

    static float per3d (float x, float y, float z, PlanetOptions p) {
        float div = p.noiseScale;

        x /= div; y /= div; z /= div;
        x += 38291.5f + p.seed; y += -28199.5f - p.seed; z += 7065.5f + p.seed;

        float n = 0f;

        for (int i = 0; i < p.noiseOctaves; i++) {
            x *= 2f; y *= 2f; z *= 2f;
            n += (noisef (x, y) + noisef (y, z) + noisef (x, z)) / 3f / Mathf.Pow (2f, i);
        }

        n /= (2f - 2f * Mathf.Pow (0.5f, p.noiseOctaves));

        n = n * 2f - 1f; // set middle to zero

        return n;
    }

    static float per3d (Vector3 point, PlanetOptions p) {
        return per3d (point.x, point.y, point.z, p);
    }

    static float ridge3d (float x, float y, float z, PlanetOptions p) {
        x /= p.noiseScale;
        y /= p.noiseScale;
        z /= p.noiseScale;
        //return (ridgenoise (y, x) + ridgenoise (z, y) + ridgenoise (z, x)) / 3f;
        return 1f - Mathf.Abs (2f * per3d (y, z, x, p) - 1f);
    }

    static float WorldNoise (float x, float y, float z, PlanetOptions p) {
        return (per3d (x, y, z, p) - 0.25f * ridge3d (x, y, z, p)) * p.noiseHeight;
    }

    static float WorldNoise (Vector3 x, PlanetOptions p) => WorldNoise (x.x, x.y, x.z, p);

static float ridgenoise (float x, float y) {
        return 1f - Mathf.Abs (Mathf.PerlinNoise (x, y) * 2f - 1f);
    }

static float craters (float x, float y, float z, PlanetOptions p) {
    float sum = 0f;

    for(int i = 0; i < p.craters.Length; i++) {
        float d = Vector3.SqrMagnitude (new Vector3 (x, y, z) - p.craters[i].position);
        //if (d < Mathf.Pow (2f * p.craters[i].size, 2f))
            sum += crater (p.craters[i], Mathf.Sqrt (d) * p.radius) / p.radius;
    }

    //sum -= crater (p.craters[0], 100f) * p.craters.Length;

    return sum;
}
static float crater (Crater c, float dist) {
    float parabola = (1 / c.size * dist * dist - c.size) * 0.5f;
    float ridge = c.size * c.size / (1f + dist * dist);
    float floor = -0.1f;
    float S = 5f; // smoothing parameter
    float P = 0.008f; // peak parameter
    return SmoothMin (SmoothMin (ridge, parabola, S), floor, -S) + P * ridge;
}

// based on https://iquilezles.org/www/articles/smin/smin.htm
static float SmoothMin (float a, float b, float k) {
    return -1f / k * Mathf.Log (Mathf.Exp (-a * k) + Mathf.Exp (-b * k));
}
*/

/* This one is for the CPU
public static Texture3D CreateCraterMap (PlanetOptions options) {
    byte r = 64;
    Texture3D t = new Texture3D (r, r, r, TextureFormat.Alpha8, false);

    Color[] colours = new Color[(int) Mathf.Pow (r, 3)];

    Debug.Log (CraterMapCoordToWorldCoord (0f, options) + "," + CraterMapCoordToWorldCoord (1f, options));

    for(int z = 0; z < t.depth; z++) {
        for(int y = 0; y < t.height; y++) {
            for(int x = 0; x < t.width; x++) {
                int idx = x + (y * r) + (z * (r * r));
                //colours[idx] = Color.white * WorldNoise (x, y, z, options); // TESTING ONLY

                float wx = CraterMapCoordToWorldCoord (x / (float) r, options);
                float wy = CraterMapCoordToWorldCoord (y / (float) r, options);
                float wz = CraterMapCoordToWorldCoord (z / (float) r, options);

                colours[idx] = Color.black * (2f - craters (wx, wy, wz, options));
            }
        }
    }

    t.SetPixels (colours);
    t.Apply ();

    return t;
}
*/