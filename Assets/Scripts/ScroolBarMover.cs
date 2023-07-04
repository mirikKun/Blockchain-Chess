using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScroolBarMover : MonoBehaviour
{
    [SerializeField] private Transform contentTransform;
    [SerializeField] private float step;
    public  void GoRight()
    {
        contentTransform.position-=Vector3.right*step;
    }    
    public  void GoLeft()
    {
        contentTransform.position+=Vector3.right*step;
    }
}
