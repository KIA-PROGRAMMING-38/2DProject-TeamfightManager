using UnityEngine;

/// <summary>
/// 파일럿 정보 및 현재 픽한 챔피언 이미지를 화면에 보여주는 UI..
/// </summary>
public class ChampFightInfoUI : UIBase
{
    private ChampBattleDataUI _champBattleDataUI;
    private PilotDataUI _pilotDataUI;

	private void Awake()
	{
		_champBattleDataUI = GetComponentInChildren<ChampBattleDataUI>();
		_pilotDataUI = GetComponentInChildren<PilotDataUI>();
	}

	// 챔피언의 전투 정보가 갱신될 때 호출되는 콜백 함수..
	public void UpdateData(BattleInfoData data)
    {
		_champBattleDataUI.UpdateData(data);
    }

	// 챔피언 소속 팀에 따라 배경색을 다르게 해주려고 만든 함수..
	public void SetBackgroundImage(BattleTeamKind teamKind)
	{
#if UNITY_EDITOR
		Debug.Assert(null != _champBattleDataUI);
		Debug.Assert(null != _pilotDataUI);
#endif

		_champBattleDataUI.SetBackgroundImage(teamKind);
		_pilotDataUI.SetBackgroundImage(teamKind);
	}

	// PilotDataUI에서 파일럿에 따라 초기화해야 하기 때문에 받아서 넘겨준다..
	public void SetPilot(Pilot pilot)
	{
		_pilotDataUI.pilot = pilot;
	}
}
