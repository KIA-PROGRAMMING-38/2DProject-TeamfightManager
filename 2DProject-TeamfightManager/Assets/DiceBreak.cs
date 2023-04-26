using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBreak : StateMachineBehaviour
{
    private float _clipPlayLength = 0f;
    private float _elapsedTime = 0f;

    private SummonStructure_RandomDice randomDiceComponent;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(null == randomDiceComponent)
        {
            randomDiceComponent = animator.GetComponentInParent<SummonStructure_RandomDice>();
        }

        _clipPlayLength = stateInfo.length;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _elapsedTime += Time.deltaTime;
        if(_elapsedTime >= _clipPlayLength)
        {
            randomDiceComponent.OnDiceBreakAnimEnd();
        }
    }

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
