using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class TestCreatePilotFile : MonoBehaviour
{
    [Serializable]
    public class ChampLevelData
    {
        public string champName;
        public int level;
    }

	public string DefaultPath;
	public string FileExtension;

	public string pilotName;
	public int atkStat;
	public int defStat;
	public List<ChampLevelData> champSkillLevelContainer;

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			GameSaveLoader.SavePilotFile(CreatePilotData(), DefaultPath, pilotName, FileExtension);
			Debug.Log($"���Ϸ� ���� ���� �Ϸ�!! : {Path.Combine(DefaultPath, "Pilot", pilotName + FileExtension)}");
		}
	}

	public PilotData CreatePilotData()
	{
        PilotData pilotData = new PilotData
		{
			name = pilotName,
			atkStat = atkStat,
			defStat = defStat,
		};

		foreach (var champLevelData in champSkillLevelContainer)
			pilotData.champSkillLevelContainer.Add(new(champLevelData.champName, champLevelData.level));

		return pilotData;
	}
}
