using UnityEngine;
using MH_AIFramework;

public class ChampionBlackboard : Blackboard
{
	private void OnEnable()
	{
		SetupBlackboardKey();
	}

	private void SetupBlackboardKey()
	{
		// Bool 값들 기본 세팅..
		SetBoolValue( "isAttack", true );
		SetBoolValue( "isDeath", false );
		SetBoolValue( "isMoveLock" , false);

		// float 값들 기본 세팅..
		SetFloatValue("targetDistance", float.MaxValue);

		// int 값들 기본 세팅..

		// Object 값들 기본 세팅..
		SetObjectValue( "target", null );
	}
}