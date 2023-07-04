using System.Collections;
using System.Collections.Generic;
using BzKovSoft.ObjectSlicer;
using UnityEngine;

public class BishopFatality : FigureCuttingFatality
{

    [SerializeField] private Transform katana;
    private float _timeOfHot1 = 1.3f;
    [SerializeField] private GameObject blood1;
    [SerializeField] private GameObject bloodDecal1;

    private float _timeOfHot2 = 2.73f;
    [SerializeField] private GameObject blood2;

    private float _timeOfHot3 = 4.08f;
    [SerializeField] private GameObject blood3;
    [SerializeField] private GameObject bloodDecal2;
    [SerializeField] private GameObject bloodDecal3;

    private float _timeOfHot4 = 5.58f;
    [SerializeField] private GameObject blood4;


    private static readonly int FatalityNaginata052_death = Animator.StringToHash("FatalityNaginata052_death");
    private static readonly int FatalityNaginata052_attack = Animator.StringToHash("FatalityNaginata052_attack");
    private float fatalityTime = 8.5f;
    private float timeToCutOff = 5.75f;
    private float timeToFall = 7.2f;

    public override IEnumerator FatalityAnimation(figureMover target)
    {
        _anim.SetTrigger(FatalityNaginata052_attack);

        _anim.speed = 0;
        //yield return new WaitForSeconds(1.5f);
        _anim.speed = 1;

        yield return new WaitForSeconds(fatalityTime);
    }

    public override IEnumerator FigureFatalityDying(bool inFatality,FigureFatality opponent)
    {
      if(slicePlane)
            {
            
            characterSlicerSampleFast.dawnPartMain = true;

            animSliceConfiguration.NewAnimSliceConfiguration(AnimSliceType.Ragdoll, AnimSliceType.Ragdoll, new Vector3(0,0,-0), Vector3.zero,rootBone,childBone,true);

            foreach (var curKeepOneSecond in keepOneSecond)
            {
                curKeepOneSecond.SliceType = SliceType.KeepOneSecond;
            }
            foreach (var curKeepOneFirst in keepOneFirst)
            {
                curKeepOneFirst.SliceType = SliceType.KeepOne;
            }
            _anim.SetTrigger(FatalityNaginata052_death);
            _anim.speed = 0;
            //yield return new WaitForSeconds(1.5f);
            _anim.speed = 1;
            if (inFatality&&GameSettingsScript.BloodEnable)
            {
                StartCoroutine(BishopHits());
                StartCoroutine(RookSlice());
                StartCoroutine(DetuchKatana());
            }
        }
        else
        {
            
            _anim.SetTrigger(FatalityNaginata052_death);
            _anim.speed = 0;
            //yield return new WaitForSeconds(1.5f);
            _anim.speed = 1;
            if (inFatality&&GameSettingsScript.BloodEnable)
            {
                StartCoroutine(BishopHits());
            }
        }




        yield return new WaitForSeconds(fatalityTime);
    }

    public override void TurnOffAnimation()
    {
    }

    private IEnumerator DetuchKatana()
    {
        yield return new WaitForSeconds(timeToCutOff-1.2f);
        if(katana)
        {
           // katana.parent = transform.parent;
            
            //katana.SetParent(transform.parent,true);
            //katana.Set
        } 
    }
    private IEnumerator RookSlice()
    {
        yield return new WaitForSeconds(timeToCutOff);

        characterSlicerSampleFast.PlaneSlice(slicePlane);
        
        while (!transform.parent.Find(gameObject.name + "_pos"))
        {
            yield return null;
        }
        Transform cuttedTransform = transform.parent.Find(gameObject.name + "_pos");
        
        cuttedTransform.Find("Blood").gameObject.SetActive(false);
        yield return new WaitForSeconds(timeToFall-timeToCutOff);
        foreach (Transform child in transform.parent)
        {
            child.GetComponent<figureMover>().DisablePhysic();
        }

    }
    private IEnumerator BishopHits()
    {
        yield return new WaitForSeconds(_timeOfHot1);
        if (blood1)
            blood1.SetActive(true);
        if (bloodDecal1)
            bloodDecal1.SetActive(true);

        yield return new WaitForSeconds(_timeOfHot2 - _timeOfHot1);
        if (blood2)
            blood2.SetActive(true);
        yield return new WaitForSeconds(_timeOfHot3 - _timeOfHot2);
        if (blood3)
            blood3.SetActive(true);
        if (bloodDecal2)
            bloodDecal2.SetActive(true);
        if (bloodDecal3)
            bloodDecal3.SetActive(true);
        yield return new WaitForSeconds(_timeOfHot4 - _timeOfHot3);
        if (blood4)
            blood4.SetActive(true);
    }

    public override void SetCuttingParameters(Transform[] cuttingParameters)
    {
        rootBone = cuttingParameters[0];
        childBone = cuttingParameters[1];
        katana = cuttingParameters[2];
    }

    public override Transform[] GetCuttingParameters()
    {
        return new[] {rootBone, childBone,katana};
    }
}