using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class OneOptionDropDowns : MonoBehaviour
{
    [System.Serializable]
    public class OneOptionDropDown
    {
        public Button button;
        public RectTransform arrow;

        public GameObject content;
        public bool opened;
    }

    
    private OneOptionDropDown _curOneOptionDropDown;
    private RectTransform _lastArrow;
    [SerializeField] private OneOptionDropDown[] oneOptionDropDowns;
    [SerializeField] private ScrollRect scrollRect;

    private void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        scrollRect.vertical=false;
#endif
    
        
        foreach (var  oneOptionDropDown in oneOptionDropDowns)
        {
            if (_curOneOptionDropDown==null)
            {
                _curOneOptionDropDown = oneOptionDropDown;
                oneOptionDropDown.opened = true;

            }

            if (!_lastArrow)
            {
                _lastArrow = oneOptionDropDown.arrow;
                ChangeArrowRotation(0);
            }
            oneOptionDropDown.button.onClick.AddListener(delegate
            {
                Debug.Log(1);
                if(!oneOptionDropDown.opened)
                {
                    oneOptionDropDown.opened = true;
                    
                    
#if UNITY_ANDROID && !UNITY_EDITOR
                    ChangeArrowRotation(90);
                    _curOneOptionDropDown.content.SetActive(false);
                    _curOneOptionDropDown.opened = false;
#endif
                   

                    _lastArrow = oneOptionDropDown.arrow;
                    ChangeArrowRotation(0);

                    _curOneOptionDropDown = oneOptionDropDown;
                    oneOptionDropDown.content.SetActive(true);
                }
                else
                {                               
                    _lastArrow = oneOptionDropDown.arrow;
                    ChangeArrowRotation(90);
                    oneOptionDropDown.content.SetActive(false);
                    oneOptionDropDown.opened = false;
                }
            });
            
        }
    }

    private void ChangeArrowRotation(int angle)
    {
        _lastArrow.eulerAngles = new Vector3(0, 0, angle);
    }
}
