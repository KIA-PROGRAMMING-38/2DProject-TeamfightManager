using UnityEngine;

namespace MH_AIFramework
{
	[System.Serializable]
	public sealed class DN_CheckStringValue : DecoratorNode
	{
		[SerializeField] private string _cmpValue;
		[SerializeField] private string _bbKey;

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
