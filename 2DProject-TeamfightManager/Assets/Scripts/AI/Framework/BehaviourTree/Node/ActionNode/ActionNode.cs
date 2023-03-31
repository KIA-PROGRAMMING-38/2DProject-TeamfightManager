using UnityEngine;

namespace MH_AIFramework
{
	/// <summary>
	/// AI 에서 실행할 행동의 최상위 노드(모든 행동들은 이 노드를 상속 받음)..
	/// 이 노드는 자식이 없는 최하위 노드(Leaf Node)다..
	/// </summary>
	public abstract class ActionNode : Node
	{
		// 자식 관련 함수 호출하는 것 자체가 에러임(자식이 없으니까)..
		public override sealed void AddChild( Node child )
		{
			throw new System.InvalidProgramException();
		}

		public override sealed void RemoveChild( Node child )
		{
			throw new System.InvalidProgramException();
		}
	}
}