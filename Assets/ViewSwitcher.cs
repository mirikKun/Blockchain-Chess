using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSwitcher : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private FlagSwitch flagSwitch;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private PanelSwitchComplex panelSwitchComplex;
    [SerializeField] private GameObject[] objectsToHide;
    [SerializeField] private Transform FigureMoversHolder;
    private bool _in2DView;

    private void Start()
    {
        GameManager.In2dView = false;
    }

    public bool Get2DView()
    {
        return _in2DView;
    }

    public void Switch()
    {
        if (_in2DView)
        {
            _in2DView = false;
            Set3DView();
        }
        else
        {
            _in2DView = true;
            Set2DView();
        }

        //flagSwitch.SetFlagsFrame(!_in2DView);
    }

    private void Set2DView()
    {
        GameManager.In2dView = true;

        foreach (var objectToHide in objectsToHide)
        {
            objectToHide.SetActive(false);
        }

        panelSwitchComplex.ClosePanel();
        flagSwitch.SwitchTo2DView();
        cameraController.SaveCameraPosition();
        
        cameraController.SetUpperCameraPosition();
        // mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
        SetFiguresActive(false);
    }

    private void Set3DView()
    {
        GameManager.In2dView = false;

        foreach (var objectToHide in objectsToHide)
        {
            objectToHide.SetActive(true);
        }
        flagSwitch.SwitchTo3DView();
        panelSwitchComplex.ReturnToLastState();
        cameraController.LoadLastCameraPosition();

        //cameraController.CheckDistance();
        SetFiguresActive(true);
    }

    public void SetFiguresActive(bool active)
    {
        foreach (Transform child in FigureMoversHolder)
        {
            foreach (Transform nextChild in child)
            {
                nextChild.gameObject.SetActive(active);
            }
        }
    }
}