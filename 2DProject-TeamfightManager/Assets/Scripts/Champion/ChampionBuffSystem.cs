using System;
using System.Collections.Generic;

public class ChampionBuffSystem
{
	class BuffInfo
	{
		public float amount;
		public float impactTime;
	}

	public event Action OnChangedStatus;

	public ChampionStatus status { get; private set; }
	public ChampionStatus championBaseStatus { private get; set; }

	private Champion _ownerChampion;

	private int _activeBuffTable; // 각 비트자리마다 그 자리에 매칭되는 버프가 활성화되있는지를 체크(BuffImpactType의 값 == 비트 자리)..
	private int _buffCount = 0;
	private List<BuffInfo>[] _buffInfoContainer;
	private int[] _buffInfoContainerActiveCount;
	private readonly int _buffInfoContainerDefaultSize = 4;

	public bool isEnded { get => _activeBuffTable == 0; }

	public ChampionBuffSystem(Champion ownerChampion)
	{
		_ownerChampion = ownerChampion;

		status = new ChampionStatus();

		// 버프 관련 정보들을 저장할 공간을 어느정도 미리 확보해둔다..
		_buffCount = (int)BuffImpactType.End;
		_buffInfoContainer = new List<BuffInfo>[_buffCount];
		_buffInfoContainerActiveCount = new int[_buffCount];
		for ( int i = 0; i < _buffCount; ++i)
		{
			_buffInfoContainer[i] = new List<BuffInfo>(_buffInfoContainerDefaultSize);
			_buffInfoContainerActiveCount[i] = 0;

			for( int j = 0; j < _buffInfoContainerDefaultSize; ++j)
			{
				_buffInfoContainer[i].Add(new BuffInfo());
			}
		}
	}

	public void Update(float deltaTime)
	{
		for (int i = 0; i < _buffCount; ++i)
		{
			bool curBitDigit = ((_activeBuffTable & (1 << i)) != 0);

			// 비활성화된 버프라면 검사에서 제외..
			if (false == curBitDigit)
			{
				continue;
			}

			// 버프 지속시간 갱신..
			bool isReadjustStatus = false;
			for( int j = 0; j < _buffInfoContainerActiveCount[i]; ++j)
			{
				BuffInfo curBuffInfo = _buffInfoContainer[i][j];

				curBuffInfo.impactTime -= deltaTime;

				if (curBuffInfo.impactTime <= 0f)
				{
					--_buffInfoContainerActiveCount[i];
					--j;

					int lastIndex = _buffInfoContainerActiveCount[i];

					_buffInfoContainer[i][j].amount = _buffInfoContainer[i][lastIndex].amount;
					_buffInfoContainer[i][j].impactTime = _buffInfoContainer[i][lastIndex].impactTime;

					if (_buffInfoContainerActiveCount[i] == 0)
						SetActiveBuff((BuffImpactType)i, false);

					isReadjustStatus = true;
				}
			}

			// 스탯 재조정을 해야한다면..
			if (isReadjustStatus)
			{
				UpdateStatus((BuffImpactType)i);
			}
		}
	}

	public void Reset()
	{
		_activeBuffTable = 0;

		status.atkStat = 0;
		status.atkSpeed = 0f;
		status.range = 0f;
		status.defence = 0;
		status.hp = 0;
		status.moveSpeed = 0f;
		status.skillCooltime = 0f;
	}

	public void AddBuff(BuffImpactType type, float amount, float impactTime)
	{
		int index = (int)type;

		if (_buffInfoContainer[index].Count <= _buffInfoContainerActiveCount[index])
			_buffInfoContainer[index].Add(new BuffInfo());

		_buffInfoContainer[index][_buffInfoContainerActiveCount[index]].amount = amount;
		_buffInfoContainer[index][_buffInfoContainerActiveCount[index]].impactTime = impactTime;

		if (0 == _buffInfoContainerActiveCount[index])
			SetActiveBuff(type, true);

		++_buffInfoContainerActiveCount[index];

		UpdateStatus(type);
	}

	private void SetActiveBuff(BuffImpactType type, bool isActive) 
	{
		int value = 1 << (int)type;

		if(true == isActive)
		{
			if((_activeBuffTable & value) == 0)
			{
				_activeBuffTable = _activeBuffTable | value;
			}
		}
		else
		{
			if ((_activeBuffTable & value) != 0)
			{
				_activeBuffTable = _activeBuffTable ^ value;
			}
		}
	}

	// 버프가 추가 혹은 끝남으로 인해 스탯 재조정이 필요할 때 호출되는 함수..
	private void UpdateStatus(BuffImpactType buffType)
	{
		switch (buffType)
		{
			case BuffImpactType.Hill:	// 힐은 한 번만 적용되기 때문에 적용하고 바로 초기화..
				{
					int hillAmount = 0;
					ComputeAddStatFigure(buffType, out hillAmount);

					_ownerChampion.RecoverHP(_ownerChampion, hillAmount);

					_buffInfoContainerActiveCount[(int)buffType] = 0;
				}

				return;
			case BuffImpactType.DefenceUp_Add:
			case BuffImpactType.DefenceUp_Percent:
				status.defence = ReadjustStatusElement(championBaseStatus.defence, BuffImpactType.DefenceUp_Add, BuffImpactType.DefenceUp_Percent);

				break;
			case BuffImpactType.AtkStatUp_Add:
			case BuffImpactType.AtkStatUp_Percent:
				status.atkStat = ReadjustStatusElement(championBaseStatus.atkStat, BuffImpactType.AtkStatUp_Add, BuffImpactType.AtkStatUp_Percent);

				break;
			case BuffImpactType.AtkSpeedUp_Percent:
				ComputeMulStatFigure(buffType, out status.atkSpeed);
				status.atkSpeed *= championBaseStatus.atkSpeed;

				break;
			case BuffImpactType.RangeUp_Percent:
				ComputeMulStatFigure(buffType, out status.range);
				status.range *= championBaseStatus.range;

				break;
			case BuffImpactType.MoveSpeedUp_Percent:
				ComputeMulStatFigure(buffType, out status.moveSpeed);
				status.moveSpeed *= championBaseStatus.moveSpeed;

				break;
		}

		OnChangedStatus?.Invoke();
	}

	// =========================================================================================================
	// --- 합연산 및 곱연산을 계산해주는 기능을 제공하는 함수들..
	// =========================================================================================================
	private void ComputeAddStatFigure(BuffImpactType addCalcType, out int resultAddCalc)
	{
		resultAddCalc = 0;

		int index = (int)addCalcType;
		int loopCount = _buffInfoContainerActiveCount[index];

		// 합연산 적용..
		for (int i = 0; i < loopCount; ++i)
		{
			BuffInfo curBuffInfo = _buffInfoContainer[index][i];

			resultAddCalc += (int)curBuffInfo.amount;
		}
	}

	private void ComputeMulStatFigure(BuffImpactType mulCalcType, out float resultMulCal)
	{
		resultMulCal = 1f;

		// 곱연산 적용..
		int index = (int)mulCalcType;
		int loopCount = _buffInfoContainerActiveCount[index];
		for (int i = 0; i < loopCount; ++i)
		{
			BuffInfo curBuffInfo = _buffInfoContainer[index][i];

			resultMulCal *= 1f + curBuffInfo.amount * 0.01f;
		}
	}

	private void ComputeAddAndMulStatFigure(BuffImpactType addCalcType, BuffImpactType mulCalcType, out int resultAddCalc, out float resultMulCal)
	{
		// 합연산 적용..
		ComputeAddStatFigure(addCalcType, out resultAddCalc);

		// 곱연산 적용..
		ComputeMulStatFigure(mulCalcType, out resultMulCal);
	}

	// 스탯 중 하나의 요소 재조정..
	// baseStatElement : 재조정할 요소의 base값(방어력의 경우 baseStat.defence가 된다)
	private int ReadjustStatusElement(int baseStatElement, BuffImpactType addCalcType, BuffImpactType mulCalcType)
	{
		// 곱연산, 합연산 계산..
		int resultAddCalc = 0;
		float resultMulCalc = 0f;
		ComputeAddAndMulStatFigure(addCalcType, mulCalcType, out resultAddCalc, out resultMulCalc);

		// 곱연산, 합연산 적용..
		return resultAddCalc + (int)(baseStatElement * resultMulCalc);
	}
}