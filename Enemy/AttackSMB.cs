using UnityEngine;

public class AttackSMB : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) =>
        animator.GetComponentInParent<Enemy>()?.OnAttackEnd();
}