using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{

    WorkshopManager wm;

    public RectTransform buttonsParent;
    public GameObject buttonPrefab;
    public InputField planetName;
    public Poppable LoadPoppable;

    public static string directory {
        get {
            switch (Application.platform) {
                case RuntimePlatform.WindowsEditor:
                    return Application.dataPath.Replace ("/Assets", "/Saves");
                default:
                    return Application.dataPath.Replace ("/Planet Workshop_Data", "/Saves");
            }
        }
    }

    private void Start () {
        wm = GetComponent<WorkshopManager> ();
        ListFiles ();
    }

    public void Save () {

        if (!Directory.Exists (directory)) Directory.CreateDirectory (directory);

        string dir = directory + "/" + planetName.text + ".pws";
        StringBuilder file = new StringBuilder ();

        file.AppendLine ("RADIUS:" + wm.radiusPanel.GetValue ());
        file.AppendLine ("RESOLUTION:" + wm.resolutionPanel.GetValue ());
        file.AppendLine ("SEED:" + wm.seed.text);

        file.AppendLine ("NUMLAYERS:" + wm.noiseLayerPanels.Count);
        for (int i = 0; i < wm.noiseLayerPanels.Count; i++) {
            file.AppendLine ("NOISELAYER:" + i + ":NAME:" + wm.noiseLayerPanels[i].name.text);
            file.AppendLine ("NOISELAYER:" + i + ":TYPE:" + wm.noiseLayerPanels[i].type.value);
            file.AppendLine ("NOISELAYER:" + i + ":SCALE:" + wm.noiseLayerPanels[i].scale.GetValue ());
            file.AppendLine ("NOISELAYER:" + i + ":HEIGHT:" + wm.noiseLayerPanels[i].height.GetValue ());
            file.AppendLine ("NOISELAYER:" + i + ":OCTAVES:" + wm.noiseLayerPanels[i].octaves.GetValue ());
        }

        file.AppendLine ("NUMCRATERS:" + wm.numCratersPanel.GetValue ());
        file.AppendLine ("CRATERMAXSIZE:" + wm.craterMaxSizePanel.GetValue ());
        file.AppendLine ("CRATERDEPTH:" + wm.craterDepthPanel.GetValue ());

        file.AppendLine ("RANDOMCOLOUR1:" + EncodeColour (wm.surfaceColour1.Colour));
        file.AppendLine ("RANDOMCOLOUR2:" + EncodeColour (wm.surfaceColour2.Colour));
        file.AppendLine ("COLOURBLENDING:" + wm.colourBlendingPanel.GetValue ());
        file.AppendLine ("COLOURSCALE:" + wm.colourScalePanel.GetValue ());
        file.AppendLine ("COLOUROCTAVES:" + wm.colourOctavesPanel.GetValue ());
        file.AppendLine ("LOWCOLOUR:" + EncodeColour (wm.surfaceColourLow.Colour));
        file.AppendLine ("HIGHCOLOUR:" + EncodeColour (wm.surfaceColourHigh.Colour));
        file.AppendLine ("GRADIENTBLENDING:" + wm.gradientBlendingPanel.GetValue ());
        file.AppendLine ("GRADIENTBLENDINGNOISE:" + wm.gradientBlendingNoisePanel.GetValue ());
        file.AppendLine ("GRADIENTBLENDINGSMOOTHNESS:" + wm.gradientBlendingSmoothnessPanel.GetValue ());
        file.AppendLine ("CRATERCOLOUR:" + EncodeColour (wm.craterBottomColour.Colour));
        file.AppendLine ("CRATERCOLOURSTRENGTH:" + wm.craterColourStrengthPanel.GetValue ());

        File.WriteAllText (dir, file.ToString ());

        ListFiles (); // make the new file show up straightaway
    }

    public void Load (string filename) {

        if (!Directory.Exists (directory)) Directory.CreateDirectory (directory);

        string dir = directory + "/" + filename; // extension included

        planetName.text = filename.Split ('.')[0]; // put the file name minus extension in the input field to allow easy updates

        string[] lines = File.ReadAllLines (dir);

        foreach(string line in lines) {
            string[] components = line.Split (':');
            switch(components[0]) {
                case "RADIUS":
                    wm.radiusPanel.SetValue (float.Parse (components[1]));
                    break;
                case "RESOLUTION":
                    wm.resolutionPanel.SetValue (float.Parse (components[1]));
                    break;
                case "SEED":
                    wm.randomiseSeed.isOn = false;
                    wm.seed.text = components[1];
                    break;
                case "NUMLAYERS":
                    // Remove old noise layers:
                    for(int i = 0; i < wm.noiseLayerPanels.Count; i++) {
                        wm.RemoveNoiseLayer (wm.noiseLayerPanels[0]);
                    }
                    // Add enough new layers
                    int numNewLayers = int.Parse (components[1]);
                    for (int i = 0; i < numNewLayers; i++)
                        wm.AddNoiseLayer ();
                    break;
                case "NOISELAYER": // will ideally always be called after NUMLAYERS unless file has been manually manipulated otherwise
                    int idx = int.Parse (components[1]);
                    string property = components[2];
                    string value = components[3];
                    switch (property) {
                        case "NAME":
                            wm.noiseLayerPanels[idx].name.text = value;
                            break;
                        case "TYPE":
                            wm.noiseLayerPanels[idx].type.value = int.Parse (value);
                            break;
                        case "SCALE":
                            wm.noiseLayerPanels[idx].scale.SetValue (float.Parse (value));
                            break;
                        case "HEIGHT":
                            wm.noiseLayerPanels[idx].height.SetValue (float.Parse (value));
                            break;
                        case "OCTAVES":
                            wm.noiseLayerPanels[idx].octaves.SetValue (float.Parse (value));
                            break;
                    }
                    break;
                case "NUMCRATERS":
                    wm.numCratersPanel.SetValue (float.Parse (components[1]));
                    break;
                case "CRATERMAXSIZE":
                    wm.craterMaxSizePanel.SetValue (float.Parse (components[1]));
                    break;
                case "CRATERDEPTH":
                    wm.craterDepthPanel.SetValue (float.Parse (components[1]));
                    break;
                case "RANDOMCOLOUR1":
                    wm.surfaceColour1.SetValue (DecodeColour (components[1]));
                    break;
                case "RANDOMCOLOUR2":
                    wm.surfaceColour2.SetValue (DecodeColour (components[1]));
                    break;
                case "COLOURBLENDING":
                    wm.colourBlendingPanel.SetValue (float.Parse (components[1]));
                    break;
                case "COLOURSCALE":
                    wm.colourScalePanel.SetValue (float.Parse (components[1]));
                    break;
                case "COLOUROCTAVES":
                    wm.colourOctavesPanel.SetValue (float.Parse (components[1]));
                    break;
                case "LOWCOLOUR":
                    wm.surfaceColourLow.SetValue (DecodeColour (components[1]));
                    break;
                case "HIGHCOLOUR":
                    wm.surfaceColourHigh.SetValue (DecodeColour (components[1]));
                    break;
                case "GRADIENTBLENDING":
                    wm.gradientBlendingPanel.SetValue (float.Parse (components[1]));
                    break;
                case "GRADIENTBLENDINGNOISE":
                    wm.gradientBlendingNoisePanel.SetValue (float.Parse (components[1]));
                    break;
                case "GRADIENTBLENDINGSMOOTHNESS":
                    wm.gradientBlendingSmoothnessPanel.SetValue (float.Parse (components[1]));
                    break;
                case "CRATERCOLOUR":
                    wm.craterBottomColour.SetValue (DecodeColour (components[1]));
                    break;
                case "CRATERCOLOURSTRENGTH":
                    wm.craterColourStrengthPanel.SetValue (float.Parse (components[1]));
                    break;
                default:
                    Debug.LogError ("Unrecognised value: " + components[0]);
                    break;
            }
        }

        wm.UpdatePlanet ();
    }

    public void Load (LoadButton b) => Load (b.GetTitle ());

    public void ListFiles () {

        if (!Directory.Exists (directory)) Directory.CreateDirectory (directory);

        for (int i = 0; i < buttonsParent.childCount; i++) {
            if(buttonsParent.GetChild(i) != buttonPrefab.transform)
                Destroy (buttonsParent.GetChild (i).gameObject);
        }

        foreach(string s in Directory.GetFiles (directory)) {
            string filename = s.Split (new char[] { '\\', '/' })[s.Split (new char[] { '\\', '/' }).Length - 1];

            if (!filename.Contains (".pws")) continue;

            GameObject g = Instantiate (buttonPrefab);
            g.transform.parent = buttonsParent;
            g.transform.localScale = Vector3.one;
            g.SetActive (true);
            g.GetComponent<LoadButton> ().SetTitle (filename);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate (buttonsParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate (LoadPoppable.transform.parent.GetComponent<RectTransform> ());
    }

    public static string EncodeColour (Color c) {
        //return Mathf.RoundToInt(c.r * 256).ToString ("X2") + Mathf.RoundToInt (c.g * 256).ToString ("X2") + Mathf.RoundToInt (c.b * 256).ToString ("X2");
        return ColorUtility.ToHtmlStringRGB (c);
    }

    public static Color DecodeColour (string s) {
        Color output = Color.magenta;
        ColorUtility.TryParseHtmlString ("#" + s, out output);
        return output;
    }
}