using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class FigureFatality:MonoBehaviour
{
   protected Animator _anim;
   public string FatalityName;
   public abstract IEnumerator FatalityAnimation(figureMover target);
   public abstract IEnumerator FigureFatalityDying(bool inFatality,FigureFatality opponent);

   public abstract void TurnOffAnimation();
   
   protected void Awake()
   {
      _anim = GetComponent<Animator>();
   }


}
