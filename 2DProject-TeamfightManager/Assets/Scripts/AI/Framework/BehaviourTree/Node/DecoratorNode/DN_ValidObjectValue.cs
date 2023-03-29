using UnityEngine;

namespace MH_AIFramework
{
	public class DN_ValidObjectValue : DecoratorNode
	{
		[SerializeField] private string _bbKey;

		public DN_ValidObjectValue( string bbKey )
		{
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
			if ( null != blackboard.GetObjectValue( _bbKey ) )
				return State.Success;

			return State.Failure;
		}
	}
}
