using System;

public class BattleStageDataTable
{
	public event Action<float> OnUpdateBattleRemainTime;

	private float _gameBattleTime = 0f;
	public float updateTime
	{
		set
		{
			_gameBattleTime -= value;

			_gameBattleTime = MathF.Max(0f, _gameBattleTime);
			OnUpdateBattleRemainTime?.Invoke(_gameBattleTime);
		}
	}

	public void Initialize(float gameBattleTime)
	{
		_gameBattleTime = gameBattleTime;
	}

	public void Reset()
	{
		OnUpdateBattleRemainTime = null;
	}
}