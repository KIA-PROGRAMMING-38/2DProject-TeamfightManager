using System;
using System.Collections;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
	public Transform[] transforms;
	public Vector3[] pos;

	private void Awake()
	{
		pos = new Vector3[transforms.Length];
		for ( int i = 0; i < transforms.Length; ++i)
		{
			pos[i] = transforms[i].position;
		}
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Return))
		{
			Debug.Log("Start");

			for( int i = 0; i < transforms.Length; ++i)
			{
				transforms[i].position = pos[i];
			}

			GetComponent<CircleCollider2D>().enabled = true;

			StartCoroutine(asdsdads());
		}
	}

	IEnumerator asdsdads()
	{
		yield return new WaitForSeconds(1f);

		GetComponent<CircleCollider2D>().enabled = false;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Debug.Log("asd");
		collision.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
	}
}
