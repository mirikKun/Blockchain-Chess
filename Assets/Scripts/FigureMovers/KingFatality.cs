using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingFatality : FigureFatality
{
    [SerializeField] private GameObject[] blood;
    private float _delayToHit = 5.3f;
    private static readonly int Fatality055_death = Animator.StringToHash("Fatality055_death");
    private static readonly int Fatality055_attack = Animator.StringToHash("Fatality055_attack");
    private float fatalityTime = 13.5f;
    public override IEnumerator FatalityAnimation( figureMover target)
    {
       // yield return new WaitForSeconds(1);

        _anim.SetTrigger(Fatality055_attack);
        yield return new WaitForSeconds(fatalityTime);
    }

    public override IEnumerator FigureFatalityDying(bool inFatality,FigureFatality opponent)
    {
      //  yield return new WaitForSeconds(1);

        if(inFatality&&GameSettingsScript.BloodEnable)
        {
            StartCoroutine(KingHit());
            StartCoroutine(EnablePhysic());
            
        }        _anim.SetTrigger(Fatality055_death);
        yield return new WaitForSeconds(fatalityTime);
    }

    public override void TurnOffAnimation()
    {
        
    }

    private IEnumerator KingHit()
    {
        yield return new WaitForSeconds(_delayToHit);
        foreach (var newBlood in blood)
        {
            if (newBlood)
                newBlood.SetActive(true);
        }
    }    
    private IEnumerator EnablePhysic()
    {
        if (GetComponentInParent<FatalityController>().physicDisabled)
        {
            yield break;
        }
        yield return new WaitForSeconds(8.13f);
        GetComponent<figureMover>().SwitchToPhysics();
    }
}
