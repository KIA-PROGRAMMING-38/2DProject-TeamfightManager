using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �����ϴ� �Ŵ��� Ŭ����..
/// </summary>
public class TeamManager : MonoBehaviour
{
	public GameManager gameManager { private get; set; }
	public DataTableManager dataTableManager { private get; set; }
	public PilotManager pilotManager { private get; set; }

	private void Awake()
	{
		
	}
}
