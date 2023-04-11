using System.Collections;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
    private IEnumerator Cor;
    private IEnumerator Cor2;
	private Coroutine co;

	private void Start()
	{
		Cor = Test();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			StartCoroutine(Cor);
		}
	}

	IEnumerator Test()
	{
		Debug.Log("Start");

		yield return new WaitForSeconds(0.1f);

		Debug.Log("End");
	}
}
