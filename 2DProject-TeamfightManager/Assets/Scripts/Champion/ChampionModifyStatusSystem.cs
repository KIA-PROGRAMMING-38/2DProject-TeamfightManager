using System;
using UnityEngine;

public class ChampionModifyStatusSystem
{
	public event Action<ChampionStatus> OnChangedStatus;

	private ChampionStatus _status;

	private ChampionBuffSystem _buffSystem;
	private ChampionDeBuffSystem _debuffSystem;

	public EffectManager effectManager
	{
		set
		{
			_debuffSystem.effectManager = value;
		}
	}

	public bool isEnded { get => _buffSystem.isEnded && _debuffSystem.isEnded; }
	public ChampionStatus championBaseStatus
	{
		set
		{
			_buffSystem.championBaseStatus = value;
			_debuffSystem.championBaseStatus = value;
		}
	}

	public ChampionModifyStatusSystem(Champion ownerChampion)
	{
		_status = new ChampionStatus();

		_buffSystem = new ChampionBuffSystem(ownerChampion);
		_debuffSystem = new ChampionDeBuffSystem(ownerChampion);

		_buffSystem.OnChangedStatus -= UpdateStatus;
		_buffSystem.OnChangedStatus += UpdateStatus;

		_debuffSystem.OnChangedStatus -= UpdateStatus;
		_debuffSystem.OnChangedStatus += UpdateStatus;
	}

	public void Update()
	{
#if UNITY_EDITOR
		Debug.Assert(null != _buffSystem);
		Debug.Assert(null != _debuffSystem);
#endif

		float deltaTime = Time.deltaTime;

		_buffSystem.Update(deltaTime);
		_debuffSystem.Update(deltaTime);
	}

	public void Reset()
	{
		_buffSystem?.Reset();
		_debuffSystem?.Reset();

		UpdateStatus();
	}

	private void UpdateStatus()
	{
#if UNITY_EDITOR
		Debug.Assert(null != _buffSystem);
		Debug.Assert(null != _debuffSystem);
#endif

		_status.atkStat = _buffSystem.status.atkStat - _debuffSystem.status.atkStat;
		_status.atkSpeed = _buffSystem.status.atkSpeed - _debuffSystem.status.atkSpeed;
		_status.range = _buffSystem.status.range - _debuffSystem.status.range;
		_status.defence = _buffSystem.status.defence - _debuffSystem.status.defence;
		_status.hp = _buffSystem.status.hp - _debuffSystem.status.hp;
		_status.moveSpeed = _buffSystem.status.moveSpeed - _debuffSystem.status.moveSpeed;
		_status.skillCooltime = _buffSystem.status.skillCooltime - _debuffSystem.status.skillCooltime;

		OnChangedStatus?.Invoke(_status);

		Debug.Log("버프 디버프로 인한 스탯 갱신..");
	}

	public int CalcHillDebuff(int hillAmount)
	{
		return (int)(hillAmount * (2f - _debuffSystem.hillAmountDebuff));
	}

	public void AddBuff(BuffImpactType type, Champion didChampion, float amount, float impactTime)
	{
		_buffSystem?.AddBuff(type, didChampion, amount, impactTime);
	}

	public void AddDebuff(DebuffImpactType type, Champion didChampion, float amount, float impactTime)
	{
		_debuffSystem?.AddDebuff(type, didChampion, amount, impactTime);
	}
}