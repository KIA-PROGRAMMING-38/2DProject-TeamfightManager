using UnityEngine;

namespace MH_AIFramework
{
	[System.Serializable]
	public sealed class DN_CheckQuaternionValue : DecoratorNode
	{
		[SerializeField] private Quaternion _cmpValue;
		[SerializeField] private string _bbKey;

		public DN_CheckQuaternionValue( Quaternion cmpValue, string bbKey)
		{
			_cmpValue = cmpValue;
			_bbKey = bbKey;
		}

		protected override void OnStart()
		{
			
		}

		protected override void OnStop()
		{
			
		}

		protected override State OnUpdate()
		{
			if ( blackboard.GetRotatorValue( _bbKey ) == _cmpValue )
				return State.Success;

			return State.Failure;
		}
	}
}
