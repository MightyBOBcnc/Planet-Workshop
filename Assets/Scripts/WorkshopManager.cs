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

    public Texture3D t3d;

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
            p.craters[i] = new Crater (Random.onUnitSphere * p.radius, Mathf.Pow (Random.value, 3f) * craterMaxSizePanel.GetValue ());
        }

        PlanetMaker.CreatePlanet (p);
        t3d = PlanetMaker.instance.tex3d;
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
    public Crater (Vector3 position, float size) {
        this.position = position;
        this.size = size;
    }
}
