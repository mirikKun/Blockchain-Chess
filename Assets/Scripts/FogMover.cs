using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogMover : MonoBehaviour
{
    [SerializeField] private float fogSpeed=7;

    [SerializeField] private RectTransform[] childFogTransforms;

 
    private void Update()
    {
        Vector2 offset=   fogSpeed*Time.deltaTime *Vector2.right;
        foreach (var childFogTransform in childFogTransforms)
        {
            if (childFogTransform.anchoredPosition.x >= childFogTransform.sizeDelta.x * childFogTransforms.Length/2f)
            {
                childFogTransform.anchoredPosition = new Vector2(childFogTransform.anchoredPosition.x-childFogTransforms.Length*childFogTransform.sizeDelta.x, childFogTransform.anchoredPosition.y);
            }

            childFogTransform.anchoredPosition +=   offset;
        }
    }
}