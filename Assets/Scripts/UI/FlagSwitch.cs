using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagSwitch : MonoBehaviour
{
    [SerializeField] private Image flagStateImage;
    [SerializeField] private Transform flagsHolder;

    [SerializeField] private Sprite flagOn;
    [SerializeField] private Sprite flagOff;
    [SerializeField] private Sprite flag3DFlag;
    private FlagType _flagType=FlagType.Off;
    private FlagType _lastType=FlagType.Off;

    
    public figureMover[] _figureMovers;
    private enum FlagType
    {
        On,
        Off,
        Flag3D
    }

    public void AddFigureMover(List<figureMover> newFigureMovers)
    {
        _figureMovers=newFigureMovers.ToArray();
    }

    public void SwitchToOff()
    {
        _flagType = FlagType.Flag3D;
        SwitchFlag();
    }

    public void SwitchTo2DView()
    {
        _lastType = _flagType;
        for (int i = 0; i < 3; i++)
        {
            if (_flagType == FlagType.On)
            {
                return;
            }
            SwitchFlag();
        }
        
    }
    public void SwitchTo3DView()
    {
        for (int i = 0; i < 3; i++)
        {
            if (_flagType == _lastType)
            {
                return;
            }
            SwitchFlag();
        }
        
    }

    // public void SetFlagsFrame(bool active)
    // {
    //     foreach (FLag2d fLag2d in flagsHolder.GetComponentsInChildren<FLag2d>())
    //     {
    //         fLag2d.SetFlagFrame(active);
    //     }
    // }
    public void SwitchFlag()
    {

        if (_flagType==FlagType.Flag3D)
        {
             _flagType = FlagType.Off;
            foreach (var figureMover in _figureMovers)
            {
                if (figureMover)
                {
                    
                    figureMover.HideFlag();
                }
                else
                {
                   // _figureMovers.Remove(figureMover);
                }
            }
            //onOffText.text = "OFF";
            flagStateImage.sprite = flagOff;
        }
        else if(_flagType==FlagType.Off)
        {
             _flagType = FlagType.On;
            flagStateImage.sprite = flagOn;

            flagsHolder.gameObject.SetActive(true);
        }
        else
        {

             _flagType = FlagType.Flag3D;

            foreach (var figureMover in _figureMovers)
            {
                
                if (figureMover)
                {
                    figureMover.ShowFlag();
                }
                else
                {
                    //_figureMovers.Remove(figureMover);
                }
            }
            //onOffText.text = "FLAG";
            flagStateImage.sprite = flag3DFlag;

            flagsHolder.gameObject.SetActive(false);
        }

    }
}
