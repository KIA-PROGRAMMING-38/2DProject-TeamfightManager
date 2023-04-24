using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ü���� ���Ϸ����� �ʿ��� ��� �� ���Ϸ��� �ٸ� ��� ��ũ��Ʈ���� �����ϴ� ���..
/// </summary>
public class Pilot : MonoBehaviour
{
	public PilotManager pilotManager { private get; set; }
    public PilotData data { get; private set; }
    public PilotBattle battleComponent { get; private set; }

	private void Awake()
	{
		battleComponent = GetComponent<PilotBattle>();
	}

	public void Initialize(PilotData data)
	{
		this.data = data;
	}

	public void Release()
	{
		pilotManager.ReleasePilot(this);
	}
}
