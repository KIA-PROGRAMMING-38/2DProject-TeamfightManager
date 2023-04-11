using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 챔피언의 애니메이션 및 스프라이트와 관련된 기능들을 제공하는 클래스..
/// </summary>
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
	private static int s_onAnimEndKeyHash = 0;

	// 적에게 피격 시 흰색으로 깜빡거리는 효과 관련 필드..
	private readonly static float s_hitEffectTime = 0.1f;
	private WaitForSeconds _hitEffectWaitForSecInstance;
	private IEnumerator _showHitMatCoroutine;
	private bool _isRunningHitEvent = false;

	// 공격 관련 애니메이션 종료 시 챔피언에게 몇 초 있다가 이벤트를 전달하는 기능 관련 필드..
	private readonly static float s_delayreceiveAnimEndEventTime = 0.1f;
	private WaitForSeconds _delayReceiveToChampionWaitSecInst;
	private IEnumerator _delayReceiveToChampionCoroutine;

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
		_hitEffectWaitForSecInstance = YieldInstructionStore.GetWaitForSec(s_hitEffectTime);
		_delayReceiveToChampionWaitSecInst = YieldInstructionStore.GetWaitForSec(s_delayreceiveAnimEndEventTime);

		_showHitMatCoroutine = ShowHitMaterial();
		_delayReceiveToChampionCoroutine = StartReceiveEventDelay();
	}

	private void OnDisable()
	{
		_spriteRenderer.material = _originMaterial;
	}

	public void OnHit()
	{
		if (false == _isRunningHitEvent)
			StartCoroutine(_showHitMatCoroutine);
	}

	IEnumerator ShowHitMaterial()
	{
		while(true)
		{
			_isRunningHitEvent = true;

			_spriteRenderer.material = _hitMaterial;
			yield return _hitEffectWaitForSecInstance;
			_spriteRenderer.material = _originMaterial;

			StopCoroutine(_showHitMatCoroutine);

			_isRunningHitEvent = false;
			yield return null;
		}
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
		s_onAnimEndKeyHash = Animator.StringToHash(AnimKeyTable.onAnimEnd);

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

				break;
			case AnimState.Skill:
				_animator.SetTrigger(s_skillKeyHash);

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

	// 애니메이션 이벤트 : 애니메이션이 종료될 때 호출..
	private void OnAnimationEnd()
	{
		_animator.SetTrigger(s_onAnimEndKeyHash);

		StartCoroutine(_delayReceiveToChampionCoroutine);
	}

	// 애니메이션이 끝나자마자 챔피언에게 종료 사실을 전달하지 않고 딜레이를 주기 위한 코루틴..
	IEnumerator StartReceiveEventDelay()
	{
		while(true)
		{
			yield return _delayReceiveToChampionWaitSecInst;
			OnAnimationEvent("OnAnimEnd");

			StopCoroutine(_delayReceiveToChampionCoroutine);

			yield return null;
		}
	}
}
