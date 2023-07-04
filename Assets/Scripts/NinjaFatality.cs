using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaFatality : FigureFatality
{

    private float _timeOfHot1 = 1.75f;
    [SerializeField] private GameObject blood1;
    [SerializeField] private GameObject bloodDecal1;

    private float _timeOfHot2 = 2.4f;
    [SerializeField] private GameObject blood2;
    [SerializeField] private GameObject bloodDecal2;

    private float _timeOfHot3 = 2.911f;
    [SerializeField] private GameObject blood3;

    private float _timeOfHot4 = 3.53f;
    [SerializeField] private GameObject blood4;


    private static readonly int FatalityTanto_death = Animator.StringToHash("FatalityTantoDeath");
    private static readonly int FatalityTanto_attack = Animator.StringToHash("FatalityTantoAttack");
    private float fatalityTime = 7.5f;

    private float _speedMultiplayer=0.8f;
    public override IEnumerator FatalityAnimation(figureMover target)
    {
        _anim.SetTrigger(FatalityTanto_attack);

        _anim.speed = 0;
        //yield return new WaitForSeconds(1.2f);
        yield return new WaitForSeconds(0.4333f);
        _anim.speed = 1/_speedMultiplayer;

        yield return new WaitForSeconds(fatalityTime*_speedMultiplayer);
        _anim.speed = 1;

    }

    public override IEnumerator FigureFatalityDying(bool inFatality,FigureFatality opponent)
    {
    
        _anim.SetTrigger(FatalityTanto_death);
        _anim.speed = 0;
        //yield return new WaitForSeconds(1.2f);
        _anim.speed = 1/(_speedMultiplayer);
        

        if (inFatality&&GameSettingsScript.BloodEnable)
        {
            StartCoroutine(NinjaHits());
        }


        yield return new WaitForSeconds(fatalityTime);
    }

    public override void TurnOffAnimation()
    {
        _anim.speed = 1;

    }

    private IEnumerator NinjaHits()
    {
        yield return new WaitForSeconds(_timeOfHot1*_speedMultiplayer);
        if (blood1)
            blood1.SetActive(true);
        if (bloodDecal1)
            bloodDecal1.SetActive(true);

        yield return new WaitForSeconds((_timeOfHot2 - _timeOfHot1)*_speedMultiplayer);
        if (blood2)
            blood2.SetActive(true);
        yield return new WaitForSeconds((_timeOfHot3 - _timeOfHot2)*_speedMultiplayer);
        if (blood3)
            blood3.SetActive(true);
        if (bloodDecal2)
            bloodDecal2.SetActive(true);
        yield return new WaitForSeconds((_timeOfHot4 - _timeOfHot3)*_speedMultiplayer);
        if (blood4)
            blood4.SetActive(true);
    }

}
