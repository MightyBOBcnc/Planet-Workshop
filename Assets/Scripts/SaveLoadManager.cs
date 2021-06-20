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
                    return Application.dataPath + "/Saves";
            }
        }
    }

    private void Start () {
        wm = GetComponent<WorkshopManager> ();
    }

    public void Save () {
        string dir = directory + "/" + planetName.text + ".pws";
        StringBuilder file = new StringBuilder ();

        file.AppendLine ("RADIUS:" + wm.radiusPanel.GetValue ());
        file.AppendLine ("RESOLUTION:" + wm.resolutionPanel.GetValue ());
        file.AppendLine ("NUMLAYERS:" + wm.noiseLayerPanels.Count);

        for (int i = 0; i < wm.noiseLayerPanels.Count; i++) {
            file.AppendLine ("NOISELAYER-" + i + ":NAME:" + wm.noiseLayerPanels[i].name.text);
            file.AppendLine ("NOISELAYER-" + i + ":TYPE:" + wm.noiseLayerPanels[i].type.value);
            file.AppendLine ("NOISELAYER-" + i + ":SCALE:" + wm.noiseLayerPanels[i].scale.GetValue ());
            file.AppendLine ("NOISELAYER-" + i + ":HEIGHT:" + wm.noiseLayerPanels[i].height.GetValue ());
            file.AppendLine ("NOISELAYER-" + i + ":OCTAVES:" + wm.noiseLayerPanels[i].octaves.GetValue ());
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
    }

    public void Load () {

    }

    public void ListFiles () {

        for(int i = 0; i < buttonsParent.childCount; i++) {
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
    }

    public static string EncodeColour (Color c) {
        return Mathf.RoundToInt(c.r * 256).ToString ("X2") + Mathf.RoundToInt (c.g * 256).ToString ("X2") + Mathf.RoundToInt (c.b * 256).ToString ("X2");
    }
}
