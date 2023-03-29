using UnityEngine;

namespace MH_AIFramework
{
	[System.Serializable]
	public sealed class AN_Wait : ActionNode
	{
		[SerializeField] private float _waitTime = 0.0f;	// 기다릴 시간..
		[HideInInspector] private float _pivotTime = 0.0f;   // 중심 시간( 현재 시간 - 중심 시간 >= 기다릴 시간 ) == Success

		public AN_Wait( float waitTime )
		{
			_waitTime = waitTime;
		}

		public void SetWaitTime(float waitTime)
		{
			_waitTime = Mathf.Max( 0.0f, waitTime );
		}

		protected override void OnStart()
		{
			_pivotTime = Time.time;
		}

		protected override void OnStop()
		{
			_pivotTime = 0.0f;
		}

		protected override State OnUpdate()
		{
			// 현재 시간 - 중심 시간 < 기다릴 시간 == 아직 기다려야 한다..
			if ( Time.time - _pivotTime < _waitTime )
				return State.Running;

			return State.Success;
		}
	}
}
