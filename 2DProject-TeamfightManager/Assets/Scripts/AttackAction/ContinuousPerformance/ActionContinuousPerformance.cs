/// <summary>
/// 공격 행동 퍼포먼스 로직 최상위 클래스..
/// </summary>
public abstract class ActionContinuousPerformance
{
	protected AttackPerformanceData performanceData { get; private set; }
	protected AttackAction attackAction { get; private set; }
	public bool isEndPerformance { get; protected set; }

	public ActionContinuousPerformance(AttackAction attackAction, AttackPerformanceData performanceData)
	{
		this.performanceData = performanceData;
		this.attackAction = attackAction;
	}

	public Champion ownerChampion { protected get; set; }

	public abstract void OnStart();
	public abstract void OnUpdate();
	public abstract void OnAction();
	public abstract void OnEnd();
}