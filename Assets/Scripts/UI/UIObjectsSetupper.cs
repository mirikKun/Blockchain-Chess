using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObjectsSetupper : MonoBehaviour
{
    [SerializeField] private GameObject profileImage;
    [SerializeField] private GameObject playerName;
    [SerializeField] private GameObject fixedCameraIcon;
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject roleFlagMode;
    
    
    
    [SerializeField] private JoystickTypeSetupper joystickTypeSetupper;
    [SerializeField] private CharecterIconsTypeSetupper iconsTypeSetupper;


    private void Start()
    {
        SetupProfileImageUI();
        SetupPlayerNameUI();
        SetupFixedCameraIconUI();
        SetupTimerUI();
        RoleFlagModeUI();
        SetupJoystickTypeUI();
    }

    public void SetupProfileImageUI()
    {
        if(profileImage)
            profileImage.SetActive(GameSettingsScript.ProfileImage);
    }
    public void SetupPlayerNameUI()
    {
        if(playerName)
            playerName.SetActive(GameSettingsScript.PlayerName);
    }
    public void SetupFixedCameraIconUI()
    {
        if(fixedCameraIcon)
            fixedCameraIcon.SetActive(GameSettingsScript.FixedCameraIcon);
    }
    public void SetupTimerUI()
    {
        if(timer)
            timer.SetActive(GameSettingsScript.Timer);
    }

    public void SetupJoystickTypeUI()
    {
        if (GameSettingsScript.CurJoystick == GameSettingsScript.JoystickType.Solid)
        {
            joystickTypeSetupper.ChooseSolidJoystick();
        }
        else if (GameSettingsScript.CurJoystick == GameSettingsScript.JoystickType.Red)
        {
            joystickTypeSetupper.ChooseRedJoystick();
        }
        else if (GameSettingsScript.CurJoystick == GameSettingsScript.JoystickType.Black)
        {
            joystickTypeSetupper.ChooseBlackJoystick();
        }
        else
        {
            joystickTypeSetupper.ChooseTransparentJoystick();
        }
    }
    public void SetupCharacterIconsType()
    {
       iconsTypeSetupper.ChangeIcons();
    }
    public void RoleFlagModeUI()
    {
        if(roleFlagMode)
            roleFlagMode.SetActive(GameSettingsScript.RoleFlagMode);
    }
}
