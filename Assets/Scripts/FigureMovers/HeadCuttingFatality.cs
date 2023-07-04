using System.Collections;
using System.Collections.Generic;
using BzKovSoft.ObjectSlicer;
using UnityEngine;

public class HeadCuttingFatality : FigureCuttingFatality
{
    [SerializeField] private Transform attackerHand;
    [SerializeField] private  Transform headBone;
    //private Vector3 headPosition=new Vector3(0.142f,-0.028f,0.229f);
    private Vector3 headPosition=new Vector3(0.0931f,0.0085f,0.186f);
    private float fatalityTime = 7.8f;

    [SerializeField] private GameObject bloodDecal1;
    [SerializeField] private GameObject blood1;
    private float hitTime1 = 1.44f;
    [SerializeField] private GameObject bloodDecal2;
    [SerializeField] private GameObject blood2;
    private float hitTime2 = 2.92f;
    [SerializeField] private GameObject blood3;
    private float hitTime3 = 3.183f;
    [SerializeField] private GameObject blood4;
    private float hitTime4 = 5.3f;
    
    
    private float timeToHeadCutting = 2.92f;

    private float attackSpeedMultiplayer = 1.1f;

    private static readonly int HeadCuttingDeath = Animator.StringToHash("HeadCuttingDeath");
    private static readonly int HeadCuttingAttack = Animator.StringToHash("HeadCuttingAttack");


    public override IEnumerator FatalityAnimation(figureMover target)
    {        
        _anim.SetTrigger(HeadCuttingAttack);
        yield return null;

        _anim.speed = 0;
        yield return new WaitForSeconds(0.833f-0.4167f);
        _anim.speed = 1;

        yield return new WaitForSeconds(fatalityTime);
    }

    public Transform GetHand()
    {
        return attackerHand;
    }
    
    public override IEnumerator FigureFatalityDying(bool inFatality,FigureFatality opponent)
    {
        //_dying = true;
        Debug.Log("________________");
        if(slicePlane)
        {
            characterSlicerSampleFast.dawnPartMain = true;
            animSliceConfiguration.NewAnimSliceConfiguration(AnimSliceType.Animation, AnimSliceType.Object, new Vector3(-2,0.4f,1), Vector3.zero,rootBone,childBone,true);
           
            foreach (var curKeepOneSecond in keepOneSecond)
            {
                curKeepOneSecond.SliceType = SliceType.KeepOneSecond;
            }
            foreach (var curKeepOneFirst in keepOneFirst)
            {
                curKeepOneFirst.SliceType = SliceType.KeepOne;
            }
            _anim.SetTrigger(HeadCuttingDeath);

            _anim.speed = 0;
            //yield return new WaitForSeconds(0.4167f);
            _anim.speed = 1;
            yield return new WaitForSeconds(0.5f-0.4167f);

            if (inFatality)
            {
                if(GameSettingsScript.BloodEnable)
                {
                    StartCoroutine(FigureFatalityHits());
                } 
                StartCoroutine(RookSlice(opponent));
            }
        }
        
        yield return new WaitForSeconds(fatalityTime);
    }

    private IEnumerator RookSlice(FigureFatality opponent)
    {
        yield return new WaitForSeconds(timeToHeadCutting*attackSpeedMultiplayer);
        characterSlicerSampleFast.PlaneSlice(slicePlane);
        Debug.Log("________-");
        HeadCuttingFatality opponentHeadCuttingFatality = opponent.GetComponent<HeadCuttingFatality>();

        while (!transform.parent.Find(gameObject.name + "_pos"))
        {
            Debug.Log("333-");

            yield return null;
        }
        Transform cuttedTransform = transform.parent.Find(gameObject.name + "_pos");
        HeadCuttingFatality cuttedHeadCuttingFatality = cuttedTransform.GetComponent<HeadCuttingFatality>();


        if (cuttedTransform&&opponentHeadCuttingFatality&&opponentHeadCuttingFatality.GetHand())
        {
            cuttedTransform.Find("Blood").gameObject.SetActive(false);

            cuttedTransform.parent = opponentHeadCuttingFatality.GetHand();
            
            //yield return null;
            cuttedTransform.localEulerAngles = new Vector3(-21, -130, 102);
            cuttedTransform.position +=-cuttedHeadCuttingFatality.headBone.position+ opponentHeadCuttingFatality.GetHand().position;
            cuttedTransform.localPosition += headPosition;
            //cuttedTransform.localPosition =headPosition;
            //cuttedTransform.localEulerAngles = headRotation;
        }
    }
    public override void TurnOffAnimation()
    {
    }

    private IEnumerator FigureFatalityHits()
    {
        yield return new WaitForSeconds(hitTime1*attackSpeedMultiplayer);
      
        if (blood1)
        {
            blood1.SetActive(true);
        }         
        if(bloodDecal1)
        {
            bloodDecal1.SetActive(true);
        }

        var delay = hitTime2 - hitTime1;
        yield return new WaitForSeconds(delay*attackSpeedMultiplayer);
      
        if (blood2)
        {
            blood2.SetActive(true);
        }   
        if(bloodDecal2)
        {
            bloodDecal2.SetActive(true);
        }
        delay = hitTime3 - hitTime2;
        yield return new WaitForSeconds(delay*attackSpeedMultiplayer);
      
        if (blood3)
        {
            blood3.SetActive(true);
        }          
        delay = hitTime4 - hitTime3;

        yield return new WaitForSeconds(delay*attackSpeedMultiplayer);
      
        if (blood4)
        {
            blood4.SetActive(true);
        }   
        
    }
    public override void SetCuttingParameters(Transform[] cuttingParameters)
    {
        rootBone = cuttingParameters[0];
        childBone = cuttingParameters[1];
        headBone = cuttingParameters[2];
    }

    public override Transform[] GetCuttingParameters()
    {
        return new[] {rootBone, childBone,headBone};
    }
  
}
