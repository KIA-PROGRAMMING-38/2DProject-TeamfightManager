using System;
using UnityEngine;

public class SummonStructure_FireSprit : SummonStructure
{
	[SerializeField] private string _fireProjectileName;
	[SerializeField] private Vector3 _projectileOffsetPos;

	private Animator _animator;
	private SpriteRenderer _spriteRenderer;

	private static readonly int s_OnDeathAnimHashKey = Animator.StringToHash("OnDeath");
	private static readonly int s_OnAttackAnimHashKey = Animator.StringToHash("OnAttack");
	private static readonly int s_OnEndAnimationAnimHashKey = Animator.StringToHash("OnEndAnimation");

	new private void Awake()
	{
		base.Awake();
		
		_animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnDisable()
	{
		
	}

	protected override void Action()
	{
		Debug.Log("Fire");

		int targetCount = _targetFindFunc.Invoke(transform.position, _targetArray);
		if (0 < targetCount)
		{
			Debug.Log("Fire Projectile Create");

			float directionX = (_targetArray[0].transform.position - transform.position).x;
			bool flipX = directionX < 0f;

			_spriteRenderer.flipX = flipX;

			Projectile projectile = summonObjectManager.GetSummonObject<Projectile>(_fireProjectileName);

			projectile.OnExecuteImpact -= OnProjectileImpact;
			projectile.OnExecuteImpact += OnProjectileImpact;

			projectile.OnRelease -= OnProjectileRelease;
			projectile.OnRelease += OnProjectileRelease;

			projectile.gameObject.SetActive(true);

			Vector3 projectileOffset = _projectileOffsetPos;
			if (flipX)
				projectileOffset.x *= -1f;

			projectile.transform.position = transform.position + projectileOffset;
			projectile.SetAdditionalData(LayerTable.Number.CalcOtherTeamLayer(_getLayerMask), _targetArray[0], _targetFindFunc);

			_animator.SetTrigger(s_OnAttackAnimHashKey);
		}
	}

	private void OnProjectileImpact(SummonObject summonObject, Champion[] _championArray, int targetCount)
	{
		_targetArray = _championArray;

		ReceiveImpactExecuteEvent(targetCount);
	}

	private void OnProjectileRelease(SummonObject summonObject)
	{
		summonObject.OnExecuteImpact -= OnProjectileImpact;
		summonObject.OnRelease -= OnProjectileRelease;
	}

	public void OnEndAnimation()
	{
		_animator.SetTrigger(s_OnEndAnimationAnimHashKey);
	}
}