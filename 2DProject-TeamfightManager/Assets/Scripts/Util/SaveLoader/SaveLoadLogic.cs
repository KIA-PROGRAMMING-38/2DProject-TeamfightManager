using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class SaveLoadLogic
{
	public static void LoadGameFile(string fileName, DataTableManager dataTableManager)
	{
		
	}

	public static void SaveGameFile(string fileName, DataTableManager dataTableManager)
	{

	}

	public static bool LoadChampionFile(string fileName, out ChampionStatus getChampionStatus, out ChampionData getChampData, out ChampionResourceData getResourceData)
	{
		if (false == File.Exists(fileName))
		{
			getChampionStatus = null;
			getChampData = null;
			getResourceData = null;
			return false;
		}

		string[] loadData = File.ReadAllLines(fileName);

		// Champion Status 데이터 설정..
		string[] championStatusDatas = loadData[0].Split(',');

		getChampionStatus = new ChampionStatus();

		getChampionStatus.atkStat = int.Parse(championStatusDatas[1]);
		getChampionStatus.atkSpeed = float.Parse(championStatusDatas[2]);
		getChampionStatus.range = float.Parse(championStatusDatas[3]);
		getChampionStatus.defence = int.Parse(championStatusDatas[4]);
		getChampionStatus.hp = int.Parse(championStatusDatas[5]);
		getChampionStatus.moveSpeed = float.Parse(championStatusDatas[6]);

		// Champion Data 데이터 설정..
		string[] championDatas = loadData[1].Split(',');

		getChampData = new ChampionData();

		getChampData.name = championDatas[1];
		getChampData.type = (ChampionClassType)int.Parse(championDatas[2]);
		getChampData.atkEffectName = championDatas[3];
		getChampData.skillEffectName = championDatas[4];
		getChampData.ultimateEffectName = championDatas[5];
		getChampData.champDescription = loadData[2];

		// Champion Resource 데이터 설정..
		string[] champResourceData = loadData[3].Split(',');

		getResourceData = new ChampionResourceData();

		getResourceData.champIconImagePath = champResourceData[0];
		getResourceData.skillIconImagePath = champResourceData[1];
		getResourceData.ultimateIconImagePath = champResourceData[2];

		return true;
	}

	public static void SaveChampionFile(ChampionStatus championStatus, ChampionData championData, ChampionResourceData champResourceData
		, string baseGameSaveFilePath, string championName, string fileExtension)
	{
		string filePath = Path.Combine(baseGameSaveFilePath, "Champion", championName + fileExtension);

		string[] saveDatas =
		{
			new string(
				championStatus.atkStat.ToString() + "," +
				championStatus.atkSpeed.ToString() + "," +
				championStatus.range.ToString() + "," +
				championStatus.defence.ToString() + "," +
				championStatus.hp.ToString() + "," +
				championStatus.moveSpeed.ToString()),

			new string(
					championData.name + "," +
					(int)championData.type + "," +
					championData.atkEffectName + "," +
					championData.skillEffectName + "," +
					championData.ultimateEffectName
					),

			championData.champDescription,

			champResourceData.champIconImagePath + "," +
			champResourceData.skillIconImagePath + "," +
			champResourceData.ultimateIconImagePath,
		};

		File.WriteAllLines(filePath, saveDatas);
	}

	public static bool LoadPilotFile(string fileName, out PilotData getPilotData)
	{
		if (false == File.Exists(fileName))
		{
			getPilotData = null;
			return false;
		}

		getPilotData = new PilotData();

		// 파일럿 데이터 초기화..
		string[] loadData = File.ReadAllLines(fileName);
		string[] pilotDatas = loadData[0].Split(',');

		getPilotData.name = pilotDatas[0];
		getPilotData.atkStat = int.Parse(pilotDatas[1]);
		getPilotData.defStat = int.Parse(pilotDatas[2]);

		// 파일럿의 챔피언 숙련도 데이터 초기화..
		string[] pilotChampSkillLevelDatas = loadData[1].Split(',');
		int champSkillLevelDataCount = pilotChampSkillLevelDatas[0].Length;

		getPilotData.champSkillLevelContainer = new List<(string champName, int level)>();
		getPilotData.champSkillLevelContainer.Capacity = champSkillLevelDataCount;

		for (int i = 0; i < champSkillLevelDataCount; ++i)
		{
			getPilotData.champSkillLevelContainer.Add(new(pilotChampSkillLevelDatas[i * 2 + 1], int.Parse(pilotChampSkillLevelDatas[i * 2 + 2])));
		}

		return true;
	}

	public static void SavePilotFile(PilotData pilotData, string baseGameSaveFilePath, string pilotName, string fileExtension)
	{
		string filePath = Path.Combine(baseGameSaveFilePath, "Pilot", pilotName + fileExtension);

		string saveDatas =
			pilotData.name + "," +
			pilotData.atkStat.ToString() + "," +
			pilotData.defStat.ToString() + "\n";

		if (null != pilotData.champSkillLevelContainer)
		{
			saveDatas += pilotData.champSkillLevelContainer.Count;

			foreach (var champSkillLevel in pilotData.champSkillLevelContainer)
			{
				saveDatas += "," + champSkillLevel.champName + "," + champSkillLevel.level;
			}
		}
		else
		{
			saveDatas += "0";
		}

		File.WriteAllText(filePath, saveDatas);
	}

	public static bool LoadEffectFile(string fileName, out EffectData getEffectData)
	{
		if (false == File.Exists(fileName))
		{
			getEffectData = null;
			return false;
		}

		string loadData = File.ReadAllText(fileName);
		string[] effectDatas = loadData.Split(',');

		getEffectData = new EffectData();
		getEffectData.name = effectDatas[0];
		getEffectData.animClipPath = effectDatas[1];
		getEffectData.offsetPos = new Vector3(float.Parse(effectDatas[2]), float.Parse(effectDatas[3]), float.Parse(effectDatas[4]));
		getEffectData.isAutoDestroy = bool.Parse(effectDatas[5]);
		getEffectData.isUseLifeTime = bool.Parse(effectDatas[6]);
		getEffectData.lifeTime = float.Parse(effectDatas[7]);

		return true;
	}

	public static void SaveEffectFile(EffectData effectData, string baseGameSaveFilePath, string fileExtension)
	{
		string filePath = Path.Combine(baseGameSaveFilePath, "Effect", effectData.name + fileExtension);

		string saveDatas =
			effectData.name + "," +
			effectData.animClipPath + "," +
			effectData.offsetPos.x + "," +
			effectData.offsetPos.y + "," +
			effectData.offsetPos.z + "," +
			effectData.isAutoDestroy.ToString() + "," +
			effectData.isUseLifeTime.ToString() + "," +
			effectData.lifeTime.ToString();

		File.WriteAllText(filePath, saveDatas);
	}

	// ==================================== AttackAction Save Load.. ==================================== //
	public static bool LoadAttackActionFile(string fileName, out AttackActionData getActionData, out List<AttackImpactData> getImpactDatas)
	{
		if (false == File.Exists(fileName))
		{
			getActionData = null;
			getImpactDatas = null;
			return false;
		}

		string[] loadData = File.ReadAllLines(fileName);

		string[] attackActionData = loadData[0].Split(',');

		getActionData = new AttackActionData();
		getActionData.uniqueKey = int.Parse(attackActionData[0]);
		getActionData.isPassive = bool.Parse(attackActionData[1]);
		getActionData.impactRange = float.Parse(attackActionData[2]);
		getActionData.impactRangeType = int.Parse(attackActionData[3]);
		getActionData.actionStartPointKind = int.Parse(attackActionData[4]);

		int impactDataCount = int.Parse(loadData[1]);
		getImpactDatas = new List<AttackImpactData>(impactDataCount);

		for (int i = 0; i < impactDataCount; ++i)
		{
			string[] impactData = loadData[i + 2].Split(',');

			getImpactDatas.Add(
				new AttackImpactData
				{
					kind = int.Parse(impactData[0]),
					detailKind = int.Parse(impactData[1]),
					amount = int.Parse(impactData[2]),
					duration = float.Parse(impactData[3]),
					tickTime = float.Parse(impactData[4]),
					targetDecideKind = int.Parse(impactData[5]),
					targetTeamKind = int.Parse(impactData[6]),
				});
		}

		return true;
	}

	public static void SaveAttackActionFile(
		AttackActionData attackAcionData, List<AttackImpactData> attackImpactDatas, string baseGameSaveFilePath, string attackActionName, string fileExtension)
	{
		string filePath = Path.Combine(baseGameSaveFilePath, "AttackAction", attackActionName + fileExtension);

		string[] saveDatas = new string[attackImpactDatas.Count + 2];

		saveDatas[0] = new string(
				attackAcionData.uniqueKey + "," +
				attackAcionData.isPassive + "," +
				attackAcionData.impactRange + "," +
				attackAcionData.impactRangeType + "," +
				attackAcionData.actionStartPointKind
				);

		int impactDataCount = attackImpactDatas.Count;
		saveDatas[1] = impactDataCount.ToString();

		for( int i = 0; i < impactDataCount; ++i)
		{
			saveDatas[i + 2] = new string(
				attackImpactDatas[i].kind + "," +
				attackImpactDatas[i].detailKind + "," +
				attackImpactDatas[i].amount + "," +
				attackImpactDatas[i].duration + "," +
				attackImpactDatas[i].tickTime + "," +
				attackImpactDatas[i].targetDecideKind + "," +
				attackImpactDatas[i].targetTeamKind
				);
		}

		File.WriteAllLines(filePath, saveDatas);
	}
}