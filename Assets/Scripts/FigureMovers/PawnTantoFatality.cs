using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnTantoFatality : FigureFatality
{
    [SerializeField] private GameObject[] blood;
    private float _delayToHit = 0.9f;
    private static readonly int FatalityTanto_attack = Animator.StringToHash("FatalityTanto_attack");
    private static readonly int FatalityTanto_death = Animator.StringToHash("FatalityTanto_death");
    private float fatalityTime = 3.3f;
    private float attackAnimDelay = 0.33f;
    private float attackAnimTime = 2.6f;

    public override IEnumerator FatalityAnimation(figureMover target)
    {  
        //yield return new WaitForSeconds(1);

        yield return new WaitForSeconds(attackAnimDelay);
        _anim.SetBool(FatalityTanto_attack, true);
        yield return new WaitForSeconds(attackAnimTime);
        _anim.SetBool(FatalityTanto_attack, false);
        yield return new WaitForSeconds(fatalityTime-attackAnimTime);
    }
    public override void TurnOffAnimation()
    {
        _anim.SetBool(FatalityTanto_attack, false);

    }
    public override IEnumerator FigureFatalityDying(bool inFatality,FigureFatality opponent)
    {
        //yield return new WaitForSeconds(1);

        _anim.SetBool(FatalityTanto_death,true);
        if(inFatality&&GameSettingsScript.BloodEnable)
            StartCoroutine(PawnHit());
        yield return new WaitForSeconds(fatalityTime);
    }

    private IEnumerator PawnHit()
    {
        yield return new WaitForSeconds(_delayToHit);
        foreach (var newBlood in blood)
        {
            if(newBlood)
                newBlood.SetActive(true);
        }
    }
}
