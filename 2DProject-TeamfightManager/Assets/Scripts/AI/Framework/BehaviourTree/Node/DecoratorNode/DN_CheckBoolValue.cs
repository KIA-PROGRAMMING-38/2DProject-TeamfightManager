using UnityEngine;

namespace MH_AIFramework
{
	public sealed class DN_CheckBoolValue : DecoratorNode
	{
		private bool _cmpValue;
		private string _bbKey;

		public DN_CheckBoolValue( bool cmpValue, string bbKey )
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
			if ( blackboard.GetBoolValue( _bbKey ) == _cmpValue )
				return State.Success;

			return State.Failure;
		}
	}
}
