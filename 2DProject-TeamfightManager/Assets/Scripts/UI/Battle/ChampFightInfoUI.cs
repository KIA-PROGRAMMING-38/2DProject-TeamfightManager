using UnityEngine;

/// <summary>
/// ���Ϸ� ���� �� ���� ���� è�Ǿ� �̹����� ȭ�鿡 �����ִ� UI..
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

	// è�Ǿ��� ���� ������ ���ŵ� �� ȣ��Ǵ� �ݹ� �Լ�..
	public void UpdateData(BattleInfoData data)
    {
		_champBattleDataUI.UpdateData(data);
    }

	// è�Ǿ� �Ҽ� ���� ���� ������ �ٸ��� ���ַ��� ���� �Լ�..
	public void SetBackgroundImage(BattleTeamKind teamKind)
	{
#if UNITY_EDITOR
		Debug.Assert(null != _champBattleDataUI);
		Debug.Assert(null != _pilotDataUI);
#endif

		_champBattleDataUI.SetBackgroundImage(teamKind);
		_pilotDataUI.SetBackgroundImage(teamKind);
	}

	// PilotDataUI���� ���Ϸ��� ���� �ʱ�ȭ�ؾ� �ϱ� ������ �޾Ƽ� �Ѱ��ش�..
	public void SetPilot(Pilot pilot)
	{
		_pilotDataUI.pilot = pilot;
	}
}
