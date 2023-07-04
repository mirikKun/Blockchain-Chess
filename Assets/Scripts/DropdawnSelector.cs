using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdawnSelector : MonoBehaviour
{
   [SerializeField] private TMP_Dropdown dropdown;
   [SerializeField] private Sprite black;
   [SerializeField] private Sprite red;

   private void Start()
   {
      dropdown.onValueChanged.AddListener(delegate(int arg0)
      {
         if (arg0 == 0)
         {
            dropdown.captionText.color=Color.gray;
            dropdown.image.sprite = black;
         }
         else
         {
            dropdown.captionText.color=Color.white;

            dropdown.image.sprite = red;
         }
      });
   }
}
