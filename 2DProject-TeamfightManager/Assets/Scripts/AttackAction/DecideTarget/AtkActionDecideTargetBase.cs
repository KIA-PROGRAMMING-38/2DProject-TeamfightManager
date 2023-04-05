using UnityEngine;

public abstract class AtkActionDecideTargetBase
{
	public Champion ownerChampion { protected get; set; }
	protected AttackActionData actionData { get; private set; }

	public AtkActionDecideTargetBase(AttackActionData actionData)
	{
		this.actionData = actionData;
	}

	public abstract void OnStart();
	public abstract int FindTarget(Champion[] getTargetArray);
	public abstract void OnEnd();
}