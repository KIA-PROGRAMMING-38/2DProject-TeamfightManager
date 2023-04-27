using System;
using UnityEngine;

public class SummonChamp : SummonObject
{
	private Champion _controlChampion;
	private ChampStatusBar _statusBar;

	[SerializeField] private string _summonChampName;
	[SerializeField] private bool _isHideUltIcon;

	new private void Awake()
	{
		_statusBar = GetComponentInChildren<ChampStatusBar>();
	}

	private void Start()
	{
		_statusBar.SetUltimateIconHide(_isHideUltIcon);
	}

	public void SetAdditionalData(Champion owner)
	{
		// 컨트롤할 챔피언 가져오고 챔피언의 pilot을 ownerChampion의 pilot으로 설정
		_controlChampion = championManager.GetChampionInstance(_summonChampName);
		owner.pilotBattleComponent.AddSummonChampion(_controlChampion);
		_controlChampion.transform.position = transform.position;
		_controlChampion.gameObject.SetActive(true);
		_controlChampion.StartFight();

		_statusBar.target = _controlChampion.transform;
		_statusBar.teamKind = _controlChampion.pilotBattleComponent.myTeam.battleTeamKind;

		// Status Bar 갱신을 위한 이벤트만 구독..
		_controlChampion.OnChangedHPRatio -= OnChangeChampionHPRatio;
		_controlChampion.OnChangedHPRatio += OnChangeChampionHPRatio;

		_controlChampion.OnChangedHPRatio -= OnChangeChampionMPRatio;
		_controlChampion.OnChangedHPRatio += OnChangeChampionMPRatio;

		_controlChampion.OnChangedBarrierRatio -= OnChangeChampionBarrierRatio;
		_controlChampion.OnChangedBarrierRatio += OnChangeChampionBarrierRatio;

		_controlChampion.isSkillCooltime = false;

		if (false == _isHideUltIcon)
		{
			_statusBar.SetUltimateIconSprite(dataTableManager.championDataTable.GetUltimateIconImage(_summonChampName));
			_statusBar.SetUltimateActive(false);
		}
	}

	public void OnChangeChampionHPRatio(float ratio)
	{
		if(ratio <= 0f)
		{
			ReceiveReleaseEvent();
			summonObjectManager.ReleaseSummonObject(this);

			return;
		}

		_statusBar.SetHPRatio(ratio);
	}

	public void OnChangeChampionMPRatio(float ratio)
	{
		_statusBar.SetMPRatio(ratio);
	}

	public void OnChangeChampionBarrierRatio(float ratio)
	{
		_statusBar.SetBarrierRatio(ratio);
	}
}
