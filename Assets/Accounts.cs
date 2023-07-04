using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BayatGames.SaveGameFree;
using Characters;
using UnityEngine;

public class Accounts : MonoBehaviour
{
    [Serializable]
    public class Account
    {
        public int AccountIndex = 0;
        public Army Army;
        public int ColorIndex;
        public string PlayerName;
        public string Email;
        public string Clan;
        public DateTime BirthDate;

        public List<Character> AvailableCharacters;

        public Account(string playerName, string email, string clan, DateTime birthDate, int armyColorIndex)
        {
            PlayerName = playerName;
            Email = email;
            Clan = clan;
            BirthDate = birthDate;
            ColorIndex = armyColorIndex;
        }
    }

    [SerializeField] private AccountSwitcher accountSwitcher;
    [SerializeField] private Character[] characters;

    private static List<Account> _availableAccounts = new List<Account>();

    public static int MaxAccountCount { get; } = 4;

    private void Start()
    {
        PlayerData.Characters = characters;


        if (_availableAccounts.Count == 0)
        {
            LoadSavedAccountsValues();
        }

        if (_availableAccounts.Count > 0)
        {
            PlayerData.SwitchAccount(_availableAccounts[0]);
            //  accountSwitcher.SwitchAccount();
        }

        accountSwitcher.CreateAllButtons(_availableAccounts.ToArray());
    }

    public static int GetAccountCount()
    {
        return _availableAccounts.Count;
    }

    public static void RemoveAccount(Account account)
    {
        _availableAccounts.Remove(account);
    }

    public Account CreateAccount(string playerName, string email, string clan, DateTime birthDate, int armyColorIndex)
    {
        Account account = new Account(playerName, email, clan, birthDate, armyColorIndex);
        _availableAccounts.Add(account);
        accountSwitcher.AddAccount(account);

        int index = 0;
        foreach (var availableAccount in _availableAccounts)
        {
            if (availableAccount.AccountIndex != index)
            {
                account.AccountIndex = index;
                break;
            }

            index++;
        }

        return account;
    }

    public void SwitchAccount()
    {
        Account nextAccount = accountSwitcher.SwitchAccount();
        LocalLoadCharacterProgress(nextAccount);
    }

    public void DeleteAccount()
    {
        RemoveAccountData(accountSwitcher.GetCurAccount().AccountIndex);
        _availableAccounts.Remove(accountSwitcher.GetCurAccount());
        accountSwitcher.DeleteAccount();
    }

    private Account GetAccount(int index)
    {
        return _availableAccounts[index];
    }

    public void RemoveAccountData(int accountIndex)
    {
        if (!PlayerPrefs.HasKey("PlayerName" + accountIndex))
        {
            return;
        }

        PlayerPrefs.DeleteKey("PlayerName" + accountIndex);
        PlayerPrefs.DeleteKey("Email" + accountIndex);
        PlayerPrefs.DeleteKey("Clan" + accountIndex);
        PlayerPrefs.DeleteKey("Date" + accountIndex);
        PlayerPrefs.DeleteKey("ColorIndex" + accountIndex);
        RemoveArmyData(accountIndex, true);
        RemoveAvailableCharactersData(accountIndex, true);
    }

    public void RemoveArmyData(int accountIndex, bool accountDeleting = false)
    {

        for (int j = 0; j < 16; j++)
        {
            if (PlayerPrefs.HasKey("ArmyCharacter" + accountIndex + "Index" + j))
            {
                if (accountDeleting)
                {
                    CharacterProgress.removeCharacterProgress("Character" + accountIndex +
                                                              PlayerPrefs.GetInt("ArmyCharacter" + accountIndex +
                                                                  "Index" + j), accountIndex);
                }

                PlayerPrefs.DeleteKey("ArmyCharacter" + accountIndex + "Index" + j);
            }
        }
    }

    public void RemoveAvailableCharactersData(int accountIndex, bool accountDeleting = false)
    {
        for (int j = 0; j < 32; j++)
        {
            if (PlayerPrefs.HasKey("AvailableCharacter" + accountIndex + "Index" + j))
            {
                if (accountDeleting)
                {
                    CharacterProgress.removeCharacterProgress("Character" + accountIndex +
                                                              PlayerPrefs.GetInt("ArmyCharacter" + accountIndex +
                                                                  "Index" + j), accountIndex);
                }

                PlayerPrefs.DeleteKey("AvailableCharacter" + accountIndex + "Index" + j);
            }
        }
    }

    public void RemoveAllData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void LocalSaveAccount(Account account)
    {
        PlayerPrefs.SetString("PlayerName" + account.AccountIndex, account.PlayerName);
        PlayerPrefs.SetString("Email" + account.AccountIndex, account.Email);
        PlayerPrefs.SetString("Clan" + account.AccountIndex, account.Clan);
        PlayerPrefs.SetInt("AccountIndex" + account.AccountIndex, account.AccountIndex);
        PlayerPrefs.SetInt("ColorIndex" + account.AccountIndex, account.ColorIndex);
        PlayerPrefs.SetString("Date" + account.AccountIndex, account.BirthDate.ToBinary().ToString());

        SaveArmy(account);
    }

    public void SaveArmy(Account account)
    {
        var indexes = account.Army._characters.Select(x => x.index).ToArray();
        var savedCharacters = account.Army._characters;
        for (int i = 0; i < indexes.Length; i++)
        {
            PlayerPrefs.SetInt("ArmyCharacter" + account.AccountIndex + "Index" + i, indexes[i]);
            CharacterProgress.saveCharacterProgress(savedCharacters[i].characterProgress,
                "Character" + account.AccountIndex + indexes[i], account.AccountIndex);
        }

        SaveAvailableCharacters(account);
    }

    public void SaveAvailableCharacters(Account account)
    {
        var indexes = account.AvailableCharacters.Select(x => x.index).ToArray();
        var savedCharacters = account.AvailableCharacters.ToArray();
        for (int i = 0; i < indexes.Length; i++)
        {
            PlayerPrefs.SetInt("AvailableCharacter" + account.AccountIndex + "Index" + i, indexes[i]);
            CharacterProgress.saveCharacterProgress(savedCharacters[i].characterProgress,
                "Character" + account.AccountIndex + indexes[i], account.AccountIndex);
        }
    }

    public void LoadSavedAccountsValues()
    {
        for (int i = 0; i < MaxAccountCount; i++)
        {
            if (!PlayerPrefs.HasKey("PlayerName" + i))
            {
                continue;
            }

            Account newAccount = new Account(PlayerPrefs.GetString("PlayerName" + i),
                PlayerPrefs.GetString("Email" + i),
                PlayerPrefs.GetString("Clan" + i), DateTime.FromBinary(long.Parse(PlayerPrefs.GetString("Date" + i))),
                PlayerPrefs.GetInt("ColorIndex" + i));

            newAccount.AccountIndex = PlayerPrefs.GetInt("AccountIndex" + i);


            List<int> indexes = new List<int>();
            for (int j = 0; j < 16; j++)
            {
                if (PlayerPrefs.HasKey("ArmyCharacter" + i + "Index" + j))
                {
                    indexes.Add(PlayerPrefs.GetInt("ArmyCharacter" + i + "Index" + j));
                }
                else
                {
                    indexes = null;
                    break;
                }
            }

            if (indexes != null)
            {
                Army army = new Army();

                army.SetupArmy(Character.GetCharactersByIndexes(indexes.ToArray()).ToArray());
                newAccount.Army = army;
            }
            else
            {
                newAccount.Army = null;
            }

            indexes = new List<int>();
            for (int j = 0; j < 32; j++)
            {
                if (PlayerPrefs.HasKey("AvailableCharacter" + i + "Index" + j))
                {
                    indexes.Add(PlayerPrefs.GetInt("AvailableCharacter" + i + "Index" + j));
                }
                else
                {
                    break;
                }
            }

            newAccount.AvailableCharacters = Character.GetCharactersByIndexes(indexes.ToArray());
            _availableAccounts.Add(newAccount);

            LocalLoadCharacterProgress(newAccount);
        }
    }

    public void LocalLoadCharacterProgress(Account newAccount)
    {
        if (newAccount.Army != null)
        {

            foreach (var curCharacter in newAccount.Army._characters)
            {
                
                if (SaveGame.Exists("Character" + newAccount.AccountIndex + curCharacter.index))
                {
                    curCharacter.characterProgress =
                        CharacterProgress.getCharacterProgress(
                            "Character" + newAccount.AccountIndex + curCharacter.index, newAccount.AccountIndex);
                 
                    
                }
                else
                {
                    curCharacter.characterProgress = new CharacterProgress();
                    curCharacter.SetupPrimaryStances();
                }

                curCharacter.SetupLevel();
            }

            foreach (var curCharacter in newAccount.AvailableCharacters)
            {
                if (SaveGame.Exists("Character" + newAccount.AccountIndex + curCharacter.index))
                {
                    curCharacter.characterProgress =
                        CharacterProgress.getCharacterProgress(
                            "Character" + newAccount.AccountIndex + curCharacter.index, newAccount.AccountIndex);
                }
                else
                {
                    curCharacter.characterProgress = new CharacterProgress();
                    curCharacter.SetupPrimaryStances();
                }

                curCharacter.SetupLevel();
            }
        }
    }


}