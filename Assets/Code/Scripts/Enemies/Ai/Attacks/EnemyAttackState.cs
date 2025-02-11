using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;

[System.Serializable]
public class EnemyAttackState : EnemyState
{
    [SerializeField] private List<EnemyAttack> attacks;

    private float delay;
    private float timeToExit;
    private int attackToPerformIndex;
    private bool alreadyPerformed = false;
    private float timeSinceEnter = 0f;

    public override void Setup(EnemyNavigation controller)
    {
        base.Setup(controller);
        foreach (EnemyAttack attack in attacks)
        {
            attack.SetupAttack(controller);
        }
    }
    public override void Enter()
    {
        Debug.Log("entered attackState");
        CanExit = false;
        alreadyPerformed = false;
        timeSinceEnter = 0f;

        controller.Agent.speed = attacks[attackToPerformIndex].MoveSpeedWhileAttacking;
        controller.animator.Animator.CrossFadeInFixedTime(attacks[attackToPerformIndex].AttackAnimation.name, 0.1f);

        timeToExit = attacks[attackToPerformIndex].AttackAnimation.length;
        delay = attacks[attackToPerformIndex].AttackDelay;

        base.Enter();
    }

	public override void Exit()
	{
        Debug.Log("exited attack state");
		controller.animator.Animator.SetBool("IsAttacking", false);
	}
	public bool CheckAllConditions()
    {
        for (int i = 0; i< attacks.Count; i++)
        {
            EnemyAttack attack = attacks[i];
            if (attack.CheckAttackConditon())
            {
                attackToPerformIndex = i;
                return true;
            }
        }
        return false;
        
    }

    public override void Handle()
    {
        timeSinceEnter += Time.deltaTime;
        if (timeSinceEnter > timeToExit)
        {
            CanExit = true;
        }
        if (timeSinceEnter > delay && !alreadyPerformed)
        {
            Debug.Log("performing attack");
            alreadyPerformed = true;
            attacks[attackToPerformIndex].PerformAttack();
        }
        base.Handle();
    }
}
