using System;
using UnityEngine;
using Util.Pool;

/// <summary>
/// 이펙트를 관리할 매니저 클래스..
/// </summary>
public class EffectManager : MonoBehaviour
{
	public event Action OnForcedRelease;

	public class EffectPooler : ObjectPooler<Effect>
	{
		public EffectPooler(Func<Effect> cretaeFunc, Action<Effect> actionInGet = null, Action<Effect> actionInRelease = null, Action<Effect> actionInDestroy = null, int defaultCapacity = 10, int defaultCreateCount = 10, int maxSize = 10000)
			: base(cretaeFunc, actionInGet, actionInRelease, actionInDestroy, defaultCapacity, defaultCreateCount, maxSize)
		{
		}
	}

	// 이펙트 기본 프리팹 관련 변수들..
	private Effect _effectDefaultPrefab;

	public GameManager gameManager
	{
		set
		{
			_effectDataTable = value.dataTableManager.effectDataTable;
			_effectDefaultPrefab = Resources.Load<Effect>(_effectDataTable.DEFAULT_EFFECT_PREFAB_PATH);

			value.OnChangeScene -= OnChangeScene;
			value.OnChangeScene += OnChangeScene;
		}
	}
	private EffectDataTable _effectDataTable;

	// 이펙트 오브젝트 풀링을 위한 풀러 객체..
	private EffectPooler _pooler;

	private void Awake()
	{
		Effect.s_effectManager = this;
		Champion.s_effectManager = this;
		ChampionBT.s_effectManager = this;
	}

	private void Start()
	{
		_pooler = new EffectPooler(CreateEffect, OnGetEffect, OnReleaseEffect, null, 20, 20, 40);
	}

	private void OnChangeScene()
	{
		OnForcedRelease?.Invoke();
	}

	/// <summary>
	/// 이펙트 오브젝트 풀러에서 사용할 이펙트 객체 생성 함수..
	/// </summary>
	/// <returns></returns>
	private Effect CreateEffect()
	{
		Effect newEffect = Instantiate<Effect>(_effectDefaultPrefab);

		DontDestroyOnLoad(newEffect.gameObject);

		OnGetEffect(newEffect);

		return newEffect;
	}

	/// <summary>
	/// 외부에서 이펙트를 실행할 때 사용할 함수..
	/// </summary>
	/// <param name="effectName"> 가져오고 싶은 이펙트 이름.. </param>
	/// /// <param name="position"> 이펙트 기본 위치.. </param>
	public void ShowEffect(string effectName, in Vector3 position, bool flipX = false)
	{
		Effect outEffect = _pooler.Get();

		outEffect.data = _effectDataTable.GetEffectInfo(effectName);
		outEffect.clip = _effectDataTable.GetEffectAnimClip(effectName);
		outEffect.flipX = flipX;

		outEffect.transform.position = position;

		outEffect.gameObject.SetActive(true);
	}

	/// <summary>
	/// 외부에서 이펙트를 가져오려고 할 때 사용할 함수..
	/// </summary>
	/// <param name="effectName"> 가져오고 싶은 이펙트 이름.. </param>
	public Effect GetEffect(string effectName, in Vector3 position)
    {
		Effect outEffect = _pooler.Get();

		outEffect.data = _effectDataTable.GetEffectInfo(effectName);
		outEffect.clip = _effectDataTable.GetEffectAnimClip(effectName);

		outEffect.transform.position = position;

		return outEffect;
    }

	/// <summary>
	/// 외부에서 이펙트를 가져오려고 할 때 사용할 함수..
	/// </summary>
	/// <param name="effectName"> 가져오고 싶은 이펙트 이름.. </param>
	public Effect GetEffect(string effectName, in Vector3 position, bool flipX = false)
	{
		Effect outEffect = _pooler.Get();

		outEffect.data = _effectDataTable.GetEffectInfo(effectName);
		outEffect.clip = _effectDataTable.GetEffectAnimClip(effectName);
		outEffect.flipX = flipX;

		outEffect.transform.position = position;

		return outEffect;
	}

	/// <summary>
	/// 이펙트가 할 일을 끝내고 다시 풀러에 들어갈 때 호출될 함수..
	/// </summary>
	public void ReleaseEffect(Effect effect)
	{
		_pooler.Release(effect);
	}

	/// <summary>
	/// 사용하는 이펙트는 나의 자식에서 뺀다(추후에 최종 빌드 시에는 제거할 예정)..
	/// </summary>
	private void OnGetEffect(Effect effect)
	{
#if UNITY_EDITOR
		Debug.Assert(null != effect, "Effect is null");
#endif

		effect.transform.parent = null;
	}

	/// <summary>
	/// Hierarchy 창 관리를 위해 사용하지 않는 이펙트를 나의 자식으로 둔다(추후에 최종 빌드 시에는 제거할 예정)..
	/// </summary>
	private void OnReleaseEffect(Effect effect)
	{
#if UNITY_EDITOR
		Debug.Assert(null != effect, "Effect is null");
#endif

		effect.transform.parent = transform;
		effect.gameObject.SetActive(false);
	}
}
