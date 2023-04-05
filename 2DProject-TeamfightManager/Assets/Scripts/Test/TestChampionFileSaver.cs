using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestChampionFileSaver : MonoBehaviour
{
	public string DefaultPath;
	public string FileName;
	public string FileExtension;

	public int atkStat;
	public float atkSpeed;
	public float range;
	public int defence;
	public int hp;
	public float moveSpeed;

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Return))
		{
			AttackActionData attackAction = new AttackActionData
			{
				isPassive = false,
				uniqueKey = 1,
				impactRange = 2,
				impactRangeType = (int)ImpactRangeKind.Range_Circle
			};

			AttackImpactData attackImpactData = new AttackImpactData
			{
				amount = 20,
				detailKind = (int)AttackImpactType.DefaultAttack,
				duration = 0,
				kind = (int)AttackImpactEffectKind.Attack,
				tickTime = 0,
				targetDecideKind = (int)TargetDecideKind.AllTarget
			};

			AttackImpactData attackImpactData2 = new AttackImpactData
			{
				amount = 3,
				detailKind = (int)DebuffImpactType.DecreaseDefneceStat,
				duration = 1f,
				kind = (int)AttackImpactEffectKind.Debuff,
				tickTime = 0,
				targetDecideKind = (int)TargetDecideKind.AllTarget
			};

			List<AttackImpactData> attackImpactDataList = new List<AttackImpactData>();
			attackImpactDataList.Add(attackImpactData);
			attackImpactDataList.Add(attackImpactData2);

			GameSaveLoader.SaveAttackActionFile(attackAction, attackImpactDataList, "Assets/Data", "Swordman_Skill", ".data");
			//GameSaveLoader.SaveChampionFile(CreateChampionStatus(), CreateChampionData(), CreateChampionResourceData(), DefaultPath, FileName, FileExtension);
			//Debug.Log($"챔피언 파일 생성 완료!! : {Path.Combine(DefaultPath, "Champion", FileName + FileExtension)}");
		}
	}

	public ChampionStatus CreateChampionStatus()
	{
		return new ChampionStatus
		{
			atkStat = atkStat,
			atkSpeed = atkSpeed,
			range = range,
			defence = defence,
			hp = hp,
			moveSpeed = moveSpeed,
		};
	}

	public ChampionData CreateChampionData()
	{
		return new ChampionData();
	}

	public ChampionResourceData CreateChampionResourceData()
	{
		return new ChampionResourceData();
	}
}
