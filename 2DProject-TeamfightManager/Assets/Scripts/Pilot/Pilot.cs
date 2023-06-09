using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전체적인 파일럿에게 필요한 기능 및 파일럿의 다른 기능 스크립트들을 관리하는 기능..
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
