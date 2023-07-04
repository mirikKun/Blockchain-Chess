using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanelSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject armyPanel;
    [SerializeField] private GameObject chooseColorPanel;
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject dontHaveAccountPopup;
    private GameObject _lastScreen;

    private void Start()
    {
        _lastScreen = mainMenuScreen;
    }

    public void GoToTheArmyScreen()
    {
        if (PlayerData.Account == null)
        {
            dontHaveAccountPopup.SetActive(true);
        }
        else
        {
            OpenPanel(armyPanel);
        }

    }


    public void BackToMainScreen()
    {
        OpenPanel(mainMenuScreen);
    }

    public void OpenPanel(GameObject newPanel)
    {
        if (_lastScreen)
        {
            _lastScreen.SetActive(false);
            
        }
        newPanel.SetActive(true);
        _lastScreen = newPanel;
    }
    
}
