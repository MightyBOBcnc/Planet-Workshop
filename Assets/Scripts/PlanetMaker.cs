using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlanetMaker {

    public static void CreatePlanet (PlanetOptions options) {

        GameObject planetParent = new GameObject ("PlanetParent");

        options.material.mainTexture = CreateTexture (options);

        for (byte i = 1; i <= 6; i++) {
            GameObject g = CreatePlane (i, options);
            g.transform.parent = planetParent.transform;
        }

        planetParent.transform.localScale = 1f / options.radius * Vector3.one;
    }

    public static GameObject CreatePlane (byte idx, PlanetOptions options) {

        // IDX:
        // 1=top, 2=bottom, 3=left, 4=right, 5=back, 6=front;

        GameObject g = new GameObject ("Plane " + idx);

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
            vertices[i] += WorldNoise (wp.x, wp.y, wp.z, options) * vertices[i].normalized * options.radius;
        }

        Mesh m = new Mesh ();
        m.vertices = vertices;
        m.triangles = triangles;
        m.uv = uvs;

        m.RecalculateNormals ();
        m.RecalculateTangents ();

        g.AddComponent<MeshFilter> ().sharedMesh = m;
        g.AddComponent<MeshRenderer> ().material = options.material;

        return g;
    }

    public static Texture2D CreateTexture (PlanetOptions p) {

        int xRes = 1024;
        int yRes = 512;

        Texture2D t = new Texture2D (xRes, yRes);
        Color[] pixels = new Color[xRes * yRes];

        for (int x = 0; x < xRes; x++) {
            for (int y = 0; y < yRes; y++) {
                float longitude = (-180 + x * 360f / xRes) * Mathf.Deg2Rad;
                float latitude = (-90 + y * 180f / yRes) * Mathf.Deg2Rad;

                Vector3 point3d = p.radius * new Vector3 (Mathf.Cos (longitude) * Mathf.Cos (latitude), Mathf.Sin (latitude), Mathf.Sin (longitude) * Mathf.Cos (latitude));

                //pixels[x + y * xRes] = new Color (Mathf.Pow (point3d.x / p.radius / 2f + 0.5f, 7), Mathf.Pow (point3d.y / p.radius / 2f + 0.5f, 7), Mathf.Pow (point3d.z / p.radius / 2f + 0.5f, 7));
                pixels[x + y * xRes] = (WorldNoise (point3d, p) / 2f + 0.5f) * Color.Lerp (p.col1, p.col2, (per3d (-point3d, p) / 2f / (p.colourBlending + 0.0001f)) + 0.5f);
            }
        }

        t.SetPixels (pixels);
        t.Apply ();

        return t;
    }

    static float WorldNoise (float x, float y, float z, PlanetOptions p) {
        return (per3d (x, y, z, p) - 0.25f * ridge3d (x, y, z, p)) * p.noiseHeight + craters (x, y, z, p) / 3f;
    }

    static float WorldNoise (Vector3 x, PlanetOptions p) => WorldNoise (x.x, x.y, x.z, p);

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

    static float noisef (float x, float y) {
        //return 
        return Mathf.PerlinNoise (x, y);
    }

    static float ridgenoise (float x, float y) {
        return 1f - Mathf.Abs (Mathf.PerlinNoise (x, y) * 2f - 1f);
    }

    static float ridge3d (float x, float y, float z, PlanetOptions p) {
        x /= p.noiseScale;
        y /= p.noiseScale;
        z /= p.noiseScale;
        //return (ridgenoise (y, x) + ridgenoise (z, y) + ridgenoise (z, x)) / 3f;
        return 1f - Mathf.Abs (2f * per3d (y, z, x, p) - 1f);
    }

    static float craters (float x, float y, float z, PlanetOptions p) {
        float sum = 0f;

        for(int i = 0; i < p.craters.Length; i++) {
            float d = Vector3.SqrMagnitude (new Vector3 (x, y, z) - p.craters[i].position);
            //if (d < Mathf.Pow (2f * p.craters[i].size, 2f))
                sum += crater (p.craters[i], Mathf.Sqrt (d) * p.radius) / p.radius;
        }

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

    static public float CalcSignedCentralAngle (Vector3 dir1, Vector3 dir2, Vector3 normal) // https://forum.unity.com/threads/is-vector3-signedangle-working-as-intended.694105/#post-5546026
        => Mathf.Rad2Deg * Mathf.Atan2 (Vector3.Dot (Vector3.Cross (dir1, dir2), normal), Vector3.Dot (dir1, dir2));
}
