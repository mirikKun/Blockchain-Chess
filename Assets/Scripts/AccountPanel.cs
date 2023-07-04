using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountPanel : MonoBehaviour
{
    [SerializeField] private GameObject resourcesPanel;
    [SerializeField] private Image resourcesImage;

    [SerializeField] private GameObject activityPanel;
    [SerializeField] private Image activityImage;

    public void SwitchToActivityPanel()
    {
        resourcesPanel.SetActive(false);
        resourcesImage.enabled = false;
        activityPanel.SetActive(true);
        activityImage.enabled = true;

        
    }
    public void SwitchToResourcesPanel()
    {
        activityPanel.SetActive(false);
        activityImage.enabled = false;
        
        resourcesPanel.SetActive(true);
        resourcesImage.enabled = true;

        

        
    }
}
