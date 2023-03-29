using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChampionAnimation : MonoBehaviour
{
	public enum AnimState
	{
		Idle,
		Move,
		Attack,
		Skill,
		Ultimate,
		Dead,
	}

	private Champion _champion;
	private SpriteRenderer _spriteRenderer;
	private Animator _animator;

	private AnimState _state;
	private AnimatorOverrideController _animatorOverrideController;

	private float _atkAnimRuntime = 0f;
	private float _skillAnimRuntime = 0f;
	private float _ultAnimRuntime = 0f;

	// 애니메이터에서 쓰는 파라미터들 이름을 미리 해시값으로 가지고 있기(전부 다 똑같고 다 쓰니까 static으로 해서 하나만 ㄱㄱ)..
	private static bool s_isHaveKeyHash = false;
	private static int s_isMoveKeyHash = 0;
	private static int s_attackKeyHash = 0;
	private static int s_skillKeyHash = 0;
	private static int s_ultKeyHash = 0;
	private static int s_deathKeyHash = 0;
	private static int s_revivalKeyHash = 0;

	public bool flipX
	{
		set
		{
			_spriteRenderer.flipX = value;
		}
	}

	public ChampionAnimData animData
	{
		set
		{
#if UNITY_EDITOR
			Debug.Assert(null != value);

			SetupAnimator(value);
#endif
		}
	}

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_champion = GetComponentInParent<Champion>();

		if (false == s_isHaveKeyHash)
		{
			SetupAnimatorKeyToHash();
		}
	}

	void Start()
    {
		
	}

	// 애니메이터에서 쓰는 파라미터들 이름을 해시값으로 변환하는 함수..
	private static void SetupAnimatorKeyToHash()
	{
		s_isMoveKeyHash = Animator.StringToHash("isMove");
		s_attackKeyHash = Animator.StringToHash("OnAttack");
		s_skillKeyHash = Animator.StringToHash("OnSkill");
		s_ultKeyHash = Animator.StringToHash("OnUltimate");
		s_deathKeyHash = Animator.StringToHash("OnDeath");
		s_revivalKeyHash = Animator.StringToHash("OnRevival");

		s_isHaveKeyHash = true;
	}

	// 애니메이션 상태 변경..
	public void ChangeState(AnimState newState)
	{
		if (_state == AnimState.Move && newState != _state)
			_animator.SetBool(s_isMoveKeyHash, false);

		// 새로운 애니메이션 상태에 따른 파라미터 값 갱신..
		switch (newState)
		{
			case AnimState.Idle:
				if (_state == AnimState.Dead)
					_animator.SetTrigger(s_revivalKeyHash);

				break;
			case AnimState.Move:
				_animator.SetBool(s_isMoveKeyHash, true);

				break;
			case AnimState.Attack:
				_animator.SetTrigger(s_attackKeyHash);
				StartCoroutine(WaitAnimationEnd(_atkAnimRuntime, 0.1f));

				break;
			case AnimState.Skill:
				_animator.SetTrigger(s_skillKeyHash);
				StartCoroutine(WaitAnimationEnd(_skillAnimRuntime, 0.1f));

				break;
			case AnimState.Ultimate:
				_animator.SetTrigger(s_ultKeyHash);

				break;
			case AnimState.Dead:
				_animator.SetTrigger(s_deathKeyHash);

				break;
		}

		_state = newState;
	}

	// 애니메이션 재생 시간이 끝나게 되면 파라미터 값 변경하고 챔피언에게 이벤트 전달..
	IEnumerator WaitAnimationEnd(float waitSecond, float delayEventTime)
	{
		yield return YieldInstructionStore.GetWaitForSec(waitSecond);
		_animator.SetTrigger("OnAnimEnd");

		yield return YieldInstructionStore.GetWaitForSec(delayEventTime);
		_champion.OnAnimationEnd();
	}

	// 애니메이션 초기화..
	public void ResetAnimation()
	{
		_animator.SetBool(s_isMoveKeyHash, false);
		_animator.ResetTrigger(s_attackKeyHash);
		_animator.ResetTrigger(s_skillKeyHash);
		_animator.ResetTrigger(s_ultKeyHash);
		_animator.ResetTrigger(s_deathKeyHash);
		_animator.ResetTrigger(s_revivalKeyHash);

		ChangeState(AnimState.Idle);
	}

	// 애니메이터의 애니메이션 클립들을 현재 챔피언에 맞는 애니메이션으로 변환하는 과정..
	private void SetupAnimator( ChampionAnimData animData )
    {
#if UNITY_EDITOR
		Debug.Assert(null != _animator);
#endif

		// 기존 애니메이터 컨트롤러를 오버라이드해 캐릭터에 맞는 애니메이션을 설정해주기 위한 컴포넌트 생성..
		if (null == _animatorOverrideController)
		{
			_animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);

			// 새로 생성한 컴포넌트를 작동시키도록 한다..
			_animator.runtimeAnimatorController = _animatorOverrideController;
		}

		_animatorOverrideController["Idle"] = animData.idleAnim;
		_animatorOverrideController["Move"] = animData.moveAnim;
		_animatorOverrideController["Attack"] = animData.atkAnim;
		_animatorOverrideController["Skill"] = animData.skillAnim;
		_animatorOverrideController["Ultimate"] = animData.ultAnim;
		_animatorOverrideController["Death"] = animData.deathAnim;
		_animatorOverrideController["DeadLoop"] = animData.deadLoopAnim;

		_atkAnimRuntime = animData.atkAnim.length;
		_skillAnimRuntime = animData.skillAnim.length;
		_ultAnimRuntime = animData.ultAnim.length;
	}
}
