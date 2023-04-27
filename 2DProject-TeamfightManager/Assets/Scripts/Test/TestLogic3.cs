using System;
using UnityEngine;

public class TestLogic3 : MonoBehaviour
{
    public TestLogic t1;

	public void Awake()
	{

	}

	private void OnTestEvent()
	{
		Debug.Log("이벤트 온");
	}
}
