using UnityEngine;

namespace MH_AIFramework
{
	public sealed class DN_CheckObjectValue : DecoratorNode
	{
		private object _cmpValue;
		private string _bbKey;

		public DN_CheckObjectValue( object cmpValue, string bbKey)
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
			if ( blackboard.GetObjectValue( _bbKey ) == _cmpValue )
				return State.Success;

			return State.Failure;
		}
	}
}
