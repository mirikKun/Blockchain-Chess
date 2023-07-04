using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterShower : MonoBehaviour
{
    [SerializeField] private Image figureIcon;

    public void ShowCurFigure(Character character)
    {
        if(!character)
        {
            return;
        }
 
        if(character&&character)
        {
               
            figureIcon.sprite = character.characterSprite;
        }
      
      
    }
}
