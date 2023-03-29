using UnityEngine;

namespace MH_AIFramework
{
	[System.Serializable]
	public sealed class AN_PrintLog : ActionNode
	{
		[SerializeField] private string _message;

		public AN_PrintLog( string message )
		{
			_message = message;
		}

		protected override void OnStart()
		{
			
		}

		protected override void OnStop()
		{
			
		}

		protected override State OnUpdate()
		{
			Debug.Log( _message );

			return State.Success;
		}
	}
}
