using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleProperty : MonoBehaviour
{

    public enum ValueConversionMode { Normal, PowerOfTwo };
    public ValueConversionMode valueConversionMode;

    public Slider valueSlider;
    public Text valueText;

    private void Update () {
        valueText.text = string.Format ("({0})", GetValue ());
    }

    public float GetValue () {
        switch(valueConversionMode) {
            case ValueConversionMode.Normal:
                return valueSlider.value;
            case ValueConversionMode.PowerOfTwo:
                return Mathf.Pow(2f, valueSlider.value);
            default:
                return 0;
        }
    }
}
