using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class AccountCreator : MonoBehaviour
{
    [SerializeField] private TMP_InputField select_name;
    [SerializeField] private TMP_InputField select_email;
    [SerializeField] private TMP_Dropdown select_day;
    [SerializeField] private TMP_Dropdown select_month;
    [SerializeField] private TMP_Dropdown select_year;
    [SerializeField] private TMP_Dropdown select_clan;
    [SerializeField] private GameObject ageRestrictedArea;
    [SerializeField] private GameObject notAllFieldsAreFilled;
    [SerializeField] private int min_Restricted_Age = 18;
    [SerializeField] private GameObject chooseClanPopup;
    [SerializeField] private GameObject clanChooserPanel;
    [SerializeField] private Accounts accounts;

    [SerializeField] private Image clanBackground;
    [SerializeField] private TextMeshProUGUI clanName;

    [SerializeField] private Sprite blackBack;
    [SerializeField] private Sprite redBlack;
    [SerializeField] private MainMenuPanelSwitcher mainMenuPanelSwitcher;
    [SerializeField] private ArmyController armyController;
    private string player_name;
    private string email;
    private string clan;
    private DateTime birth_date;

    private Clan curClan;


    private void Start()
    {
        UpdateAccountUI();
    }

    private void OnEnable()
    {
        if (PlayerData.ColorIndex == -1)
        {
            clanBackground.sprite = blackBack;
            clanName.text = "NO CLAN";
        }
        else if (PlayerData.ColorIndex == 0)
        {
            curClan = Clan.ODA;
            clanBackground.sprite = redBlack;
            clanName.text = curClan + " CLAN";
        }
        else if (PlayerData.ColorIndex == 1)
        {
            curClan = Clan.HANZO;
            clanBackground.sprite = redBlack;
            clanName.text = curClan + " CLAN";
        }
        else if (PlayerData.ColorIndex == 2)
        {
            curClan = Clan.MUSASHI;
            clanBackground.sprite = redBlack;
            clanName.text = curClan + " CLAN";
        }
    }

    private void UpdateAccountUI()
    {
    }

    private bool AgeCheck()
    {
        bool result = true;
        DateTime try_birth_date = new DateTime(int.Parse(select_year.options[select_year.value].text),
            select_month.value, int.Parse(select_day.options[select_day.value].text));
        if (DateTime.Now.AddYears(-min_Restricted_Age) < try_birth_date)
        {
            result = false;
        }

        return result;
    }

    private void OpenAgeRestrictedArea()
    {
        ageRestrictedArea.SetActive(true);
    }

    public void ChooseArmy()
    {
        mainMenuPanelSwitcher.OpenPanel(clanChooserPanel);
    }

    public void SaveAccountInfo()
    {
        if (select_name.text == "" || select_email.text == "" || select_clan.value == 0 ||
            select_day.value == 0 || select_month.value == 0 || select_year.value == 0)
        {
            notAllFieldsAreFilled.SetActive(true);
            return;
        }

        if (PlayerData.ColorIndex == -1)
        {
            chooseClanPopup.SetActive(true);
            return;
        }

        if (!AgeCheck())
        {
            OpenAgeRestrictedArea();
            return;
        }

        UpdateAccountUI();
        StartCoroutine(SaveAccountDelay());
    }


    private IEnumerator SaveAccountDelay()
    {
        PlayerData.PlayerName = player_name = select_name.text;
        PlayerData.email = email = select_email.text;
        PlayerData.clan = clan = curClan.ToString();
        PlayerData.birth_date = birth_date = new DateTime(select_year.value, select_month.value, select_day.value);
        PlayerData.Army = null;
        Accounts.Account account = accounts.CreateAccount(player_name, email, clan, birth_date, PlayerData.ColorIndex);
        PlayerData.Account = account;
        armyController.gameObject.SetActive(true);

        yield return null;        
        armyController.ShowArmySaved();
        foreach (var curCharacter in armyController.GetAllCharacters(account.ColorIndex))
        {    

            curCharacter.SetupPrimaryStances();
        }  
        armyController.CheckExitSaveArmy();

 
             
        // foreach (var curCharacter in account.)
        // {
        //     curCharacter.SetupPrimaryStances();
        // }
        armyController.gameObject.SetActive(false);
        accounts.LocalSaveAccount(account);

        accounts.SwitchAccount();
    }
}