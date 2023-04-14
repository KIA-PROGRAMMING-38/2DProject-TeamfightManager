using System.Collections;
using UnityEngine;

public class TestLogic2 : MonoBehaviour
{
	private IEnumerator test;

	private void Awake()
	{
		test = Test();
	}

	private void OnEnable()
	{
		Debug.Log("¾Æ");
		StartCoroutine(test);
	}

	private IEnumerator Test()
	{
		while(true)
		{
			Debug.Log("Test");
			yield return null;
		}
	}
}
