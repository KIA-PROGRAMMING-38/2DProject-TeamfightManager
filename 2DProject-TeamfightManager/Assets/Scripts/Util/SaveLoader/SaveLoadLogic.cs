using System.Collections.Generic;
using UnityEngine;

public static class SaveLoadLogic
{
	#region Champion Save Load Logic
	// ============================================================================================== //
	// ==================================== Champion Save Load.. ==================================== //
	// ============================================================================================== //
	public static bool LoadChampionFile(string fileData, out ChampionStatus getChampionStatus, out ChampionData getChampData, out ChampionResourceData getResourceData)
	{
		string[] loadData = fileData.Split(',');
		int index = 1;

		getChampionStatus = new ChampionStatus();
		getChampData = new ChampionData();
		getResourceData = new ChampionResourceData();

		// Champion Status 데이터 설정..
		getChampionStatus.atkStat = int.Parse(loadData[index++]);
		getChampionStatus.atkSpeed = float.Parse(loadData[index++]);
		getChampionStatus.range = float.Parse(loadData[index++]);
		getChampionStatus.defence = int.Parse(loadData[index++]);
		getChampionStatus.hp = int.Parse(loadData[index++]);
		getChampionStatus.moveSpeed = float.Parse(loadData[index++]);
		getChampionStatus.skillCooltime = float.Parse(loadData[index++]);

		// Champion Data 설정..
		getChampData.name = loadData[index++];
		getChampData.nameKR = loadData[0];
		getChampData.type = (ChampionClassType)int.Parse(loadData[index++]);
		getChampData.atkActionUniqueKey = int.Parse(loadData[index++]);
		getChampData.skillActionUniqueKey = int.Parse(loadData[index++]);
		getChampData.ultimateActionUniqueKey = int.Parse(loadData[index++]);
		getChampData.champDescription = loadData[index++];

		getChampData.findTargetData = new FindTargetData();
		getChampData.findTargetData.targetDecideKind = (TargetDecideKind)int.Parse(loadData[index++]);
		getChampData.findTargetData.targetTeamKind = (TargetTeamKind)int.Parse(loadData[index++]);
		getChampData.findTargetData.impactRange = float.Parse(loadData[index++]);
		getChampData.findTargetData.actionStartPointKind = (ActionStartPointKind)int.Parse(loadData[index++]);
		getChampData.findTargetData.isIncludeMe = bool.Parse(loadData[index++]);
		getChampData.findTargetData.priorityKind = (FindTargetPriorityKind)int.Parse(loadData[index++]);

		// Champion Resource 데이터 설정..
		getResourceData.champIconImagePath = loadData[index++];
		getResourceData.skillIconImagePath = loadData[index++];
		getResourceData.ultimateIconImagePath = loadData[index++];

		return true;
	}
	#endregion

	#region Pilot Save Load Logic
	// ============================================================================================== //
	// ==================================== Pilot Save Load.. ======================================= //
	// ============================================================================================== //
	public static bool LoadPilotFile(string fileData, out PilotData getPilotData)
	{
		getPilotData = new PilotData();

		// 파일럿 데이터 초기화..
		string[] loadData = fileData.Split(',');

		int index = 0;

		getPilotData.name = loadData[index++];
		getPilotData.atkStat = int.Parse(loadData[index++]);
		getPilotData.defStat = int.Parse(loadData[index++]);

		// 파일럿의 챔피언 숙련도 데이터 초기화..
		int champSkillLevelDataCount = int.Parse(loadData[index++]);

		getPilotData.champSkillLevelContainer = new List<ChampionSkillLevelInfo>(champSkillLevelDataCount);

		for (int i = 0; i < champSkillLevelDataCount; ++i)
		{
			getPilotData.champSkillLevelContainer.Add(new ChampionSkillLevelInfo
			{
				champName = loadData[index++],
				level = int.Parse(loadData[index++])
			});
		}

		index = 12;
		getPilotData.hairNumber = int.Parse(loadData[index++]);

		return true;
	}
	#endregion

	#region Effect Save Load Logic
	// ============================================================================================== //
	// ==================================== Effect Save Load.. ====================================== //
	// ============================================================================================== //
	public static bool LoadEffectFile(string fileData, out EffectData getEffectData)
	{
		string[] loadData = fileData.Split(',');
		int index = 0;

		// 이펙트 Data 파일에서 불러와 저장..
		getEffectData = new EffectData();
		getEffectData.name = loadData[index++];
		getEffectData.animClipPath = loadData[index++];
		getEffectData.offsetPos = new Vector3(float.Parse(loadData[index++]), float.Parse(loadData[index++]), float.Parse(loadData[index++]));
		getEffectData.isAutoDestroy = bool.Parse(loadData[index++]);
		getEffectData.isUseLifeTime = bool.Parse(loadData[index++]);
		getEffectData.lifeTime = float.Parse(loadData[index++]);
		getEffectData.rotationType = (EffectRotationType)int.Parse(loadData[index++]);
		getEffectData.isBecomeTargetChild = bool.Parse(loadData[index++]);
		getEffectData.sortingOrder = int.Parse(loadData[index++]);

		return true;
	}
	#endregion

	#region AttackAction Save Load Logic
	// ============================================================================================== //
	// ================================ AttackAction Save Load.. ==================================== //
	// ============================================================================================== //
	public static bool LoadAttackActionFile(string[] fileData, out AttackActionData getActionData, out List<AttackImpactData> getImpactDatas,
		out AttackPerformanceData getPerformaceData, out AttackActionEffectData getEffectData)
	{
		string[] loadData = null;
		int index = 0;

		// Action Data 저장..
		loadData = fileData[0].Split(',');
		index = 1;

		getActionData = new AttackActionData();
		getActionData.uniqueKey = int.Parse(loadData[index++]);

		getActionData.findTargetData = new FindTargetData();
		getActionData.findTargetData.targetDecideKind = (TargetDecideKind)int.Parse(loadData[index++]);
		getActionData.findTargetData.targetTeamKind = (TargetTeamKind)int.Parse(loadData[index++]);
		getActionData.findTargetData.impactRange = float.Parse(loadData[index++]);
		getActionData.findTargetData.actionStartPointKind = (ActionStartPointKind)int.Parse(loadData[index++]);
		getActionData.findTargetData.isIncludeMe = bool.Parse(loadData[index++]);
		getActionData.findTargetData.priorityKind = (FindTargetPriorityKind)int.Parse(loadData[index++]);
		getActionData.description = loadData[index++];
		getActionData.isInvincible = bool.Parse(loadData[index++]);
		getActionData.rangeType = (AtkRangeType)int.Parse(loadData[index++]);
		getActionData.atkRange = float.Parse(loadData[index++]);

		// Summon Data 저장..
		getActionData.isSummon = bool.Parse(loadData[index++]);
		if (true == getActionData.isSummon)
		{
			getActionData.summonData = new AtkActionSummonData();

			getActionData.summonData.summonObjectType = (SummonObjectType)int.Parse(loadData[index++]);
			getActionData.summonData.summonObjectName = loadData[index++];
			getActionData.summonData.isSummonOnce = bool.Parse(loadData[index++]);
			getActionData.summonData.tickTime = float.Parse(loadData[index++]);
			getActionData.summonData.offsetPosition = new Vector3(
				float.Parse(loadData[index++]), float.Parse(loadData[index++]), float.Parse(loadData[index++]));
		}

		// Default Shoow Effect 저장..
		index = 20;
		getEffectData = new AttackActionEffectData();
		getEffectData.isShowEffect = bool.Parse(loadData[index++]);
		if(true == getEffectData.isShowEffect)
		{
			getEffectData.showEffectName = loadData[index++];
			getEffectData.effectPointKind = (ActionStartPointKind)int.Parse(loadData[index++]);
		}

		// Passive Data 저장..
		index = 23;
		getActionData.isPassive = bool.Parse(loadData[index++]);
		if(true == getActionData.isPassive)
		{
			getActionData.passiveData = new AtkActionPassiveData();

			getActionData.passiveData.impactTimeKind = (PassiveImpactTimeKind)int.Parse(loadData[index++]);
			getActionData.passiveData.isPlayUltAnim = bool.Parse(loadData[index++]);

			int ContainerSize = int.Parse(loadData[index++]);
			getActionData.passiveData.alwaysImpactDatas = new List<AttackImpactData>(ContainerSize);
			for( int i = 0; i < ContainerSize; ++i)
			{
				index = 25 + 13 * i;

				AttackImpactData impactData = new AttackImpactData();

				impactData.mainData = new AttackImpactMainData();
				impactData.mainData.kind = (AttackImpactEffectKind)int.Parse(loadData[index++]);
				impactData.mainData.detailKind = int.Parse(loadData[index++]);
				impactData.mainData.amount = float.Parse(loadData[index++]);
				impactData.mainData.duration = float.Parse(loadData[index++]);
				impactData.mainData.tickTime = float.Parse(loadData[index++]);

				impactData.isSeparateTargetFindLogic = bool.Parse(loadData[index++]);
				if(impactData.isSeparateTargetFindLogic)
				{
					impactData.findTargetData = new FindTargetData();
					impactData.findTargetData.targetDecideKind = (TargetDecideKind)int.Parse(loadData[index++]);
					impactData.findTargetData.targetTeamKind = (TargetTeamKind)int.Parse(loadData[index++]);
					impactData.findTargetData.impactRange = float.Parse(loadData[index++]);
					impactData.findTargetData.actionStartPointKind = (ActionStartPointKind)int.Parse(loadData[index++]);
				}

				index = 35 + 13 * i;
				impactData.isShowEffect = bool.Parse(loadData[index++]);
				if(impactData.isShowEffect)
				{
					impactData.effectData = new AttackActionEffectData();
					impactData.effectData.isShowEffect = true;
					impactData.effectData.showEffectName = loadData[index++];
					impactData.effectData.effectPointKind = (ActionStartPointKind)int.Parse(loadData[index++]);
				}

				getActionData.passiveData.alwaysImpactDatas.Add(impactData);
			}
		}

		// Attack Impact Data 저장..
		loadData = fileData[1].Split(',');
		index = 1;

		int impactDataSize = int.Parse(loadData[index++]);
		getImpactDatas = new List<AttackImpactData>(impactDataSize);
		for( int i = 0; i < impactDataSize; ++i)
		{
			index = 2 + 13 * i;

			AttackImpactData impactData = new AttackImpactData();

			impactData.mainData = new AttackImpactMainData();
			impactData.mainData.kind = (AttackImpactEffectKind)int.Parse(loadData[index++]);
			impactData.mainData.detailKind = int.Parse(loadData[index++]);
			impactData.mainData.amount = float.Parse(loadData[index++]);
			impactData.mainData.duration = float.Parse(loadData[index++]);
			impactData.mainData.tickTime = float.Parse(loadData[index++]);

			impactData.isSeparateTargetFindLogic = bool.Parse(loadData[index++]);
			if (impactData.isSeparateTargetFindLogic)
			{
				impactData.findTargetData = new FindTargetData();
				impactData.findTargetData.targetDecideKind = (TargetDecideKind)int.Parse(loadData[index++]);
				impactData.findTargetData.targetTeamKind = (TargetTeamKind)int.Parse(loadData[index++]);
				impactData.findTargetData.impactRange = float.Parse(loadData[index++]);
				impactData.findTargetData.actionStartPointKind = (ActionStartPointKind)int.Parse(loadData[index++]);
			}

			index = 12 + 13 * i;
			impactData.isShowEffect = bool.Parse(loadData[index++]);
			if (impactData.isShowEffect)
			{
				impactData.effectData = new AttackActionEffectData();
				impactData.effectData.isShowEffect = true;
				impactData.effectData.showEffectName = loadData[index++];
				impactData.effectData.effectPointKind = (ActionStartPointKind)int.Parse(loadData[index++]);
			}

			getImpactDatas.Add(impactData);
		}


		// Attack Performance 저장..
		loadData = fileData[2].Split(',');
		index = 1;

		getPerformaceData = new AttackPerformanceData();
		getPerformaceData.isUsePerf = bool.Parse(loadData[index++]);
		if(true == getPerformaceData.isUsePerf)
		{
			getPerformaceData.perfType = (AttackPerformanceType)int.Parse(loadData[index++]);
			getPerformaceData.detailType = int.Parse(loadData[index++]);

			int vectorDataSize = int.Parse(loadData[index++]);
			int floatDataSize = int.Parse(loadData[index++]);
			if (vectorDataSize > 0)
			{
				getPerformaceData.vectorData = new Vector3[vectorDataSize];
				for (int i = 0; i < vectorDataSize; ++i)
				{
					getPerformaceData.vectorData[i] = new Vector3(
						float.Parse(loadData[index++]), float.Parse(loadData[index++]), float.Parse(loadData[index++]));
				}
			}

			if (floatDataSize > 0)
			{
				getPerformaceData.floatData = new float[floatDataSize];
				for (int i = 0; i < floatDataSize; ++i)
				{
					getPerformaceData.floatData[i] = float.Parse(loadData[index++]);
				}
			}
		}

		return true;
	}
	#endregion

	#region Team Save Load Logic
	// ============================================================================================== //
	// ================================ Team Save Load.. ==================================== //
	// ============================================================================================== //
	public static bool LoadTeamFile(string fileData, out TeamData getData, out TeamBelongPilotData getBelongPilotData, out TeamResourceData getResourceData)
	{
		int curIndex = 0;
		string[] loadData = fileData.Split(',');

		getData = new TeamData();
		getData.name = loadData[curIndex++];

		getBelongPilotData = new TeamBelongPilotData();
		getBelongPilotData.pilotCount = int.Parse(loadData[curIndex++]);
		getBelongPilotData.pilotNameContainer = new List<string>(getBelongPilotData.pilotCount);
		for (int i = 0; i < getBelongPilotData.pilotCount; ++i)
		{
			getBelongPilotData.pilotNameContainer.Add(loadData[curIndex++]);
		}

		curIndex = 6;

		getResourceData = new TeamResourceData
		{
			logoImagePath = loadData[curIndex++],
		};

		return true;
	}
	#endregion
}