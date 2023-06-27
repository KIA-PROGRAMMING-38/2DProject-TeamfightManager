using UnityEngine;

namespace MH_AIFramework
{
	public sealed class DN_CheckVectorValue : DecoratorNode
	{
		private Vector3 _cmpValue;
		private string _bbKey;

		private Vector3 _bbValue;

		public DN_CheckVectorValue(Node node, in Vector3 cmpValue, string bbKey)
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
			blackboard.GetVectorValue(_bbKey, out _bbValue);
			if (_bbValue == _cmpValue)
			{
				return base.OnUpdate();
			}

			return State.Failure;
		}
	}
}
