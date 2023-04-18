﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TestCreateDataSaveFile : MonoBehaviour
{
	public TestAttackActionScriptableObject attackActionDataPrefab;
	public KeyCode attackActionCreateKey = KeyCode.A;

	public TestChampionDataScriptableObject championDataPrefab;
	public KeyCode championCreateKey = KeyCode.S;

	public TestEffectDataScriptableObject effectDataPrefab;
	public KeyCode effectCreateKey = KeyCode.D;

	public KeyCode allCreateKey = KeyCode.Space;
	public TestScriptableObjectContainer container;

	public Text text;

	public void Awake()
	{
		if (null == text)
			return;

		text.text = $"공격 행동 생성 키 : {attackActionCreateKey.ToString()}\n"
			+ $"챔피언 생성 키 : {championCreateKey.ToString()}\n"
			+ $"이펙트 생성 키 : {effectCreateKey.ToString()}\n"
			+ $"모두 생성 키 : {allCreateKey.ToString()}\n"
			;
	}

	private void Update()
	{
		if (Input.GetKeyDown(attackActionCreateKey))
		{
			CreateAtkActionFile(attackActionDataPrefab);
		}

		if (Input.GetKeyDown(championCreateKey))
		{
			CreateChampionFile(championDataPrefab);

			Debug.Log($"{championDataPrefab.championData.name} 파일 생성 완료");
		}

		if (Input.GetKeyDown(effectCreateKey))
		{
			CreateEffectFile(effectDataPrefab);

			Debug.Log($"{effectDataPrefab.effectData.name} 파일 생성 완료");
		}

		if (Input.GetKeyDown(allCreateKey))
		{
			CreateAll(true);
		}
	}

	public void CreateAll(bool isShowLog = false)
	{
		TestAttackActionScriptableObject[] allAtkActionPrefab = container.allAtkActionPrefab;
		TestChampionDataScriptableObject[] allChampionDataPrefab = container.allChampionDataPrefab;
		TestEffectDataScriptableObject[] allEffectDataPrefab = container.allEffectDataPrefab;
		TestPilotScriptableObject[] allPilotDataPrefab = container.allPilotDataPrefab;
        TestTeamDataScriptableObject[] allTeamDataPrefab = container.allTeamDataPrefab;

        for (int i = 0; i < allAtkActionPrefab.Length; ++i)
		{
			CreateAtkActionFile(allAtkActionPrefab[i], isShowLog);
		}

		for (int i = 0; i < allChampionDataPrefab.Length; ++i)
		{
			CreateChampionFile(allChampionDataPrefab[i], isShowLog);
		}

		for (int i = 0; i < allEffectDataPrefab.Length; ++i)
		{
			CreateEffectFile(allEffectDataPrefab[i], isShowLog);
		}

        for ( int i = 0; i < allPilotDataPrefab.Length; ++i )
        {
            CreatePilotFile( allPilotDataPrefab[i], isShowLog );
        }

        for ( int i = 0; i < allTeamDataPrefab.Length; ++i )
        {
            CreateTeamFile( allTeamDataPrefab[i], isShowLog );
        }

        if (isShowLog)
			Debug.Log($"모든 파일 생성 완료");
	}

	private void CreateAtkActionFile(TestAttackActionScriptableObject obj, bool isShowLog = true)
	{
		SaveLoadLogic.SaveAttackActionFile(obj.actionData, obj.impactData,
				obj.performanceData, obj.effectData,
				obj.baseFilePath, obj.name, obj.extension);

		if (isShowLog)
			Debug.Log($"{obj.name} 파일 생성 완료");
	}

	private void CreateChampionFile(TestChampionDataScriptableObject obj, bool isShowLog = true)
	{
		SaveLoadLogic.SaveChampionFile(obj.championStatus, obj.championData, obj.championResourceData,
				obj.baseFilePath, obj.championData.name, obj.extension);

		if (isShowLog)
			Debug.Log($"{obj.name} 파일 생성 완료");
	}

	private void CreateEffectFile(TestEffectDataScriptableObject obj, bool isShowLog = true)
	{
		SaveLoadLogic.SaveEffectFile(obj.effectData, obj.baseFilePath, obj.extension);

		if (isShowLog)
			Debug.Log($"{obj.name} 파일 생성 완료");
	}

    private void CreatePilotFile( TestPilotScriptableObject obj, bool isShowLog = true )
    {
        SaveLoadLogic.SavePilotFile( obj.pilotData, obj.baseFilePath, obj.pilotData.name, obj.extension );

        if ( isShowLog )
            Debug.Log( $"{obj.name} 파일 생성 완료" );
    }

    private void CreateTeamFile( TestTeamDataScriptableObject obj, bool isShowLog = true )
    {
        SaveLoadLogic.SaveTeamFile( obj.teamData, obj.belongPilotData, obj.resourceData, obj.baseFilePath, obj.extension );

        if ( isShowLog )
            Debug.Log( $"{obj.name} 파일 생성 완료" );
    }
}