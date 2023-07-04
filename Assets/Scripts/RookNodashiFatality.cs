using System.Collections;
using System.Collections.Generic;
using BzKovSoft.CharacterSlicer.Samples;
using BzKovSoft.ObjectSlicer;
using BzKovSoft.ObjectSlicer.Samples.Scripts;
using UnityEngine;

public class RookNodashiFatality : FigureCuttingFatality
{
    private float _timeOfHot1 = 2f;
    [SerializeField] private Transform katanaHolder;
    [SerializeField] private GameObject blood1;
    [SerializeField] private GameObject bloodDecal1;

    private float _timeOfHot2 = 2.73f;
    [SerializeField] private GameObject blood2;

    private float _timeOfHot3 = 4.14f;
    [SerializeField] private GameObject blood3;
    [SerializeField] private GameObject bloodDecal2;
    
    private float _timeOfHot4 = 5.43f;
    [SerializeField] private GameObject blood4;
    [SerializeField] private GameObject cutOfBlood5;
    private static readonly int FatalityNodashiRook_attack = Animator.StringToHash("FatalityNodashiRook_attack");
    private static readonly int FatalityNodashiRook_death= Animator.StringToHash("FatalityNodashiRook_death");
    private float fatalityTime = 7.7f;
    private float timeToCutOff = 5.2f;
    private float timeToFall = 6.65f;

    
    public override IEnumerator FatalityAnimation( figureMover target)
    {
        _anim.SetTrigger(FatalityNodashiRook_attack);

        _anim.speed = 0;
       // yield return new WaitForSeconds(0.5f);
        _anim.speed = 1;

        yield return new WaitForSeconds(fatalityTime);

    }
    public override void TurnOffAnimation()
    {

    }


    public override IEnumerator FigureFatalityDying(bool inFatality,FigureFatality opponent)
    {
        
         if(slicePlane)
        {
            animSliceConfiguration.NewAnimSliceConfiguration(AnimSliceType.Ragdoll, AnimSliceType.Ragdoll, new Vector3(0,-0.2f,-0.9f), Vector3.zero,rootBone,childBone,true);
            characterSlicerSampleFast.dawnPartMain = true;

            _anim.SetTrigger(FatalityNodashiRook_death);
            foreach (var curKeepOneSecond in keepOneSecond)
            {
                curKeepOneSecond.SliceType = SliceType.KeepOneSecond;
            }
            foreach (var curKeepOneSecond in keepOneSecond)
            {
                curKeepOneSecond.SliceType = SliceType.KeepOneSecond;
            }
            _anim.speed = 0;
           // yield return new WaitForSeconds(0.5f);  
            if (inFatality&&GameSettingsScript.BloodEnable)
            {
                StartCoroutine(RookHits());
                StartCoroutine(RookSlice());
            }
            yield return new WaitForSeconds(0.5f);  

            _anim.speed = 1f;
            
        }
        else
        {
            _anim.SetTrigger(FatalityNodashiRook_death);
       
            _anim.speed = 0;
            //yield return new WaitForSeconds(0.5f);  
            if (inFatality&&GameSettingsScript.BloodEnable)
            {
                StartCoroutine(RookHits());
            }
            yield return new WaitForSeconds(0.5f);  

            _anim.speed = 1f;
        }

        yield return new WaitForSeconds(fatalityTime);
    }

    private IEnumerator RookSlice()
    {
        yield return new WaitForSeconds(timeToCutOff);
        if (katanaHolder)
        {
            katanaHolder.parent = transform.parent;
        }
        characterSlicerSampleFast.PlaneSlice(slicePlane);
        
        while (!transform.parent.Find(gameObject.name + "_pos"))
        {
            yield return null;
        }
        Transform cuttedTransform = transform.parent.Find(gameObject.name + "_pos").Find("Root_M");

        foreach (var newCollider in cuttedTransform.GetComponentsInChildren<Collider>(true))
        {
            newCollider.enabled=true;
        }
        yield return new WaitForSeconds(timeToFall-timeToCutOff);
        foreach (figureMover child in transform.parent.GetComponentsInChildren<figureMover>())
        {
            child.DisablePhysic();
        }


    }
    private IEnumerator RookHits()
    {

        yield return new WaitForSeconds(_timeOfHot1);
        if(blood1)
            blood1.SetActive(true);
        if(bloodDecal1)
            bloodDecal1.SetActive(true);

        yield return new WaitForSeconds(_timeOfHot2-_timeOfHot1);
        if(blood2)
            blood2.SetActive(true);
        yield return new WaitForSeconds(_timeOfHot3-_timeOfHot2);
        if(blood3)
            blood3.SetActive(true);
        if(bloodDecal2)
            bloodDecal2.SetActive(true);
        yield return new WaitForSeconds(_timeOfHot4-_timeOfHot3);
        if(blood4)
            blood4.SetActive(true);
    }

    private IEnumerator RookCutting()
    {

        yield return new WaitForSeconds(timeToCutOff);
        if(cutOfBlood5)
            cutOfBlood5.SetActive(true);
    }
    public override void SetCuttingParameters(Transform[] cuttingParameters)
    {
        rootBone = cuttingParameters[0];
        childBone = cuttingParameters[1];
        katanaHolder = cuttingParameters[2];
    }

    public override Transform[] GetCuttingParameters()
    {
        return new[] {rootBone, childBone,katanaHolder};
    }
}
