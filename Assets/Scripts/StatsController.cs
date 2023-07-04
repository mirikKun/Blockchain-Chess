using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsController : MonoBehaviour
{
    [SerializeField] private Text fpsText;
    [SerializeField] private Text weightText;

    public float deltaTime;
 
    void Update () {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = "FPS "+Mathf.Ceil (fps);

        weightText.text= "Memory "+((GC.GetTotalMemory(false) / 1024) / 1024).ToString();

    }
}
