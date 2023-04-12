﻿using System;
using System.Collections.Generic;

public class ChampionDeBuffSystem
{
	class DebuffInfo
	{
		public float amount;
		public float impactTime;
	}

	public event Action OnChangedStatus;

	public ChampionStatus status { get; private set; }
	public ChampionStatus championBaseStatus { private get; set; }

	private Champion _ownerChampion;

	private int _activeDebuffTable; // 각 비트자리마다 그 자리에 매칭되는 버프가 활성화되있는지를 체크(DeBuffImpactType의 값 == 비트 자리)..
	private int _debuffCount = 0;
	private List<DebuffInfo>[] _debuffInfoContainer;
	private int[] _debuffInfoContainerActiveCount;
	private readonly int _debuffInfoContainerDefaultSize = 4;

	public float hillAmountDebuff { get; private set; }

	public bool isEnded { get => _activeDebuffTable == 0; }

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

		hillAmountDebuff = 1f;
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
	}

	public void AddBuff(DebuffImpactType type, float amount, float impactTime)
	{
		int index = (int)type;

		if (_debuffInfoContainer[index].Count <= _debuffInfoContainerActiveCount[index])
			_debuffInfoContainer[index].Add(new DebuffInfo());

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
			case DebuffImpactType.HillAmount_Percent:
				float result = 0f;
				ComputeMulStatFigure(type, out result);
				hillAmountDebuff = result;

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
		}

		OnChangedStatus?.Invoke();
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