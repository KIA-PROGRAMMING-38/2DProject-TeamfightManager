using UnityEngine;

namespace MH_AIFramework
{
	/// <summary>
	/// 디버깅 용도로 Log를 찍는 Node..
	/// </summary>
	public sealed class AN_PrintLog : ActionNode
	{
		private string _message;

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
