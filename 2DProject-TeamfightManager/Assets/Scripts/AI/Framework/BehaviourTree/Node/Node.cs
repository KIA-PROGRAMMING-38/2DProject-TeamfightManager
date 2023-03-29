using UnityEngine;

namespace MH_AIFramework
{
	[System.Serializable]
	public abstract class Node
	{
		public enum State
		{
			Running,
			Success,
			Failure
		}

		[SerializeField] protected State _state;
		[SerializeField] private bool _isStarted = false;

		public Blackboard blackboard { get; set; }
		public AIController aiController { get; set; }

		public Node()
		{
			
		}

		public State Update()
		{
			if ( false == _isStarted )
			{
				OnStart();
				_isStarted = true;
			}

			_state = OnUpdate();

			if ( _state == State.Failure || _state == State.Success )
			{
				OnStop();
				_isStarted = false;
			}

			return _state;
		}

		public abstract void AddChild( Node child );
		public abstract void RemoveChild( Node child );

		public virtual void OnCreate() { }
		protected abstract void OnStart();
		protected abstract void OnStop();
		protected abstract State OnUpdate();
	}
}
