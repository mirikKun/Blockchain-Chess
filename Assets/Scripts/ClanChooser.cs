using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClanChooser : MonoBehaviour
{
    [SerializeField] private ClanButton[] clans;
    [SerializeField] private Sprite redTitle;
    [SerializeField] private Sprite blackTitle;
    [SerializeField] private MainMenuPanelSwitcher mainMenuPanelSwitcher;
    [SerializeField] private GameObject accountCreationPanel;
    private int _clanIndex;
    private ClanButton _lastClanButton;
    
    [Serializable]
    public class ClanButton
    {
        public Button ChooseClan;
        public Image RedTitle;
        public GameObject RedOutline;
    }



    private void Start()
    {
        ChooseClan(clans[0], 0);
        int i = 0;
        foreach (var clan in clans)
        {
            var index = i;
            clan.ChooseClan.onClick.AddListener(delegate
            {
                ChooseClan(clan, index);
            });
            i++;
        }
    }

    public void ChooseClan(ClanButton clanButton,int index)
    {
        if (_lastClanButton!=null)
        {
            UnselectLastClan(_lastClanButton);
        }
        _clanIndex = index;
        clanButton.RedOutline.SetActive(true);
        _lastClanButton = clanButton;
        clanButton.RedTitle.sprite = redTitle;
    }

    private void UnselectLastClan(ClanButton clanButton)
    {
        clanButton.RedOutline.SetActive(false);
        clanButton.RedTitle.sprite = blackTitle;
        
    }

    public void ConfirmClan()
    {
        PlayerData.SaveArmyIndex(_clanIndex);

        mainMenuPanelSwitcher.OpenPanel(accountCreationPanel);
        
    }
}
