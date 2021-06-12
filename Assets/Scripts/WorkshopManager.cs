using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopManager : MonoBehaviour
{

    public Material planetMaterial;

    // PLANET OPTIONS
    public PropertyPanel radiusPanel;

    // NOISE
    public PropertyPanel noiseScalePanel;
    public PropertyPanel noiseHeightPanel;
    public PropertyPanel noiseOctavesPanel;

    // CRATERS
    public PropertyPanel numCratersPanel;
    public PropertyPanel craterMaxSizePanel;

    // COLOUR
    public ColourProperty surfaceColour1;
    public ColourProperty surfaceColour2;
    public PropertyPanel colourBlendingPanel;
    public PropertyPanel colourScalePanel;

    // EXPORT
    public SimpleProperty heightmapResolutionPanel;
    public SimpleProperty textureResolutionPanel;
    public InputField planetName;

    PlanetOptions p;

    private void Start () {
        UpdatePlanet ();
    }

    public void UpdatePlanet () {
        if (GameObject.Find ("PlanetParent") != null)
            Destroy (GameObject.Find ("PlanetParent"));

        p = new PlanetOptions ();
        p.resolution = 255;
        p.radius = radiusPanel.GetValue ();
        p.material = planetMaterial;
        p.noiseScale = noiseScalePanel.GetValue ();
        p.noiseOctaves = Mathf.RoundToInt (noiseOctavesPanel.GetValue ());
        p.noiseHeight = noiseHeightPanel.GetValue ();
        p.seed = Random.Range (-10000f, 10000f);

        p.col1 = surfaceColour1.Colour;
        p.col2 = surfaceColour2.Colour;
        p.colourBlending = colourBlendingPanel.GetValue ();
        p.colourScale = colourScalePanel.GetValue ();

        p.texRes = (int)textureResolutionPanel.GetValue ();
        p.hgtRes = (int)heightmapResolutionPanel.GetValue ();

        p.craters = new Crater[(int) numCratersPanel.GetValue ()];
        for (int i = 0; i < p.craters.Length; i++) {
            p.craters[i] = new Crater (Random.onUnitSphere * p.radius, craterMaxSizePanel.GetValue ());
        }

        GetComponent<PlanetMaker> ().CreatePlanet (p);
    }

    public void ExportPlanet () {
        Texture2D heightmap = GetComponent<PlanetMaker> ().CreateHeightmap (p, p.hgtRes);
        Texture2D tex = PlanetMaker.CreateTexture (p, p.texRes);

        byte[] heightmapBytes = heightmap.EncodeToPNG ();
        byte[] texBytes = tex.EncodeToPNG ();

        File.WriteAllBytes (Application.dataPath + "/" + planetName.text + "_height.png", heightmapBytes);
        File.WriteAllBytes (Application.dataPath + "/" + planetName.text + "_texture.png", texBytes);
    }
}

public class PlanetOptions {
    public int resolution;
    public Material material;

    public float radius;
    public float noiseScale;
    public int noiseOctaves;
    public float noiseHeight;
    public float seed;

    public Color col1, col2;
    public float colourBlending;
    public float colourScale;

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
