﻿using UnityEngine;

namespace MH_AIFramework
{
	[System.Serializable]
	public sealed class DN_CheckVectorValue : DecoratorNode
	{
		[SerializeField] private Vector3 _cmpValue;
		[SerializeField] private string _bbKey;

		private Vector3 _bbValue;

		public DN_CheckVectorValue( Vector3 cmpValue, string bbKey)
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
				return State.Success;

			return State.Failure;
		}
	}
}
