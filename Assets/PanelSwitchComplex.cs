using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//TurnOfOne special UI component with offset
public class PanelSwitchComplex : MonoBehaviour
{
    
    
    [SerializeField] private RectTransform playersPanel;
    private bool _panelClosed;
    private Vector3 _startPosition;
    private RectTransform _rectTransform;
    private bool _lastPanelCloset;
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _startPosition = _rectTransform.anchoredPosition;
    }

    public void Switch()
    {
        if (_panelClosed)
        {
            OpenPanel();
        }
        else
        {
            ClosePanel();

        }
    }

    public void ClosePanel()
    {
        playersPanel.gameObject.SetActive(false);
        _rectTransform.anchoredPosition = playersPanel.anchoredPosition;
        _panelClosed = true;
        _lastPanelCloset = _panelClosed;
    }

    public void OpenPanel()
    {
        playersPanel.gameObject.SetActive(true);
        _rectTransform.anchoredPosition = _startPosition;
        _panelClosed = false;
        _lastPanelCloset = _panelClosed;

    }
    public void ReturnToLastState()
    {
        _panelClosed=_lastPanelCloset;
        Switch();
    }
}
