using MH_AIFramework;
using UnityEngine;

/// <summary>
/// ¿Ã∆Â∆Æ ≈Õ∂ﬂ∑¡¡÷¥¬ Node..
/// </summary>
public class AN_ShowEffect : ActionNode
{
	private EffectManager _effectManager;

	private Transform _transform;
	private Champion _champion;

	private Effect _effect;
	private string _effectCategory;

	public AN_ShowEffect(EffectManager effectManager, string effectCategory)
	{
		_effectManager = effectManager;
		_effectCategory = effectCategory;
	}

	public override void OnDisable()
	{
		base.OnDisable();

		if(null != _effect)
		{
			_effect.OnDisableEvent -= ReceiveDisableEffectEvent;
			_effect.Release();
		}
	}

	public override void OnCreate()
	{
		base.OnCreate();

		_champion = behaviourTree.gameObject.GetComponent<Champion>();
		_transform = behaviourTree.gameObject.transform;
	}

	protected override void OnStart()
	{

	}

	protected override void OnStop()
	{
		
	}

	protected override State OnUpdate()
	{
#if UNITY_EDITOR
		Debug.Assert(null != _effectManager);
		Debug.Assert(null != _transform);
		Debug.Assert(null != _champion);
#endif

		if (null != _effect)
			_effect.OnDisableEvent -= ReceiveDisableEffectEvent;

		string effectName = _champion.ComputeEffectName(_effectCategory);

		_effect = _effectManager.GetEffect(effectName, _transform.position, _champion.flipX);

		_effect.OnDisableEvent -= ReceiveDisableEffectEvent;
		_effect.OnDisableEvent += ReceiveDisableEffectEvent;

		_effect.gameObject.SetActive(true);

		return State.Success;
	}

	private void ReceiveDisableEffectEvent(Effect effect)
	{
		effect.OnDisableEvent -= ReceiveDisableEffectEvent;
		_effect = null;
	}
}
