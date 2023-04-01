using UnityEditor;
using UnityEngine;

/// <summary>
/// 게임의 저장 및 블러오기를 담당할 클래스..
/// </summary>
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
		hp = 500,
		moveSpeed = 3,
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

	private EffectInfo _testAtkEffectData =
		new EffectInfo("Effect_Swordman_Attack", "Assets/Animations/Effect/Champion/Effect_Swordman_Attack.anim", new Vector3(-0.1f, 0.18f, 0f));

	private EffectInfo _testSkillEffectData =
		new EffectInfo("Effect_Swordman_Skill", "Assets/Animations/Effect/Champion/Effect_Swordman_Skill.anim", new Vector3(0.11f, 0.12f, 0f));

	/// <summary>
	/// 게임 파일을 불러와 데이터 테이블에 저장한다..
	/// </summary>
	/// <param name="fileName"> 게임 파일 경로 </param>
	public void LoadGameFile(string fileName)
	{
		string champName = "Swordman";
		
		_dataTableManager.championDataTable.AddChampionStatus(champName, _testChampStatus);
		_dataTableManager.championDataTable.AddChampionAnimData(champName, CreateChampAnimData(champName));

		_dataTableManager.pilotDataTable.AddPilotData(_testPilotData.name, _testPilotData);

		_dataTableManager.effectDataTable.AddEffectInfo(_testAtkEffectData.name, _testAtkEffectData);
		_dataTableManager.effectDataTable.AddEffectInfo(_testSkillEffectData.name, _testSkillEffectData);
	}

	/// <summary>
	/// 챔피언 이름을 넣어주면 그에 맞는 애니메이션 경로를 계산해 ChampionAnimData 클래스를 만들어 넣어준다..
	/// </summary>
	/// <param name="championName"> 챔피언 이름 </param>
	/// <returns> 해당 이름의 챔피언에 맞는 애니메이션 정보 </returns>
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