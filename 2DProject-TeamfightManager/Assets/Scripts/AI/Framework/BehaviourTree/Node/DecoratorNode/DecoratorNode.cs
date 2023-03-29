using UnityEngine;

namespace MH_AIFramework
{
	[System.Serializable]
	public abstract class DecoratorNode : Node
	{
		public override void AddChild( Node child )
		{
			throw new System.ArgumentException();
		}

		public override void RemoveChild( Node child )
		{
			throw new System.ArgumentException();
		}
	}
}