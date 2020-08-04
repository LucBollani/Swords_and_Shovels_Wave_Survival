using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cooldownBar : MonoBehaviour
{
    public Slider slider;

    public void setMaxCDValue(float maxValue)
    {
        slider.maxValue = maxValue;
    }

    public void setCDValue(float value)
    {
        slider.value = value;
    }
}
