using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopManager : MonoBehaviour
{

    public Material planetMaterial;

    // PLANET OPTIONS
    [Header("Planet Options")]
    public PropertyPanel radiusPanel;

    // NOISE
    [Header("Noise Options")]
    public GameObject noiseLayerPanelPrefab;
    public RectTransform noiseLayerPanelParent;
    public List<NoiseLayerPanel> noiseLayerPanels;

    // CRATERS
    [Header("Crater Options")]
    public PropertyPanel numCratersPanel;
    public PropertyPanel craterMaxSizePanel;

    // COLOUR
    [Header("Colour Options")]
    public ColourProperty surfaceColour1;
    public ColourProperty surfaceColour2;
    public PropertyPanel colourBlendingPanel;
    public PropertyPanel colourScalePanel;
    public PropertyPanel colourOctavesPanel;
    public ColourProperty surfaceColourLow;
    public ColourProperty surfaceColourHigh;
    public SimpleProperty gradientBlendingPanel;
    public PropertyPanel gradientBlendingNoisePanel;
    public PropertyPanel gradientBlendingSmoothnessPanel;

    // EXPORT
    [Header("Export Options")]
    public SimpleProperty heightmapResolutionPanel;
    public SimpleProperty textureResolutionPanel;
    public InputField planetName;

    PlanetOptions p;

    private void Start () {
        UpdatePlanet ();
    }

    private void Update () {
        // Keep these updating since we want any exports to reflect these values (not the values at last planet generation)
        p.texRes = (int) textureResolutionPanel.GetValue ();
        p.hgtRes = (int) heightmapResolutionPanel.GetValue ();
    }

    public void UpdatePlanet () {
        if (GameObject.Find ("PlanetParent") != null)
            Destroy (GameObject.Find ("PlanetParent"));

        CreatePlanetOptions ();

        GetComponent<PlanetMaker> ().CreatePlanet (p);
    }

    public void CreatePlanetOptions () {
        // SETUP and PLANET OPTIONS
        p = new PlanetOptions ();
        p.resolution = 255;
        p.radius = radiusPanel.GetValue ();
        p.material = planetMaterial;
        p.seed = Random.Range (-10000f, 10000f);

        // NOISE
        p.layers = new NoiseLayer[noiseLayerPanels.Count];
        for (int i = 0; i < p.layers.Length; i++) {
            p.layers[i] = noiseLayerPanels[i].GetLayer (i, p.seed);
            Debug.Log ("Layer " + i + ": " + p.layers[i].Evaluate (0f, 0f, 0f) + " " + p.layers[i].Evaluate (0.1f, 0.1f, 0.1f));
        }

        // COLOUR
        p.col1 = surfaceColour1.Colour;
        p.col2 = surfaceColour2.Colour;
        p.colourBlending = colourBlendingPanel.GetValue ();
        p.colourScale = colourScalePanel.GetValue ();
        p.colourOctaves = (int) colourOctavesPanel.GetValue ();
        p.colLow = surfaceColourLow.Colour;
        p.colHigh = surfaceColourHigh.Colour;
        p.gradientBlending = gradientBlendingPanel.GetValue ();
        p.gradientBlendingNoise = gradientBlendingNoisePanel.GetValue ();
        p.gradientBlendingNoiseSmoothness = gradientBlendingSmoothnessPanel.GetValue ();

        // RESOLUTION
        p.texRes = (int) textureResolutionPanel.GetValue ();
        p.hgtRes = (int) heightmapResolutionPanel.GetValue ();

        // CRATERS
        p.craters = new Crater[(int) numCratersPanel.GetValue ()];
        for (int i = 0; i < p.craters.Length; i++) {
            p.craters[i] = new Crater (Random.onUnitSphere * p.radius, craterMaxSizePanel.GetValue ());
        }
    }

    public void ExportPlanet () {
        Texture2D heightmap = GetComponent<PlanetMaker> ().CreateHeightmap (p, p.hgtRes);
        Texture2D tex = GetComponent<PlanetMaker> ().CreateTexture (p, p.texRes);

        byte[] heightmapBytes = heightmap.EncodeToPNG ();
        byte[] texBytes = tex.EncodeToPNG ();

        File.WriteAllBytes (Application.dataPath + "/" + planetName.text + "_height.png", heightmapBytes);
        File.WriteAllBytes (Application.dataPath + "/" + planetName.text + "_texture.png", texBytes);
    }

    public void AddNoiseLayer () {
        GameObject g = Instantiate (noiseLayerPanelPrefab);
        g.transform.parent = noiseLayerPanelParent;
        g.transform.localScale = Vector3.one; // idk why but without this line it scales to 2x
        noiseLayerPanels.Add (g.GetComponent<NoiseLayerPanel> ());
        //LayoutRebuilder.ForceRebuildLayoutImmediate (noiseLayerPanelParent);
        g.GetComponent<Poppable> ().Rebuild ();
    }

    public void RemoveNoiseLayer (NoiseLayerPanel p) {
        noiseLayerPanels.Remove (p);
        Poppable parent = p.transform.parent.parent.GetComponent<Poppable> ();
        Destroy (p.gameObject);
        //parent.Rebuild ();
        //LayoutRebuilder.MarkLayoutForRebuild (parent.GetComponent<RectTransform> ());
        StartCoroutine (RebuildPoppableNextFrame (parent));
    }

    IEnumerator RebuildPoppableNextFrame (Poppable p) {
        yield return new WaitForEndOfFrame ();
        p.Rebuild ();
    }
}

public class PlanetOptions {
    public int resolution;
    public Material material;

    public float radius;

    public NoiseLayer[] layers;

    public float seed;

    public Color col1, col2;
    public float colourBlending;
    public float colourScale;
    public int colourOctaves;

    public Color colLow, colHigh;
    public float gradientBlending;
    public float gradientBlendingNoise;
    public float gradientBlendingNoiseSmoothness;

    public Crater[] craters;

    public int texRes;
    public int hgtRes;
}

public struct Crater {
    public Vector3 position;
    public float size;
    public Crater (Vector3 position, float maxSize) {
        this.position = position;
        float v = Random.value;
        this.size = maxSize * v;// (0.112f * Mathf.Pow (1.61f, v) + 0.811f * Mathf.Pow (v, 5f));
    }
}
