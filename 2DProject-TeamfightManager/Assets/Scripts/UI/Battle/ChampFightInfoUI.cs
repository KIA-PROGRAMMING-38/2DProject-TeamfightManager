using UnityEngine;

public class ChampFightInfoUI : UIBase
{
    private ChampBattleDataUI _champBattleDataUI;
    private PilotDataUI _pilotDataUI;

	private void Awake()
	{
		_champBattleDataUI = GetComponentInChildren<ChampBattleDataUI>();
		_pilotDataUI = GetComponentInChildren<PilotDataUI>();
	}

	public void UpdateData(BattleInfoData data)
    {
		_champBattleDataUI.UpdateData(data);
    }

	public void SetBackgroundImage(BattleTeamKind teamKind)
	{
#if UNITY_EDITOR
		Debug.Assert(null != _champBattleDataUI);
		Debug.Assert(null != _pilotDataUI);
#endif

		_champBattleDataUI.SetBackgroundImage(teamKind);
		_pilotDataUI.SetBackgroundImage(teamKind);
	}
}
