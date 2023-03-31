namespace MH_AIFramework
{
	/// <summary>
	/// 모든 BehaviourTree 의 소속된 Node 들의 최상위 Node..
	/// 여기서는 모든 Node에게 필요한 필드 및 메서드들을 정의한다(메서드 중 강제로 재정의해야 한다면 순수 가상 메서드로 만듬)..
	/// </summary>
	public abstract class Node
	{
		public enum State
		{
			Running,
			Success,
			Failure
		}

		protected State _state = State.Running;
		private bool _isStarted = false;

		public Blackboard blackboard { get; set; }
		public BehaviourTree behaviourTree { get; set; }

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

		// 자식 Node 추가 및 제거 관련 함수(자식이 있는 Node의 경우 재정의해서 사용)..
		public abstract void AddChild( Node child );
		public abstract void RemoveChild( Node child );

		// 노드가 생성 시 호출될 이벤트 함수..
		public virtual void OnCreate() { }

		// 노드가 활성화될 때(정확히는 BehaviourTree가 활성화될 때)호출될 이벤트 함수..
		public virtual void OnEnable()
		{
			_isStarted = false;
			_state = State.Failure;
		}

		// 노드가 비활성화될 때(정확히는 BehaviourTree가 비활성화될 때)호출될 이벤트 함수..
		public virtual void OnDisable()
		{
			_isStarted = false;
			_state = State.Failure;
		}

		// 노드가 종료되었다가 시작될 때 호출될 이벤트 함수..
		protected abstract void OnStart();

		// 노드가 행동을 종료될 때 호출될 이벤트 함수..
		protected abstract void OnStop();

		// 실제 노드 Update 함수..
		protected abstract State OnUpdate();
	}
}
