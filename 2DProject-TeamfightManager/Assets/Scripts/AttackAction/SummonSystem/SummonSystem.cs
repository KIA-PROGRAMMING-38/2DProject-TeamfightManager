using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SummonSystem
{
	public SummonObjectManager summonObjectManager { private get; set; }

	private Champion _ownerChampion;
	public Champion ownerChampion
	{
		private get => _ownerChampion;
		set
		{
			_ownerChampion = value;
			_ownerAnimComponent = _ownerChampion.GetComponentInChildren<ChampionAnimation>();
		}
	}

	private ChampionAnimation _ownerAnimComponent;

	private AttackAction _attackAction;
	private AtkActionSummonData _summonData;
	private List<AttackImpactData> _impactDatas = new List<AttackImpactData>();
	private Champion[] _targetArray;
	private Func<Vector3, Champion[], int> _findImpactTargetFunc;
	private Func<Champion> _findTargetFunc;
	private Action<AttackImpactData, int, Champion[]> _impactFunc;

	private float _elapsedTime = 0f;

	public TargetTeamKind atkImpactTeamKind { private get; set; }

	public SummonSystem(AttackAction attackAction, AtkActionSummonData summonData, Func<Vector3, Champion[], int> findImpactTargetFunc, Func<Champion> findTargetFunc)
	{
		_attackAction = attackAction;
        _summonData = summonData;

		_findImpactTargetFunc = findImpactTargetFunc;
		_findTargetFunc = findTargetFunc;
	}

	public void OnStart(Champion[] targetCacheArray)
	{
		_targetArray = targetCacheArray;
		SummonObject();
	}

	public void Update()
	{
		if (false == _summonData.isSummonOnce)
		{
			_elapsedTime += Time.deltaTime * _ownerAnimComponent.animationSpeed;
			if (_elapsedTime >= _summonData.tickTime)
			{
				SummonObject();
				_elapsedTime -= _summonData.tickTime;
			}
		}
	}

	public void AddImpactData(AttackImpactData impactData)
	{
		_impactDatas.Add(impactData);
	}

	// 공격 행동 시 소환해야할 소환물 소환하는 함수..
	private void SummonObject()
	{
		switch (_summonData.summonObjectType)
		{
			case SummonObjectType.Champion:
				{
					SummonChamp summonChampion = summonObjectManager.GetSummonObject<SummonChamp>(_summonData.summonObjectName);

					Vector3 moveDirection = _summonData.offsetPosition;
					int layer = (atkImpactTeamKind == TargetTeamKind.Team) ? ownerChampion.buffSummonLayer : ownerChampion.atkSummonLayer;
					if (ownerChampion.flipX)
						moveDirection.x *= -1f;

					summonChampion.transform.position = ownerChampion.transform.position + moveDirection;
					summonChampion.gameObject.SetActive(true);
					summonChampion.SetAdditionalData(ownerChampion);
				}

				break;
			case SummonObjectType.Projectile:
				{
					Champion target = _findTargetFunc.Invoke();

					if (null != target && false == target.isDead)
					{
						// 발사체 가져와서 offset 좌표 세팅하고 필요한 정보 넘긴 뒤 Active On..
						Projectile projectile = summonObjectManager.GetSummonObject<Projectile>(_summonData.summonObjectName);

						Vector3 moveDirection = _summonData.offsetPosition;
						int layer = (atkImpactTeamKind == TargetTeamKind.Team) ? ownerChampion.buffSummonLayer : ownerChampion.atkSummonLayer;
						if (ownerChampion.flipX)
							moveDirection.x *= -1f;

						projectile.gameObject.SetActive(true);

						projectile.transform.position = ownerChampion.transform.position + moveDirection;
						projectile.SetAdditionalData(layer, target, _findImpactTargetFunc);

						// 이벤트 구독..
						projectile.OnExecuteImpact -= OnSummonObjectExecuteImpact;
						projectile.OnExecuteImpact += OnSummonObjectExecuteImpact;

						projectile.OnRelease -= OnSummonRelease;
						projectile.OnRelease += OnSummonRelease;
					}
				}

				break;
			case SummonObjectType.Structure:
				{
					Champion target = _findTargetFunc.Invoke();

					if (null != target && false == target.isDead)
					{
						// SummonStructure 가져와서 offset 좌표 세팅하고 필요한 정보 넘긴 뒤 Active On..
						SummonStructure summonObject = summonObjectManager.GetSummonObject<SummonStructure>(_summonData.summonObjectName);

						int layer = (atkImpactTeamKind == TargetTeamKind.Team) ? ownerChampion.buffSummonLayer : ownerChampion.atkSummonLayer;
						layer = LayerTable.Number.CalcOtherTeamLayer(layer);

						Vector3 moveDirection = _summonData.offsetPosition;
						if (ownerChampion.flipX)
							moveDirection.x *= -1f;

						summonObject.gameObject.SetActive(true);

						summonObject.transform.position = ownerChampion.transform.position + moveDirection;
						summonObject.SetAdditionalData(layer, target, ownerChampion, _findImpactTargetFunc);

						// 이벤트 구독..
						summonObject.OnExecuteImpact -= OnSummonObjectExecuteImpact;
						summonObject.OnExecuteImpact += OnSummonObjectExecuteImpact;

						summonObject.OnRelease -= OnSummonRelease;
						summonObject.OnRelease += OnSummonRelease;
					}
				}

				break;
		}
	}

	// Summon Object가 효과 발동 시 호출되는 콜백 함수(공격이라면 공격 효과를 준다)..
	private void OnSummonObjectExecuteImpact(SummonObject summonObject, Champion[] targetArray, int targetCount)
	{
        for (int targetIndex = 0; targetIndex < targetCount; ++targetIndex)
		{
			int impactCount = _impactDatas.Count;
            for ( int impactIndex = 0; impactIndex < impactCount; ++impactIndex)
			{
                AttackImpactData curImpactData = _impactDatas[impactIndex];

                // 찾은 타겟 개수만큼 효과 부여..
                _attackAction.ImpactTarget(curImpactData, targetCount, targetArray);
			}
		}
	}

	// Summon Object가 Release될 때 호출되는 콜백 함수..
	private void OnSummonRelease(SummonObject summonObject)
	{
		summonObject.OnExecuteImpact -= OnSummonObjectExecuteImpact;
		summonObject.OnRelease -= OnSummonRelease;
	}
}