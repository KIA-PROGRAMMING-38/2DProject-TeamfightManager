using UnityEngine;

/// <summary>
/// 이펙트의 애니메이터의 실행 클립에 부착되는 스크립트..
/// 이펙트의 재생이 종료되면 이펙트에게 이벤트를 전달하는 역할을 한다..
/// </summary>
public class EffectAnim : StateMachineBehaviour
{
    private Effect _effectComponent;

    private float _animPlayTime = 0f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animPlayTime = stateInfo.length;

        _effectComponent = animator.GetComponent<Effect>();
	}

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animPlayTime -= Time.deltaTime;

        if(_animPlayTime <= 0f)
        {
            _effectComponent?.OnEndAnimation();
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
