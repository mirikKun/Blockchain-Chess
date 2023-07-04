using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
   [SerializeField] private GameObject panel;
   [SerializeField] private bool panelActive;

   public void Switch()
   {
      if (panelActive)
      {
         panel.SetActive(false);
         panelActive = false;
      }
      else
      {
         panel.SetActive(true);
         panelActive = true;
      }
   }
}
