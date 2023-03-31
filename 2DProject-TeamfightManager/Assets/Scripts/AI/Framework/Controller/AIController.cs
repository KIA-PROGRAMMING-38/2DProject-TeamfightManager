﻿using UnityEngine;

namespace MH_AIFramework
{
	/// <summary>
	/// 실제 BehaviourTree 를 실행시키는 존재..
	/// </summary>
	public class AIController : MonoBehaviour
	{
		protected BehaviourTree _behaviourTree = null;

		protected void Awake()
		{
			Debug.Assert( null != _behaviourTree );

			_behaviourTree.aiController = this;
		}

		private void OnEnable()
		{
			_behaviourTree.OnEnable();
		}

		private void OnDisable()
		{
			_behaviourTree.OnDisable();
		}

		protected void Update()
		{
			if(null != _behaviourTree)
			{
				_behaviourTree.Run();
			}
		}
	}
}
