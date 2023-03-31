using UnityEngine;

namespace MH_AIFramework
{
	/// <summary>
	/// 조건을 검사하기 위한 Node..
	/// Composite Node에 자식으로 들어아게 되며 조건을 만족 못할 경우 Composite Node의 자식들을 실행하지 않는다..
	/// </summary>
	public abstract class DecoratorNode : Node
	{
		// 얘는 자식이 없으므로 자식 관련 함수 호출 시 에러 발생시킴..
		public override void AddChild( Node child )
		{
			throw new System.InvalidProgramException();
		}

		public override void RemoveChild( Node child )
		{
			throw new System.InvalidProgramException();
		}
	}
}