using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start () {
        UpdatePlanet ();
    }

    public void UpdatePlanet () {
        if (GameObject.Find ("PlanetParent") != null)
            Destroy (GameObject.Find ("PlanetParent"));

        PlanetOptions p = new PlanetOptions ();
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

        p.craters = new Crater[(int) numCratersPanel.GetValue ()];
        for(int i = 0; i < p.craters.Length; i++) {
            p.craters[i] = new Crater (Random.onUnitSphere * p.radius, craterMaxSizePanel.GetValue ());
        }

        GetComponent<PlanetMaker> ().CreatePlanet (p);
    }
}

public struct PlanetOptions {
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
