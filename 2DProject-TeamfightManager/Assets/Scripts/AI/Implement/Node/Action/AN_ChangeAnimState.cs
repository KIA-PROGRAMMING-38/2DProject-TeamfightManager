using MH_AIFramework;
using UnityEngine;

public class AN_ChangeAnimState : ActionNode
{
    private ChampionAnimation _championAnimation;
	private ChampionAnimation.AnimState _changeAnimState;
	private bool _isUpdateFlipX = false;

	public AN_ChangeAnimState(ChampionAnimation.AnimState changeAnimState, bool isUpdateFlipX = false)
	{
		_changeAnimState = changeAnimState;
		_isUpdateFlipX = isUpdateFlipX;
	}

	public override void OnCreate()
	{
		base.OnCreate();

		_championAnimation = blackboard.gameObject.GetComponentInChildren<ChampionAnimation>();
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
		Debug.Log("Change Anim State Update Ω√¿€");
		Debug.Assert(null != _championAnimation);
#endif

		if (true == _isUpdateFlipX)
		{
			Vector3 moveDirection;
			blackboard.GetVectorValue("moveDirection", out moveDirection);
			float moveDirX = moveDirection.x;
			if (0f != moveDirX)
			{
				bool flipX = moveDirX < 0f;
				blackboard.SetBoolValue("spriteFlipX", flipX);
				_championAnimation.flipX = flipX;
			}
		}

		_championAnimation.ChangeState(_changeAnimState);

#if UNITY_EDITOR
		Debug.Log("Change Anim State Update ≥°");
#endif

		return State.Success;
	}
}
