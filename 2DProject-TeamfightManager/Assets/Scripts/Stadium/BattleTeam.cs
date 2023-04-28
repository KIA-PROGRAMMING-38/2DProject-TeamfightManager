using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// 배틀 스테이지에서 배틀을 하는 팀(하나의 팀)을 관리하기 위한 클래스..
/// </summary>
public class BattleTeam : MonoBehaviour
{
	public GameManager gameManager
	{
		set
		{
			_championManager = value.championManager;
			_pilotManager = value.pilotManager;
			_battleStageManager = value.battleStageManager;
			_battleStageDataTable = value.dataTableManager.battleStageDataTable;
			_championDataTable = value.dataTableManager.championDataTable;
			_effectManager = value.effectManager;

			_playerTeamName = value.gameGlobalData.playerTeamName;
		}
	}

	public List<Pilot> belongPilot
	{
		set
		{
			int pilotCount = value.Count;

			_pilots = new List<PilotBattle>(pilotCount);
			_activeChampions = new List<Champion>(pilotCount);
			_allChampions = new List<Champion>(pilotCount);

			for( int i = 0; i < pilotCount; ++i)
			{
				PilotBattle pilotBattle = value[i].GetComponent<PilotBattle>();

				_pilots.Add(pilotBattle);
				_allChampions.Add(null);

				SetupPilot(pilotBattle, i);
			}
		}
	}

	private ChampionManager _championManager;
	private PilotManager _pilotManager;
	private BattleStageManager _battleStageManager;
	private BattleStageDataTable _battleStageDataTable;
	private ChampionDataTable _championDataTable;
	private EffectManager _effectManager;

	public BattleTeam enemyTeam { private get; set; }

	public Vector2[] spawnArea;
	private Vector3 randomSpawnPoint
	{
		get
		{
			return new Vector3(UnityEngine.Random.Range(spawnArea[0].x, spawnArea[1].x), UnityEngine.Random.Range(spawnArea[0].y, spawnArea[1].y), 0f);
		}
	}

	private Team _teamComponent;
	public string teamName { get => _teamComponent.data.name; }

	private string _playerTeamName;
	private BattleTeamKind _battleTeamKind;
	public BattleTeamKind battleTeamKind
	{
		get => _battleTeamKind;
		set
		{
			_battleTeamKind = value;

			// 팀 종류에 따라 레이어 설정..
			switch (_battleTeamKind)
			{
				case BattleTeamKind.RedTeam:
					championLayer = LayerTable.Number.REDTEAM_CHAMPION;
					atkSummonLayer = LayerTable.Number.REDTEAM_ATKSUMMON;
					buffSummonLayer = LayerTable.Number.REDTEAM_BUFFSUMMON;

					break;
				case BattleTeamKind.BlueTeam:
					championLayer = LayerTable.Number.BLUETEAM_CHAMPION;
					atkSummonLayer = LayerTable.Number.BLUETEAM_ATKSUMMON;
					buffSummonLayer = LayerTable.Number.BLUETEAM_BUFFSUMMON;

					break;
			}
		}
	}

	private List<PilotBattle> _pilots;
	public List<BattlePilotFightData> battlePilotFightData
	{
		get
		{
			List<BattlePilotFightData> result = new List<BattlePilotFightData>();
			for (int i = 0; i < _pilots.Count; ++i)
				result.Add(_pilots[i].battlePilotFightData);

			return result;
		}
	}
	private List<Champion> _activeChampions;
	private List<Champion> _allChampions;

	// 챔피언 정보가 갱신될 때마다 외부의 구독자들에게 알려줄 이벤트들..
	public event Action<BattleTeamKind, int, BattleInfoData> OnChangedChampionBattleInfoData;
	public event Action<BattleTeamKind, int, float> OnChangedChampionHPRatio;
	public event Action<BattleTeamKind, int, float> OnChangedChampionMPRatio;
	public event Action<BattleTeamKind, int> OnChampionUseUltimate;
	public event Action<BattleTeamKind, int, float> OnChangedChampionBarrierRatio;

	// 부활 관련 필드들..
	private float _revivalWaitTime = 3.5f;
	private float _revivalDelayTime = 0.5f;
    private WaitForSeconds _revivalWaitSecInst;
	private WaitForSeconds _revivalDelaySecInst;

    private List<IEnumerator> _revivalCoroutines = new List<IEnumerator>();

	private bool _isBattleEnd = false;

	public int championLayer { get; private set; }
	public int atkSummonLayer { get; private set; }
	public int buffSummonLayer { get; private set; }

	private void Awake()
	{
		_teamComponent = GetComponent<Team>();
	}

	private void Start()
	{
		_revivalWaitSecInst = YieldInstructionStore.GetWaitForSec(_revivalWaitTime);
		_revivalDelaySecInst = YieldInstructionStore.GetWaitForSec(_revivalDelayTime);
    }

	/// <summary>
	/// 소속될 파일럿을 추가해주는 함수..
	/// 배틀 스테이지에서 사용될 파일럿과 챔피언을 받아와 저장한다..
	/// </summary>
	/// <param name="pilotName"></param>
	/// <param name="champName"></param>
	private void SetupPilot(PilotBattle pilotBattle, int index)
    {
#if UNITY_EDITOR
		Debug.Assert(null != pilotBattle);
#endif
		// 초기화..
		pilotBattle.myTeam = this;
		pilotBattle.battleTeamIndexKey = index;

		// Pilot Battle Component 를 나의 팀으로 넣기..
		_pilots[index] = pilotBattle;

		// 파일럿 이벤트 연결..
		pilotBattle.OnChangedBattleInfoData -= OnChangedChampionBattleData;
		pilotBattle.OnChangedBattleInfoData += OnChangedChampionBattleData;

		pilotBattle.OnChangedChampionHPRatio -= UpdateChampionHPRatio;
		pilotBattle.OnChangedChampionHPRatio += UpdateChampionHPRatio;

		pilotBattle.OnChangedChampionMPRatio -= UpdateChampionMPRatio;
		pilotBattle.OnChangedChampionMPRatio += UpdateChampionMPRatio;

		pilotBattle.OnChampionUseUltimate -= UpdateChampionUseUltimateState;
		pilotBattle.OnChampionUseUltimate += UpdateChampionUseUltimateState;

		pilotBattle.OnChangedChampionBarrierRatio -= UpdateChampionBarrierRatio;
		pilotBattle.OnChangedChampionBarrierRatio += UpdateChampionBarrierRatio;

		// 코루틴 등록..
		_revivalCoroutines.Add(WaitRevival(_pilots.Count - 1));
	}

	public void AddChampion(int index, string championName)
	{
		// 매니저에게서 인스턴스를 받아온다..
		Champion champion = _championManager.GetChampionInstance(championName);
		PilotBattle pilotBattleComponent = _pilots[index];

#if UNITY_EDITOR
		Debug.Assert(null != champion);
		Debug.Assert(null != pilotBattleComponent);

		champion.gameObject.name = championName;
#endif

		// 초기화..
		pilotBattleComponent.controlChampion = champion;
		champion.transform.position = randomSpawnPoint;
		champion.gameObject.layer = championLayer;
		champion.gameObject.SetActive(false);

		// 챔피언 목록 및 활성화 챔피언 목록에 추가..
		_allChampions[index] = champion;
	}

	public void StartBattle()
	{
		_isBattleEnd = false;

		int loopCount = _allChampions.Count;
		for( int i = 0; i < loopCount; ++i)
		{
			if (null == _allChampions[i])
				continue;

			_pilots[i].gameObject.SetActive(true);
			_pilots[i].StartBattle((BattleTeamKind.BlueTeam == battleTeamKind) ? 15f : 30f);

			_allChampions[i].gameObject.SetActive(true);
			_allChampions[i].StartFight();
			_activeChampions.Add(_allChampions[i]);
		}
	}

	public Champion GetChampion(int index)
	{
		return _pilots[index].controlChampion;
	}

	public Pilot GetPilot(int index)
	{
		return _pilots[index].pilot;
	}

	public void OnChampionDead(int pilotIndex)
	{
		// 활성화 목록에서 지운다..
		int activeChampCount = _activeChampions.Count;
		for( int i = 0; i < activeChampCount; ++i)
		{
			if (_activeChampions[i] == _allChampions[pilotIndex])
			{
				_activeChampions.RemoveAt(i);

				_battleStageManager.OnChampionDeadState(battleTeamKind, pilotIndex);

				StartCoroutine(_revivalCoroutines[pilotIndex]);

				break;
			}
		}
	}

	IEnumerator WaitRevival(int pilotIndex)
	{
		while(true)
		{
			yield return _revivalWaitSecInst;

			// 부활 이펙트 On..
			Vector3 spawnPosition = randomSpawnPoint;
            Effect effect = _effectManager.GetEffect("Effect_Revival", spawnPosition);
            effect.gameObject.SetActive( true );

            yield return _revivalDelaySecInst;

            _battleStageManager.OnChampionRevivalState(battleTeamKind, pilotIndex);

			OnSuccessRevival(_allChampions[pilotIndex], spawnPosition);

			StopCoroutine(_revivalCoroutines[pilotIndex]);

			yield return null;
		}
	}

	public void OnSuccessRevival(Champion champion, in Vector3 spawnPosition)
	{
		if (true == _isBattleEnd)
			return;

		champion.Revival();

		champion.transform.position = spawnPosition;
		champion.gameObject.SetActive(true);

        _activeChampions.Add( champion );
    }

	public void OnBattleEnd()
	{
		StopAllCoroutines();

		int loopCount = _pilots.Count;
		for (int i = 0; i < loopCount; ++i)
		{
			_pilots[i].StopChampionLogic();
		}

		_isBattleEnd = true;
	}

	public void ExitBattleStage()
	{
		int loopCount = _pilots.Count;
		for (int i = 0; i < loopCount; ++i)
		{
			_pilots[i].Release();
			_allChampions[i] = null;
		}

		_activeChampions.Clear();
	}

	public void ProgressMyTurnBanpick(BanpickRunner banpickRunner)
	{
		banpickRunner.SetReceiveButtonEventState(true);

		return;

		bool isPlayerTeam = teamName == _playerTeamName;

		banpickRunner.SetReceiveButtonEventState(isPlayerTeam);

		// Player Team이 아니라면 AI에 의해 고르도록 한다..
		if (false == isPlayerTeam)
		{
			StartCoroutine(DecideBanpickChampion(banpickRunner));
		}
	}

	// AI가 banpick할 챔피언을 결정하는 함수..
	IEnumerator DecideBanpickChampion(BanpickRunner banpickRunner)
	{
		float randomWaitTime = UnityEngine.Random.Range(1f, 3f);
		yield return YieldInstructionStore.GetWaitForSec(randomWaitTime);

		List<string> banpickChampionList = _battleStageDataTable.banpickChampionList;
		int banpickChampCount = banpickChampionList.Count;

		int totalChampCount = _championDataTable.GetTotalChampionCount();
		while (true)
		{
			bool isSuccessSelect = true;

			// 챔피언을 선택한 뒤 이미 밴 또는 픽한 챔피언인지 검사한다..
			int selectIndex = UnityEngine.Random.Range(0, totalChampCount);
			string championName = _championDataTable.GetChampionName(selectIndex);
			for( int i = 0; i < banpickChampCount; ++i)
			{
				if (banpickChampionList[i] == championName)
				{
					isSuccessSelect = false;
					break;
				}
			}

			if( true == isSuccessSelect)
			{
				Debug.Log($"AI가 챔피언을 고름 : {championName}");
				banpickRunner.OnSelectChampion(championName);

				break;
			}
		}
	}

	// ==================================== 적을 찾는 함수들.. ====================================

	// originPoint를 기준으로 가장 가까운 적을 찾는 함수..
	public Champion FindTarget(Champion champion, FindTargetData findTargetData)
	{
		Champion target = null;
		Vector3 championPosition = champion.transform.position;
		float targetDistance = 0f;

		// 타겟 팀 종류에 따라 찾을 타겟 리스트 결정 후 찾음..
		switch (findTargetData.targetTeamKind)
		{
			case TargetTeamKind.Enemy:
				{
					int loopCount = enemyTeam._activeChampions.Count;
					for( int i = 0; i < loopCount; ++i)
					{
						Champion checkChampion = enemyTeam._activeChampions[i];

						if (checkChampion.isDead)
						{
							continue;
						}

						float distance = (checkChampion.transform.position - championPosition).magnitude;
						if(CheckPriorityLogic(checkChampion, target, targetDistance, distance, findTargetData.priorityKind))
						{
							target = checkChampion;
							targetDistance = distance;
						}
					}
				}

				break;
			case TargetTeamKind.Team:
				{
					int loopCount = _activeChampions.Count;
					for (int i = 0; i < loopCount; ++i)
					{
						Champion checkChampion = _activeChampions[i];
						if (checkChampion.isDead)
						{
							continue;
						}
						if (false == findTargetData.isIncludeMe && checkChampion == champion)
						{
							continue;
						}

						float distance = (checkChampion.transform.position - championPosition).magnitude;
						if (CheckPriorityLogic(checkChampion, target, targetDistance, distance, findTargetData.priorityKind))
						{
							target = checkChampion;
							targetDistance = distance;
						}
					}
				}

				break;
			case TargetTeamKind.Both:
				{
					int loopCount = enemyTeam._activeChampions.Count;
					for (int i = 0; i < loopCount; ++i)
					{
						Champion checkChampion = enemyTeam._activeChampions[i];
						if (checkChampion.isDead)
						{
							continue;
						}

						float distance = (checkChampion.transform.position - championPosition).magnitude;
						if (CheckPriorityLogic(checkChampion, target, targetDistance, distance, findTargetData.priorityKind))
						{
							target = checkChampion;
							targetDistance = distance;
						}
					}

					loopCount = _activeChampions.Count;
					for (int i = 0; i < loopCount; ++i)
					{
						Champion checkChampion = _activeChampions[i];
						if (checkChampion.isDead)
						{
							continue;
						}
						if (false == findTargetData.isIncludeMe && checkChampion == champion)
						{
							continue;
						}

						float distance = (checkChampion.transform.position - championPosition).magnitude;
						if (CheckPriorityLogic(checkChampion, target, targetDistance, distance, findTargetData.priorityKind))
						{
							target = checkChampion;
							targetDistance = distance;
						}
					}
				}

				break;
		}

		return target;
	}

	// 적을 찾는 로직을 받아와 계산한다..
	public int ComputeEnemyTarget(Func<Vector3, bool> findLogicFunction, Champion[] championCache, FindTargetData findTargetData)
	{
		int targetCount = 0;
		int championCacheLength = championCache.Length;

		List<Champion> computeContainer = null;
		switch (findTargetData.targetTeamKind)
		{
			case TargetTeamKind.Enemy:
				computeContainer = enemyTeam._activeChampions;
				break;
			case TargetTeamKind.Team:
				computeContainer = _activeChampions;
				break;

			default:
				return 0;
		}

		int loopCount = computeContainer.Count;
		for (int i = 0; i < loopCount; ++i)
		{
			Champion target = computeContainer[i];

			if( true == findLogicFunction(target.transform.position) )
			{
				if (championCacheLength <= targetCount)
					break;

				championCache[targetCount++] = target;
			}
		}

		return targetCount;
	}

	public Champion ComputeRandomEnemyInRange(in Vector3 originPoint, float range, TargetTeamKind teamKind)
	{
		int randomNumber = 0;
		int maxCount = 0;

		switch (teamKind)
		{
			case TargetTeamKind.Enemy:
				maxCount = enemyTeam._activeChampions.Count;
				if (0 == maxCount)
					return null;
				randomNumber = UnityEngine.Random.Range(0, maxCount);

				return enemyTeam._activeChampions[randomNumber];
			case TargetTeamKind.Team:
				maxCount = _activeChampions.Count;
				if (0 == maxCount)
					return null;
				randomNumber = UnityEngine.Random.Range(0, maxCount);

				return _activeChampions[randomNumber];
			case TargetTeamKind.Both:
				maxCount = _activeChampions.Count + enemyTeam._activeChampions.Count;
				if (0 == maxCount)
					return null;
				randomNumber = UnityEngine.Random.Range(0, maxCount);
				if (_activeChampions.Count <= randomNumber)
					return enemyTeam._activeChampions[randomNumber - _activeChampions.Count];
				else
					return _activeChampions[randomNumber];
		}

		return null;
    }

	// Priority Kind에 따라 추가적인 검사 진행하는 함수..
	private bool CheckPriorityLogic(Champion checkChampion, Champion target, float targetDistance, float curDistance, FindTargetPriorityKind priorityKind)
	{
		if (null == target)
			return true;

		switch (priorityKind)
		{
			case FindTargetPriorityKind.None:
				if (targetDistance > curDistance)
				{
					return true;
				}

				break;
			case FindTargetPriorityKind.MostLowHP:
				if (checkChampion.curHp < target.curHp)
				{
					return true;
				}
				else if (checkChampion.curHp == target.curHp)
				{
					if (targetDistance > curDistance)
					{
						return true;
					}
				}

				break;

			case FindTargetPriorityKind.MostLowHPRatio:
				{
					if (checkChampion.hpRatio < target.hpRatio)
					{
						return true;
					}
					else if (checkChampion.hpRatio == target.hpRatio)
					{
						if (targetDistance > curDistance)
						{
							return true;
						}
					}
				}

				break;
			case FindTargetPriorityKind.MostFarDist:
				if (targetDistance < curDistance)
				{
					return true;
				}

				break;
		}

		return false;
	}

	public void RemoveActiveChampionList(Champion champion)
	{
		_activeChampions.Remove(champion);
	}

	public void AddActiveChampionList(Champion champion)
	{
		_activeChampions.Add(champion);
	}

	// ================================================================================================================================================
	// --- 이벤트에 의해 호출될 콜백 함수들(챔피언 정보와 관련된 함수들)..
	// ================================================================================================================================================
	private void OnChangedChampionBattleData(int index, BattleInfoData data)
	{
		OnChangedChampionBattleInfoData?.Invoke(battleTeamKind, index, data);
	}

	private void UpdateChampionHPRatio(int index, float ratio)
	{
		OnChangedChampionHPRatio?.Invoke(battleTeamKind, index, ratio);
	}

	private void UpdateChampionMPRatio(int index, float ratio)
	{
		OnChangedChampionMPRatio?.Invoke(battleTeamKind, index, ratio);
	}

	private void UpdateChampionUseUltimateState(int index)
	{
		OnChampionUseUltimate?.Invoke(battleTeamKind, index);
	}

	private void UpdateChampionBarrierRatio(int index, float ratio)
	{
		OnChangedChampionBarrierRatio?.Invoke(battleTeamKind, index, ratio);
	}
}
