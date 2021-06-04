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
        LayoutRebuilder.ForceRebuildLayoutImmediate (popContent.transform.parent.parent.GetComponent<RectTransform> ());
    }

    public void PopIn() {
        popContent.gameObject.SetActive (false);
        arrowIcon.rectTransform.eulerAngles = Vector3.forward * 90f;
        LayoutRebuilder.ForceRebuildLayoutImmediate (popContent.transform.parent.parent.GetComponent<RectTransform> ());
    }

    public void SmartPop () {
        // note: not the popcorn!
        if (popContent.gameObject.activeSelf) PopIn ();
        else PopOut ();
    }
}
