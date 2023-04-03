using System;
using UnityEngine;

namespace MH_AIFramework
{
	public sealed class DN_CheckFloatValue : DecoratorNode
	{
		private float _cmpValue;
		private string _bbKey;

		public DN_CheckFloatValue( float cmpValue, string bbKey)
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
			float dist = Math.Abs(blackboard.GetFloatValue( _bbKey ) - _cmpValue);
			if ( dist <= float.Epsilon )
				return State.Success;

			return State.Failure;
		}
	}
}
