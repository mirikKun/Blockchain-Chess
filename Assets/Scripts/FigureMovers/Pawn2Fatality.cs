using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn2Fatality : FigureFatality
{
    [SerializeField] private GameObject[] blood;
    [SerializeField] private Rigidbody weapon;

    private float _delayToHit = 0.79f;
    private static readonly int Katana006 = Animator.StringToHash("Katana006");
    private static readonly int Katana010_right = Animator.StringToHash("Katana010_right");
    private float fatalityTime = 3.3f;
    private float attackAnimDelay = 0.33f;
    private float timeToWeaponDrop = 1.137f;

    private float attackAnimTime = 2.6f;

    public override IEnumerator FatalityAnimation(figureMover target)
    {  
        yield return new WaitForSeconds(1);

        yield return new WaitForSeconds(attackAnimDelay);
        _anim.SetBool(Katana010_right, true);
        yield return new WaitForSeconds(attackAnimTime);
        _anim.SetBool(Katana010_right, false);
        yield return new WaitForSeconds(fatalityTime-attackAnimTime);
    }
    public override void TurnOffAnimation()
    {
        _anim.SetBool(Katana010_right, false);

    }
    public override IEnumerator FigureFatalityDying(bool inFatality,FigureFatality opponent)
    {
        yield return new WaitForSeconds(1);

        _anim.SetBool(Katana006,true);
        if(inFatality&&GameSettingsScript.BloodEnable)
            StartCoroutine(PawnHit());
        yield return new WaitForSeconds(timeToWeaponDrop);
        if (weapon)
        {
            Vector3 oldPos=weapon.transform.position;
            Vector3 oldRot=weapon.transform.eulerAngles;
            weapon.transform.parent = transform.parent;
            float time = _anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

            _anim.Rebind ();
            _anim.SetBool(Katana006,true);
           // yield return new WaitForEndOfFrame();
            _anim.Play("death",0,time);

            weapon.isKinematic = false;
            weapon.transform.position = oldPos;
            weapon.transform.eulerAngles = oldRot;
        }
        yield return new WaitForSeconds(fatalityTime-timeToWeaponDrop);
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
