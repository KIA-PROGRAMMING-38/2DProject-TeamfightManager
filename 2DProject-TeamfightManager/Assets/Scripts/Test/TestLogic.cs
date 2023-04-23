using System;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
	public event Action OnTestEvent;

	public void asdasd()
	{
		OnTestEvent?.Invoke();
	}
}
