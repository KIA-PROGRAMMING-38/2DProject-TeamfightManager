using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveLoadLogic
{
	public static bool LoadChampionFile(string fileName, out ChampionStatus getChampionStatus, out ChampionData getChampData, out ChampionResourceData getResourceData)
	{
		// 파일이 있는지 검사..
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

		getChampionStatus.atkStat = int.Parse(championStatusDatas[0]);
		getChampionStatus.atkSpeed = float.Parse(championStatusDatas[1]);
		getChampionStatus.range = float.Parse(championStatusDatas[2]);
		getChampionStatus.defence = int.Parse(championStatusDatas[3]);
		getChampionStatus.hp = int.Parse(championStatusDatas[4]);
		getChampionStatus.moveSpeed = float.Parse(championStatusDatas[5]);
		getChampionStatus.skillCooltime = float.Parse(championStatusDatas[6]);

		// Champion Data 데이터 설정..
		string[] championDatas = loadData[1].Split(',');

		getChampData = new ChampionData();

		getChampData.name = championDatas[0];
		getChampData.type = (ChampionClassType)int.Parse(championDatas[1]);
		getChampData.atkEffectName = championDatas[2];
		getChampData.skillEffectName = championDatas[3];
		getChampData.ultimateEffectName = championDatas[4];
		getChampData.atkActionUniqueKey = int.Parse(championDatas[5]);
		getChampData.skillActionUniqueKey = int.Parse(championDatas[6]);
		getChampData.ultimateActionUniqueKey = int.Parse(championDatas[7]);

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
			// 챔피언 status 저장..
			new string(
				championStatus.atkStat + "," +
				championStatus.atkSpeed + "," +
				championStatus.range + "," +
				championStatus.defence + "," +
				championStatus.hp + "," +
				championStatus.moveSpeed + "," +
				championStatus.skillCooltime
				),

			// 챔피언 Data 저장..
			new string(
					championData.name + "," +
					(int)championData.type + "," +
					championData.atkEffectName + "," +
					championData.skillEffectName + "," +
					championData.ultimateEffectName + "," +
					championData.atkActionUniqueKey + "," +
					championData.skillActionUniqueKey + "," +
					championData.ultimateActionUniqueKey
					),

			championData.champDescription,

			// 챔피언 Resource Data 저장..
			champResourceData.champIconImagePath + "," +
			champResourceData.skillIconImagePath + "," +
			champResourceData.ultimateIconImagePath,
		};

		File.WriteAllLines(filePath, saveDatas);
	}

	public static bool LoadPilotFile(string fileName, out PilotData getPilotData)
	{
		// 파일이 있는지 검사..
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
		int champSkillLevelDataCount = int.Parse(pilotChampSkillLevelDatas[0]);

		getPilotData.champSkillLevelContainer = new List<ChampionSkillLevelInfo>();
		getPilotData.champSkillLevelContainer.Capacity = champSkillLevelDataCount;

		for (int i = 0; i < champSkillLevelDataCount; ++i)
		{
			getPilotData.champSkillLevelContainer.Add(new ChampionSkillLevelInfo
			{
				champName = pilotChampSkillLevelDatas[i * 2 + 1],
				level = int.Parse(pilotChampSkillLevelDatas[i * 2 + 2])
			});
		}

		return true;
	}

	public static void SavePilotFile(PilotData pilotData, string baseGameSaveFilePath, string pilotName, string fileExtension)
	{
		string filePath = Path.Combine(baseGameSaveFilePath, "Pilot", pilotName + fileExtension);

		// 파일럿 data 저장..
		string saveDatas =
			pilotData.name + "," +
			pilotData.atkStat + "," +
			pilotData.defStat + "\n";

		// 파일럿 숙련도 저장..
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
		// 파일이 있는지 여부 판단..
		if (false == File.Exists(fileName))
		{
			getEffectData = null;
			return false;
		}

		string loadData = File.ReadAllText(fileName);

		// 이펙트 Data 파일에서 불러와 저장..
		string[] effectDatas = loadData.Split(',');

		getEffectData = new EffectData();
		getEffectData.name = effectDatas[0];
		getEffectData.animClipPath = effectDatas[1];
		getEffectData.offsetPos = new Vector3(float.Parse(effectDatas[2]), float.Parse(effectDatas[3]), float.Parse(effectDatas[4]));
		getEffectData.isAutoDestroy = bool.Parse(effectDatas[5]);
		getEffectData.isUseLifeTime = bool.Parse(effectDatas[6]);
		getEffectData.lifeTime = float.Parse(effectDatas[7]);
		getEffectData.rotationType = (EffectRotationType)int.Parse(effectDatas[8]);

		return true;
	}

	public static void SaveEffectFile(EffectData effectData, string baseGameSaveFilePath, string fileExtension)
	{
		string filePath = Path.Combine(baseGameSaveFilePath, "Effect", effectData.name + fileExtension);

		// 이펙트 data 저장..
		string saveDatas =
			effectData.name + "," +
			effectData.animClipPath + "," +
			effectData.offsetPos.x + "," +
			effectData.offsetPos.y + "," +
			effectData.offsetPos.z + "," +
			effectData.isAutoDestroy + "," +
			effectData.isUseLifeTime + "," +
			effectData.lifeTime + "," +
			effectData.rotationType;

		File.WriteAllText(filePath, saveDatas);
	}

	// ==================================== AttackAction Save Load.. ==================================== //
	public static bool LoadAttackActionFile(string fileName, out AttackActionData getActionData, out List<AttackImpactData> getImpactDatas, 
		out AttackPerformanceData getPerformaceData)
	{
		// 파일 있는지 유무 판단..
		if (false == File.Exists(fileName))
		{
			getActionData = null;
			getImpactDatas = null;
			getPerformaceData = null;
			return false;
		}

		string[] loadData = File.ReadAllLines(fileName);

		// 공격 행동 Data 저장..
		string[] attackActionData = loadData[0].Split(',');

		getActionData = new AttackActionData();
		getActionData.uniqueKey = int.Parse(attackActionData[0]);
		getActionData.isPassive = bool.Parse(attackActionData[1]);
		getActionData.findTargetData = new FindTargetData
		{
			targetDecideKind = (TargetDecideKind)int.Parse(attackActionData[2]),
			targetTeamKind = (TargetTeamKind)int.Parse(attackActionData[3]),
			impactRange = float.Parse(attackActionData[4]),
			actionStartPointKind = (ActionStartPointKind)int.Parse(attackActionData[5]),
		};

		// Attack Impact Data 저장..
		int impactDataCount = int.Parse(loadData[1]);
		getImpactDatas = new List<AttackImpactData>(impactDataCount);

		int index = 2;
		for (int i = 0; i < impactDataCount; ++i)
		{
			string[] impactData = loadData[index++].Split(',');

			AttackImpactData newImpactData = new AttackImpactData
			{
				mainData = new AttackImpactMainData
				{
					kind = (AttackImpactEffectKind)int.Parse(impactData[0]),
					detailKind = int.Parse(impactData[1]),
					amount = float.Parse(impactData[2]),
					duration = float.Parse(impactData[3]),
					tickTime = float.Parse(impactData[4])
				},

				isSeparateTargetFindLogic = bool.Parse(impactData[5])
			};

			if(newImpactData.isSeparateTargetFindLogic)
			{
				newImpactData.findTargetData = new FindTargetData
				{
					targetDecideKind = (TargetDecideKind)int.Parse(impactData[6]),
					targetTeamKind = (TargetTeamKind)int.Parse(impactData[7]),
					impactRange = float.Parse(impactData[8]),
					actionStartPointKind = (ActionStartPointKind)int.Parse(impactData[9]),
				};
			}

			getImpactDatas.Add(newImpactData);
		}

		// Attack Performance 저장..
		string[] performaceDatas = loadData[index++].Split(',');

		getPerformaceData = new AttackPerformanceData();

		getPerformaceData.isUsePerf = bool.Parse(performaceDatas[0]);
		getPerformaceData.perfType = (AttackPerformanceType)int.Parse(performaceDatas[1]);
		getPerformaceData.detailType = int.Parse(performaceDatas[2]);

		string[] performaceVectprDatas = loadData[index++].Split(',');
		int vectorDataSize = int.Parse(performaceVectprDatas[0]);
		if (0 < vectorDataSize)
		{
			getPerformaceData.vectorData = new Vector3[vectorDataSize];
			for (int i = 0; i < vectorDataSize; ++i)
			{
				Vector3 vectorData = new Vector3(float.Parse(performaceVectprDatas[i * 3 + 1]),
					float.Parse(performaceVectprDatas[i * 3 + 2]), float.Parse(performaceVectprDatas[i * 3 + 3]));
				getPerformaceData.vectorData[i] = vectorData;
			}
		}

		string[] performaceFloatDatas = loadData[index++].Split(',');
		int floatDataSize = int.Parse(performaceFloatDatas[0]);
		if (0 < floatDataSize)
		{
			getPerformaceData.floatData = new float[floatDataSize];
			for (int i = 0; i < floatDataSize; ++i)
			{
				getPerformaceData.floatData[i] = float.Parse(performaceFloatDatas[i + 1]);
			}
		}

		return true;
	}

	public static void SaveAttackActionFile(
		AttackActionData attackActionData, List<AttackImpactData> attackImpactDatas, AttackPerformanceData performanceData
		, string baseGameSaveFilePath, string attackActionName, string fileExtension)
	{
		string filePath = Path.Combine(baseGameSaveFilePath, "AttackAction", attackActionName + fileExtension);

		// 공격 Data 저장..
		string[] saveDatas = new string[attackImpactDatas.Count + 2 + 3];

		saveDatas[0] = new string(
				attackActionData.uniqueKey + "," +
				attackActionData.isPassive + "," +
				(int)attackActionData.findTargetData.targetDecideKind + "," +
				(int)attackActionData.findTargetData.targetTeamKind + "," +
				attackActionData.findTargetData.impactRange + "," +
				(int)attackActionData.findTargetData.actionStartPointKind
				);

		// Attack Impact Data 저장..
		int impactDataCount = attackImpactDatas.Count;
		saveDatas[1] = impactDataCount.ToString();

		for( int i = 0; i < impactDataCount; ++i)
		{
			saveDatas[i + 2] = new string(
				(int)attackImpactDatas[i].mainData.kind + "," +
				attackImpactDatas[i].mainData.detailKind + "," +
				attackImpactDatas[i].mainData.amount + "," +
				attackImpactDatas[i].mainData.duration + "," +
				attackImpactDatas[i].mainData.tickTime + "," +
				attackImpactDatas[i].isSeparateTargetFindLogic
			);

			if (true == attackImpactDatas[i].isSeparateTargetFindLogic)
			{
				saveDatas[i + 2] += new string(
					"," + (int)attackImpactDatas[i].findTargetData.targetDecideKind +
					"," + (int)attackImpactDatas[i].findTargetData.targetTeamKind +
					"," + attackImpactDatas[i].findTargetData.impactRange +
					"," + (int)attackImpactDatas[i].findTargetData.actionStartPointKind
					);
			}
		}

		int index = impactDataCount + 2;

		// Attack Performance Data 저장..
		saveDatas[index++] = new string(
			performanceData.isUsePerf + "," +
			(int)performanceData.perfType + "," +
			performanceData.detailType
			);

		int vectorDataSize = (null != performanceData.vectorData) ? performanceData.vectorData.Length : 0;
		saveDatas[index] = vectorDataSize.ToString();

		for (int i = 0; i < vectorDataSize; ++i)
		{
			saveDatas[index] += "," + performanceData.vectorData[i].x + "," + performanceData.vectorData[i].y + "," + performanceData.vectorData[i].z;
		}

		++index;

		int floatDataSize = (null != performanceData.floatData) ? performanceData.floatData.Length : 0;
		saveDatas[index] = floatDataSize.ToString();

		for (int i = 0; i < floatDataSize; ++i)
		{
			saveDatas[index] += "," + performanceData.vectorData[i].x + "," + performanceData.vectorData[i].y + "," + performanceData.vectorData[i].z;
		}

		File.WriteAllLines(filePath, saveDatas);
	}
}