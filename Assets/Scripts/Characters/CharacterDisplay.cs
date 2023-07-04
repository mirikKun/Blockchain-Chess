using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDisplay : MonoBehaviour
{
    [SerializeField] public Character character;
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] public TextMeshProUGUI roleText;
    [SerializeField] public TextMeshProUGUI levelText;
    [SerializeField] public Image characterImage;
    
    public Action<CharacterDisplay> onGoToTheList;
    public Action<CharacterDisplay> onGoOutOfTheList;
    private RectTransform _rectTransform;
    private RectTransform _imageRectTransform;
    private Vector2 _inListSize=new Vector2(275,60);
    private Vector2 _inListImageSize = new Vector2(60, 60);
    private Vector2 _squareSize=new Vector2(126,126);
    public bool WasInList=true;


    public void SwitchView()
    {
        if (WasInList)
        {
            TurnToListView();

        }
        else
        {
            TurnToSquareView();

        }
    }
    public void SetupCharacterDisplay(Character newCharacter)
    {
        character = newCharacter;
        nameText.text = character.characterName;
        roleText.text = "ROLE: "+character.characterRole.ToString();
        levelText.text = "LEVEL: "+character.characterProgress.Level.ToString();
        characterImage.sprite = character.characterSprite;
        _rectTransform = GetComponent<RectTransform>();
        _imageRectTransform = characterImage.GetComponent<RectTransform>();
    }

    public void TurnToListView()
    {
        nameText.enabled = true;
        roleText.enabled = true;
        levelText.enabled = true;
        _rectTransform.sizeDelta=_inListSize;
        _imageRectTransform.sizeDelta=_inListImageSize;
        onGoToTheList?.Invoke(this);
    }


    public void TurnToSquareView()
    {
        nameText.enabled = false;
        roleText.enabled = false;
        levelText.enabled = false;
        _rectTransform.sizeDelta=_squareSize;
        _imageRectTransform.sizeDelta=_squareSize;
        onGoOutOfTheList?.Invoke(this );

    }
    
    public Role GetRole()
    {
        return character.characterRole;
    }
    public Class GetClass()
    {
        return character.characterClass;
    }
    public Weapon GetWeapon()
    {
        return character.characterWeapon;
    }

    public Character GetCharacter()
    {
        return character;
    }

    public bool IsInHolder()
    {
        return !WasInList;
    }
}
