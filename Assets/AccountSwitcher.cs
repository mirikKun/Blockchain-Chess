using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountSwitcher : MonoBehaviour
{
    [SerializeField] private AccountButton accountButtonPrefab;

    [SerializeField] private Transform buttonHolder;

    //[SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject accountPanel;
    [SerializeField] private GameObject accountSwitchingPopup;
    [SerializeField] private GameObject youHaveTooMuchAccountPopup;
    [SerializeField] private GameObject accountCreationPanel;
    [SerializeField] private MainMenuPanelSwitcher mainMenuPanelSwitcher;
    [SerializeField] private TextMeshProUGUI accountName;
    [SerializeField] private TextMeshProUGUI clanName;
    [SerializeField] private Image switchButtonImage;

    private List<AccountButton> _accountButtons = new List<AccountButton>();
    private AccountButton _lastAccountButton;
    private AccountButton _buttonOfCurrentAccount;
    private static Accounts.Account _lastAccount;
    private Accounts.Account _curAccount;

    public void OpenAccountCreationPanel()
    {
        if (Accounts.GetAccountCount() < Accounts.MaxAccountCount)
        {
            mainMenuPanelSwitcher.OpenPanel(accountCreationPanel);
            accountSwitchingPopup.SetActive(false);
        }
        else
        {
            // accountSwitchingPopup.SetActive(false);
            youHaveTooMuchAccountPopup.SetActive(true);
        }
    }

    public void CreateAllButtons(Accounts.Account[] allAccounts)
    {
        bool firstAccount = true;
        foreach (var allAccount in allAccounts)
        {
            if (firstAccount)
            {
                var newAccountButton = CreateAccountButton(allAccount);

                _lastAccount = allAccounts[0];
                if (_lastAccountButton)
                {
                    _lastAccountButton.UnselectPrevious();
                }

                _lastAccountButton = newAccountButton;
                _buttonOfCurrentAccount = _lastAccountButton;
                newAccountButton.SelectCurrent();
                _curAccount = allAccounts[0];
                firstAccount = false;
            }
            else
            {
                CreateAccountButton(allAccount);
            }
        }
    }

    public void OpenAccountScreen()
    {
        if (_lastAccount != null)
        {
            mainMenuPanelSwitcher.OpenPanel(accountPanel);
            SetupCurAccountInfo(_lastAccount);
        }
        else
        {
            mainMenuPanelSwitcher.OpenPanel(accountCreationPanel);
        }
    }

    public Accounts.Account GetCurAccount()
    {
        return _lastAccount;
    }

    public void DeleteAccount()
    {
        _accountButtons.Remove(_buttonOfCurrentAccount);
        Accounts.RemoveAccount(_lastAccount);
        _lastAccount = null;
        Destroy(_buttonOfCurrentAccount.gameObject);
        if (_accountButtons.Count > 0)
        {
            _accountButtons[0].GetButton().onClick?.Invoke();
            SwitchAccount();
        }
        else
        {
            mainMenuPanelSwitcher.OpenPanel(accountCreationPanel);
            PlayerData.Account = null;
            PlayerData.ColorIndex = -1;
            PlayerData.Army = null;
        }
    }

    public AccountButton CreateAccountButton(Accounts.Account newAccount)
    {
        AccountButton newAccountButton = Instantiate(accountButtonPrefab, buttonHolder);
        _accountButtons.Add(newAccountButton);
        newAccountButton.transform.SetSiblingIndex(1);
        newAccountButton.SetAccountButton(newAccount.PlayerName);
        newAccountButton.GetButton().onClick.AddListener(delegate
        {
            if (_lastAccountButton)
            {
                _lastAccountButton.UnselectPrevious();
            }

            newAccountButton.SelectCurrent();

            _lastAccountButton = newAccountButton;

            _curAccount = newAccount;

            switchButtonImage.enabled = _curAccount != _lastAccount;
        });
        return newAccountButton;
    }

    public void AddAccount(Accounts.Account newAccount)
    {
        AccountButton newAccountButton = CreateAccountButton(newAccount);
        _lastAccount = newAccount;
        if (_lastAccountButton)
        {
            _lastAccountButton.UnselectPrevious();
        }

        _lastAccountButton = newAccountButton;

        newAccountButton.SelectCurrent();
        _curAccount = newAccount;
    }

    public Accounts.Account SwitchAccount()
    {
        _buttonOfCurrentAccount = _lastAccountButton;
        _lastAccount = _curAccount;
        switchButtonImage.enabled = false;
        mainMenuPanelSwitcher.OpenPanel(accountPanel);
        SetupCurAccountInfo(_curAccount);
        accountSwitchingPopup.SetActive(false);

        PlayerData.SwitchAccount(_curAccount);
        return _curAccount;
    }

    public void SetupCurAccountInfo(Accounts.Account account)
    {
        accountName.text = account.PlayerName;
        clanName.text = account.Clan + " CLAN";
    }
}