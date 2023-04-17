using System.Collections.Generic;
using UnityEngine;

public enum BanpickStageKind
{
	None,
	Ban,
	Pick
}

[System.Serializable]
public class BanpickStageData
{
	public BanpickStageKind kind;
	public List<BattleTeamKind> orders;
}

[CreateAssetMenu(fileName = "BanpickGlobalData", menuName = "Config/BanpickGlobalData")]
public class BanpickStageGlobalData : ScriptableObject
{
	public BanpickStageData[] stagesDataContainer;
}