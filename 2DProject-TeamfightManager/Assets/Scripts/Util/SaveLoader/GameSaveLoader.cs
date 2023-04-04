using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 게임의 저장 및 블러오기를 담당할 클래스..
/// </summary>
public static class GameSaveLoader
{
	public static ChampionStatus _testChampStatus = new ChampionStatus
	{
		atkSpeed = 1,
		atkStat = 10,
		defence = 0,
		hp = 30,
		moveSpeed = 3,
		range = 1,
	};

	public static PilotData _testPilotData = new PilotData
	{
		name = "Test",
		atkStat = 10,
		defStat = 10,
		champSkillLevelContainer = null,
	};

	public static EffectData _testAtkEffectData =
		new EffectData("Effect_Swordman_Attack", "Animations/Effect/Champion/Effect_Swordman_Attack", new Vector3(-0.1f, 0.18f, 0f));

	public static EffectData _testSkillEffectData =
		new EffectData("Effect_Swordman_Skill", "Animations/Effect/Champion/Effect_Swordman_Skill", new Vector3(0.11f, 0.12f, 0f));

	/// <summary>
	/// 챔피언 이름을 넣어주면 그에 맞는 애니메이션 경로를 계산해 ChampionAnimData 클래스를 만들어 넣어준다..
	/// </summary>
	/// <param name="championName"> 챔피언 이름 </param>
	/// <returns> 해당 이름의 챔피언에 맞는 애니메이션 정보 </returns>
	private static ChampionAnimData CreateChampAnimData(string championName)
	{
		ChampionAnimData newChampionAnimData = new ChampionAnimData();

		// 기본 애니메이션 경로..
		string defaultPath = "Animations\\Champion";

		// 기본 애니메이션 파일 이름..
		string idleFileName = "Idle";
		string moveFileName = "Move";
		string atkFileName = "Attack";
		string skillFileName = "Skill";
		string ultFileName = "Ultimate";
		string deathFileName = "Death";
		string deadLoopFileName = "DeadLoop";

		// 파일 경로를 가지고 애니메이션 파일을 찾아낸다..
		newChampionAnimData.idleAnim = Resources.Load<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, idleFileName));
		newChampionAnimData.moveAnim = Resources.Load<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, moveFileName));
		newChampionAnimData.atkAnim = Resources.Load<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, atkFileName));
		newChampionAnimData.skillAnim = Resources.Load<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, skillFileName));
		newChampionAnimData.ultAnim = Resources.Load<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, ultFileName));
		newChampionAnimData.deathAnim = Resources.Load<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, deathFileName));
		newChampionAnimData.deadLoopAnim = Resources.Load<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, deadLoopFileName));

		return newChampionAnimData;
	}

	/// <summary>
	/// 게임 파일을 불러와 데이터 테이블에 저장한다..
	/// </summary>
	/// <param name="fileName"> 게임 파일 경로 </param>
	public static void LoadGameFile(string fileName, DataTableManager dataTableManager)
	{
		SaveLoadLogic.LoadGameFile(fileName, dataTableManager);
	}

	// ==================================== Champion Save Load.. ==================================== //
	public static bool LoadChampionFile(string fileName, out ChampionStatus championStatus, out ChampionData getChampData, out ChampionResourceData getResourceData)
	{
		return SaveLoadLogic.LoadChampionFile(fileName, out championStatus, out getChampData, out getResourceData);
	}

	public static void SaveChampionFile(ChampionStatus championStatus, ChampionData championData, ChampionResourceData champResourceData, string baseGameSaveFilePath, string championName, string fileExtension)
	{
		SaveLoadLogic.SaveChampionFile(championStatus, championData, champResourceData, baseGameSaveFilePath, championName, fileExtension);
	}

	// ==================================== Pilot Save Load.. ==================================== //
	public static bool LoadPilotFile(string fileName, out PilotData getPilotData)
	{
		return SaveLoadLogic.LoadPilotFile(fileName, out getPilotData);
	}

	public static void SavePilotFile(PilotData pilotData, string baseGameSaveFilePath, string pilotName, string fileExtension)
	{
		SaveLoadLogic.SavePilotFile(pilotData, baseGameSaveFilePath, pilotName, fileExtension);
	}

	// ==================================== Effect Save Load.. ==================================== //
	public static bool LoadEffectFile(string fileName, out EffectData getEffectData)
	{
		return SaveLoadLogic.LoadEffectFile(fileName, out getEffectData);
	}

	public static void SaveEffectFile(EffectData effectData, string baseGameSaveFilePath, string fileExtension)
	{
		SaveLoadLogic.SaveEffectFile(effectData, baseGameSaveFilePath, fileExtension);
	}

	// ==================================== AttackAction Save Load.. ==================================== //
	public static bool LoadAttackActionFile(string fileName, out AttackActionData getAttackAction, out List<AttackImpactData> getImpactDatas)
	{
		return SaveLoadLogic.LoadAttackActionFile(fileName, out getAttackAction, out getImpactDatas);
	}

	public static void SaveAttackActionFile(AttackActionData attackAction, List<AttackImpactData> impactDatas, string baseGameSaveFilePath, string actionName, string fileExtension)
	{
		SaveLoadLogic.SaveAttackActionFile(attackAction, impactDatas, baseGameSaveFilePath, actionName, fileExtension);
	}
}