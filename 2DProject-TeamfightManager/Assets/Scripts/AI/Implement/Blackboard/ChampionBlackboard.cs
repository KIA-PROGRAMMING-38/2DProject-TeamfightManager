using UnityEngine;
using MH_AIFramework;

public class ChampionBlackboard : Blackboard
{
	private void Awake()
	{
		
	}

	private void OnEnable()
	{
		SetupBlackboardKey();
	}

	private void SetupBlackboardKey()
	{
		// Bool ���� �⺻ ����..
		SetBoolValue( "isAttack", true );
		SetBoolValue( "isDeath", false );
		SetBoolValue( "isMoveLock" , false);

		// float ���� �⺻ ����..
		SetFloatValue("targetDistance", float.MaxValue);

		// int ���� �⺻ ����..

		// Object ���� �⺻ ����..
		SetObjectValue( "target", null );
	}
}