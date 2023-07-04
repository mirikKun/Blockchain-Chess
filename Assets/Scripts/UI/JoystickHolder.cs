using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickHolder : MonoBehaviour
{
    [SerializeField]  private RectTransform[] sticks;
    [SerializeField] private Joystick[] joysticks;

    private void OnDisable()
    {

        foreach (var joystick in joysticks)
        {
            joystick.OnPointerUp(null);
        }
    }
}
