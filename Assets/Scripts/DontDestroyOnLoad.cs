using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public GameObject board;
    void Awake()
    {
        DontDestroyOnLoad[] objs = FindObjectsOfType<DontDestroyOnLoad>();

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    
    }
}
