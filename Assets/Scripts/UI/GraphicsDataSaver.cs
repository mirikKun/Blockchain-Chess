using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using ShadowQuality = UnityEngine.ShadowQuality;

public class GraphicsDataSaver : MonoBehaviour
{
    [SerializeField] private Toggle blood;
    [SerializeField] private Toggle shadows;


    private void Start()
    {
        blood.onValueChanged.AddListener(ChangeBloodEnableBool);
        shadows.onValueChanged.AddListener(ChangeShadowsEnableBool);
        
        blood.isOn = GameSettingsScript.BloodEnable;
        shadows.isOn = GameSettingsScript.ShadowsEnable;
    }

    private void ChangeBloodEnableBool(bool enable)
    {
        GameSettingsScript.BloodEnable = enable;
    }    
    private void ChangeShadowsEnableBool(bool enable)
    {
        GameSettingsScript.ShadowsEnable = enable;
        if (enable)
        {
            // UniversalRenderPipelineAsset asset = (UniversalRenderPipelineAsset) QualitySettings.renderPipeline;
            // asset.
            //  (QualitySettings.renderPipeline).shad
          //  QualitySettings.shadows = ShadowQuality.All;

        }
        else
        {
           // QualitySettings.shadows = ShadowQuality.Disable;

        }
    }
    
}
