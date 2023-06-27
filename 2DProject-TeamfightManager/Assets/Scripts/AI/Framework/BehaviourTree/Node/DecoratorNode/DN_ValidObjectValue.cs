using UnityEngine;

namespace MH_AIFramework
{
	public class DN_ValidObjectValue : DecoratorNode
	{
		private string _bbKey;

		public DN_ValidObjectValue(Node node, string bbKey )
			: base( node )
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
			{
				return base.OnUpdate();
			}

			return State.Failure;
		}
	}
}
