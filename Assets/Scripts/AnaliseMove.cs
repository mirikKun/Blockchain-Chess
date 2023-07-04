using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnaliseMove : MonoBehaviour
{
    private RectTransform _rectTransform;
    private CharacterShower _characterShower;
    private Button _button;
    [SerializeField] private Button seeExecutionButton;
    [SerializeField] private GameObject redBackground;

    [SerializeField] private TextMeshProUGUI fromTo;
    [SerializeField] private TextMeshProUGUI time;
    [SerializeField] private TextMeshProUGUI moveNumber;
    
    [SerializeField] private int heightOfCurrent=191;
    [SerializeField] private int heightOfDefault=125;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SelectMove(bool withFatality)
    {
        if(withFatality)
        {
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, heightOfCurrent);
                    seeExecutionButton.gameObject.SetActive(true);

        }
        redBackground.SetActive(true);
    }
    public void UnselectMove()
    {
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, heightOfDefault);
        seeExecutionButton.gameObject.SetActive(false);
        redBackground.SetActive(false);
    }

    public void SetupMove(figureMover character, string newFromTo,string newTime, int newMoveNumber,Action<int> onMoveClick,Action<int> onSeeExecution)
    {
        _button = GetComponent<Button>();
        _characterShower = GetComponent<CharacterShower>();

        _button.onClick.AddListener(delegate { onMoveClick.Invoke(newMoveNumber); });
        seeExecutionButton.onClick.AddListener(delegate { onSeeExecution.Invoke(newMoveNumber); });
 
        if(character)
            _characterShower.ShowCurFigure(character.thisCharacter);
        fromTo.text = newFromTo;
        time.text = newTime;
        if (newMoveNumber == 0)
        {
            moveNumber.text = "GAME START";
        }
        else
        {
            moveNumber.text = "MOVE "+newMoveNumber;
        }
    }
}
