using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static Army Army;
    public static int ColorIndex=-1;
    public static string PlayerName="Noname";
    public static string email;
    public static string clan;
    public static DateTime birth_date;
    public static Accounts.Account Account;
    public static Character[] Characters=new Character[0];
    //public static List<Character> AvailableCharacters;


    public static void SaveArmyIndex(int newClanIndex)
    {
        ColorIndex = newClanIndex;
        // if(Account!=null)
        //     Account.ColorIndex = newClanIndex;
    }
    public static void SaveArmy(Army newArmy)
    {
        Army = newArmy;
        if (Account != null)
        {

            Account.Army = newArmy;
        }
    }
    public static void AddNewCharacter(Character newCharacter)
    {        
        //AvailableCharacters.Add(newCharacter);

        
        Account?.AvailableCharacters.Add(newCharacter);
    }

    public static void SwitchAccount(Accounts.Account newAccount)
    {
        PlayerName = newAccount.PlayerName;
        email = newAccount.Email;
        Army = newAccount.Army;
        clan = newAccount.Clan;
        ColorIndex = newAccount.ColorIndex;
        Account = newAccount;
        //AvailableCharacters = newAccount.AvailableCharacters;
    }
}
