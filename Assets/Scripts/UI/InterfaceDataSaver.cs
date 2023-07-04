using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceDataSaver : MonoBehaviour
{
    [SerializeField] private UIObjectsSetupper uiObjectsSetupper;
    [SerializeField] private Toggle profileImage;
    [SerializeField] private Toggle playerName;
    [SerializeField] private Toggle fixedCameraIcon;
    [SerializeField] private Toggle timer;
    [SerializeField] private Toggle roleFlagMode;
    [SerializeField] private OptionSelector joystickSelector;
    [SerializeField] private OptionSelector characterIconSelector;
    private void Start()
    {
        profileImage.onValueChanged.AddListener(ChangeProfileImageBool);
        playerName.onValueChanged.AddListener(ChangePlayerNameBool);
        fixedCameraIcon.onValueChanged.AddListener(ChangeFixedCameraIconBool);
        timer.onValueChanged.AddListener(ChangeTimerBool);
        roleFlagMode.onValueChanged.AddListener(ChangeRoleFlagModeBool);

        profileImage.isOn = GameSettingsScript.ProfileImage;
        playerName.isOn = GameSettingsScript.PlayerName;
        fixedCameraIcon.isOn = GameSettingsScript.FixedCameraIcon;
        timer.isOn = GameSettingsScript.Timer;
        roleFlagMode.isOn = GameSettingsScript.RoleFlagMode;

        if (GameSettingsScript.CurJoystick == GameSettingsScript.JoystickType.Solid)
        {
            joystickSelector.SelectThisOption(0);
        }
        else if (GameSettingsScript.CurJoystick == GameSettingsScript.JoystickType.Red)
        {
            joystickSelector.SelectThisOption(1);

        }
        else if (GameSettingsScript.CurJoystick == GameSettingsScript.JoystickType.Black)
        {
            joystickSelector.SelectThisOption(2);

        }
        else
        {
            joystickSelector.SelectThisOption(3);

        }

        joystickSelector.AddListenerByIndex(0).onClick.AddListener(SelectSolidJoystick);
        joystickSelector.AddListenerByIndex(1).onClick.AddListener(SelectRedJoystick);
        joystickSelector.AddListenerByIndex(2).onClick.AddListener(SelectBlackJoystick);
        joystickSelector.AddListenerByIndex(3).onClick.AddListener(SelectTransparentJoystick);        
        
        if (GameSettingsScript.CurIconType == GameSettingsScript.IconType.Default)
        {
            characterIconSelector.SelectThisOption(0);
        }

      
        else
        {
            characterIconSelector.SelectThisOption(1);

        }

        characterIconSelector.AddListenerByIndex(0).onClick.AddListener(SelectDefaultIconType);
        characterIconSelector.AddListenerByIndex(1).onClick.AddListener(SelectSamuraiIconType);
        characterIconSelector.AddListenerByIndex(2).onClick.AddListener(SelectMedievalIconType);

        
    }

    private void ChangeProfileImageBool(bool enable)
    {
        GameSettingsScript.ProfileImage = enable;
        if(uiObjectsSetupper)
            uiObjectsSetupper.SetupProfileImageUI();
    }
    private void ChangePlayerNameBool(bool enable)
    {
        GameSettingsScript.PlayerName = enable;
        if(uiObjectsSetupper)
            uiObjectsSetupper.SetupPlayerNameUI();
    }
    private void ChangeFixedCameraIconBool(bool enable)
    {
        GameSettingsScript.FixedCameraIcon = enable;
        if(uiObjectsSetupper)
            uiObjectsSetupper.SetupFixedCameraIconUI();
    }
    private void ChangeTimerBool(bool enable)
    {
        GameSettingsScript.Timer = enable;
        if(uiObjectsSetupper)
            uiObjectsSetupper.SetupTimerUI();
    }    
    
    private void SelectSolidJoystick()
    {
        GameSettingsScript.CurJoystick = GameSettingsScript.JoystickType.Solid;
        if (uiObjectsSetupper)
            uiObjectsSetupper.SetupJoystickTypeUI();
    }       
    private void SelectRedJoystick()
    {
        GameSettingsScript.CurJoystick = GameSettingsScript.JoystickType.Red;
        if (uiObjectsSetupper)
            uiObjectsSetupper.SetupJoystickTypeUI();
    }    
    private void SelectBlackJoystick()
    {
        GameSettingsScript.CurJoystick = GameSettingsScript.JoystickType.Black;
        if (uiObjectsSetupper)
            uiObjectsSetupper.SetupJoystickTypeUI();
    }    
    private void SelectTransparentJoystick()
    {

        GameSettingsScript.CurJoystick = GameSettingsScript.JoystickType.Transparent;
        if (uiObjectsSetupper)
            uiObjectsSetupper.SetupJoystickTypeUI();
    }
    
    private void ChangeRoleFlagModeBool(bool enable)
    {
        GameSettingsScript.RoleFlagMode = enable;
        if(uiObjectsSetupper)
            uiObjectsSetupper.RoleFlagModeUI();
    }
    
    private void SelectDefaultIconType()
    {
        GameSettingsScript.CurIconType = GameSettingsScript.IconType.Default;
        if (uiObjectsSetupper)
            uiObjectsSetupper.SetupCharacterIconsType();
    }       
    private void SelectSamuraiIconType()
    {
        GameSettingsScript.CurIconType = GameSettingsScript.IconType.Samurai;
        if (uiObjectsSetupper)
            uiObjectsSetupper.SetupCharacterIconsType();
    }       
    private void SelectMedievalIconType()
    {
        GameSettingsScript.CurIconType = GameSettingsScript.IconType.Medieval;
        if (uiObjectsSetupper)
            uiObjectsSetupper.SetupCharacterIconsType();
    }    
    
}
