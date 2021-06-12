using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoiseLayerPanel : MonoBehaviour
{
    public InputField name;
    public Text header;
    public Dropdown type;
    public PropertyPanel scale;
    public PropertyPanel height;
    public PropertyPanel octaves;

    private void Start () {
        transform.Find ("PopContent").Find ("DeleteButton").GetComponent<Button> ().onClick.AddListener (delegate { GameObject.Find ("Handler").GetComponent<WorkshopManager> ().RemoveNoiseLayer (this); });
    }

    private void Update () {
        header.text = name.text != "" ? name.text : "Unnamed Noise Layer";
    }

    public NoiseLayer GetLayer (int index, float seed) {
        return new NoiseLayer ((NoiseLayer.NoiseType) type.value, scale.GetValue (), height.GetValue (), (int) octaves.GetValue (), seed * (index + 1));
    }
}

public class NoiseLayer {
    public enum NoiseType { Perlin, Ridge, Plateau };

    public NoiseType type;
    public float scale;
    public float height;
    public int octaves;
    public float seed;

    public NoiseLayer (NoiseType type, float scale, float height, int octaves, float seed) {
        this.type = type;
        this.scale = scale;
        this.height = height;
        this.octaves = octaves;
        this.seed = seed;
    }

    public float Evaluate (float x, float y, float z) {
        float div = scale;

        x /= div; y /= div; z /= div;
        x += 38291.5f + seed; y += -28199.5f - seed; z += 7065.5f + seed;

        float n = 0f;

        for(int i = 0; i < octaves; i++) {
            x *= 2f; y *= 2f; z *= 2f;
            n += noisef (x, y, z) / Mathf.Pow (2f, i);
        }

        n /= (2f - 2f * Mathf.Pow (0.5f, octaves));

        return n * height;
    }

    float noisef (float x, float y, float z) {
        float val = Perlin.Noise (x - seed, y, z + seed);
        switch (type) {
            case NoiseType.Ridge:
                return PlanetMaker.TransformRidge (val);
            case NoiseType.Plateau:
                return PlanetMaker.TransformPlateau (val);
        }
        return val;
    }
}
