using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettingsScript
{
  /// <summary>
  /// Interface data
  /// </summary>
  public static bool ProfileImage=true;
  public static bool PlayerName=true;
  public static bool FixedCameraIcon=true;
  public static bool Timer=true;
  public static bool RoleFlagMode=true;
  public enum JoystickType
  {
    Solid,
    Red,
    Black,
    Transparent
  }  
  public enum IconType
  {
    Default,
    Samurai,
    Medieval

  }

  public static JoystickType CurJoystick = JoystickType.Solid;
  public static IconType CurIconType = IconType.Default;

  /// <summary>
  /// Sound data
  /// </summary>
  public static float GlobalVolume=1;
  public static float MusicVolume=0.5f;
  public static float SfxVolume=0.5f;

  /// <summary>
  /// //////
  /// </summary>
  public static bool BloodEnable = true;
  public static bool ShadowsEnable= true;

}
