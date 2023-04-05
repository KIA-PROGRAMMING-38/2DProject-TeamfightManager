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

	private Material _originMaterial;
	[SerializeField] private Material _hitMaterial;

	private Animator _animator;
	private AnimState _state;
	private AnimatorOverrideController _animatorOverrideController;

	// 공격, 스킬, 궁극기의 애니메이션 시간을 저장할 변수..
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

	public bool flipX { get => _spriteRenderer.flipX; set => _spriteRenderer.flipX = value; }

	public ChampionAnimData animData
	{
		set
		{
#if UNITY_EDITOR
			Debug.Assert(null != value);
#endif

			SetupAnimator(value);
		}
	}

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_champion = GetComponentInParent<Champion>();

		_originMaterial = _spriteRenderer.material;

		if (false == s_isHaveKeyHash)
		{
			SetupAnimatorKeyToHash();
		}
	}

	void Start()
    {
		
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	public void OnHit()
	{
		StartCoroutine(ShowHitMaterial(0.1f));
	}

	IEnumerator ShowHitMaterial(float time)
	{
		_spriteRenderer.material = _hitMaterial;
		yield return YieldInstructionStore.GetWaitForSec(time);
		_spriteRenderer.material = _originMaterial;
	}

	/// <summary>
	/// 애니메이터에서 쓰는 파라미터들 이름을 해시값으로 변환하는 함수..
	/// </summary>
	private static void SetupAnimatorKeyToHash()
	{
		s_isMoveKeyHash = Animator.StringToHash(AnimKeyTable.isMove);
		s_attackKeyHash = Animator.StringToHash(AnimKeyTable.onAttack);
		s_skillKeyHash = Animator.StringToHash(AnimKeyTable.onSkill);
		s_ultKeyHash = Animator.StringToHash(AnimKeyTable.onUltimate);
		s_deathKeyHash = Animator.StringToHash(AnimKeyTable.onDeath);
		s_revivalKeyHash = Animator.StringToHash(AnimKeyTable.onRevival);

		s_isHaveKeyHash = true;
	}

	/// <summary>
	/// 애니메이션 상태 변경..
	/// </summary>
	/// <param name="newState"></param>
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

	/// <summary>
	/// 애니메이션 재생 시간이 끝나게 되면 파라미터 값 변경하고 챔피언에게 이벤트 전달..
	/// </summary>
	/// <param name="waitSecond"></param>
	/// <param name="delayEventTime"></param>
	/// <returns></returns>
	IEnumerator WaitAnimationEnd(float waitSecond, float delayEventTime)
	{
		yield return YieldInstructionStore.GetWaitForSec(waitSecond);
		_animator.SetTrigger("OnAnimEnd");

		yield return YieldInstructionStore.GetWaitForSec(delayEventTime);
		_champion.OnAnimEvent("OnAnimEnd");
	}

	/// <summary>
	/// 애니메이터 파라미터 초기화..
	/// </summary>
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

	/// <summary>
	/// 애니메이터의 애니메이션 클립들을 현재 챔피언에 맞는 애니메이션으로 변환하는 과정..
	/// </summary>
	/// <param name="animData"></param>
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

		// 애니메이터의 클립들을 현재 챔피언에 맞는 애니메이션 클립들로 변경..
		_animatorOverrideController["Idle"] = animData.idleAnim;
		_animatorOverrideController["Move"] = animData.moveAnim;
		_animatorOverrideController["Attack"] = animData.atkAnim;
		_animatorOverrideController["Skill"] = animData.skillAnim;
		_animatorOverrideController["Ultimate"] = animData.ultAnim;
		_animatorOverrideController["Death"] = animData.deathAnim;
		_animatorOverrideController["DeadLoop"] = animData.deadLoopAnim;

		// 애니메이션 클립을 통해 공격, 스킬, 궁극기 애니메이션 실행 시간 계산..
		_atkAnimRuntime = animData.atkAnim.length;
		_skillAnimRuntime = animData.skillAnim.length;
		_ultAnimRuntime = animData.ultAnim.length;
	}

	public void OnAnimationEvent(string eventName)
	{
		_champion?.OnAnimEvent(eventName);
	}
}
