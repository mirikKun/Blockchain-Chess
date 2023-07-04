using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;

[CreateAssetMenu(fileName = "New character", menuName = "Character")]
public class Character : ScriptableObject
{
    public int index;
    public Sprite characterSprite;
    public string characterName;
    public Role characterRole;
    public Class characterClass;
    public Weapon characterWeapon;
    public Clan characterClan;

    public figureMover figurePrefab;

    public CharacterStats characterStats;

    public CharacterProgress characterProgress;
    public CharacterProgress enemyCharacterProgress;


    public string GetCharacterClassString()
    {
        return ClassNameParser.GetDescriptionFromEnum(characterClass);
    }
    public string GetCharacterWeaponString()
    {
        return ClassNameParser.GetDescriptionFromEnum(characterWeapon);
    }

    public static List<Character> GetCharactersByIndexes(int[] indexes)
    {
        if (indexes == null)
            return null;

        List<Character> charactersList = new List<Character>();
        foreach (var index in indexes)
        {
            foreach (var character in PlayerData.Characters)
            {
                if (character.index == index)
                {
                    charactersList.Add(character);
                    break;
                }
            }
        }

        return charactersList;
    }


    // public int GetLevelEdge(int curLevel)
    // {
    //     return _expEdges[curLevel];
    // }
    public void SetupPrimaryStances()
    {
        characterProgress.CurStrength = characterStats.Strength;
        characterProgress.CurArmor = characterStats.Armor;
        characterProgress.CurDamage = characterStats.Damage;
        characterProgress.CurDexterity = characterStats.Dexterity;
        characterProgress.CurEndurance = characterStats.Endurance;
        characterProgress.CurIntelligence = characterStats.Intelligence;
    }
    public void SetupLevel(bool enemy = false)
    {
        if (enemy)
        {
            enemyCharacterProgress.SetupLevel();
        }
        else
        {
            characterProgress.SetupLevel();
        }
    }

    public void AddExperience(int exp, bool enemy = false)
    {
        if (enemy)
        {
            enemyCharacterProgress.Experience += exp;
        }
        else
        {
            characterProgress.Experience += exp;
        }

        SetupLevel(enemy);
    }
}