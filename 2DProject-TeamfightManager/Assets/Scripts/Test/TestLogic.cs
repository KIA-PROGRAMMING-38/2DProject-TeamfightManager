using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
	public string FilePath = "Assets/Data/Save1/PilotDataFile.csv";

	private void Awake()
	{
		
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Return))
		{
			string[] loadData = File.ReadAllLines(FilePath);

			PilotData[] pilotDatas = new PilotData[loadData.Length];
			int loopCount = loadData.Length;
			for (int i = 1; i < loopCount; ++i)
			{
				PilotData pilotData = Calc(loadData[i]);
				PilotData p = new PilotData();
				p.champSkillLevelContainer = new List<ChampionSkillLevelInfo>();
				p.champSkillLevelContainer.Add(new ChampionSkillLevelInfo());
				p.champSkillLevelContainer.Add(new ChampionSkillLevelInfo());
				p.champSkillLevelContainer.Add(new ChampionSkillLevelInfo());
				p.champSkillLevelContainer.Add(new ChampionSkillLevelInfo());

				int jLoopCount = 10;
				for (int j = 0; j < jLoopCount; ++j)
				{
					int stat = UnityEngine.Random.Range(20, 40);
					int atkStat = (int)(UnityEngine.Random.Range(pilotData.atkStat - 5, pilotData.atkStat + 5) * 0.01f * stat);
					int defStat = (int)(UnityEngine.Random.Range(pilotData.defStat - 5, pilotData.defStat + 5) * 0.01f * stat);

					p.atkStat += atkStat;
					p.defStat += defStat;

					int l = 20 / pilotData.champSkillLevelContainer.Count;
					for( int k = 0; k < pilotData.champSkillLevelContainer.Count; ++k)
					{
						int random = UnityEngine.Random.Range((int)(l * 0.5f), (int)(l * 2f));

						p.champSkillLevelContainer[k].champName = pilotData.champSkillLevelContainer[k].champName;
						p.champSkillLevelContainer[k].level += random;
					}
				}

				p.atkStat /= jLoopCount;
				p.defStat /= jLoopCount;

				string s = $"이름 : {p.name}, 데미지 : {p.atkStat}, 방어력 : {p.defStat}";
				foreach(var a in p.champSkillLevelContainer)
				{
					a.level /= jLoopCount;

					s += $", 숙련도 챔 이름 : {a.champName}, 레벨 : {a.level}";
				}

				Debug.Log(s);
			}
		}
	}

	private PilotData Calc(string fileData)
	{
		PilotData getPilotData = new PilotData();

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

		return getPilotData;
	}
}
