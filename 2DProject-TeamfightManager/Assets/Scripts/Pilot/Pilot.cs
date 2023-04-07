using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ü���� ���Ϸ����� �ʿ��� ��� �� ���Ϸ��� �ٸ� ��� ��ũ��Ʈ���� �����ϴ� ���..
/// </summary>
public class Pilot : MonoBehaviour
{
    public PilotData data { get; set; }
    public PilotBattle battleComponent { get; private set; }

	private void Awake()
	{
		battleComponent = GetComponent<PilotBattle>();
	}
}
