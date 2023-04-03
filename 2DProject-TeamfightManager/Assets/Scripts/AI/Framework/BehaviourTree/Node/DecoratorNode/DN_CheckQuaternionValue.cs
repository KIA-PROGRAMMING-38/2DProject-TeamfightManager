using UnityEngine;

namespace MH_AIFramework
{
	public sealed class DN_CheckQuaternionValue : DecoratorNode
	{
		private Quaternion _cmpValue;
		private string _bbKey;

		private Quaternion _bbValue;

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
			blackboard.GetRotatorValue(_bbKey, out _bbValue);
			if (_bbValue == _cmpValue)
				return State.Success;

			return State.Failure;
		}
	}
}
