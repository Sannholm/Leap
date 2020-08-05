using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSetting : MonoBehaviour
{
    public Slider slider;

    public float GetValue()
    {
        return slider.value;
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }

    public Slider.SliderEvent GetOnValueChanged()
    {
        return slider.onValueChanged;
    }
}
