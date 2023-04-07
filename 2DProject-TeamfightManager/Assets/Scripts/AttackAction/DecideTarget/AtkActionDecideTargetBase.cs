using UnityEngine;

/// <summary>
/// 공격 행동의 효과를 적용받는 대상을 찾는 로직의 최상위 클래스..
/// </summary>
public abstract class AtkActionDecideTargetBase
{
	public Champion ownerChampion { protected get; set; }
	protected AttackAction attackAction { get; private set; }
	protected AttackActionData actionData { get; private set; }

	public AtkActionDecideTargetBase(AttackAction attackAction, AttackActionData actionData)
	{
		this.actionData = actionData;
		this.attackAction = attackAction;
	}

	public abstract void OnStart();
	public abstract int FindTarget(Champion[] getTargetArray);
	public abstract void OnEnd();
}