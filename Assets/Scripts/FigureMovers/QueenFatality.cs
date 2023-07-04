using System;
using System.Collections;
using System.Collections.Generic;
using BzKovSoft.ObjectSlicer;
using UnityEngine;

public class QueenFatality : FigureCuttingFatality
{
    [SerializeField] private GameObject legBlood;
    [SerializeField] private GameObject legBloodDecal;
    private float legCutTime = 0.62f;

    [SerializeField] private GameObject[] blood1;
    private float headHitTime1 = 1.9f;

    [SerializeField] private GameObject blood2;
    private float fatalityTime = 7.2f;

    private float headHitTime2 = 3.05f;


    private static readonly int Fatality051_death = Animator.StringToHash("Fatality051_death");
    private static readonly int Fatality051_attack = Animator.StringToHash("Fatality051_attack");


    public override IEnumerator FatalityAnimation(figureMover target)
    {
        _anim.SetTrigger(Fatality051_attack);
        yield return null;

        _anim.speed = 0;
        //yield return new WaitForSeconds(0.8f);
        yield return null;

        //yield return new WaitForSeconds(1);
        _anim.speed = 1;

        yield return new WaitForSeconds(fatalityTime);
    }

    public override IEnumerator FigureFatalityDying(bool inFatality, FigureFatality opponent)
    {
        //_dying = true;

        if (slicePlane)
        {
            
            foreach (var rightLegBzSlice in keepOneSecond)
            {
                rightLegBzSlice.SliceType = SliceType.KeepOneSecond;
            }            
            foreach (var leftLegBzSlice in keepOneFirst)
            {
                leftLegBzSlice.SliceType = SliceType.KeepOne;
            }
            animSliceConfiguration.NewAnimSliceConfiguration(AnimSliceType.Ragdoll, AnimSliceType.Animation,
                new Vector3(-2.8f, 0.6f, 2.8f), Vector3.zero, rootBone, childBone, false);
            

            _anim.SetTrigger(Fatality051_death);

            _anim.speed = 0;
            //yield return new WaitForSeconds(0.8f);
            yield return null;
            _anim.speed = 1;

            if (inFatality&&GameSettingsScript.BloodEnable)
            {
                StartCoroutine(FigureFatalityHeadHit1());
                StartCoroutine(FigureFatalityHeadHit2());
                StartCoroutine(LegSlice());
                StartCoroutine(EnablePhysic());
            }
        }
        else
        {
            _anim.SetTrigger(Fatality051_death);

            _anim.speed = 0;
            yield return new WaitForSeconds(1);
            _anim.speed = 1;

            if (inFatality&&GameSettingsScript.BloodEnable)
            {
                StartCoroutine(FigureFatalityHeadHit1());
                StartCoroutine(FigureFatalityHeadHit2());
            }
        }


        yield return new WaitForSeconds(fatalityTime);
    }

    private IEnumerator LegSlice()
    {
        yield return new WaitForSeconds(legCutTime);
        if (legBloodDecal)
            legBloodDecal.SetActive(true);
        characterSlicerSampleFast.PlaneSlice(slicePlane);
        if (legBlood)
            legBlood.SetActive(true);
    }

    public override void TurnOffAnimation()
    {
    }

    private IEnumerator FigureFatalityLegCut()
    {
        yield return new WaitForSeconds(legCutTime);
        if (legBlood)
            legBlood.SetActive(true);
        if (legBloodDecal)
            legBloodDecal.SetActive(true);
    }

    private IEnumerator FigureFatalityHeadHit1()
    {
        yield return new WaitForSeconds(headHitTime1);
        foreach (var blood in blood1)
        {
            if (blood)
                blood.SetActive(true);
        }
    }

    private IEnumerator FigureFatalityHeadHit2()
    {
        yield return new WaitForSeconds(headHitTime2);
        if (transform.parent.childCount > 1)
        {
            //Debug.Log(transform.parent.GetChild(1).gameObject);
            transform.parent.GetChild(1).GetComponent<figureMover>().DisablePhysic();
        }
        
        if (blood2)
            blood2.SetActive(true);
    }

    public override void SetCuttingParameters(Transform[] cuttingParameters)
    {
        rootBone = cuttingParameters[0];
        childBone = cuttingParameters[1];
    }

    public override Transform[] GetCuttingParameters()
    {
        return new[] {rootBone, childBone};
    }

    private IEnumerator EnablePhysic()
    {
        if (GetComponentInParent<FatalityController>().physicDisabled)
        {
            yield break;
        }
        yield return new WaitForSeconds(5.3f);
        GetComponent<figureMover>().SwitchToPhysics();
    }
}