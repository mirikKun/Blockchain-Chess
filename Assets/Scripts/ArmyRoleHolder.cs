using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyRoleHolder : MonoBehaviour
{
    [SerializeField] private CharacterDisplay currentCharacterDisplay;

    public void SetupCharacter(CharacterDisplay newCharacterDisplay)
    {
        currentCharacterDisplay = newCharacterDisplay;
    }

    public bool SameCharacter(CharacterDisplay newCharacterDisplay)
    {
        return newCharacterDisplay == currentCharacterDisplay;
    }

    public Character GetCharacter()
    {
        if (currentCharacterDisplay)
        {
            return currentCharacterDisplay.GetCharacter();
        }

        return null;
    }
}
