using UnityEngine;

namespace MH_AIFramework
{
	[System.Serializable]
	public abstract class ServiceNode : Node
	{
		[SerializeField] protected float _updateTick = 0.0f;
		[HideInInspector] protected float _pivotTime = 0.0f;

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