using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DevLog : MonoBehaviour
{
   
   private static DevLog Inst;
   private void Awake()
   {
      if(!Inst)
      {
         Inst = this;
         DontDestroyOnLoad(gameObject);
      }
      else
      {
         Destroy(gameObject);
      }
   }

   private void Update()
   {
      
      Debug.developerConsoleVisible = true;
      
   }
}
