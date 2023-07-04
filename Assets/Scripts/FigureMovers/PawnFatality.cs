using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnFatality : FigureFatality
{
    [SerializeField] private GameObject[] blood;
    [SerializeField] private Rigidbody weapon;
    private float _delayToHit = 0.8f;
    private static readonly int Katana003 = Animator.StringToHash("Katana003");
    private static readonly int Katana001 = Animator.StringToHash("Katana001");
    private float fatalityTime = 3.3f;
    private float dieAnimDelay = 0.31f;
    private float timeToWeaponDrop = 1.137f;
    private float attackAnimTime = 2.6f;
    

    public override IEnumerator FatalityAnimation(figureMover target)
    {
        yield return new WaitForSeconds(1);

        _anim.SetBool(Katana001, true);
        yield return new WaitForSeconds(attackAnimTime);
        _anim.SetBool(Katana001, false);
        yield return new WaitForSeconds(fatalityTime-attackAnimTime);
    }
    public override void TurnOffAnimation()
    {
        _anim.SetBool(Katana001, false);
    }

    public override IEnumerator FigureFatalityDying(bool inFatality,FigureFatality opponent)
    {
  
        yield return new WaitForSeconds(1);

        if(inFatality&&GameSettingsScript.BloodEnable)
            StartCoroutine(PawnHit());
        yield return new WaitForSeconds(dieAnimDelay);
        _anim.SetBool(Katana003,true);
        yield return new WaitForSeconds(timeToWeaponDrop);
        if (weapon)
        {
            Vector3 oldPos=weapon.transform.position;
            Vector3 oldRot=weapon.transform.eulerAngles;
            weapon.transform.parent = transform.parent;
            float time = _anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

            _anim.Rebind ();
            _anim.SetBool(Katana003,true);
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
