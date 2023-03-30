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

	// �ִϸ����Ϳ��� ���� �Ķ���͵� �̸��� �̸� �ؽð����� ������ �ֱ�(���� �� �Ȱ��� �� ���ϱ� static���� �ؼ� �ϳ��� ����)..
	private static bool s_isHaveKeyHash = false;
	private static int s_isMoveKeyHash = 0;
	private static int s_attackKeyHash = 0;
	private static int s_skillKeyHash = 0;
	private static int s_ultKeyHash = 0;
	private static int s_deathKeyHash = 0;
	private static int s_revivalKeyHash = 0;

	public bool flipX
	{
		get
		{
			return _spriteRenderer.flipX;
		}
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

	// �ִϸ����Ϳ��� ���� �Ķ���͵� �̸��� �ؽð����� ��ȯ�ϴ� �Լ�..
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

	// �ִϸ��̼� ���� ����..
	public void ChangeState(AnimState newState)
	{
		if (_state == AnimState.Move && newState != _state)
			_animator.SetBool(s_isMoveKeyHash, false);

		// ���ο� �ִϸ��̼� ���¿� ���� �Ķ���� �� ����..
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

	// �ִϸ��̼� ��� �ð��� ������ �Ǹ� �Ķ���� �� �����ϰ� è�Ǿ𿡰� �̺�Ʈ ����..
	IEnumerator WaitAnimationEnd(float waitSecond, float delayEventTime)
	{
		yield return YieldInstructionStore.GetWaitForSec(waitSecond);
		_animator.SetTrigger("OnAnimEnd");

		yield return YieldInstructionStore.GetWaitForSec(delayEventTime);
		_champion.OnAnimationEnd();
	}

	// �ִϸ��̼� �ʱ�ȭ..
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

	// �ִϸ������� �ִϸ��̼� Ŭ������ ���� è�Ǿ� �´� �ִϸ��̼����� ��ȯ�ϴ� ����..
	private void SetupAnimator( ChampionAnimData animData )
    {
#if UNITY_EDITOR
		Debug.Assert(null != _animator);
#endif

		// ���� �ִϸ����� ��Ʈ�ѷ��� �������̵��� ĳ���Ϳ� �´� �ִϸ��̼��� �������ֱ� ���� ������Ʈ ����..
		if (null == _animatorOverrideController)
		{
			_animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);

			// ���� ������ ������Ʈ�� �۵���Ű���� �Ѵ�..
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
