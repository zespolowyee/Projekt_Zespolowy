using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;

[System.Serializable]
public class EnemyAttackState : EnemyState
{
    [SerializeField] private List<EnemyAttack> attacks;
    private int attackToPerformId;

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
        attacks[attackToPerformId].PerformAttack();
        controller.animator.Animator.SetBool("IsAttacking", true);
        base.Enter();
    }

	public override void Exit()
	{
		controller.animator.Animator.SetBool("IsAttacking", false);
	}
	public bool CheckAllConditions()
    {
        for (int i = 0; i< attacks.Count; i++)
        {
            EnemyAttack attack = attacks[i];
            if (attack.CheckAttackConditon())
            {
                attackToPerformId = i;
                return true;
            }
        }
        return false;
        
    }
}
