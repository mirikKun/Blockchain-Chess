using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightDeathFromKnightFatality : FigureFatality
{
    [SerializeField] private GameObject ArrowEffect;

    [SerializeField] private GameObject blood;
    [SerializeField] private Animator riderAnimator;
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int KnightDying = Animator.StringToHash("Knight_death_from_knight");
    private float attackAnimTime = 4.17f;
    private float fatalityTime = 6f;
    private float delayToSecondKnightAnimation = 1.72f;
    private float delayToHit = 2.8f;

    private float bloodOffset = 0.0631f;

    public override IEnumerator FatalityAnimation(figureMover target)
    {
        yield return new WaitForSeconds(1);
        riderAnimator.SetBool(Shoot, true);
        yield return new WaitForSeconds(attackAnimTime);
        riderAnimator.SetBool(Shoot, false);
        yield return new WaitForSeconds(fatalityTime - attackAnimTime);
    }


    public override void TurnOffAnimation()
    {
    }

    public override IEnumerator FigureFatalityDying(bool inFatality, FigureFatality opponent)
    {
        yield return new WaitForSeconds(1 + delayToSecondKnightAnimation);

        riderAnimator.SetTrigger(KnightDying);

        yield return new WaitForSeconds(delayToHit - delayToSecondKnightAnimation);
        // riderAnimator.speed = 0.4f;

        if (inFatality && ArrowEffect && GameSettingsScript.BloodEnable)
        {
            ArrowEffect.SetActive(true);
            if (blood)
            {
                blood.SetActive(true);
                blood.transform.position = ArrowEffect.transform.position - Vector3.up * bloodOffset;


                foreach (var bloodEffect in blood.GetComponentsInChildren<BFX_BloodSettings>())
                {
                    bloodEffect.AnimationSpeed = 1.4f;
                }
            }
        }

        // riderAnimator.speed = 1.25f;


        yield return new WaitForSeconds(fatalityTime - delayToHit);
    }
}