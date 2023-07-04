using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCanger : MonoBehaviour
{
    [SerializeField] private Transform objectTransform;
    [SerializeField] private Vector3 firstPosition;
    [SerializeField] private Vector3 firstRotation;
    [SerializeField] private Vector3 secondPosition;
    [SerializeField] private Vector3 secondRotation;

    private bool _onFirstPosition=true;
    public void ChangePosition()
    {
        if (_onFirstPosition)
        {
            objectTransform.localPosition = secondPosition;
            objectTransform.eulerAngles = secondRotation;
        }
        else
        {
            objectTransform.localPosition = firstPosition;
            objectTransform.eulerAngles = firstRotation;
        }

        _onFirstPosition = !_onFirstPosition;
    }
}
