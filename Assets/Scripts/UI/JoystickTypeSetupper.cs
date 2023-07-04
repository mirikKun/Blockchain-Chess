using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class JoystickTypeSetupper : MonoBehaviour
{
    [SerializeField] private Sprite redJoystick;
    [SerializeField] private Sprite blackJoystick;
    [SerializeField] private Sprite solidJoystick;
    [SerializeField] private Sprite solidJoystickArea;
    [SerializeField] private Sprite transparentJoystickArea;
    [SerializeField] private Image leftJoystick;
    [SerializeField] private Image leftJoystickArea;
    [SerializeField] private Image leftJoystickIcon;
    [SerializeField] private Image rightJoystick;
    [SerializeField] private Image rightJoystickArea;
    [SerializeField] private Image rightJoystickIcon;
    [SerializeField] private float bigJoystickSize=182;
    [SerializeField] private float smallJoystickSize=125;

    public void ChooseSolidJoystick()
    {
        leftJoystick.sprite = solidJoystick;
        rightJoystick.sprite = solidJoystick;
        leftJoystickArea.sprite = solidJoystickArea;
        rightJoystickArea.sprite = solidJoystickArea;
        MakeJoysticksVisible();
        MakeJoystickIconsTransparent();
        SetBigJoystick();
    }
    public void ChooseRedJoystick()
    {
        leftJoystick.sprite = redJoystick;
        rightJoystick.sprite = redJoystick;
        MakeJoysticksVisible();
        SetSmallJoystick();
        MakeJoystickIconsVisible();
        leftJoystickArea.sprite = transparentJoystickArea;
        rightJoystickArea.sprite = transparentJoystickArea;
    }
    public void ChooseBlackJoystick()
    {
        leftJoystick.sprite = blackJoystick;
        
        rightJoystick.sprite = blackJoystick;
        MakeJoysticksVisible();
        MakeJoystickIconsVisible();
        SetSmallJoystick();
        leftJoystickArea.sprite = transparentJoystickArea;
        rightJoystickArea.sprite = transparentJoystickArea;
    }
    public void ChooseTransparentJoystick()
    {
        Debug.Log("trans");
        leftJoystickArea.sprite = transparentJoystickArea;
        rightJoystickArea.sprite = transparentJoystickArea;
        leftJoystick.color = new Color(leftJoystick.color.r, leftJoystick.color.g, leftJoystick.color.b, 1/255);        
        rightJoystick.color = new Color(rightJoystick.color.r, rightJoystick.color.g, rightJoystick.color.b, 1/255);
        MakeJoystickIconsTransparent();
    }

    private void MakeJoystickIconsTransparent()
    {
        leftJoystickIcon.color = new Color(leftJoystickIcon.color.r, leftJoystickIcon.color.g, leftJoystickIcon.color.b, 1/255); 
        rightJoystickIcon.color = new Color(rightJoystickIcon.color.r, rightJoystickIcon.color.g, rightJoystickIcon.color.b, 1/255);

    }

    private void MakeJoystickIconsVisible()
    {
        leftJoystickIcon.color = new Color(leftJoystickIcon.color.r, leftJoystickIcon.color.g, leftJoystickIcon.color.b, 255);        
        rightJoystickIcon.color = new Color(rightJoystickIcon.color.r, rightJoystickIcon.color.g, rightJoystickIcon.color.b, 255);

    }

    private void MakeJoysticksVisible()
    {
        leftJoystick.color = new Color(leftJoystick.color.r, leftJoystick.color.g, leftJoystick.color.b, 255);        
        rightJoystick.color = new Color(rightJoystick.color.r, rightJoystick.color.g, rightJoystick.color.b, 255);
    }

    private void SetBigJoystick()
    {
        leftJoystick.GetComponent<RectTransform>().sizeDelta = new Vector2(bigJoystickSize, bigJoystickSize);
        rightJoystick.GetComponent<RectTransform>().sizeDelta = new Vector2(bigJoystickSize, bigJoystickSize);
    }    private void SetSmallJoystick()
    {
        leftJoystick.GetComponent<RectTransform>().sizeDelta = new Vector2(smallJoystickSize, smallJoystickSize);
        rightJoystick.GetComponent<RectTransform>().sizeDelta = new Vector2(smallJoystickSize, smallJoystickSize);
    }

}
