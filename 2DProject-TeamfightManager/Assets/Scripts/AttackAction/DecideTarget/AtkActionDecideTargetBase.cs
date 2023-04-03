using UnityEngine;

public abstract class AtkActionDecideTargetBase
{
	protected Champion ownerChampion { get; private set; }
	protected float realizeRange { get; private set; }

	public AtkActionDecideTargetBase(Champion champion, float realizeRange)
	{
		ownerChampion = champion;
		this.realizeRange = realizeRange;
	}

	public abstract void OnStart();
	public abstract int FindTarget(Champion[] getTargetArray);
	public abstract void OnEnd();
}