using UnityEngine;

namespace MH_AIFramework
{
	public sealed class DN_CheckStringValue : DecoratorNode
	{
		private string _cmpValue;
		private string _bbKey;

		public DN_CheckStringValue( string cmpValue, string bbKey)
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
			if ( blackboard.GetStringValue( _bbKey ) == _cmpValue )
				return State.Success;

			return State.Failure;
		}
	}
}
