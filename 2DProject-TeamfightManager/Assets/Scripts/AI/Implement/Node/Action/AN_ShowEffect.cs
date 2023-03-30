using MH_AIFramework;
using UnityEngine;

public class AN_ShowEffect : ActionNode
{
	private EffectManager _effectManager;
	private Transform _transform;
	private Champion _champion;

	private Effect _effect;

	public AN_ShowEffect(EffectManager effectManager)
	{
		_effectManager = effectManager;
	}

	public override void OnDisable()
	{
		base.OnDisable();

		_effect?.Release();
	}

	public override void OnCreate()
	{
		base.OnCreate();

		_champion = blackboard.gameObject.GetComponent<Champion>();
		_transform = blackboard.gameObject.transform;
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

		_effect = _effectManager.GetEffect(_champion.atkEffectName, _transform.position, blackboard.GetBoolValue("spriteFlipX"));
		_effect.gameObject.SetActive(true);

		return State.Success;
	}
}
