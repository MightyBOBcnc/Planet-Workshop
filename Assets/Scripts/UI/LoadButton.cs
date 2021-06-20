using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{

    public Text buttonTitle;

    public void SetTitle (string title) {
        buttonTitle.text = title;
    }

    public string GetTitle () {
        return buttonTitle.text;
    }
}
