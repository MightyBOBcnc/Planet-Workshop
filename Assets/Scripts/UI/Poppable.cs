using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poppable : MonoBehaviour {

    public Image arrowIcon;
    public RectTransform popContent;

    public void PopOut () {
        popContent.gameObject.SetActive (true);
        arrowIcon.rectTransform.eulerAngles = Vector3.zero;
        Rebuild ();
    }

    public void PopIn() {
        popContent.gameObject.SetActive (false);
        arrowIcon.rectTransform.eulerAngles = Vector3.forward * 90f;
        Rebuild ();
    }

    public void SmartPop () {
        // note: not the popcorn!
        if (popContent.gameObject.activeSelf) PopIn ();
        else PopOut ();
    }

    public void Rebuild () {
        if (popContent.transform.parent.parent.name == "ManagementScrollContent") {
            LayoutRebuilder.ForceRebuildLayoutImmediate (popContent.transform.parent.parent.GetComponent<RectTransform> ());
        } else {
            for (int i = 0; i < 2; i++) {
                LayoutRebuilder.ForceRebuildLayoutImmediate (popContent.transform.parent.parent.GetComponent<RectTransform> ());
                popContent.transform.parent.parent.parent.GetComponent<Poppable> ().Rebuild ();
            }
        }
        //if (popContent.transform.parent.parent.parent.GetComponent<Poppable> () != null) popContent.transform.parent.parent.parent.GetComponent<Poppable> ().Rebuild ();
        //LayoutRebuilder.ForceRebuildLayoutImmediate (GameObject.Find ("ManagementScrollContent").GetComponent<RectTransform> ());
    }
}
