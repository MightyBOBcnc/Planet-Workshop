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
    public SimpleProperty resolutionPanel;
    public InputField seed;
    public Toggle randomiseSeed;

    // NOISE
    [Header("Noise Options")]
    public GameObject noiseLayerPanelPrefab;
    public RectTransform noiseLayerPanelParent;
    public List<NoiseLayerPanel> noiseLayerPanels;

    // CRATERS
    [Header("Crater Options")]
    public PropertyPanel numCratersPanel;
    public PropertyPanel craterMaxSizePanel;
    public PropertyPanel craterDepthPanel;

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
    public ColourProperty craterBottomColour;
    public SimpleProperty craterColourStrengthPanel;

    // EXPORT
    [Header("Export Options")]
    public SimpleProperty heightmapResolutionPanel;
    public SimpleProperty textureResolutionPanel;
    public InputField planetName;

    [Header ("Miscellaneous")]
    public TextAsset randomWords;
    string[] words;

    PlanetOptions p;
    System.Random seedInitialiser;

    private void Start () {

        words = randomWords.text.Split ('\n');

        seedInitialiser = new System.Random ();
        Random.InitState (seedInitialiser.Next (-10000, 10000));
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

        if(randomiseSeed.isOn) {
            seed.text = RandomSeed ();
        }

        CreatePlanetOptions ();

        GetComponent<PlanetMaker> ().CreatePlanet (p);
    }

    public void CreatePlanetOptions () {
        // SETUP and PLANET OPTIONS
        p = new PlanetOptions ();
        p.resolution = (int)resolutionPanel.GetValue();
        p.radius = radiusPanel.GetValue ();
        p.material = planetMaterial;
        p.seed = SeedHash(seed.text);
        print (p.seed);

        // NOISE
        p.layers = new NoiseLayer[noiseLayerPanels.Count];
        for (int i = 0; i < p.layers.Length; i++) {
            p.layers[i] = noiseLayerPanels[i].GetLayer (i, p.seed);
            //Debug.Log ("Layer " + i + ": " + p.layers[i].Evaluate (0f, 0f, 0f) + " " + p.layers[i].Evaluate (0.1f, 0.1f, 0.1f));
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
        p.colCrater = craterBottomColour.Colour;
        p.craterColouring = craterColourStrengthPanel.GetValue ();

        // RESOLUTION
        p.texRes = (int) textureResolutionPanel.GetValue ();
        p.hgtRes = (int) heightmapResolutionPanel.GetValue ();

        // CRATERS
        p.craterDepth = craterDepthPanel.GetValue ();
        p.craters = new Crater[(int) numCratersPanel.GetValue ()];
        Random.InitState ((int) (p.seed * 1000f));
        for (int i = 0; i < p.craters.Length; i++) {
            p.craters[i] = new Crater (Random.onUnitSphere * p.radius, craterMaxSizePanel.GetValue ());
        }
    }

    public void ExportPlanetTextures () {

        Texture2D heightmap = GetComponent<PlanetMaker> ().CreateHeightmap (p, p.hgtRes);
        Texture2D tex = GetComponent<PlanetMaker> ().CreateTexture (p, p.texRes);

        byte[] heightmapBytes = heightmap.EncodeToPNG ();
        byte[] texBytes = tex.EncodeToPNG ();

        File.WriteAllBytes (Application.dataPath.Replace ("Planet Workshop_Data", "Saves") + "/" + planetName.text + "_height.png", heightmapBytes);
        File.WriteAllBytes (Application.dataPath.Replace ("Planet Workshop_Data", "Saves") + "/" + planetName.text + "_texture.png", texBytes);
    }

    public void ExportPlanetMesh () {
        Quaternion q = GameObject.Find ("PlanetParent").transform.rotation;
        GameObject.Find ("PlanetParent").transform.rotation = Quaternion.identity; // prevent longitude calculation from being messed up by rotation
        GetComponent<PlanetMaker> ().SaveOBJ (planetName.text);
        //GetComponent<PlanetMaker> ().SaveCubemapTexture (p, planetName.text, 256);
        GameObject.Find ("PlanetParent").transform.rotation = q;
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

    public void Quit() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public static float SeedHash (string s) {
        return (System.BitConverter.ToInt32 (System.Security.Cryptography.MD5.Create ().ComputeHash (System.Text.Encoding.UTF8.GetBytes (s)), 0)) / 1000f % 10000f;
    }

    public string RandomSeed () {
        string seed = "";
        while (seed.Length < 13) {
            seed += words[seedInitialiser.Next(0, words.Length - 1)];
        }
        while(seed.Length < 16) {
            seed += Random.Range (0, 9);
        }
        return seed;
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

    public Color colCrater;
    public float craterColouring;

    public Crater[] craters;
    public float craterDepth;

    public int texRes;
    public int hgtRes;
}

public struct Crater {
    public Vector3 position;
    public float size;
    public Crater (Vector3 position, float maxSize) {
        this.position = position;
        float v = Random.value;
        this.size = maxSize * (0.112f * Mathf.Pow (1.61f, v) + 0.811f * Mathf.Pow (v, 5f)); //maxSize * v;
    }
}
