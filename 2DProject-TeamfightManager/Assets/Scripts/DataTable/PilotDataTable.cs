using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���Ϸ��� ���õ� �������� �����ϴ� ������ ���̺�..
/// </summary>
public class PilotDataTable
{
	private List<Sprite> _conditionSpriteContainer = new List<Sprite>();

	private Dictionary<string, PilotData> _pilotDataContainer = new Dictionary<string, PilotData>();

	public Dictionary<string, PilotData> pilotDataContainer { get => _pilotDataContainer; }

	public Pilot DefaultPilotPrefab { get; private set; }

	// Champion Status Getter Setter..
	public void AddPilotData(string championName, PilotData championStatus)
	{
		pilotDataContainer.Add(championName, championStatus);
	}

	public PilotData GetPilotData(string championName)
	{
		return pilotDataContainer[championName];
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
