using System.Collections;
using UnityEngine;

public class TestAnimPlay : MonoBehaviour
{
	public GameObject NewGameObject;
	public Animator anim;

	public float waitTime = 0f;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			anim.SetTrigger("On");
			NewGameObject.SetActive(true);
			NewGameObject.GetComponent<Animator>().SetTrigger("On");
			//StartCoroutine(ss());
		}
	}

	IEnumerator ss()
	{
		yield return new WaitForSeconds(waitTime);

		NewGameObject.SetActive(false);
	}
}
