using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ������ ���� �� �����⸦ ����� Ŭ����..
/// </summary>
public static class GameSaveLoader
{
	/// <summary>
	/// ���� ������ �ҷ��� ������ ���̺� �����Ѵ�..
	/// </summary>
	/// <param name="fileName"> ���� ���� ��� </param>
	public static void LoadGameFile(int loadFileNumber, GameManager gameManager)
	{
		DataTableManager dataTableManager = gameManager.dataTableManager;

		string filePath = "Assets/Data/Save1";

		{
			// Pilot ���������� ���� ���� �ҷ��´�..
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

			loopCount = gameManager.gameGlobalData.pilotHairSprite.Length;
			dataTableManager.pilotDataTable.trunkIconSprite = gameManager.gameGlobalData.pilotTrunkSprite;
            for ( int i = 0; i < loopCount; ++i)
			{
				dataTableManager.pilotDataTable.AddHairSprite(gameManager.gameGlobalData.pilotHairSprite[i]);
			}
		}

		{
			// Attack Action Data ������ �ҷ��´�..
			string atkActionDataFilePath = Path.Combine(filePath, gameManager.gameGlobalData.attackActionDataFileName);
			string[] loadActionData = File.ReadAllLines(atkActionDataFilePath);

			// Attack Impact Data ������ �ҷ��´�..
			string atkImpactDataFilePath = Path.Combine(filePath, gameManager.gameGlobalData.attackActionImpactDataFileName);
			string[] loadImpactData = File.ReadAllLines(atkImpactDataFilePath);

			// Attack Performance Data ������ �ҷ��´�..
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

				// ������ ���� �ൿ���� �ʿ��� ������ ������ �ҷ��� ������ ���̺� ����..
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
			// Champion�� ���ϵ� �ҷ��´�..
			string championDataDefaultPath = Path.Combine(filePath, gameManager.gameGlobalData.championFileName);
			string[] loadData = File.ReadAllLines(championDataDefaultPath);
			
			int loopCount = loadData.Length;
			for (int i = 1; i < loopCount; ++i)
			{
				// ������ è�Ǿ𿡰� �ʿ��� ������ ������ �ҷ��� ������ ���̺� ����..
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
			// Effect�� ���ϵ� �ҷ��´�..
			string effectDataDefaultPath = Path.Combine(filePath, gameManager.gameGlobalData.effectFileName);
			string[] loadData = File.ReadAllLines(effectDataDefaultPath);
			
			int loopCount = loadData.Length;
			for (int i = 1; i < loopCount; ++i)
			{
				// ������ ����Ʈ���� �ʿ��� ������ ������ �ҷ��� ������ ���̺� ����..
				EffectData effectData;
			
				if (true == SaveLoadLogic.LoadEffectFile(loadData[i], out effectData))
				{
					dataTableManager.effectDataTable.AddEffectInfo(effectData.name, effectData);
				}
			}
		}

		{
			// Pilot�� ���ϵ� �ҷ��´�..
			string pilotDataDefaultPath = Path.Combine(filePath, gameManager.gameGlobalData.pilotFileName);
			string[] loadData = File.ReadAllLines(pilotDataDefaultPath);
			
			int loopCount = loadData.Length;
			for (int i = 1; i < loopCount; ++i)
			{			
				// ������ ���Ϸ����� �ʿ��� ������ ������ �ҷ��� ������ ���̺� ����..
				PilotData pilotData;
			
				if (true == SaveLoadLogic.LoadPilotFile(loadData[i], out pilotData))
				{
					dataTableManager.pilotDataTable.AddPilotData(pilotData.name, pilotData);
				}
			}
		}

        {
            // Team�� ���ϵ� �ҷ��´�..
            string teamDataFilePath = Path.Combine(filePath, gameManager.gameGlobalData.teamFileName);
			string[] loadData = File.ReadAllLines(teamDataFilePath);

            int loopCount = loadData.Length;
            for ( int i = 1; i < loopCount; ++i )
            {
                // ������ ���Ϸ����� �ʿ��� ������ ������ �ҷ��� ������ ���̺� ����..
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
	/// è�Ǿ� �̸��� �־��ָ� �׿� �´� �ִϸ��̼� ��θ� ����� ChampionAnimData Ŭ������ ����� �־��ش�..
	/// </summary>
	/// <param name="championName"> è�Ǿ� �̸� </param>
	/// <returns> �ش� �̸��� è�Ǿ� �´� �ִϸ��̼� ���� </returns>
	private static ChampionAnimData CreateChampAnimData(string championName)
	{
		ChampionAnimData newChampionAnimData = new ChampionAnimData();

		// �⺻ �ִϸ��̼� ���..
		string defaultPath = "Animations\\Champion";

		// �⺻ �ִϸ��̼� ���� �̸�..
		string idleFileName = championName + "_Idle";
		string moveFileName = championName + "_Move";
		string atkFileName = championName + "_Attack";
		string skillFileName = championName + "_Skill";
		string ultFileName = championName + "_Ultimate";
		string deathFileName = championName + "_Death";
		string deadLoopFileName = championName + "_DeadLoop";

		// ���� ��θ� ������ �ִϸ��̼� ������ ã�Ƴ���..
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