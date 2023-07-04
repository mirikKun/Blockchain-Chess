using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPanelsSwitch : MonoBehaviour
{
   [SerializeField] private SubPanel lastSubPanel;
   [Serializable]
   public class SubPanel
   {
      public GameObject[] panels;
      public GameObject buttonHighlight;
   }

   [SerializeField] private SubPanel[] allSubPanels;

   public void SwitchPanel(int panelIndex)
   {
      foreach (var panel in lastSubPanel.panels)
      {
         panel.SetActive(false);
      }
      lastSubPanel.buttonHighlight.SetActive(false);
      lastSubPanel = allSubPanels[panelIndex];
      foreach (var panel in lastSubPanel.panels)
      {
         panel.SetActive(true);
      }
      lastSubPanel.buttonHighlight.SetActive(true);
   }
}
