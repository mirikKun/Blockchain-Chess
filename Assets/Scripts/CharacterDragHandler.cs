using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterDragHandler : MonoBehaviour,IDragHandler, IEndDragHandler,IBeginDragHandler
{
    [SerializeField] private Image background;
    [SerializeField] private Button button;
    private CharacterDisplay _characterDisplay;
    private CanvasGroup _canvasGroup;
    private Transform _lastTransform;
    private Transform _armyTransform;
    
    public Action<CharacterDragHandler> OnChangingPlace;
    private void Start()
    {
        SetupDragHolder();
    }

    private void SetupDragHolder()
    {
        _lastTransform = transform.parent;

        _characterDisplay = GetComponent<CharacterDisplay>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _armyTransform = GetComponentInParent<ArmyController>().transform;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        transform.localPosition=Vector3.zero;
        if(transform.parent==_armyTransform)
        {            
            transform.position = _lastTransform.position;
            transform.parent = _lastTransform;
            _characterDisplay.SwitchView();
        }    
    }



    public CharacterDisplay GetCharacterDisplay()
    {
        if (!_characterDisplay)
        {
            SetupDragHolder();
        }
        return _characterDisplay;
    }

    public void BecomeListView()
    {
        _characterDisplay.TurnToListView();
        _characterDisplay.WasInList = true;

    }
    public void BecomeSquareView()
    {
        if (!_characterDisplay)
        {
            SetupDragHolder();
        }

        SetHighlightActive(false);
        _characterDisplay.TurnToSquareView();
        _characterDisplay.WasInList = false;
    }

    public void ChangeLastParent(Transform newParent)
    {
        _lastTransform = newParent;
    }
    public void SetHighlightActive(bool active)
    {
        background.enabled = active;

        // if (_characterDisplay.WasInList)
        // {
        //     background.enabled = active;
        // }
        // else
        // {
        //     background.enabled = false;
        // }
    }

    public Transform GetLastParent()
    {
        return _lastTransform;
    }

    public Button GetButton()
    {
        return button;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastTransform = transform.parent;
        transform.parent = _armyTransform;
        _characterDisplay.TurnToSquareView();
        _canvasGroup.blocksRaycasts = false;
    }
}
