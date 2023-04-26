using System;
using System.Collections.Generic;

public class ChampionBarrierSystem
{
	class BarrierInfo
	{
		public int barrierAmount;
		public BuffImpactType plusBuffType;
		public float buffAmount;
	}

	public event Action OnChangedStatus;
	public event Action OnChangedBarrier;

	public ChampionStatus status { get; private set; }
	public ChampionStatus championBaseStatus { private get; set; }

	private Champion _ownerChampion;

	public int totalBarrierAmount { get; private set; }

	private List<BarrierInfo> _barrierInfoContainer;
	private BarrierInfo _curBarrierInfo;
	private int _barrierContainerLastIndex;
	private readonly int CONTAINER_DEFAULTSIZE = 5;

	public ChampionBarrierSystem(Champion ownerChampion)
	{
		_ownerChampion = ownerChampion;

		status = new ChampionStatus();

		// 버프 관련 정보들을 저장할 공간을 어느정도 미리 확보해둔다..
		_curBarrierInfo = null;
		_barrierInfoContainer = new List<BarrierInfo>(CONTAINER_DEFAULTSIZE);
		_barrierContainerLastIndex = -1;
		for (int i = 0; i < CONTAINER_DEFAULTSIZE; ++i)
		{
			_barrierInfoContainer.Add(new BarrierInfo());
		}
	}

	public void Reset()
	{
		totalBarrierAmount = 0;
		_barrierContainerLastIndex = -1;

		ResetStat();
	}

	private void ResetStat()
	{
		status.atkStat = 0;
		status.atkSpeed = 0f;
		status.range = 0f;
		status.defence = 0;
		status.hp = 0;
		status.moveSpeed = 0f;
		status.skillCooltime = 0f;
	}

	public int TakeDamage(int damage)
	{
		if (null == _curBarrierInfo)
			return damage;

		bool isNeedUpdate = false;

		int remainDamage = damage;
		while(remainDamage > 0)
		{
			damage = Math.Min(_curBarrierInfo.barrierAmount, damage);
			remainDamage -= damage;
			_curBarrierInfo.barrierAmount -= damage;

			if (_curBarrierInfo.barrierAmount <= 0)
			{
				isNeedUpdate = true;

				--_barrierContainerLastIndex;

				if (0 > _barrierContainerLastIndex)
				{
					_curBarrierInfo = null;

					break;
				}

				_curBarrierInfo = _barrierInfoContainer[_barrierContainerLastIndex];
			}
		}

		if (true == isNeedUpdate)
			UpdateStatus();
		else
			ReduceTotalBarrier(damage);

		return remainDamage;
	}

	public void AddBarrier(BuffImpactType type, float amount)
	{
#if UNITY_EDITOR
		if (_barrierInfoContainer.Count <= _barrierContainerLastIndex + 1)
			_barrierInfoContainer.Add(new BarrierInfo());
#endif

		switch (type)
		{
			case BuffImpactType.Barrier:
				++_barrierContainerLastIndex;
				_curBarrierInfo = _barrierInfoContainer[_barrierContainerLastIndex];

				_curBarrierInfo.barrierAmount = (int)(amount * championBaseStatus.hp);
				_curBarrierInfo.plusBuffType = BuffImpactType.Barrier_End;
				_curBarrierInfo.buffAmount = 0f;

				break;
			case BuffImpactType.Barrier_MoveSpeed:
				_curBarrierInfo.plusBuffType = BuffImpactType.Barrier_MoveSpeed;
				_curBarrierInfo.buffAmount = amount;

				break;

			case BuffImpactType.Barrier_AtkSpeed:
                _curBarrierInfo.plusBuffType = BuffImpactType.Barrier_AtkSpeed;
                _curBarrierInfo.buffAmount = amount;

                break;
		}

		UpdateStatus();
	}

	private void UpdateStatus()
	{
		totalBarrierAmount = 0;

		ResetStat();

		for ( int i = 0; i <= _barrierContainerLastIndex; ++i)
		{
			totalBarrierAmount += _barrierInfoContainer[i].barrierAmount;
			switch (_barrierInfoContainer[i].plusBuffType)
			{
				case BuffImpactType.Barrier_MoveSpeed:
                    status.moveSpeed = Math.Max(status.moveSpeed, 1f) * _barrierInfoContainer[i].buffAmount;
                    break;

				case BuffImpactType.Barrier_AtkSpeed:
					status.atkSpeed = Math.Max(status.atkSpeed, 1f) * _barrierInfoContainer[i].buffAmount;

					break;
			}
		}

		status.moveSpeed = championBaseStatus.moveSpeed * Math.Max(0f, (status.moveSpeed - 1f));
		status.atkSpeed = championBaseStatus.moveSpeed * Math.Max(0f, (status.atkSpeed - 1f));

		OnChangedStatus?.Invoke();
	}

	private void ReduceTotalBarrier(int barrier)
	{
		totalBarrierAmount -= barrier;
		if (totalBarrierAmount < 0)
			totalBarrierAmount = 0;

		OnChangedBarrier?.Invoke();
	}
}