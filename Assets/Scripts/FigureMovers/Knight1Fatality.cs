using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight1Fatality : FigureFatality
{
    [SerializeField] private GameObject ArrowEffect;

    [SerializeField] private GameObject blood;
    [SerializeField] private Animator riderAnimator;
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int Dying = Animator.StringToHash("death");
    private static readonly int Running = Animator.StringToHash("forward_run");
    private readonly float _attackAnimTime = 4.17f;
    private readonly float _fatalityTime = 6.5f;
    private readonly float _delayToHit = 3.12f;
    private readonly float _runSpeed = 2.9f;

    [SerializeField] private Rigidbody droppedWeapon;
    private float bloodOffset = 0.0631f;

    public override IEnumerator FatalityAnimation(figureMover target)
    {       
        if(GetComponentInParent<FatalityController>().knightPositionIndex>=0)
        {
            StartCoroutine(KnightRunning());
        }
        //yield return new WaitForSeconds(1);
        riderAnimator.SetBool(Shoot, true);
        yield return new WaitForSeconds(_attackAnimTime);
        riderAnimator.SetBool(Shoot, false);
        yield return new WaitForSeconds(_fatalityTime - _attackAnimTime);
    }

    private IEnumerator KnightRunning()
    {
        _anim.SetBool(Running, true);

        while (true)
        {
            float step = _runSpeed * Time.deltaTime;
            transform.localPosition += Vector3.forward * step;
            yield return null;
        }
    }


    public override void TurnOffAnimation()
    {
        _anim.SetBool(Shoot, false);
    }

    public override IEnumerator FigureFatalityDying(bool inFatality, FigureFatality opponent)
    {
        if (GetComponentInParent<FatalityController>().knightPositionIndex < 0)
        {
            yield return new WaitForSeconds(1);
        }

        _anim.SetBool(Running, true);

        float curDelayToHit = _delayToHit;

        while (curDelayToHit - 1 > 0)
        {
            curDelayToHit -= Time.deltaTime;
            float step = _runSpeed * Time.deltaTime;
            transform.localPosition += Vector3.forward * step;
            yield return null;
        }

        //yield return new WaitForSeconds(_delayToHit);
        _anim.speed = 0.4f;

        _anim.SetBool(Running, false);
        _anim.SetBool(Dying, true);
        if (inFatality && ArrowEffect && GameSettingsScript.BloodEnable)
        {
            ArrowEffect.SetActive(true);
            if (inFatality && blood)
            {
                blood.SetActive(true);
                blood.transform.position = ArrowEffect.transform.position - Vector3.up * bloodOffset;
            }
        }

        yield return new WaitForSeconds(2);
        _anim.speed = 1.25f;
        if (blood)
        {
            foreach (var bloodEffect in blood.GetComponentsInChildren<BFX_BloodSettings>())
            {
                bloodEffect.AnimationSpeed = 1.4f;
            }
        }


        // yield return new WaitForSeconds(0.4f);
        if (droppedWeapon)
        {
            Vector3 oldPos = droppedWeapon.transform.position;
            Vector3 oldRot = droppedWeapon.transform.eulerAngles;
            droppedWeapon.transform.parent = transform.parent;
            float time = _anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            _anim.Rebind();
            _anim.Play("death", 0, time);

            droppedWeapon.isKinematic = false;
            droppedWeapon.transform.position = oldPos;
            droppedWeapon.transform.eulerAngles = oldRot;
        }

        yield return new WaitForSeconds(_fatalityTime - _delayToHit - 0.4f);
    }
}