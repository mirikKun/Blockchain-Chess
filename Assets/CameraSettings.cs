using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSettings : MonoBehaviour
{
    private Camera _camera;
    private bool _shadowsWasEnabled;
    private void Start()
    {
        _camera = GetComponent<Camera>();
        // _shadowsWasEnabled=_camera.renderingPa
        // if()
    }
}
