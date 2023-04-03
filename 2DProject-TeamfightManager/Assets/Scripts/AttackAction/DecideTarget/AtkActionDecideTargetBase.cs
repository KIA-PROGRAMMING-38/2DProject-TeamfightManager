using UnityEngine;

public abstract class AtkActionDecideTargetBase
{
	protected Champion ownerChampion { get; private set; }
	protected AttackActionData actionData { get; private set; }

	public AtkActionDecideTargetBase(Champion champion, AttackActionData actionData)
	{
		ownerChampion = champion;
		this.actionData = actionData;
	}

	public abstract void OnStart();
	public abstract int FindTarget(Champion[] getTargetArray);
	public abstract void OnEnd();
}