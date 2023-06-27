using UnityEngine;

namespace MH_AIFramework
{
	public sealed class DN_CheckIntValue : DecoratorNode
	{
		private int _cmpValue;
		private string _bbKey;

		public DN_CheckIntValue(Node node, int cmpValue, string bbKey)
			: base(node)
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
			{
				return base.OnUpdate();
			}

			return State.Failure;
		}
	}
}
