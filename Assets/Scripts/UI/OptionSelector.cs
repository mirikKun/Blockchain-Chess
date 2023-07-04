using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelector : MonoBehaviour
{
   [SerializeField] private Button[] optionButtons;

   [SerializeField] private Transform curSelectedImage;
   [SerializeField] private Sprite redBack;
   [SerializeField] private Sprite blackBack;
   private Image _lastButton;

   private void Start()
   {
       for (int i = 0; i < optionButtons.Length; i++)
       {
           if(!_lastButton)
           {
               _lastButton = optionButtons[i].GetComponent<Image>();
           }

           var i1 = i;
           optionButtons[i].onClick.AddListener(delegate
           {
               SelectThisOption(i1);
           });
       }
   }

   public Button AddListenerByIndex(int index)
   {
       return optionButtons[index];
   }
   public void SelectThisOption(int index)
   {
       if(curSelectedImage)
       {
           curSelectedImage.position = optionButtons[index].transform.position;
       }       
       if (redBack && blackBack)
       {
          if (_lastButton)
               _lastButton.sprite = blackBack;
          Image curOptionImage = optionButtons[index].GetComponent<Image>();
          curOptionImage.sprite = redBack;
          _lastButton = curOptionImage;
       }
   }
}
