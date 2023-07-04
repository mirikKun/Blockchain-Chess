using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FLag2d : MonoBehaviour
{
    [SerializeField] private IconsSet[] iconsSets;

    [System.Serializable]
    public class IconsSet
    {
        public Sprite[] icons;
    }

    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private Color redColor;
    [SerializeField] private Color blackColor;
    [SerializeField] private RectTransform iconRectTransform;
    private int _lastFigureIndex;
    private int _colorIndex;


  
    public void SetColor(int colorIndex)
    {
        _colorIndex = colorIndex;
        background.color = redColor;
    }
    public void SetupNewIcon(int figureIndex)
    {
        _lastFigureIndex = figureIndex;

        SetupCurIconSprite();

        CheckSize();
    }

    public void SetupCurIconSprite()
    {
 
        icon.sprite = iconsSets[(int) GameSettingsScript.CurIconType].icons[_lastFigureIndex + 6*_colorIndex];
        
    }

    public void ChangeIconToCurrent()
    {
        SetupCurIconSprite();
        CheckSize();
    }

    public void SetDefaultIconRotation()
    {
        icon.transform.localEulerAngles = new Vector3(0, 0, 0);
        Vector3 lastRotation = icon.transform.eulerAngles;

        if (GameManager.WhiteSide)
        {
            icon.transform.eulerAngles = new Vector3(270, lastRotation.y, lastRotation.z);
        }
        else
        {
            icon.transform.eulerAngles = new Vector3(90, lastRotation.y, lastRotation.z);
        }
    }

    public void CheckSize()
    {
        iconRectTransform.sizeDelta = new Vector2(1.55f, 1.55f);
    }

    public IEnumerator FlagMoving(Vector3 newPos,bool run,float scaleMultiplayer)
    {
        while (Vector3.Distance(newPos, transform.position) > 0.01f)
        {

            yield return null;
            float step;
            if (run)
            {
                step = 2.9f* Time.deltaTime;
            }
            else
            {
                step = 1.2f * Time.deltaTime;
            }

            step = step * scaleMultiplayer;
            transform.position = Vector3.MoveTowards(transform.position, newPos, step);
        }
    }
}