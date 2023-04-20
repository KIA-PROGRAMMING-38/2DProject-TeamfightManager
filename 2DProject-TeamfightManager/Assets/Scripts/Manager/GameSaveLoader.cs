using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 게임의 저장 및 블러오기를 담당할 클래스..
/// </summary>
public static class GameSaveLoader
{
	/// <summary>
	/// 게임 파일을 불러와 데이터 테이블에 저장한다..
	/// </summary>
	/// <param name="fileName"> 게임 파일 경로 </param>
	public static void LoadGameFile(int loadFileNumber, GameManager gameManager)
	{
		DataTableManager dataTableManager = gameManager.dataTableManager;

		string filePath = "Assets/Data/Save1";

		{
			// Pilot 전역적으로 사용될 파일 불러온다..
			string pilotGlobalFilePath = Path.Combine("Assets/Data", gameManager.gameGlobalData.pilotGlobalFilePath);
			string[] loadData = File.ReadAllLines(pilotGlobalFilePath);

			string[] conditionsFilePath = loadData[0].Split(',');
			int loopCount = conditionsFilePath.Length;
			for( int i = 0; i < loopCount; ++i)
			{
				Sprite loadSprite = Resources.Load<Sprite>(conditionsFilePath[i]);
				dataTableManager.pilotDataTable.AddConditionSprite(loadSprite);
			}

			dataTableManager.pilotDataTable.SetPilotDefaultPrefab(Resources.Load<GameObject>(loadData[1]));
		}

		{
			// Attack Action Data 파일을 불러온다..
			string atkActionDataFilePath = Path.Combine(filePath, gameManager.gameGlobalData.attackActionDataFileName);
			string[] loadActionData = File.ReadAllLines(atkActionDataFilePath);

			// Attack Impact Data 파일을 불러온다..
			string atkImpactDataFilePath = Path.Combine(filePath, gameManager.gameGlobalData.attackActionImpactDataFileName);
			string[] loadImpactData = File.ReadAllLines(atkImpactDataFilePath);

			// Attack Performance Data 파일을 불러온다..
			string atkPerformanceDataFilePath = Path.Combine(filePath, gameManager.gameGlobalData.attackActionPerfDataFileName);
			string[] loadPerformanceData = File.ReadAllLines(atkPerformanceDataFilePath);

			string[] loadDatas = new string[3];
			
			int loopCount = loadActionData.Length;
			dataTableManager.attackActionDataTable.actionCount = loopCount - 1;
			
			for (int i = 1; i < loopCount; ++i)
			{
				loadDatas[0] = loadActionData[i];
				loadDatas[1] = loadImpactData[i];
				loadDatas[2] = loadPerformanceData[i];

				// 각각의 공격 행동에게 필요한 데이터 파일을 불러와 데이터 테이블에 저장..
				AttackActionData getActionData;
				List<AttackImpactData> getImpactDatas;
				AttackPerformanceData getPerformanceData;
				AttackActionEffectData getEffectData;
				if (true == SaveLoadLogic.LoadAttackActionFile(loadDatas, out getActionData, out getImpactDatas, out getPerformanceData
					, out getEffectData))
				{
					dataTableManager.attackActionDataTable.AddActionData(getActionData, getImpactDatas, getPerformanceData, getEffectData);
				}
			}
		}

		{
			// Champion의 파일들 불러온다..
			string championDataDefaultPath = Path.Combine(filePath, gameManager.gameGlobalData.championFileName);
			string[] loadData = File.ReadAllLines(championDataDefaultPath);
			
			int loopCount = loadData.Length;
			for (int i = 1; i < loopCount; ++i)
			{
				// 각각의 챔피언에게 필요한 데이터 파일을 불러와 데이터 테이블에 저장..
				ChampionStatus championStatus;
				ChampionData championData;
				ChampionResourceData resourceData;
			
				if (true == SaveLoadLogic.LoadChampionFile(loadData[i], out championStatus, out championData, out resourceData))
				{
					dataTableManager.championDataTable.AddChampionData(championData.name, championData, championStatus, resourceData);
					dataTableManager.championDataTable.AddChampionAnimData(championData.name, CreateChampAnimData(championData.name));
				}
			}
		}

		{
			// Effect의 파일들 불러온다..
			string effectDataDefaultPath = Path.Combine(filePath, gameManager.gameGlobalData.effectFileName);
			string[] loadData = File.ReadAllLines(effectDataDefaultPath);
			
			int loopCount = loadData.Length;
			for (int i = 1; i < loopCount; ++i)
			{
				// 각각의 이펙트에게 필요한 데이터 파일을 불러와 데이터 테이블에 저장..
				EffectData effectData;
			
				if (true == SaveLoadLogic.LoadEffectFile(loadData[i], out effectData))
				{
					dataTableManager.effectDataTable.AddEffectInfo(effectData.name, effectData);
				}
			}
		}

		{
			// Pilot의 파일들 불러온다..
			string pilotDataDefaultPath = Path.Combine(filePath, gameManager.gameGlobalData.pilotFileName);
			string[] loadData = File.ReadAllLines(pilotDataDefaultPath);
			
			int loopCount = loadData.Length;
			for (int i = 1; i < loopCount; ++i)
			{			
				// 각각의 파일럿에게 필요한 데이터 파일을 불러와 데이터 테이블에 저장..
				PilotData pilotData;
			
				if (true == SaveLoadLogic.LoadPilotFile(loadData[i], out pilotData))
				{
					dataTableManager.pilotDataTable.AddPilotData(pilotData.name, pilotData);
				}
			}
		}

        {
            // Team의 파일들 불러온다..
            string teamDataFilePath = Path.Combine(filePath, gameManager.gameGlobalData.teamFileName);
			string[] loadData = File.ReadAllLines(teamDataFilePath);

            int loopCount = loadData.Length;
            for ( int i = 1; i < loopCount; ++i )
            {
                // 각각의 파일럿에게 필요한 데이터 파일을 불러와 데이터 테이블에 저장..
                TeamData teamData;
                TeamBelongPilotData belongData;
				TeamResourceData resourceData;

                if ( true == SaveLoadLogic.LoadTeamFile(loadData[i], out teamData, out belongData, out resourceData ) )
                {
                    dataTableManager.teamDataTable.AddTeamInfo( teamData, belongData, resourceData );
                }
            }
        }
    }

	public static void SaveGameFile(int loadFileNumber, GameManager gameManager)
	{

	}

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
		string idleFileName = championName + "_Idle";
		string moveFileName = championName + "_Move";
		string atkFileName = championName + "_Attack";
		string skillFileName = championName + "_Skill";
		string ultFileName = championName + "_Ultimate";
		string deathFileName = championName + "_Death";
		string deadLoopFileName = championName + "_DeadLoop";

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
}