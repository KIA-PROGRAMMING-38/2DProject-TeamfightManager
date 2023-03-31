using UnityEngine;

namespace MH_AIFramework
{
	/// <summary>
	/// 일정한 주기마다 실행되는 Node..
	/// 이 Node는 Composite Node의 자식으로 추가되며 Composite Node가 Update될 때 시간을 계산해 자동을로 일정 주기마다 실행된다..
	/// </summary>
	public abstract class ServiceNode : Node
	{
		protected float _updateTick = 0.0f;
		protected float _pivotTime = 0.0f;

		public ServiceNode( float updateTick = 0.0f )
		{
			_updateTick = updateTick;
		}

		public override void AddChild( Node child )
		{
			throw new System.ArgumentException();
		}

		public override void RemoveChild( Node child )
		{
			throw new System.ArgumentException();
		}

		protected override void OnStart()
		{
			_pivotTime = Time.time;
		}

		protected override void OnStop()
		{
			_pivotTime = 0.0f;
		}

		protected sealed override State OnUpdate()
		{
			if ( Time.time - _pivotTime >= _updateTick )
			{
				return UpdateService();
			}

			return State.Running;
		}

		protected abstract State UpdateService();
	}
}