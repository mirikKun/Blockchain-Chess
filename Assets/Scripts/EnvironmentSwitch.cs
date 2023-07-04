using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSwitch : MonoBehaviour
{
    private bool _environmentDisabled;
    [SerializeField] private GameObject[] objectsToHide;
    private Material _lastSkybox;

    private void Start()
    {
        _lastSkybox = RenderSettings.skybox;
    }

    public void EnvironmentChange()
    {
        if (_environmentDisabled)
        {
            foreach (var objectToHide in objectsToHide)
            {
                objectToHide.SetActive(true);
            }
            RenderSettings.skybox=_lastSkybox;
            _environmentDisabled = false;
        }
        else
        {
            foreach (var objectToHide in objectsToHide)
            {
                objectToHide.SetActive(false);
            }
            RenderSettings.skybox=null;
            _environmentDisabled = true;
        }
    }
}
