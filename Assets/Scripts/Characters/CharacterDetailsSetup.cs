using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;

public class CharacterDetailsSetup : MonoBehaviour
{
    [SerializeField] private Character[] characters;
    [SerializeField] private CharacterStats[] charactersDetails;

    public void Setup()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].characterStats = charactersDetails[i];
          
            if (characters[i].characterClass == Class.Ashigaru)
            {
                charactersDetails[i].Strength = 20;
                charactersDetails[i].Dexterity = 20;
                charactersDetails[i].Endurance = 20;
                charactersDetails[i].Intelligence = 20;
                charactersDetails[i].Armor = 10;
            }
            else             
            if (characters[i].characterClass == Class.Samurai)
            {
                charactersDetails[i].Strength = 30;
                charactersDetails[i].Dexterity = 25;
                charactersDetails[i].Endurance = 30;
                charactersDetails[i].Intelligence = 20;
                charactersDetails[i].Armor = 30;
            }
            else             
            if (characters[i].characterClass == Class.Hatamoto)
            {
                charactersDetails[i].Strength = 40;
                charactersDetails[i].Dexterity = 30;
                charactersDetails[i].Endurance = 40;
                charactersDetails[i].Intelligence = 30;
                charactersDetails[i].Armor = 50;
            }
            else             
            if (characters[i].characterClass == Class.Oni)
            {
                charactersDetails[i].Strength = 50;
                charactersDetails[i].Dexterity = 30;
                charactersDetails[i].Endurance = 50;
                charactersDetails[i].Intelligence = 30;
                charactersDetails[i].Armor = 50;
            }
            else             
            if (characters[i].characterClass == Class.YabusameArcher)
            {
                charactersDetails[i].Strength = 25;
                charactersDetails[i].Dexterity = 30;
                charactersDetails[i].Endurance = 30;
                charactersDetails[i].Intelligence = 30;
                charactersDetails[i].Armor = 0;
            }
            else             
            if (characters[i].characterClass == Class.Ninja)
            {
                charactersDetails[i].Strength = 20;
                charactersDetails[i].Dexterity = 40;
                charactersDetails[i].Endurance = 30;
                charactersDetails[i].Intelligence = 30;
                charactersDetails[i].Armor = 0;
            }
            else      
            
            if (characters[i].characterClass == Class.Monk)
            {
                charactersDetails[i].Strength = 25;
                charactersDetails[i].Dexterity = 30;
                charactersDetails[i].Endurance = 30;
                charactersDetails[i].Intelligence = 35;
                charactersDetails[i].Armor = 0;
            }
            else              
            if (characters[i].characterClass == Class.Ronin)
            {
                charactersDetails[i].Strength = 30;
                charactersDetails[i].Dexterity = 30;
                charactersDetails[i].Endurance = 30;
                charactersDetails[i].Intelligence = 30;
                charactersDetails[i].Armor = 0;
            }
            else              
            if (characters[i].characterClass == Class.Onna_musha)
            {
                charactersDetails[i].Strength = 25;
                charactersDetails[i].Dexterity = 40;
                charactersDetails[i].Endurance = 30;
                charactersDetails[i].Intelligence = 40;
                charactersDetails[i].Armor = 20;
            }
            else   
            
            if (characters[i].characterClass == Class.Onna_bugeisha)
            {
                charactersDetails[i].Strength = 25;
                charactersDetails[i].Dexterity = 40;
                charactersDetails[i].Endurance = 30;
                charactersDetails[i].Intelligence = 40;
                charactersDetails[i].Armor = 0;
            }
            else                
            if (characters[i].characterClass == Class.Damiyo)
            {
                charactersDetails[i].Strength = 30;
                charactersDetails[i].Dexterity = 30;
                charactersDetails[i].Endurance = 40;
                charactersDetails[i].Intelligence = 40;
                charactersDetails[i].Armor = 50;
            }
            else                
            if (characters[i].characterClass == Class.GreatDamiyo)
            {
                charactersDetails[i].Strength = 25;
                charactersDetails[i].Dexterity = 40;
                charactersDetails[i].Endurance = 30;
                charactersDetails[i].Intelligence = 40;
                charactersDetails[i].Armor = 0;
            }
      
       
        }
        
    }
}
