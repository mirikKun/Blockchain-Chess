using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterDropHolder : MonoBehaviour, IDropHandler
{
    public bool squareView;
    [SerializeField] private Role role;
    [SerializeField] private RectTransform holderTransform;

    [SerializeField] private Sprite blackSquare;
    [SerializeField] private Sprite redSquare;
    private Image _image;
    public Action OnMoveToHolder;
    public Action OnDropEnd;
    private Button button;
    private ArmyRoleHolder _armyRoleHolder;

    private void Start()
    {
        _armyRoleHolder = GetComponent<ArmyRoleHolder>();
        _image = GetComponent<Image>();
        if (!holderTransform)
        {
            holderTransform = GetComponent<RectTransform>();
        }
    }

    public int GetRole()
    {
        return (int) role;
    }
    public Button GetButton()
    {
        button = GetComponent<Button>();
        return button;
    }
    public void OnDrop(PointerEventData eventData)
    {
        CharacterDragHandler newCharacterDragHandler = eventData.pointerDrag.GetComponent<CharacterDragHandler>();
        if (newCharacterDragHandler != null)
        {
            MoveCharacterToTheHolder(newCharacterDragHandler);
            OnDropEnd?.Invoke();
            
        }
    }

    public void ChangeSprite(bool red)
    {
        if (red)
        {
            _image.sprite = redSquare;
        }
        else
        {
            _image.sprite = blackSquare;

        }
    }
    public void MoveCharacterToTheHolder(CharacterDragHandler characterDragHandler)
    {
        if (!holderTransform)
        {
            holderTransform = GetComponent<RectTransform>();
            _armyRoleHolder = GetComponent<ArmyRoleHolder>();

        }       
        if (squareView)
        {
            if (role == characterDragHandler.GetComponent<CharacterDisplay>().GetRole())
            {
                characterDragHandler.BecomeSquareView();
            }
            else
            {
                return;
            }
        }
        else
        {
            characterDragHandler.BecomeListView();
        }
        if (squareView && GetComponentInChildren<CharacterDragHandler>()
                       &&GetComponentInChildren<CharacterDragHandler>()!=characterDragHandler)
        {
            CharacterDragHandler lastCharacter = GetComponentInChildren<CharacterDragHandler>();
            characterDragHandler.transform.parent = holderTransform;

            characterDragHandler.GetLastParent().GetComponentInParent<CharacterDropHolder>().MoveCharacterToTheHolder(lastCharacter);
        }
        
        characterDragHandler.OnChangingPlace?.Invoke(characterDragHandler);

        if (_armyRoleHolder)
        {

            _armyRoleHolder.SetupCharacter(characterDragHandler.GetCharacterDisplay());
        }
        characterDragHandler.OnChangingPlace += RemoveLastCharacterLink;
                       
       
        OnMoveToHolder?.Invoke();

        characterDragHandler.transform.parent = holderTransform;

      
        characterDragHandler.transform.localPosition=Vector3.zero;

    }
    public void RemoveLastCharacterLink(CharacterDragHandler characterDragHandler)
    {
        if (_armyRoleHolder&&_armyRoleHolder.SameCharacter(characterDragHandler.GetCharacterDisplay()))
        {
            _armyRoleHolder.SetupCharacter(null);
        }

        characterDragHandler.OnChangingPlace -= RemoveLastCharacterLink;
    } 
}