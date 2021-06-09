using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourProperty : MonoBehaviour
{

    public Slider redSlider, greenSlider, blueSlider;
    public RawImage redBG, greenBG, blueBG;
    public Image colourPreview;

    bool hasInitialised = false;

    float red, green, blue;

    private void Awake () {
        red = redSlider.value; green = greenSlider.value; blue = blueSlider.value;
    }

    private void Update () {
        if(!hasInitialised || redSlider.value != red || greenSlider.value != green || blueSlider.value != blue) {

            hasInitialised = true;

            red = redSlider.value; green = greenSlider.value; blue = blueSlider.value;

            Texture2D rbg = new Texture2D (256, 1);
            Texture2D gbg = new Texture2D (256, 1);
            Texture2D bbg = new Texture2D (256, 1);

            Color[] r = new Color[256];
            Color[] g = new Color[256];
            Color[] b = new Color[256];

            for(int i = 0; i < 256; i++) {
                r[i] = new Color (i / 256f, green, blue);
                g[i] = new Color (red, i / 256f, blue);
                b[i] = new Color (red, green, i / 256f);
            }

            rbg.SetPixels (r); rbg.Apply ();
            gbg.SetPixels (g); gbg.Apply ();
            bbg.SetPixels (b); bbg.Apply ();

            redBG.texture = rbg;
            greenBG.texture = gbg;
            blueBG.texture = bbg;

            colourPreview.color = Colour;
        }
    }

    public Color Colour { get => new Color (red, green, blue); }
}
