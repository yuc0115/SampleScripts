using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimIdle : StateMachineBehaviour
{
    private Enemy _enemy = null;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_enemy == null)
        {
            _enemy = animator.GetComponent<Enemy>();
        }

        if (_enemy == null)
        {
            Debug.LogError(" Enemy null");
            return;
        }
        // 타겟 추적으로 상태 변경.
        if (_enemy.IsDie == false)
        {
            _enemy.ChangeState(EnemyState.TargetChase);

            // 애니메이터 상태 초기화.
            animator.SetInteger(AnimKey.animKey, 0);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
