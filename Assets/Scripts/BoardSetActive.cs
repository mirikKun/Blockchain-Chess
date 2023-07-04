using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSetActive : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectOfType<DontDestroyOnLoad>())
        {
            FindObjectOfType<DontDestroyOnLoad>().board.SetActive(true);
        }
    }

  
}
