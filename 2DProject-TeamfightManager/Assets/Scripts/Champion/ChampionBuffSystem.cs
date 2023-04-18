using System;
using System.Collections.Generic;

public class ChampionBuffSystem
{
	class BuffInfo
	{
		public Champion didChampion;
		public float amount;
		public float impactTime;
	}

	public event Action OnChangedStatus;
	public event Action<int> OnChangedBarrierAmount;

	public ChampionStatus status { get; private set; }
	private ChampionStatus buffStatus;

	private ChampionStatus _championBaseStatus;
	public ChampionStatus championBaseStatus
	{
		private get => _championBaseStatus;
		set
		{
			_championBaseStatus = value;
			_barrierSystem.championBaseStatus = value;
		}
	}

	private Champion _ownerChampion;

	private int _activeBuffTable; // 각 비트자리마다 그 자리에 매칭되는 버프가 활성화되있는지를 체크(BuffImpactType의 값 == 비트 자리)..
	private int _buffCount = 0;
	private List<BuffInfo>[] _buffInfoContainer;
	private int[] _buffInfoContainerActiveCount;
	private readonly int BUFF_INFO_CONTAINER_DEFAULTSIZE = 4;

	private ChampionBarrierSystem _barrierSystem;
	public int totalBarrierAmount { get; private set; }

	public bool isEnded { get => _activeBuffTable == 0; }

	public ChampionBuffSystem(Champion ownerChampion)
	{
		_ownerChampion = ownerChampion;

		status = new ChampionStatus();
		buffStatus = new ChampionStatus();

		// 버프 관련 정보들을 저장할 공간을 어느정도 미리 확보해둔다..
		_buffCount = (int)BuffImpactType.End;
		_buffInfoContainer = new List<BuffInfo>[_buffCount];
		_buffInfoContainerActiveCount = new int[_buffCount];
		for ( int i = 0; i < _buffCount; ++i)
		{
			_buffInfoContainer[i] = new List<BuffInfo>(BUFF_INFO_CONTAINER_DEFAULTSIZE);
			_buffInfoContainerActiveCount[i] = 0;

			for( int j = 0; j < BUFF_INFO_CONTAINER_DEFAULTSIZE; ++j)
			{
				_buffInfoContainer[i].Add(new BuffInfo());
			}
		}

		_barrierSystem = new ChampionBarrierSystem(ownerChampion);

		_barrierSystem.OnChangedStatus -= UpdateBarrierStatus;
		_barrierSystem.OnChangedStatus += UpdateBarrierStatus;

		_barrierSystem.OnChangedBarrier -= UpdateBarrierAmount;
		_barrierSystem.OnChangedBarrier += UpdateBarrierAmount;
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
					int lastIndex = _buffInfoContainerActiveCount[i] - 1;

					_buffInfoContainer[i][j].amount = _buffInfoContainer[i][lastIndex].amount;
					_buffInfoContainer[i][j].impactTime = _buffInfoContainer[i][lastIndex].impactTime;

					if (_buffInfoContainerActiveCount[i] == 0)
						SetActiveBuff((BuffImpactType)i, false);

					isReadjustStatus = true;

					--_buffInfoContainerActiveCount[i];
					--j;
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

		_barrierSystem.Reset();

		buffStatus.atkStat = 0;
		buffStatus.atkSpeed = 0f;
		buffStatus.range = 0f;
		buffStatus.defence = 0;
		buffStatus.hp = 0;
		buffStatus.moveSpeed = 0f;
		buffStatus.skillCooltime = 0f;

		UpdateResultStatus();
	}

	public void AddBuff(BuffImpactType type, Champion didChampion, float amount, float impactTime)
	{
		// 배리어 관련 버프인지 먼저 체크..
		switch (type)
		{
			case BuffImpactType.Barrier:
			case BuffImpactType.Barrier_MoveSpeed:
				_barrierSystem.AddBarrier(type, amount);
				return;
		}

		int index = (int)type;

		if (_buffInfoContainer[index].Count <= _buffInfoContainerActiveCount[index])
			_buffInfoContainer[index].Add(new BuffInfo());

		_buffInfoContainer[index][_buffInfoContainerActiveCount[index]].didChampion = didChampion;
		_buffInfoContainer[index][_buffInfoContainerActiveCount[index]].amount = amount;
		_buffInfoContainer[index][_buffInfoContainerActiveCount[index]].impactTime = impactTime;

		if (0 == _buffInfoContainerActiveCount[index])
			SetActiveBuff(type, true);

		++_buffInfoContainerActiveCount[index];

		UpdateStatus(type);
	}

	public int TakeDamage(int damage)
	{
		return _barrierSystem.TakeDamage(damage);
	}

	private void UpdateBarrierStatus()
	{
		UpdateResultStatus(true);
	}

	private void UpdateBarrierAmount()
	{
		totalBarrierAmount = _barrierSystem.totalBarrierAmount;

		OnChangedBarrierAmount?.Invoke(totalBarrierAmount);
	}

	private void UpdateResultStatus(bool isEventFuncCall = false)
	{
		totalBarrierAmount = _barrierSystem.totalBarrierAmount;

		status.atkStat = buffStatus.atkStat + _barrierSystem.status.atkStat;
		status.atkSpeed = buffStatus.atkSpeed + _barrierSystem.status.atkSpeed;
		status.range = buffStatus.range + _barrierSystem.status.range;
		status.defence = buffStatus.defence + _barrierSystem.status.defence;
		status.hp = buffStatus.hp + _barrierSystem.status.hp;
		status.moveSpeed = buffStatus.moveSpeed + _barrierSystem.status.moveSpeed;
		status.skillCooltime = buffStatus.skillCooltime + _barrierSystem.status.skillCooltime;

		if (true == isEventFuncCall)
			OnChangedStatus?.Invoke();

		OnChangedBarrierAmount?.Invoke(totalBarrierAmount);
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
				buffStatus.defence = ReadjustStatusElement(championBaseStatus.defence, BuffImpactType.DefenceUp_Add, BuffImpactType.DefenceUp_Percent);

				break;
			case BuffImpactType.AtkStatUp_Add:
			case BuffImpactType.AtkStatUp_Percent:
				buffStatus.atkStat = ReadjustStatusElement(championBaseStatus.atkStat, BuffImpactType.AtkStatUp_Add, BuffImpactType.AtkStatUp_Percent);

				break;
			case BuffImpactType.AtkSpeedUp_Percent:
				ComputeMulStatFigure(buffType, out buffStatus.atkSpeed);
				buffStatus.atkSpeed *= championBaseStatus.atkSpeed;

				break;
			case BuffImpactType.RangeUp_Percent:
				ComputeMulStatFigure(buffType, out buffStatus.range);
				buffStatus.range *= championBaseStatus.range;

				break;
			case BuffImpactType.MoveSpeedUp_Percent:
				ComputeMulStatFigure(buffType, out buffStatus.moveSpeed);
				buffStatus.moveSpeed *= championBaseStatus.moveSpeed;

				break;
		}

		UpdateResultStatus(true);
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

			resultMulCal *= curBuffInfo.amount;
		}

		resultMulCal -= 1f;
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