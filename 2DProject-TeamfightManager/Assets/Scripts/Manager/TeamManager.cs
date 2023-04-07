using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 팀을 관리하는 매니저 클래스..
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
