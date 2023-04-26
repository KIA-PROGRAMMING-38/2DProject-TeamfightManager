using System;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
	public event Action OnTestEvent;

	public void asdasd()
	{
		OnTestEvent?.Invoke();
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("OT2");
	}
}
