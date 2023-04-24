using System;
using System.Collections.Generic;
using UnityEngine;
using Util.Pool;

public class SummonObjectManager : MonoBehaviour
{
	public event Action OnForcedRelease;

	public GameManager gameManager
	{
		set 
		{
			_gameManager = value;
			_effectManager = _gameManager.effectManager;

			int loopCount = _gameManager.gameGlobalData.spawnObjectGolbalData.prefabContainer.Length;
			_summonPoolerContainer = new Dictionary<string, ObjectPooler<SummonObject>>(loopCount);

			for (int i = 0; i < loopCount; ++i)
			{
				SummonObject prefab = _gameManager.gameGlobalData.spawnObjectGolbalData.prefabContainer[i].GetComponent<SummonObject>();

				ObjectPooler<SummonObject> pooler = new ObjectPooler<SummonObject>(
					() => CreateSummonObject(prefab),
					null, OnRelease, null, 5, 2, 100);

				_summonPoolerContainer.Add(prefab.summonObjectName, pooler);
			}

			value.OnChangeScene -= OnChangeScene;
			value.OnChangeScene += OnChangeScene;
		}
	}

	private GameManager _gameManager;
	private EffectManager _effectManager;

	private Dictionary<string, ObjectPooler<SummonObject>> _summonPoolerContainer;

	private void Awake()
	{
		Champion.s_summonObjeectManager = this;
	}

	private void OnChangeScene()
	{
		OnForcedRelease?.Invoke();
	}

	public T GetSummonObject<T>(string summonName) where T : SummonObject
	{
		return _summonPoolerContainer[summonName].Get() as T;
	}

	public void ReleaseSummonObject(SummonObject summonObject)
	{
		_summonPoolerContainer[summonObject.summonObjectName].Release(summonObject);
	}

	private SummonObject CreateSummonObject(SummonObject prefab)
	{
		SummonObject newObject = Instantiate(prefab);

		DontDestroyOnLoad(newObject.gameObject);

		newObject.summonObjectManager = this;
		newObject.effectManager = _effectManager;

		return newObject;
	}

	private void OnRelease(SummonObject summonObject)
	{
		summonObject.gameObject.SetActive(false);
	}
}