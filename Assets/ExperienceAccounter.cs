using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;

public class ExperienceAccounter
{
    public static void GainExperience(figureMover character, int experience)
    {
        if (character.playerType == PlayerType.Player)
        {
            experience = ApplyMultiplayer(experience, character.thisCharacter.characterProgress.CurIntelligence);

            character.thisCharacter.AddExperience(experience);
            // PlayerPrefs.SetInt("CharacterExp" + PlayerData.Account.AccountIndex + killerFigure.thisCharacter.index,
            //     killerFigure.thisCharacter.experience);
            //CharacterProgress.saveCharacterProgress(character.thisCharacter.characterProgress,"Character" + PlayerData.Account.AccountIndex + character.thisCharacter.index);
        }
        else
        {
            character.thisCharacter.AddExperience(experience, true);
        }
    }

    public static void GainExperience(Character character, int experience)
    {
        experience = ApplyMultiplayer(experience, character.characterProgress.CurIntelligence);

        character.AddExperience(experience);
        // PlayerPrefs.SetInt("CharacterExp" + PlayerData.Account.AccountIndex + killerFigure.thisCharacter.index,
        //     killerFigure.thisCharacter.experience);
        //CharacterProgress.saveCharacterProgress(character.thisCharacter.characterProgress,"Character" + PlayerData.Account.AccountIndex + character.thisCharacter.index);
    }

    public static void GainKillExperience(figureMover killerFigure, Character deadMan)
    {
        // int curExperience=100;
        // if (deadMan.characterRole == Role.Pawn)
        // {
        //     curExperience = 100;
        // }
        // else if (deadMan.characterRole == Role.Knight || deadMan.characterRole == Role.Bishop)
        // {
        //     curExperience = 150;
        // }     
        // else if (deadMan.characterRole == Role.Rook )
        // {
        //     curExperience = 200;
        // } 
        // else if (deadMan.characterRole == Role.Queen || deadMan.characterRole == Role.King)
        // {
        //     curExperience = 250;
        // }
        //
        // killer.AddExperience(curExperience);
        GainKillExperience(killerFigure, (int) deadMan.characterRole);
    }

    public static void GainKillExperience(figureMover killerFigure, int deadMan)
    {
        if (killerFigure.playerType == PlayerType.Computer)
        {
            return;
        }

        int curExperience = 100;
        if (deadMan == (int) Role.Pawn)
        {
            curExperience = 100;
        }
        else if (deadMan == (int) Role.Knight || deadMan == (int) Role.Bishop)
        {
            curExperience = 150;
        }
        else if (deadMan == (int) Role.Rook)
        {
            curExperience = 200;
        }
        else if (deadMan == (int) Role.Queen || deadMan == (int) Role.King)
        {
            curExperience = 250;
        }


        if (killerFigure.playerType == PlayerType.Player)
        {
            curExperience = ApplyMultiplayer(curExperience, killerFigure.thisCharacter.characterProgress.CurIntelligence);

            killerFigure.thisCharacter.AddExperience(curExperience);
            // PlayerPrefs.SetInt("CharacterExp" + PlayerData.Account.AccountIndex + killerFigure.thisCharacter.index,
            //     killerFigure.thisCharacter.experience);
            //CharacterProgress.saveCharacterProgress(killerFigure.thisCharacter.characterProgress,"Character" + PlayerData.Account.AccountIndex + killerFigure.thisCharacter.index);
        }
        else
        {
            killerFigure.thisCharacter.AddExperience(curExperience, true);
        }
    }

    private static int ApplyMultiplayer(int baseExperience, int intelligence)
    {
        return baseExperience + baseExperience * (intelligence - 20) / 100;
    }
}