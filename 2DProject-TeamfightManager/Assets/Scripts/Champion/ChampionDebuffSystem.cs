using System;
using System.Collections.Generic;
using UnityEngine;

public class ChampionDeBuffSystem
{
	class DebuffInfo
	{
		public Champion didChampion;
		public float amount;
		public float impactTime;
	}

	public event Action OnChangedStatus;

	public EffectManager effectManager
	{
		set
		{
			_prevokeEffect = value.GetEffect("Effect_Prevoke", UnityEngine.Vector3.zero);
			_prevokeEffect.SetupAdditionalData(UnityEngine.Vector3.zero, _ownerChampion.transform);

			_freezeEffect = value.GetEffect("Effect_Freeze", UnityEngine.Vector3.zero);
			_freezeEffect.SetupAdditionalData(UnityEngine.Vector3.zero, _ownerChampion.transform);

			_freezeEndEffect = value.GetEffect("Effect_FreezeEnd", UnityEngine.Vector3.zero);
			_freezeEndEffect.SetupAdditionalData(UnityEngine.Vector3.zero, _ownerChampion.transform);

			_freezeEndEffect.OnOccurEndAnimEvent -= OnFreezeEndEffectEndAnimation;
			_freezeEndEffect.OnOccurEndAnimEvent += OnFreezeEndEffectEndAnimation;
		}
	}

	public ChampionStatus status { get; private set; }
	public ChampionStatus championBaseStatus { private get; set; }

	private Champion _ownerChampion;
	private AudioSource _audioSource;

	private int _activeDebuffTable; // 각 비트자리마다 그 자리에 매칭되는 버프가 활성화되있는지를 체크(DeBuffImpactType의 값 == 비트 자리)..
	private int _debuffCount = 0;
	private List<DebuffInfo>[] _debuffInfoContainer;
	private int[] _debuffInfoContainerActiveCount;
	private readonly int _debuffInfoContainerDefaultSize = 4;

	public float healAmountDebuff { get; private set; }

	public bool isEnded { get => _activeDebuffTable == 0; }

	private Effect _prevokeEffect;

	private bool _isShowFreezeEffect = false;
	private Effect _freezeEffect;
	private Effect _freezeEndEffect;

	public ChampionDeBuffSystem(Champion ownerChampion)
	{
		_ownerChampion = ownerChampion;

		status = new ChampionStatus();

		// 디버프 관련 정보들을 저장할 공간을 어느정도 미리 확보해둔다..
		_debuffCount = (int)DebuffImpactType.End;
		_debuffInfoContainer = new List<DebuffInfo>[_debuffCount];
		_debuffInfoContainerActiveCount = new int[_debuffCount];
		for (int i = 0; i < _debuffCount; ++i)
		{
			_debuffInfoContainer[i] = new List<DebuffInfo>(_debuffInfoContainerDefaultSize);
			_debuffInfoContainerActiveCount[i] = 0;

			for (int j = 0; j < _debuffInfoContainerDefaultSize; ++j)
			{
				_debuffInfoContainer[i].Add(new DebuffInfo());
			}
		}

		healAmountDebuff = 1f;

		_audioSource = _ownerChampion.GetComponent<AudioSource>();
	}

	public void Update(float deltaTime)
	{
		for (int i = 0; i < _debuffCount; ++i)
		{
			bool curBitDigit = ((_activeDebuffTable & (1 << i)) != 0);

			// 비활성화된 버프라면 검사에서 제외..
			if (false == curBitDigit)
			{
				continue;
			}

			// 버프 지속시간 갱신..
			bool isReadjustStatus = false;
			for (int j = 0; j < _debuffInfoContainerActiveCount[i]; ++j)
			{
				DebuffInfo curBuffInfo = _debuffInfoContainer[i][j];

				curBuffInfo.impactTime -= deltaTime;

				if (curBuffInfo.impactTime <= 0f)
				{
					int lastIndex = _debuffInfoContainerActiveCount[i] - 1;

					_debuffInfoContainer[i][j].amount = _debuffInfoContainer[i][lastIndex].amount;
					_debuffInfoContainer[i][j].impactTime = _debuffInfoContainer[i][lastIndex].impactTime;

					if (_debuffInfoContainerActiveCount[i] == 0)
						SetActiveBuff((DebuffImpactType)i, false);

					isReadjustStatus = true;

					--_debuffInfoContainerActiveCount[i];
					--j;
				}
			}

			// 스탯 재조정을 해야한다면..
			if (isReadjustStatus)
			{
				UpdateStatus((DebuffImpactType)i);
			}
		}
	}

	public void Reset()
	{
		_activeDebuffTable = 0;

		status.atkStat = 0;
		status.atkSpeed = 0f;
		status.range = 0f;
		status.defence = 0;
		status.hp = 0;
		status.moveSpeed = 0f;
		status.skillCooltime = 0f;

		for (int i = 0; i < _debuffCount; ++i)
		{
			_debuffInfoContainerActiveCount[i] = 0;
		}

		SetPrevokeEffectActive(false);
		ResetFreeze();
	}

	public void AddDebuff(DebuffImpactType type, Champion didChampion, float amount, float impactTime)
	{
		int index = (int)type;

		if (_debuffInfoContainer[index].Count <= _debuffInfoContainerActiveCount[index])
			_debuffInfoContainer[index].Add(new DebuffInfo());

		_debuffInfoContainer[index][_debuffInfoContainerActiveCount[index]].didChampion = didChampion;
		_debuffInfoContainer[index][_debuffInfoContainerActiveCount[index]].amount = amount;
		_debuffInfoContainer[index][_debuffInfoContainerActiveCount[index]].impactTime = impactTime;

		if (0 == _debuffInfoContainerActiveCount[index])
			SetActiveBuff(type, true);

		++_debuffInfoContainerActiveCount[index];

		UpdateStatus(type);
	}

	private void SetActiveBuff(DebuffImpactType type, bool isActive)
	{
		int value = 1 << (int)type;

		if (true == isActive)
		{
			if ((_activeDebuffTable & value) == 0)
			{
				_activeDebuffTable = _activeDebuffTable | value;
			}
		}
		else
		{
			if ((_activeDebuffTable & value) != 0)
			{
				_activeDebuffTable = _activeDebuffTable ^ value;
			}
		}
	}

	// 버프가 추가 혹은 끝남으로 인해 스탯 재조정이 필요할 때 호출되는 함수..
	private void UpdateStatus(DebuffImpactType type)
	{
		switch (type)
		{
			case DebuffImpactType.HealAmount_Percent:
				float result = 0f;
				ComputeMulStatFigure(type, out result);
				healAmountDebuff = result;

				return;
			case DebuffImpactType.DefenceDown_Add:
			case DebuffImpactType.DefenceDown_Percent:
				status.defence = ReadjustStatusElement(championBaseStatus.defence, DebuffImpactType.DefenceDown_Add, DebuffImpactType.DefenceDown_Percent);

				break;
			case DebuffImpactType.AtkStatDown_Add:
			case DebuffImpactType.AtkStatDown_Percent:
				status.atkStat = ReadjustStatusElement(championBaseStatus.atkStat, DebuffImpactType.AtkStatDown_Add, DebuffImpactType.AtkStatDown_Percent);

				break;
			case DebuffImpactType.AtkSpeedDown_Percent:
				ComputeMulStatFigure(type, out status.atkSpeed);
				status.atkSpeed *= championBaseStatus.atkSpeed;

				break;
			case DebuffImpactType.MoveSpeedDown_Percent:
				ComputeMulStatFigure(type, out status.moveSpeed);
				status.moveSpeed *= championBaseStatus.moveSpeed;

				break;

			case DebuffImpactType.Provoke:
				if(_debuffInfoContainerActiveCount[(int)type] > 0)
				{
					int containerIndex = (int)DebuffImpactType.Provoke;
					int lastIndex = _debuffInfoContainerActiveCount[containerIndex] - 1;

					_ownerChampion.forcedTarget = _debuffInfoContainer[containerIndex][lastIndex].didChampion;

					SetPrevokeEffectActive(true);
				}
				else
				{
					_ownerChampion.forcedTarget = null;

					SetPrevokeEffectActive(false);
				}

				break;

			case DebuffImpactType.Slow:
				if (_debuffInfoContainerActiveCount[(int)type] > 0)
				{
					float mostBigAmount = 0f;
					int containerIndex = (int)type;
					int debuffInfoCount = _debuffInfoContainerActiveCount[containerIndex];

					for (int debuffInfoIndex = 0; debuffInfoIndex < debuffInfoCount; ++debuffInfoIndex)
					{
						DebuffInfo debuffInfo = _debuffInfoContainer[containerIndex][debuffInfoIndex];

						if (debuffInfo.amount > mostBigAmount)
							mostBigAmount = debuffInfo.amount;
					}

					status.moveSpeed *= championBaseStatus.moveSpeed * (mostBigAmount - 1f);
				}
				else
				{
					status.moveSpeed = 0f;
				}

				break;

			case DebuffImpactType.Freeze:
				if (_debuffInfoContainerActiveCount[(int)type] > 0)
				{
					if (false == _isShowFreezeEffect)
					{
						_audioSource.PlayOneShot(SoundStore.GetAudioClip("Freeze"));

						_freezeEffect.gameObject.SetActive(true);

						_ownerChampion.aiController.enabled = false;

						_isShowFreezeEffect = true;
					}
				}
				else
				{
					if (true == _isShowFreezeEffect)
					{
						_freezeEffect.gameObject.SetActive(false);

						_freezeEndEffect.transform.position = _ownerChampion.transform.position;
						_freezeEndEffect.gameObject.SetActive(true);

						_ownerChampion.aiController.enabled = true;
					}
				}

				break;
		}

		OnChangedStatus?.Invoke();
	}

	private void OnFreezeEndEffectEndAnimation(Effect effect)
	{
		if (effect != _freezeEndEffect)
			return;

		_freezeEndEffect.gameObject.SetActive(false);
		_isShowFreezeEffect = false;
	}

	private void ResetFreeze()
	{
		_isShowFreezeEffect = false;
		_freezeEffect.gameObject.SetActive(false);
		_freezeEndEffect.gameObject.SetActive(false);
	}

	private void SetPrevokeEffectActive(bool isActive)
	{
		if (null == _prevokeEffect)
			return;

		if(true == isActive)
		{
			_prevokeEffect.gameObject.SetActive(true);
		}
		else
		{
			_prevokeEffect.transform.localPosition = Vector3.zero;
			_prevokeEffect.gameObject.SetActive(false);
		}
	}

	// =========================================================================================================
	// --- 합연산 및 곱연산을 계산해주는 기능을 제공하는 함수들..
	// =========================================================================================================
	private void ComputeAddStatFigure(DebuffImpactType addCalcType, out int resultAddCalc)
	{
		resultAddCalc = 0;

		int index = (int)addCalcType;
		int loopCount = _debuffInfoContainerActiveCount[index];

		// 합연산 적용..
		for (int i = 0; i < loopCount; ++i)
		{
			DebuffInfo curBuffInfo = _debuffInfoContainer[index][i];

			resultAddCalc += (int)curBuffInfo.amount;
		}
	}

	private void ComputeMulStatFigure(DebuffImpactType mulCalcType, out float resultMulCal)
	{
		resultMulCal = 1f;

		// 곱연산 적용..
		int index = (int)mulCalcType;
		int loopCount = _debuffInfoContainerActiveCount[index];
		for (int i = 0; i < loopCount; ++i)
		{
			DebuffInfo curBuffInfo = _debuffInfoContainer[index][i];

			resultMulCal *= 1f + curBuffInfo.amount * 0.01f;
		}

		resultMulCal -= 1f;
	}

	private void ComputeAddAndMulStatFigure(DebuffImpactType addCalcType, DebuffImpactType mulCalcType, out int resultAddCalc, out float resultMulCal)
	{
		// 합연산 적용..
		ComputeAddStatFigure(addCalcType, out resultAddCalc);

		// 곱연산 적용..
		ComputeMulStatFigure(mulCalcType, out resultMulCal);
	}

	// 스탯 중 하나의 요소 재조정..
	// baseStatElement : 재조정할 요소의 base값(방어력의 경우 baseStat.defence가 된다)
	private int ReadjustStatusElement(int baseStatElement, DebuffImpactType addCalcType, DebuffImpactType mulCalcType)
	{
		// 곱연산, 합연산 계산..
		int resultAddCalc = 0;
		float resultMulCalc = 0f;
		ComputeAddAndMulStatFigure(addCalcType, mulCalcType, out resultAddCalc, out resultMulCalc);

		// 곱연산, 합연산 적용..
		return (int)(resultAddCalc + baseStatElement * resultMulCalc);
	}
}