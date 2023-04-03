using UnityEngine;

namespace MH_AIFramework
{
	public sealed class DN_CheckIntValue : DecoratorNode
	{
		private int _cmpValue;
		private string _bbKey;

		public DN_CheckIntValue(int cmpValue, string bbKey)
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
			if ( blackboard.GetIntValue( _bbKey ) == _cmpValue )
				return State.Success;

			return State.Failure;
		}
	}
}
