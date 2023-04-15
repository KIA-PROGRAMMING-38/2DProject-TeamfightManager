using System.Collections.Generic;
using UnityEngine;

public enum BanpickStageKind
{
	Ban,
	Pick
}

[System.Serializable]
public class BanpickStageData
{
	public BanpickStageKind kind;
	public List<BattleTeamKind> orders;
}

public class BanpickStageGlobalData : ScriptableObject
{
	public BanpickStageData[] stagesDataContainer;
}