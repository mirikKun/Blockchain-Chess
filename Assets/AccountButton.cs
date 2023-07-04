using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountButton : MonoBehaviour
{
   [SerializeField] public Image background;
   [SerializeField] public Image icon;
   [SerializeField] public TextMeshProUGUI nameText;
   [SerializeField] public TextMeshProUGUI hash;
   [SerializeField] public TextMeshProUGUI level;
   [SerializeField] public Button button;

   public void SetAccountButton(string accountName)
   {
      nameText.text = accountName;
   }
   public void SelectCurrent()
   {
      //background.enabled = true;
      background.color = new Color(background.color.r, background.color.g, background.color.b, 1);
   }

   public void UnselectPrevious()
   {
      //background.enabled = false;
      background.color = new Color(background.color.r, background.color.g, background.color.b, 1 / 255f );

   }

   public Button GetButton()
   {
      return button;
   }

}
