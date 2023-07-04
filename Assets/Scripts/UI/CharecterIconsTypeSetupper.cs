using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharecterIconsTypeSetupper : MonoBehaviour
{
    [SerializeField] private Transform flagHolder;
    
    public void ChangeIcons()
    {

        FLag2d[] flags = flagHolder.GetComponentsInChildren<FLag2d>(true);
        foreach (var flag in flags)
        {

            flag.ChangeIconToCurrent();
        }
    }
}
