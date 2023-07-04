using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight2Fatality : FigureFatality
{
    [SerializeField] private GameObject ArrowEffect;

    [SerializeField] private GameObject blood;
    [SerializeField] private Animator riderAnimator;
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int Dying = Animator.StringToHash("Fatality055_death");
    private float attackAnimTime = 4.17f;
    private float fatalityTime = 9.3f;
    private float _delayToHit = 4.12f;

    private float bloodOffset = 0.0631f;

    public override IEnumerator FatalityAnimation(figureMover target)
    {
        yield return new WaitForSeconds(2);
        riderAnimator.SetBool(Shoot, true);
        yield return new WaitForSeconds(attackAnimTime);
        riderAnimator.SetBool(Shoot, false);
        yield return new WaitForSeconds(fatalityTime - attackAnimTime);
    }


    public override void TurnOffAnimation()
    {
        _anim.SetBool(Shoot, false);
    }

    public override IEnumerator FigureFatalityDying(bool inFatality,FigureFatality opponent)
    {
        yield return new WaitForSeconds(1);

        _anim.SetTrigger(Dying);
        _anim.speed = 1.25f;

        yield return new WaitForSeconds(_delayToHit);
        _anim.speed = 0.4f;


        if (inFatality && ArrowEffect&&GameSettingsScript.BloodEnable)
        {
            Debug.Log("_______-");

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
            foreach (var bloodEffect in blood.GetComponentsInChildren<BFX_BloodSettings>())
            {
                bloodEffect.AnimationSpeed = 1.4f;
            }

        yield return new WaitForSeconds(fatalityTime - _delayToHit);
    }
}