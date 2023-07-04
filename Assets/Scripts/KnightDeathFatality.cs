using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightDeathFatality : FigureFatality
{
    [SerializeField] private GameObject[] blood;
    [SerializeField] private GameObject[] defaultArcher;
    [SerializeField] private Animator deadRiderAnimator;
    [SerializeField] private bool bishop;

    private static readonly int Attack = Animator.StringToHash("FatalityAttackOnKnight");
    private static readonly int Dying = Animator.StringToHash("Knight_death");
    private float timeToSlowMotion = 0.64f;
    private float timeInSlowMotion = 0.8f;
    private float speedMultiplayer=0.1f;
    private float fatalityTime = 5.5f;
    private float _delayToHit = 1.32f;

    

    public override IEnumerator FatalityAnimation(figureMover target)
    {
        Debug.Log("_____-11");
        if(bishop&&transform.parent.tag!="Board")
        {
            transform.parent.localEulerAngles = new Vector3(0, 81, 0);
            transform.parent.localPosition += -0.25f*Vector3.right;
            yield return new WaitForSeconds(0.2f);
        }
        
        yield return new WaitForSeconds(0.5f);
        _anim.SetTrigger(Attack);
        if (bishop)
        {
            yield return new WaitForSeconds(0.2f);
            _anim.speed = 0.3f;

        }
        else
        {
            yield return new WaitForSeconds(timeToSlowMotion);
            _anim.speed = speedMultiplayer;

        }
        
       
        yield return new WaitForSeconds(timeInSlowMotion);
    
        _anim.speed = 0.6f;

        yield return new WaitForSeconds(fatalityTime-timeInSlowMotion-timeToSlowMotion-1f);
        _anim.speed = 1f;

    }


    public override void TurnOffAnimation()
    {
        _anim.speed = 1f;

        if(deadRiderAnimator)
        {
            deadRiderAnimator.SetTrigger(Attack);
        }
    }

    public override IEnumerator FigureFatalityDying(bool inFatality,FigureFatality opponent)
    
    {
        foreach (var element in defaultArcher)
        {
            element.SetActive(false);
        }
        deadRiderAnimator.gameObject.SetActive(true);
        deadRiderAnimator.SetTrigger(Dying);
        deadRiderAnimator.speed = 0;

        yield return new WaitForSeconds(0.5f);

        
        deadRiderAnimator.speed = 1;

        yield return new WaitForSeconds(timeToSlowMotion);
        deadRiderAnimator.speed = speedMultiplayer;
        yield return new WaitForSeconds(timeInSlowMotion);
        deadRiderAnimator.speed = 1;
        yield return new WaitForSeconds(_delayToHit-timeInSlowMotion-timeToSlowMotion);
        
        if (inFatality  &&GameSettingsScript.BloodEnable)
        {
            foreach (var newBlood in blood)
            {
                newBlood.SetActive(true);
            }
        }
        yield return new WaitForSeconds(fatalityTime - _delayToHit);
    }
}
