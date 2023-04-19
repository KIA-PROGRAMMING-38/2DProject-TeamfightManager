using System.Collections;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
	public Transform t1;
	public Transform t2;

    public float moveSpeed;

    private void Start()
	{
		
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			StartCoroutine(Logic());
		}
		if(Input.GetKeyDown(KeyCode.Return))
		{
			Vector2 v1 = transform.position;
			Vector2 v2 = t1.position;
			Vector2 v3 = new Vector2();
            v3.x = ((v1.x + v2.x) * 0.5f) - ((v2.y - v1.y) * Mathf.Sqrt(3f) * 0.5f);
            v3.y = ((v1.y + v2.y) * 0.5f) + ((v2.x - v1.x) * Mathf.Sqrt(3f) * 0.5f);

            t2.position = v3;

            float angle = Vector2.Dot((v2 - v1).normalized, (v3 - v1).normalized);
			angle = Mathf.Acos(angle) * Mathf.Rad2Deg;
			Debug.Log($"Test1 : {angle}");

            angle = Vector2.Dot((v1 - v2).normalized, (v3 - v2).normalized);
            angle = Mathf.Acos(angle) * Mathf.Rad2Deg;
            Debug.Log($"Test1 : {angle}");

            angle = Vector2.Dot((v1 - v3).normalized, (v2 - v3).normalized);
            angle = Mathf.Acos(angle) * Mathf.Rad2Deg;
            Debug.Log($"Test1 : {angle}");
        }
    }

	IEnumerator Logic()
	{
		float t = 0f;
		Vector2 v1 = transform.position;
		Vector2 v2 = t1.position;
		Vector2 v3 = t2.position;

        while (true)
		{
			t = Mathf.Min(t + Time.deltaTime * moveSpeed, 1f);

			Vector2 pos = MathUtility.Bezier.QuadraticBezierCurve(v1, v2, v3, t);

			transform.position = pos;

			if (t >= 1f)
				break;

            yield return null;
        }
	}
}
