using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleProperty : MonoBehaviour
{

    public enum ValueConversionMode { Normal, PowerOfTwo, PowerOfTwoMinusOne };
    public ValueConversionMode valueConversionMode;

    public Slider valueSlider;
    public Text valueText;

    private void Update () {
        if (valueText != null)
            valueText.text = string.Format ("({0})", GetValue ());
    }

    public float GetValue () {
        switch(valueConversionMode) {
            case ValueConversionMode.Normal:
                return valueSlider.value;
            case ValueConversionMode.PowerOfTwo:
                return Mathf.Pow(2f, valueSlider.value);
            case ValueConversionMode.PowerOfTwoMinusOne:
                return Mathf.Pow (2f, valueSlider.value) - 1;
            default:
                return 0;
        }
    }

    public void SetValue (float value) {
        switch (valueConversionMode) {
            case ValueConversionMode.Normal:
                valueSlider.value = value;
                break;
            case ValueConversionMode.PowerOfTwo:
                valueSlider.value = Mathf.Log (value, 2f);
                break;
            case ValueConversionMode.PowerOfTwoMinusOne:
                valueSlider.value = Mathf.Log (value + 1, 2f);
                break;
            default:
                valueSlider.value = value;
                break;
        }
    }
}
