using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderPercents : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI percent;

    [SerializeField] private string units = "%";
    private void Start()
    {
        slider.onValueChanged.AddListener(delegate { ChangePercent(); });
        ChangePercent();
    }

    public void ChangePercent()
    {
        percent.text = ((int) (slider.value )) + units;
    }
}
