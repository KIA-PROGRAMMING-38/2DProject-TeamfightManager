using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Util.Pool;

public class EffectManager : MonoBehaviour
{
	public class EffectPooler : ObjectPooler<Effect>
	{
		public EffectPooler(Func<Effect> cretaeFunc, Action<Effect> actionInGet = null, Action<Effect> actionInRelease = null, Action<Effect> actionInDestroy = null, int defaultCapacity = 10, int defaultCreateCount = 10, int maxSize = 10000)
			: base(cretaeFunc, actionInGet, actionInRelease, actionInDestroy, defaultCapacity, defaultCreateCount, maxSize)
		{
		}
	}

	// ����Ʈ �⺻ ������ ���� ������..
	private Effect _effectDefaultPrefab;

	public GameManager gameManager
	{
		set
		{
			_effectDataTable = value.dataTableManager.effectDataTable;
			_effectDefaultPrefab = Resources.Load<Effect>(_effectDataTable.DEFAULT_EFFECT_PREFAB_PATH);
		}
	}
	private EffectDataTable _effectDataTable;

	// ����Ʈ ������Ʈ Ǯ���� ���� Ǯ�� ��ü..
	private EffectPooler _pooler;

	private void Start()
	{
		_pooler = new EffectPooler(CreateEffect, OnGetEffect, OnReleaseEffect, null, 20, 20, 40);

		Effect.s_effectManager = this;
		Champion.s_effectManager = this;
		ChampionBT.s_effectManager = this;
	}

	/// <summary>
	/// ����Ʈ ������Ʈ Ǯ������ ����� ����Ʈ ��ü ���� �Լ�..
	/// </summary>
	/// <returns></returns>
	private Effect CreateEffect()
	{
		Effect newEffect = Instantiate<Effect>(_effectDefaultPrefab);
		OnGetEffect(newEffect);

		return newEffect;
	}

	/// <summary>
	/// �ܺο��� ����Ʈ�� ������ �� ����� �Լ�..
	/// </summary>
	/// <param name="effectName"> �������� ���� ����Ʈ �̸�.. </param>
	/// /// <param name="position"> ����Ʈ �⺻ ��ġ.. </param>
	public void ShowEffect(string effectName, in Vector3 position, bool flipX = false)
	{
		Effect outEffect = _pooler.Get();

		outEffect.info = _effectDataTable.GetEffectInfo(effectName);
		outEffect.clip = _effectDataTable.GetEffectAnimClip(effectName);
		outEffect.flipX = flipX;

		outEffect.transform.position = position;

		outEffect.gameObject.SetActive(true);
	}

	/// <summary>
	/// �ܺο��� ����Ʈ�� ���������� �� �� ����� �Լ�..
	/// </summary>
	/// <param name="effectName"> �������� ���� ����Ʈ �̸�.. </param>
	public Effect GetEffect(string effectName)
    {
		Effect outEffect = _pooler.Get();

		outEffect.info = _effectDataTable.GetEffectInfo(effectName);
		outEffect.clip = _effectDataTable.GetEffectAnimClip(effectName);

		return outEffect;
    }

	/// <summary>
	/// �ܺο��� ����Ʈ�� ���������� �� �� ����� �Լ�..
	/// </summary>
	/// <param name="effectName"> �������� ���� ����Ʈ �̸�.. </param>
	public Effect GetEffect(string effectName, in Vector3 position, bool flipX = false)
	{
		Effect outEffect = _pooler.Get();

		outEffect.info = _effectDataTable.GetEffectInfo(effectName);
		outEffect.clip = _effectDataTable.GetEffectAnimClip(effectName);
		outEffect.flipX = flipX;

		outEffect.transform.position = position;

		return outEffect;
	}

	/// <summary>
	/// ����Ʈ�� �� ���� ������ �ٽ� Ǯ���� �� �� ȣ��� �Լ�..
	/// </summary>
	/// <param name="effect"></param>
	public void ReleaseEffect(Effect effect)
	{
		_pooler.Release(effect);
	}

	/// <summary>
	/// ����ϴ� ����Ʈ�� ���� �ڽĿ��� ����(���Ŀ� ���� ���� �ÿ��� ������ ����)..
	/// </summary>
	/// <param name="effect"> ��Ȱ��ȭ�� ����Ʈ </param>
	private void OnGetEffect(Effect effect)
	{
#if UNITY_EDITOR
		Debug.Assert(null != effect, "Effect is null");
#endif

		effect.transform.parent = null;
	}

	/// <summary>
	/// Hierarchy â ������ ���� ������� �ʴ� ����Ʈ�� ���� �ڽ����� �д�(���Ŀ� ���� ���� �ÿ��� ������ ����)..
	/// </summary>
	/// <param name="effect"></param>
	private void OnReleaseEffect(Effect effect)
	{
#if UNITY_EDITOR
		Debug.Assert(null != effect, "Effect is null");
#endif

		effect.transform.parent = transform;
		effect.gameObject.SetActive(false);
	}
}
