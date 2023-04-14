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

		string filePath = "Assets/Data";

		{
			// Pilot ���������� ���� ���� �ҷ��´�..
			string pilotGlobalFilePath = Path.Combine(filePath, gameManager.gameGlobalData.pilotGlobalFilePath);
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
			// Attack Action�� ���ϵ� �ҷ��´�..
			string attackDataActionDefaultPath = Path.Combine(filePath, gameManager.gameGlobalData.attackActionDirectoryName);
			string[] attackActionsFilePath = Directory.GetFiles(attackDataActionDefaultPath);

			int loopCount = attackActionsFilePath.Length;
			dataTableManager.attackActionDataTable.actionCount = loopCount / 2;

			for (int i = 0; i < loopCount; ++i)
			{
				if (attackActionsFilePath[i].Contains(".meta"))
					continue;

				// ������ ���� �ൿ���� �ʿ��� ������ ������ �ҷ��� ������ ���̺� ����..
				AttackActionData getActionData;
				List<AttackImpactData> getImpactDatas;
				AttackPerformanceData getPerformanceData;
				AttackActionEffectData getEffectData;
				if (true == SaveLoadLogic.LoadAttackActionFile(attackActionsFilePath[i], out getActionData, out getImpactDatas, out getPerformanceData
					, out getEffectData))
				{
					dataTableManager.attackActionDataTable.AddActionData(getActionData, getImpactDatas, getPerformanceData, getEffectData);
				}
			}
		}

		{
			// Champion�� ���ϵ� �ҷ��´�..
			string championDataDefaultPath = Path.Combine(filePath, gameManager.gameGlobalData.championDirectoryName);
			string[] championsFilePath = Directory.GetFiles(championDataDefaultPath);

			int loopCount = championsFilePath.Length;
			for (int i = 0; i < loopCount; ++i)
			{
				if (championsFilePath[i].Contains(".meta"))
					continue;

				// ������ è�Ǿ𿡰� �ʿ��� ������ ������ �ҷ��� ������ ���̺� ����..
				ChampionStatus championStatus;
				ChampionData championData;
				ChampionResourceData resourceData;

				if (true == SaveLoadLogic.LoadChampionFile(championsFilePath[i], out championStatus, out championData, out resourceData))
				{
					dataTableManager.championDataTable.AddChampionData(championData.name, championData, championStatus, resourceData);
					dataTableManager.championDataTable.AddChampionAnimData(championData.name, CreateChampAnimData(championData.name));
				}
			}
		}

		{
			// Effect�� ���ϵ� �ҷ��´�..
			string effectDataDefaultPath = Path.Combine(filePath, gameManager.gameGlobalData.effectDirectoryName);
			string[] effectsFilePath = Directory.GetFiles(effectDataDefaultPath);

			int loopCount = effectsFilePath.Length;
			for (int i = 0; i < loopCount; ++i)
			{
				if (effectsFilePath[i].Contains(".meta"))
					continue;

				// ������ ����Ʈ���� �ʿ��� ������ ������ �ҷ��� ������ ���̺� ����..
				EffectData effectData;

				if (true == SaveLoadLogic.LoadEffectFile(effectsFilePath[i], out effectData))
				{
					dataTableManager.effectDataTable.AddEffectInfo(effectData.name, effectData);
				}
			}
		}

		{
			// Pilot�� ���ϵ� �ҷ��´�..
			string pilotDataDefaultPath = Path.Combine(filePath, gameManager.gameGlobalData.pilotDirectoryName);
			string[] pilotsFilePath = Directory.GetFiles(pilotDataDefaultPath);

			int loopCount = pilotsFilePath.Length;
			for (int i = 0; i < loopCount; ++i)
			{
				if (pilotsFilePath[i].Contains(".meta"))
					continue;

				// ������ ���Ϸ����� �ʿ��� ������ ������ �ҷ��� ������ ���̺� ����..
				PilotData pilotData;

				if (true == SaveLoadLogic.LoadPilotFile(pilotsFilePath[i], out pilotData))
				{
					dataTableManager.pilotDataTable.AddPilotData(pilotData.name, pilotData);
				}
			}
		}
	}

	public static void SaveGameFile(int loadFileNumber, GameManager gameManager)
	{
		GameObject newGameObject = new GameObject();
		TestCreateDataSaveFile test = newGameObject.AddComponent<TestCreateDataSaveFile>();

		test.CreateAll();
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