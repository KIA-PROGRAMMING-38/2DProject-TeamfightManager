using UnityEditor;
using UnityEngine;

public class GameSaveLoader
{
	public GameManager gameManager
	{
		set
		{
			_dataTableManager = value.dataTableManager;
		}
	}

	private DataTableManager _dataTableManager;

	private ChampionStatus _testChampStatus = new ChampionStatus
	{
		atkSpeed = 1,
		atkStat = 10,
		defence = 0,
		hp = 30,
		moveSpeed = 15,
		range = 1,
	};

	private PilotData _testPilotData = new PilotData
	{
		name = "Test",
		atkStat = 10,
		defStat = 10,
		condition = PilotConditionState.Default,
		champSkillLevelContainer = null,
	};

	public void LoadGameFile(string fileName)
	{
		string champName = "Swordman";
		
		_dataTableManager.championDataTable.AddChampionStatus(champName, _testChampStatus);
		_dataTableManager.championDataTable.AddChampionAnimData(champName, CreateChampAnimData(champName));

		_dataTableManager.pilotDataTable.AddPilotData(_testPilotData.name, _testPilotData);
	}

	private ChampionAnimData CreateChampAnimData(string championName)
	{
		ChampionAnimData newChampionAnimData = new ChampionAnimData();

		// 기본 애니메이션 경로..
		string defaultPath = "Assets\\Animations\\Champion";

		// 기본 애니메이션 파일 이름..
		string idleFileName = "Idle.anim";
		string moveFileName = "Move.anim";
		string atkFileName = "Attack.anim";
		string skillFileName = "Skill.anim";
		string ultFileName = "Ultimate.anim";
		string deathFileName = "Death.anim";
		string deadLoopFileName = "DeadLoop.anim";

		// 파일 경로를 가지고 애니메이션 파일을 찾아낸다..
		newChampionAnimData.idleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, idleFileName));
		newChampionAnimData.moveAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, moveFileName));
		newChampionAnimData.atkAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, atkFileName));
		newChampionAnimData.skillAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, skillFileName));
		newChampionAnimData.ultAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, ultFileName));
		newChampionAnimData.deathAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, deathFileName));
		newChampionAnimData.deadLoopAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(System.IO.Path.Combine(defaultPath, championName, deadLoopFileName));

		return newChampionAnimData;
	}
}