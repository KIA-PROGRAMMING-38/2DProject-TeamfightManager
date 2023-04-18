using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 파일럿과 관련된 정보들을 저장하는 데이터 테이블..
/// </summary>
public class PilotDataTable
{
	private Dictionary<string, PilotData> _pilotDataContainer = new Dictionary<string, PilotData>();
	private List<Sprite> _conditionSpriteContainer = new List<Sprite>();
	private List<string> _pilotNameContainer = new List<string>();

	public Pilot DefaultPilotPrefab { get; private set; }

	public int GetTotalPilotCount() => _pilotNameContainer.Count;
	public string GetPilotName(int index) => _pilotNameContainer[index];

	// Champion Status Getter Setter..
	public void AddPilotData(string pilotName, PilotData pilotData)
	{
		_pilotDataContainer.Add(pilotName, pilotData);

		_pilotNameContainer.Add(pilotName);
	}

	public PilotData GetPilotData(string pilotName)
	{
		return _pilotDataContainer[pilotName];
	}

	public void AddConditionSprite(Sprite sprite)
	{
		_conditionSpriteContainer.Add(sprite);
	}

	public Sprite GetConditionSprite(PilotConditionState state)
	{
		return _conditionSpriteContainer[(int)state];
	}

	public void SetPilotDefaultPrefab(GameObject prefab)
	{
		DefaultPilotPrefab = prefab.GetComponent<Pilot>();
	}
}
